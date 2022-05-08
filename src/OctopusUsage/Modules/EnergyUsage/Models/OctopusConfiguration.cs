namespace OctopusUsage.Modules.EnergyUsage.Models;

[ExcludeFromCodeCoverage]
public class OctopusConfiguration
{
    public string ApiKey { get; set; } = default!;
    public string ElectricityMeterMpan { get; set; } = default!;
    public string ElectricityMeterSerial { get; set; } = default!;
    public string GasMeterMprn { get; set; } = default!;
    public string GasMeterSerial { get; set; } = default!;
}