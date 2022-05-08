using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using OctopusUsage.Constants;

namespace OctopusUsage.Modules.EnergyUsage.Jobs;

[UsedImplicitly]
[DisallowConcurrentExecution]
public class SyncOctopusGasUsage : IJob
{
    private static readonly ILogger Logger = Log.ForContext<SyncOctopusGasUsage>();
    private readonly IOctopusEnergyClient _octopusEnergyClient;
    private readonly OctopusConfiguration _octopusConfiguration;
    private readonly IMemoryCache _memoryCache;
    private readonly IHubContext<UsageHub> _usageHub;

    public SyncOctopusGasUsage(
        IOctopusEnergyClient octopusEnergyClient,
        OctopusConfiguration octopusConfiguration,
        IMemoryCache memoryCache,
        IHubContext<UsageHub> usageHub)
    {
        _octopusEnergyClient = octopusEnergyClient;
        _octopusConfiguration = octopusConfiguration;
        _memoryCache = memoryCache;
        _usageHub = usageHub;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var apiKey = _octopusConfiguration.ApiKey;
            var mpan = _octopusConfiguration.GasMeterMprn;
            var serial = _octopusConfiguration.GasMeterSerial;
            var from = DateTime.Today;
            var to = DateTime.Today.AddDays(1);

            var usage = await _octopusEnergyClient.GetGasConsumptionAsync(
                apiKey,
                mpan,
                serial,
                from,
                to,
                Interval.Day).ConfigureAwait(false);

            _memoryCache.Set(CacheKeys.GasUsage, new UsageLastDay(EnergyType.Gas, usage.Sum(x => x.Quantity)));

            await _usageHub.Clients.All.SendAsync(nameof(UsageHub.SendGasUsageAsync)).ConfigureAwait(false);
        }
        finally
        {
            stopwatch.Stop();
            Logger.Information("{JobName} executed in {Elapsed}", nameof(SyncOctopusGasUsage), stopwatch.Elapsed);
        }
    }
}