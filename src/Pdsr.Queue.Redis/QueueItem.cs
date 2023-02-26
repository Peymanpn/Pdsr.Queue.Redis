namespace Pdsr.Queue;

/// <summary>
/// The actual message that is stored in the queue
/// </summary>
/// <typeparam name="TMessage">The Message generic type</typeparam>
internal class QueueItem<TMessage>
{


    /// <summary>
    /// Creates a new instance of <see cref="QueueItem{TMessage}"/>.
    /// The message cannot be null.
    /// </summary>
    /// <param name="message"></param>
    public QueueItem(TMessage message)
    {
        Id = Guid.NewGuid().ToString();
        RequeueCount = 0;
        Message = message;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Message Unique Id
    /// </summary>
    /// <value></value>
    public string Id { get; set; }

    internal void IncrementRequeueCount()
    {
        RequeueCount++;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Number of times the message requeued
    /// </summary>
    public int RequeueCount { get; set; }

    /// <summary>
    /// The Date and Time the message was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// The Date and Time the message was updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; internal set; }

    /// <summary>
    /// The actual message
    /// </summary>
    /// <value></value>
    public TMessage Message { get; }
}
