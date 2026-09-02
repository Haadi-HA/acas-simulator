namespace AcasSimulator.Tests;

using System;
using Xunit;
using AcasSimulator.Engine;
using AcasSimulator.Models;


public class TauCalculatorTests
{
    //========================================================
    // Horizontal Tau Test
    //========================================================

    [Fact]
    public void CalculateHorizontalTau_HeadOnCollision_ReturningExactTimeToImpact()
    {
        //Aircraft 10NM appart heading directly towards eachother.
        //both 360kts (0.1 NM/s, relative closure 0.2NM/s)
        //Expect tau = 10NM / 0.2 NM/s = 50s

        //Arange:
        Aircraft ownShip = new Aircraft("OWN1", 0.0, 0.0, 10000.0, 360.0, 90.0, 0.0);
        Aircraft intShip = new Aircraft("INT1", 10.0, 0.0, 10000.0, 360.0, 270.0, 0.0);

        //Act:
        double tauH = TauCalculator.CalculateHorizontalTau(ownShip, intShip);

        //Assert:
        Assert.Equal(50.0, tauH, precision: 2);
    }

    [Fact]
    public void CalculateHorizontalTau_DivergingAircraft_ReturnsInfinity()
    {
        //Aircrafts moving away from eachother

        //Arange:
        Aircraft ownShip = new Aircraft("OWN1", 0.0, 0.0, 10000.0, 360.0, 270.0, 0.0);
        Aircraft intShip = new Aircraft("INT1", 10.0, 0.0, 10000.0, 360.0, 90.0, 0.0);

        //Act:
        double tauH = TauCalculator.CalculateHorizontalTau(ownShip, intShip);

        //Assert:
        Assert.True(double.IsPositiveInfinity(tauH));
    }

    [Fact]
    public void CalculateHorizontalTau_PerpendiculatIntersectingPaths_ReturnsTimeToCPA()
    {
        // Ownship at origin (0, 0) moving North (0 deg / 360 kts = 0.1 NM/s).
        // Intruder at (10, 10) moving West (270 deg / 360 kts = 0.1 NM/s).
        // They both reach (0, 10) at t = 10 NM / 0.1 NM/s = 100 seconds.

        //Arange:
        Aircraft ownship = new Aircraft("OWN1", 0.0, 0.0, 10000.0, 360.0, 0.0, 0.0);     // Heading North
        Aircraft  intship = new Aircraft("INT1", 10.0, 10.0, 10000.0, 360.0, 270.0, 0.0); // Heading West

        //Act:
        double tauH = TauCalculator.CalculateHorizontalTau(ownship, intship);

        //Assert:
        Assert.Equal(100.0, tauH, precision: 2);
    }

    [Fact]
    public void CalculateHorizontalTau_ParallelSameSpeed_ReturnsInfinity()
    {
        // ARRANGE: Two aircraft flying side-by-side North at identical speeds (no relative motion).
        Aircraft ownship = new Aircraft("OWN1", 0.0, 0.0, 10000.0, 300.0, 0.0, 0.0);
        Aircraft intship = new Aircraft("INT1", 3.0, 0.0, 10000.0, 300.0, 0.0, 0.0);

        // ACT:
        double tauH = TauCalculator.CalculateHorizontalTau(ownship, intship);

        // ASSERT:
        Assert.True(double.IsPositiveInfinity(tauH));
    }

    //========================================================
    // Vertical Tau Test
    //========================================================

    [Fact]
    public void CalculateVerticalTau_ConvergingAltitudes_ReturnsExactTimeToCoAltitudeame()
    {
        // Gap = 1,500 ft. Total closure rate = 50 ft/s.
        // Expected Tau = 1500 / 50 = 30.0 seconds.

        // ARRANGE:
        Aircraft ownship = new Aircraft("OWN1", 0.0, 0.0, 10000.0, 360.0, 0.0, 30.0);
        Aircraft intruder = new Aircraft("INT1", 0.0, 0.0, 11500.0, 360.0, 0.0, -20.0);

        // ACT:
        double tauV = TauCalculator.CalculateVerticalTau(ownship, intruder);

        // ASSERT:
        Assert.Equal(30.0, tauV, precision: 2);
    }
        
    [Fact]
    public void CalculateVerticalTau_DivergingVerticalMotion_ReturnsInfinity()
    {
        // Ownship at 10,000 ft descending (-20 ft/s), Intruder at 12,000 ft climbing (+20 ft/s)
        // Diverging, expect positive infinity

        // ARRANGE:
        Aircraft ownship = new Aircraft("OWN1", 0.0, 0.0, 10000.0, 360.0, 0.0, -20.0);
        Aircraft intruder = new Aircraft("INT1", 0.0, 0.0, 12000.0, 360.0, 0.0, 20.0);
    
        // ACT:
        double tauV = TauCalculator.CalculateVerticalTau(ownship, intruder);
    
        // ASSERT:
        Assert.True(double.IsPositiveInfinity(tauV));
    }

    //========================================================
    // Modified Tau Test
    //========================================================

    [Fact]
    public void CalculateModifiedTau_InsideDmodBoundary_ReturnsZero()
    {
        // DMOD is 1.1 NM intruder at 0.8 NM range.

        // ARRANGE:
        Aircraft ownship = new Aircraft("OWN1", 0.0, 0.0, 30000.0, 360.0, 90.0, 0.0);
        Aircraft intruder = new Aircraft("INT1", 0.8, 0.0, 30000.0, 360.0, 270.0, 0.0);

        // ACT:
        double tauMod = TauCalculator.CalculateModifiedTau(ownship, intruder, dmodNM: 1.1);

        // ASSERT:
        Assert.Equal(0.0, tauMod);
    }

    [Fact]
    public void CalculateModifiedTau_DivergingAircraft_ReturnsInfinity()
    {
        // Aircraft moving away from each other

        // ARRANGE:
        Aircraft ownship = new Aircraft("OWN1", 0.0, 0.0, 30000.0, 360.0, 270.0, 0.0); // West
        Aircraft intruder = new Aircraft("INT1", 5.0, 0.0, 30000.0, 360.0, 90.0, 0.0);  // East

        // ACT:
        double tauMod = TauCalculator.CalculateModifiedTau(ownship, intruder, dmodNM: 1.1);

        // ASSERT:
        Assert.True(double.IsPositiveInfinity(tauMod));
    }

    [Fact]
    public void CalculateModifiedTau_StandardConvergence_ReturnsModifiedTauValue()
    {
        // r = 10 NM, dotProduct = 10 * (-0.2) = -2.0
        // DMOD = 1.1 NM ,  DMOD^2 = 1.21
        // TauMod = -(100 - 1.21) / -2.0 = 98.79 / 2.0 = 49.395 seconds

        // ARRANGE:
        Aircraft ownship = new Aircraft("OWN1", 0.0, 0.0, 30000.0, 360.0, 90.0, 0.0);
        Aircraft intruder = new Aircraft("INT1", 10.0, 0.0, 30000.0, 360.0, 270.0, 0.0);

        // ACT
        double tauMod = TauCalculator.CalculateModifiedTau(ownship, intruder, dmodNM: 1.1);

        // ASSERT:
        Assert.Equal(49.40, tauMod, precision: 2);
    }
}
