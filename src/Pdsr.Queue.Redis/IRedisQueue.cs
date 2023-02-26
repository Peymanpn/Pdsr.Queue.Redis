namespace Pdsr.Queue.Redis;

/// <summary>
/// RedisQueue Service interface
/// </summary>
public interface IRedisQueue
{
    /// <summary>
    /// Publishes a message through the specified queue.
    /// The Queue is created if it doesn't exist.
    /// Queue name is equal to the name of the message type.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to publish to queue</typeparam>
    /// <param name="message">The message to queue</param>
    /// <returns>A System.Threading.Tasks.Task that represents the long running operations.</returns>
    Task Publish<TMessage>(TMessage message);

    /// <summary>
    /// Tries to fetch a new message from the queue. if a new message exists, it will invoke the handler with <typeparamref name="TMessage"/> as an argument
    /// If there isn't any message exists in the queue, nothing will happen
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to receive from the queue</typeparam>
    /// <param name="handler">The method to call if a new message exists in the queue</param>
    /// <returns> A System.Threading.Tasks.Task that represents the long running operations.</returns>
    Task FetchMessage<TMessage>(Func<TMessage, Task<bool>> handler);
}
