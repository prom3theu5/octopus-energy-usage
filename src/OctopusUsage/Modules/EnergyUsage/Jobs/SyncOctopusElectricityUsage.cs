using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using OctopusUsage.Constants;

namespace OctopusUsage.Modules.EnergyUsage.Jobs;

[UsedImplicitly]
[DisallowConcurrentExecution]
public class SyncOctopusElectricityUsage : IJob
{
    private static readonly ILogger Logger = Log.ForContext<SyncOctopusElectricityUsage>();
    private readonly IOctopusEnergyClient _octopusEnergyClient;
    private readonly OctopusConfiguration _octopusConfiguration;
    private readonly IMemoryCache _memoryCache;
    private readonly IHubContext<UsageHub> _usageHub;

    public SyncOctopusElectricityUsage(
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
            var mpan = _octopusConfiguration.ElectricityMeterMpan;
            var serial = _octopusConfiguration.ElectricityMeterSerial;
            var from = DateTime.Today;
            var to = DateTime.Today.AddDays(1);

            var usage = await _octopusEnergyClient.GetElectricityConsumptionAsync(
                apiKey,
                mpan,
                serial,
                from,
                to,
                Interval.Day).ConfigureAwait(false);

            _memoryCache.Set(CacheKeys.ElectricityUsage, new UsageLastDay(EnergyType.Electricity, usage.Sum(x => x.Quantity)));

            await _usageHub.Clients.All.SendAsync(nameof(UsageHub.SendElectricityUsageAsync)).ConfigureAwait(false);
        }
        finally
        {
            stopwatch.Stop();
            Logger.Information("{JobName} executed in {Elapsed}", nameof(SyncOctopusElectricityUsage), stopwatch.Elapsed);
        }
    }
}