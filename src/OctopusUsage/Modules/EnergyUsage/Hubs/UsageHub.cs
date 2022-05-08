using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using OctopusUsage.Constants;

namespace OctopusUsage.Modules.EnergyUsage.Hubs;

public class UsageHub : Hub
{
    private readonly IMemoryCache _cache;

    public UsageHub(IMemoryCache cache) => _cache = cache;

    public async Task SendElectricityUsageAsync()
    {
        if (_cache.TryGetValue(CacheKeys.ElectricityUsage, out UsageLastDay electricityUsage))
        {
            await Clients.All.SendAsync(CacheKeys.ElectricityUsage, electricityUsage).ConfigureAwait(false);
        }
    }

    public async Task SendGasUsageAsync()
    {
        if (_cache.TryGetValue(CacheKeys.GasUsage, out UsageLastDay electricityUsage))
        {
            await Clients.All.SendAsync(CacheKeys.GasUsage, electricityUsage).ConfigureAwait(false);
        }
    }
}