using Pdsr.Cache.Configurations;
using Pdsr.Queue.Redis;
using Pdsr.Queue.SampleWebApp;
using Pdsr.Queue.SampleWebApp.Workers;

var builder = WebApplication.CreateBuilder(args);

IRedisConfiguration redisConf = builder.Configuration.GetSection("Redis").Get<RedisConfiguration>() ?? throw new NullReferenceException("appsettings doesn't contain Redis configurations.");

builder.Services.AddRedisQueue(redisConf);

// Add Workers background services.
builder.Services.AddHostedService<Worker01>();
builder.Services.AddHostedService<Worker02>();


var app = builder.Build();


// Creates a message in the request
app.MapGet("/", async (IRedisQueue redisQueue) =>
{
    // create a message
    var message = new SampleMessage(Guid.NewGuid().ToString());

    await redisQueue.Publish(message);

    return Results.Ok(message);//show the message
});


app.Run();

