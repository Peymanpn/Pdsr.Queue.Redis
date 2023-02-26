using Pdsr.Cache.Configurations;
using Pdsr.Queue.Redis;

namespace Microsoft.Extensions.DependencyInjection;


/// <summary>
/// Dependency Injection extensions to help with registering <see cref="IRedisQueue"/> services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IRedisQueue"/> as a singleton along with <see cref="Pdsr.Cache.IRedisCacheManager"/>
    /// </summary>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
    /// <param name="redisConfiguration"><see cref="IRedisConfiguration"/> containing Redis configurations such as endpoints,...</param>
    /// <returns>The original Microsoft.Extensions.DependencyInjection.IServiceCollection.</returns>
    public static IServiceCollection AddRedisQueue(this IServiceCollection services, IRedisConfiguration redisConfiguration)
    {
        // Adds IRedisCache manager if it's not added already
        services.AddRedisCacheManager(redisConfiguration);

        // Registering IRedisQueue as a singleton
        services.AddSingleton<IRedisQueue, RedisQueue>();

        return services;
    }
}
