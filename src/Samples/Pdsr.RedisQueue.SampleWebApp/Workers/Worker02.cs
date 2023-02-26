using Pdsr.Queue.Redis;

namespace Pdsr.Queue.SampleWebApp.Workers;

/// <summary>
/// Worker 2
/// It's a copy of Worker 1, just to show that you can have multiple workers
/// In practice, it should be replica of the same worker, running from different machines, containers, etc.
/// </summary>
public class Worker02 : WorkerBase
{
    public Worker02(IRedisQueue redisQueue) : base(redisQueue) { }
}

