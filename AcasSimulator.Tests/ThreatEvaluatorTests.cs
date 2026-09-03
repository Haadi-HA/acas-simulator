namespace AcasSimulator.Tests;

using Xunit;
using AcasSimulator.Models;
using AcasSimulator.Engine;

public class ThreatEvaulatorTests
{
    [Fact]
    public void EvaluateThreat_HighSpeedHeadOnAtCruise_TriggersResolutionAdvisory()
    {
        // SL 7 (30,000 ft) -- RA Tau = 35s, DMOD = 1.1 NM, ZTHR = 600 ft
        // Dist = 5 NM, Closing Speed = 720 kts (0.2 NM/s) -- TauMod = 23.8s <= 35s

        // ARRANGE:
        Aircraft ownship = new Aircraft("OWN1", "400001", 0.0, 0.0, 30000.0, 360.0, 90.0, 0.0);
        Aircraft intruder = new Aircraft("INT1", "400002", 5.0, 0.0, 30000.0, 360.0, 270.0, 0.0);

        // ACT:
        ThreatLevel threat = ThreatEvaulator.EvaluateThreat(ownship, intruder);

        // ASSERT:
        Assert.Equal(ThreatLevel.ResolutionAdvisory, threat);
    }

        [Fact]
    public void EvaluateThreat_Below2350Ft_InhibitsResolutionAdvisory()
    {
        // SL 3 (2,000 ft) -- TA Tau = 15s, RA inhibited
        // Close distance (1 NM) head-on would trigger RA at higher altitudes

        // ARRANGE:
        Aircraft ownship = new Aircraft("OWN1", "400001", 0.0, 0.0, 2000.0, 360.0, 90.0, 0.0);
        Aircraft intruder = new Aircraft("INT1", "400002", 1.0, 0.0, 2000.0, 360.0, 270.0, 0.0);

        // ACT:
        ThreatLevel threat = ThreatEvaulator.EvaluateThreat(ownship, intruder);

        // ASSERT:
        Assert.Equal(ThreatLevel.TrafficAdvisory, threat);
    }

        [Fact]
    public void EvaluateThreat_ParallelFlightWithin6NMAnd1200Ft_ReturnsProximate()
    {
        // Both heading 360 at 360 kts (parallel, no Tau breach).
        // Range = 3.0 NM, Altitude Delta = 500 ft.

        // ARRANGE:
        Aircraft ownship = new Aircraft("OWN1", "400001", 0.0, 0.0, 30000.0, 360.0, 0.0, 0.0);
        Aircraft intruder = new Aircraft("INT1", "400002", 3.0, 0.0, 30500.0, 360.0, 0.0, 0.0);

        // ACT:
        ThreatLevel threat = ThreatEvaulator.EvaluateThreat(ownship, intruder);

        // ASSERT:
        Assert.Equal(ThreatLevel.Proximate, threat);
    }

        [Fact]
    public void EvaluateThreat_DivergingAircraft_ReturnsClear()
    {
        // Aircraft more than 6NM appart, clear ThreatLevel

        // ARRANGE:
        Aircraft ownship = new Aircraft("OWN1", "400001", 0.0, 0.0, 30000.0, 360.0, 90.0, 0.0);
        Aircraft intruder = new Aircraft("INT1", "400002", 10.0, 0.0, 30000.0, 360.0, 270.0, 0.0);

        // ACT:
        ThreatLevel threat = ThreatEvaulator.EvaluateThreat(ownship, intruder);

        // ASSERT:
        Assert.Equal(ThreatLevel.Clear, threat);
    }
}