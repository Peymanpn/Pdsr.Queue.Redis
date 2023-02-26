# Pdsr Simple Redis Queue

Uses Redis to act as a Message Queue.

## Getting started

### Install the package

```bash
dotnet add package Pdsr.Queue.Redis
```

### Add it to the DI

```csharp
// Add to DI
builder.Services.AddRedisQueue(new RedisConfiguration
{
    EndPoints = new[] { "localhost:6379" }
});
```

### Define some class to use as message

```csharp
/// <summary>
/// Some message sample
/// </summary>
public class SampleMessage
{
    public SampleMessage(string item)
    {
        Item = item;
    }

    // can be anything. be careful not to send large objects.
    public string Item { get;  }
}
```

### Publishing the message

```csharp
app.MapGet("/", async (IRedisQueue redisQueue) =>
{
    // create an instance of the message object
    string contents = "Peeling an apple";
    var message = new SampleMessage(contents);

    // publish it
    await redisQueue.Publish(message);

    // return the message object, just because it's cool to see what we sent to the queue
    return Results.Ok(message);//show the message
});
```

### Process messages

Inject the `IRedisQueue` in a worker or some service and fetch messages one at the time

```csharp
// fetch messages from queue and process them
await _redisQueue.FetchMessage(async (msg) =>
    {
        Console.WriteLine($"do something with the msg");
        return true; // return true to ACK (and discard the message) or return false and do a NACK requeue.
    };);
```

Preferably use a Background worker, as the whole point for me to use Message Queue was to process them separately on a worker not in Services or Controllers.

## ToDo

Make Subscriber pattern to emit events, instead of using a manual fetch

## Contribute

Please refer to [contribute](CONTRIBUTING.md).
Contributions are welcomed, refer to [ToDo](#todo)

## Documents

Above is enough for now.
