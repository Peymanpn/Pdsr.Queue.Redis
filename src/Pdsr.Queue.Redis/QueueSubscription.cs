namespace Pdsr.Queue.Redis;

internal sealed class QueueSubscription
{
    private Func<string, Task<bool>>? _handlers;
    public void Add<TMessage>(Func<string, Task<bool>> handler)
    {
        if (_handlers is null)
        {
            _handlers = new Func<string, Task<bool>>(handler);
        }
        else
        {
            _handlers += handler;
        }
    }

    public void Remove<TMessage>(Func<string, Task<bool>> handler)
    {
        if (_handlers is not null)
        {
            _handlers -= handler;
        }
    }
}
