namespace AcasSimulator.Tests;

using Xunit;
using AcasSimulator.Engine;
using AcasSimulator.Models;

public class RaSenseSelectorTests
{
    [Fact]
    public void SelectSense_IntruderAbove_SelectsDescend()
    {
        // Intruder is 300 ft above on converging course

        // ARRANGE:
        var ownship = new Aircraft("OWN1", 0.0, 0.0, 30000.0, 360.0, 90.0, 0.0);
        var intruder = new Aircraft("INT1", 3.0, 0.0, 30300.0, 360.0, 270.0, 0.0);

        // ACT:
        AdvisorySense sense = RaSenseSelector.SelectSense(ownship, intruder);

        // ASSERT:
        Assert.Equal(AdvisorySense.Descend, sense);
    }

    [Fact]
    public void SelectSense_IntruderBelow_SelectsClimb()
    {
        // Intruder is 300 ft below on converging course

        // ARRANGE:
        var ownship = new Aircraft("OWN1", 0.0, 0.0, 30000.0, 360.0, 90.0, 0.0);
        var intruder = new Aircraft("INT1", 3.0, 0.0, 29700.0, 360.0, 270.0, 0.0);

        // ACT:
        AdvisorySense sense = RaSenseSelector.SelectSense(ownship, intruder);

        // ASSERT:
        Assert.Equal(AdvisorySense.Climb, sense);
    }

    [Fact]
    public void SelectSense_LowAltitude_InhibitsDescendAndForcesClimb()
    {
        // Intruder is above, but ownship is at 800 ft
        // Forced Climb due to CFIT prevention system

        // ARRANGE:
        var ownship = new Aircraft("OWN1", 0.0, 0.0, 800.0, 150.0, 90.0, 0.0);
        var intruder = new Aircraft("INT1", 1.0, 0.0, 1100.0, 150.0, 270.0, 0.0);

        // ACT:
        AdvisorySense sense = RaSenseSelector.SelectSense(ownship, intruder);

        // ASSERT:
        Assert.Equal(AdvisorySense.Climb, sense);
    }
}