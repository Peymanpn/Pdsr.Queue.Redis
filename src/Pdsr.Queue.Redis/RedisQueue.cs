using Microsoft.Extensions.Logging;
using Pdsr.Cache;
using System.Diagnostics;
using System.Text.Json;

namespace Pdsr.Queue.Redis;

/// <inheritdoc/>
public class RedisQueue : IRedisQueue
{
    private readonly IRedisCacheManager _redis;
    //private readonly JsonSerializerOptions _jsonSerializerOptions;
    //private readonly ConcurrentDictionary<string, QueueSubscription> _subscriptions = new();
    private readonly ILogger<IRedisQueue> _logger;

    public RedisQueue(IRedisCacheManager redis,
        ILoggerFactory loggerFactory)
    {
        _redis = redis;
        //_jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General);
        _logger = loggerFactory.CreateLogger<IRedisQueue>();
    }

    /// <inheritdoc/>
    public async Task Publish<TMessage>(TMessage message)
    {
        string queue = GetQueueName<TMessage>();
        var queueItem = new QueueItem<TMessage>(message);
        string body = Serialize(queueItem);

        // Push the message
        await _redis.Redis.ListRightPushAsync(queue, body);

        // avoid using the ListLen and an extra call to Redis in production
        // so it is only compiled when TRACE is defined
#if TRACE
        var listLength = await _redis.Redis.ListLengthAsync(queue);
        _logger.LogTrace("List count :{listLength}", listLength);
#endif
    }

    /// <inheritdoc/>
    public async Task FetchMessage<TMessage>(Func<TMessage, Task<bool>> handler)
    {
        string queue = GetQueueName<TMessage>();
        string redisLock = GetLockKey<TMessage>();

        bool lockAcquireSuccess = await _redis.Redis.LockTakeAsync(redisLock, "1", TimeSpan.FromSeconds(5), flags: StackExchange.Redis.CommandFlags.DemandMaster);
        if (!lockAcquireSuccess)
        {
            _logger.LogDebug("Failed to acquire lock for {lock}", redisLock);
            return;
        }

        var listLength = await _redis.Redis.ListLengthAsync(queue);
        if (listLength == 0)
        {
            _logger.LogTrace("No new messages for the queue {queueName}", queue);
            await _redis.Redis.LockReleaseAsync(redisLock, "1");
            return;
        }

        _logger.LogTrace("Queue messages count :{count}", listLength);
        var body = await _redis.Redis.ListLeftPopAsync(queue);
        await _redis.Redis.LockReleaseAsync(redisLock, "1");

        var message = Deserialize<TMessage>(body.ToString());

        _logger.LogDebug("Popping message with id {id}", message.Id);
        bool results = false;

        var stopWatch = Stopwatch.StartNew();

        try
        {
            results = await handler(message.Message);
        }
        catch { results = false; }
        finally
        {
            _logger.LogDebug("Processing message {id} job done.", message.Id);

            _logger.LogTrace("Processing message {id} job took {elapsed}", message.Id, stopWatch.Elapsed);

        }

        if (!results)
        {
            // Nack
            message.IncrementRequeueCount();

            _logger.LogDebug("Message {id} being requeued for queue {queue}.\nMessage age: {age}\nMessage were originally created at: {createdAt} and requeued for: {requeues}", message.Id, queue, DateTimeOffset.UtcNow.Subtract(message.CreatedAt), message.CreatedAt, message.RequeueCount);

            body = Serialize(message);

            // Requeue the message
            await _redis.Redis.ListRightPushAsync(queue, body);
        }
    }

    #region Utils
    /// <summary>
    /// Gets the queue name for the message type
    /// </summary>
    private string GetQueueName<TMessage>() => $"pdsr:queue:{typeof(TMessage).FullName.ToLower()}";

    /// <summary>
    /// Get the lock Redis key
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <returns></returns>
    private string GetLockKey<TMessage>() => $"{GetQueueName<TMessage>()}:lock";

    /// <summary>
    /// Serializes the message to stor in Redis as a string
    /// </summary>
    private string Serialize<TMessage>(QueueItem<TMessage> message) => JsonSerializer.Serialize(message);

    /// <summary>
    /// Deserialized the message fetched from the queue
    /// </summary>
    /// <returns>Deserialized TMessage</returns>
    /// <exception cref="NullReferenceException"></exception>
    private QueueItem<TMessage> Deserialize<TMessage>(string message) =>
        JsonSerializer.Deserialize<QueueItem<TMessage>>(message) ?? throw new NullReferenceException();
    #endregion
}
