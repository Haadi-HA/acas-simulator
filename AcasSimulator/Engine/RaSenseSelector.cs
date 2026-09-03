namespace AcasSimulator.Engine;

using System;
using AcasSimulator.Models;

public static class RaSenseSelector
{
    // Standard DO-185B initial RA maneuver vertical rate: 1,500 fpm
    private const double StandardRaVerticalRateFpm = 1500.0;

    public static AdvisorySense SelectSense(Aircraft ownship, Aircraft intruder)
    {
        SensitivityParameters sensitivityParameters = SensitivityParameters.GetParametersForAltitude(ownship.PressureAltitudeFeet);

        // Calculate Time to Closest Approach (TCA) using Modified Tau
        double tau = TauCalculator.CalculateModifiedTau(ownship, intruder, sensitivityParameters.RaDmodNM);
        
        // If Tau is infinite or invalid, no sense is required
        if (double.IsInfinity(tau) || double.IsNaN(tau) || tau < 0)
        {
            return AdvisorySense.None;
        }

        // Predict intruder altitude at TCA (maintaining current vertical rate)
        double intruderVerticalRateFps = intruder.VerticalSpeedFpm / 60.0;
        double intruderAltAtTca = intruder.PressureAltitudeFeet + (intruderVerticalRateFps * tau);

        // Predict ownship altitude at TCA for CLIMB manuvear (+1,500 fpm)
        double ownshipClimbAltAtTca = ownship.PressureAltitudeFeet + (StandardRaVerticalRateFpm * tau);
        double climbSeparation = Math.Abs(ownshipClimbAltAtTca - intruderAltAtTca);

        // Predict ownship altitude at TCA for DESCEND manuvear (-1,500 fpm)
        double ownshipDescendAltAtTca = ownship.PressureAltitudeFeet - (StandardRaVerticalRateFpm * tau);
        double descendSeparation = Math.Abs(ownshipDescendAltAtTca - intruderAltAtTca);

        // Inhibit DESCEND below 1,000 ft AGL to prevent Controlled Flight Into Terrain (CFIT)
        if (ownship.PressureAltitudeFeet <= 1000.0)
        {
            return AdvisorySense.Climb;
        }

        // Maximizing vertical separation at TCA
        return climbSeparation >= descendSeparation ? AdvisorySense.Climb : AdvisorySense.Descend;
    }
}