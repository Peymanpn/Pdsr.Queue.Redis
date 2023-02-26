using Pdsr.Queue.Redis;

namespace Pdsr.Queue.SampleWebApp.Workers;


/// <summary>
/// Base Worker class
/// </summary>
public abstract class WorkerBase : BackgroundService
{

    private readonly IRedisQueue _redisQueue;

    /// <summary>
    /// Just to see the worker messages.
    /// Should use ILogger instead.
    /// </summary>
    private protected string? _msgPrefix;

    public WorkerBase(IRedisQueue redisQueue)
    {
        _redisQueue = redisQueue;
        _msgPrefix = $"[{this.GetType().Name}]\t";
    }

    /// <summary>
    /// BackgroundService execute method
    /// </summary>
    /// <param name="stoppingToken">Runs until cancellation token not requested</param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // fetch messages from queue and process them
            await _redisQueue.FetchMessage(DoJob());

            // print some stuff for no reason
            Console.WriteLine($"{_msgPrefix}Hearthbeat at: {DateTimeOffset.UtcNow}");

            // create a random delay between the workers to simulate some load for testing purposes.
            await Task.Delay(Random.Shared.Next(3000, 5000), stoppingToken);
        }
    }

    /// <summary>
    /// This is the job that will be executed by the worker
    /// </summary>
    /// <returns></returns>
    private Func<SampleMessage, Task<bool>> DoJob()
    {
        return async (msg) =>
        {
            Console.WriteLine($"{_msgPrefix}Running msg {msg.Item} on worker 1");
            await Task.Delay(Random.Shared.Next(3000, 10000)); // simulate some time consuming job

            // simulate if the task failed or not. 50% probability of failure and requeue the task.
            // failing tasks will requeue
            var rnd = Random.Shared.Next(0, 10) % 2;

            Console.WriteLine($"{_msgPrefix}Running job for message {msg.Item} finished on worker 1");
            Console.WriteLine($"{_msgPrefix}Tossup results:{rnd} to simulate if the task failed or not.\n\t\t50% probability of failure and requeue the task.");

            return rnd == 0;
        };
    }
}

