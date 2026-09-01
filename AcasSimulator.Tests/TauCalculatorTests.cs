namespace AcasSimulator.Tests;

using System;
using Xunit;
using AcasSimulator.Engine;
using AcasSimulator.Models;


public class TauCalculatorTests
{
    [Fact]
    public void CalculateSlantRangeTau_HeadOnCollision_ReturningExactTimeToImpact()
    {
        //Aircraft 10NM appart heading directly towards eachother.
        //both 360kts (0.1 NM/s, relative closure 0.2NM/s)
        //Expect tau = 10NM / 0.2 NM/s = 50s

        //Arange:
        Aircraft ownShip = new Aircraft("OWN1", 0.0, 0.0, 10000.0, 360, 90.0, 0.0);
        Aircraft intShip = new Aircraft("INT1", 10, 0.0, 10000.0, 360, 270.0, 0.0);
        //Act:
        double tau = TauCalculator.CalculateSlantRangeTau(ownShip, intShip);
        //Assert:
        Assert.Equal(50.0, tau, precision: 2);
    }

        [Fact]
    public void CalculateSlantRangeTau_DivergingAircraft_ReturnsInfinity()
    {
        //Aircrafts moving away from eachother

        //Arange:
        Aircraft ownShip = new Aircraft("OWN1", 0.0, 0.0, 10000.0, 320, 270.0, 0.0);
        Aircraft intShip = new Aircraft("INT1", 10.0, 0.0, 10000.0, 320, 90.0, 0.0);
        //Act:
        double tau = TauCalculator.CalculateSlantRangeTau(ownShip, intShip);
        //Assert:
        Assert.True(double.IsPositiveInfinity(tau));
    }

        [Fact]
    public void CalculateSlantRangeTau_PerpendiculatIntersectingPaths_ReturnsTimeToCPA()
    {
        // Ownship at origin (0, 0) moving North (0 deg / 360 kts = 0.1 NM/s).
        // Intruder at (10, 10) moving West (270 deg / 360 kts = 0.1 NM/s).
        // They both reach (0, 10) at t = 10 NM / 0.1 NM/s = 100 seconds.

        //Arange:
        Aircraft ownship = new Aircraft("OWN1", 0.0, 0.0, 10000.0, 360.0, 0.0, 0.0);     // Heading North
        Aircraft  intship = new Aircraft("INT1", 10.0, 10.0, 10000.0, 360.0, 270.0, 0.0); // Heading West

        //Act:
        double tau = TauCalculator.CalculateSlantRangeTau(ownship, intship);

        //Assert:
        Assert.Equal(100.0, tau, precision: 2);
    }

        [Fact]
        public void CalculateSlantRangeTau_ParallelSameSpeed_ReturnsInfinity()
    {
        // ARRANGE: Two aircraft flying side-by-side North at identical speeds (no relative motion).
        Aircraft ownship = new Aircraft("OWN1", 0.0, 0.0, 10000.0, 300.0, 0.0, 0.0);
        Aircraft intship = new Aircraft("INT1", 3.0, 0.0, 10000.0, 300.0, 0.0, 0.0);

        // ACT
        double tau = TauCalculator.CalculateSlantRangeTau(ownship, intship);

        // ASSERT
        Assert.True(double.IsPositiveInfinity(tau));
    }
}