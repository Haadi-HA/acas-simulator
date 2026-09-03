using System;
using Xunit;
using AcasSimulator.Models;

namespace AcasSimulator.Tests;

public class AircraftTests
{
    [Fact]
    public void UpdatePosition_MovesAircraftCorrectly_HeadingNorth()
    {
        // ARRANGE: Fly North (0.1 NM/sec) for 10 seconds
        Aircraft aircraft = new Aircraft(
            callSign: "TEST01",
            icaoAddress: "000000",
            cartesianX: 0.0,
            cartesianY: 0.0,
            pressureAltitudeFeet: 10000.0,
            groundSpeedKnots: 360.0,
            headingDegrees: 0.0,
            verticalSpeedFpm: 600.0 // +10 ft/sec
        );

        // ACT: 
        aircraft.UpdatePosition(10.0);

        // ASSERT: Verify coordinates and altitude changes
        Assert.Equal(0.0, aircraft.CartesianX, precision: 5);
        Assert.Equal(1.0, aircraft.CartesianY, precision: 5);
        Assert.Equal(10100.0, aircraft.PressureAltitudeFeet, precision: 5);
    }
}