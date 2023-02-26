using Pdsr.Queue.Redis;

namespace Pdsr.Queue.SampleWebApp.Workers;

/// <summary>
/// First Worker
/// It's a copy of WorkerBase , just to show that you can have multiple workers
/// In practice, it should be replica of the same worker, running from different machines, containers, etc.
/// </summary>
public class Worker01 : WorkerBase
{
    public Worker01(IRedisQueue redisQueue) : base(redisQueue) { }
}

