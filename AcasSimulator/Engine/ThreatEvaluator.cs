namespace AcasSimulator.Engine;

using System;
using System.Globalization;
using AcasSimulator.Models;

public enum ThreatLevel
{
    Clear,
    Proximate,
    TrafficAdvisory,
    ResolutionAdvisory
}

public static class ThreatEvaulator
{
    public static ThreatLevel EvaluateThreat(Aircraft ownship, Aircraft intruder)
    {
        SensitivityParameters sensitivityParameters = SensitivityParameters.GetParametersForAltitude(ownship.PressureAltitudeFeet);
        
        double deltaAltitude = Math.Abs(intruder.PressureAltitudeFeet - ownship.PressureAltitudeFeet);
        double tauV = TauCalculator.CalculateVerticalTau(ownship, intruder);

        // ResolutionAdvisory
        if (sensitivityParameters.SensitivityLevel >= 4)
        {
            double raTauMod = TauCalculator.CalculateModifiedTau(ownship, intruder, sensitivityParameters.RaDmodNM);
            bool isRaHorizontalBreach = raTauMod <= sensitivityParameters.RaTauSeconds;
            bool isRaVerticalBreach = deltaAltitude <= sensitivityParameters.RaZthrFeet || tauV <= sensitivityParameters.RaTauSeconds;

            if (isRaHorizontalBreach && isRaVerticalBreach)
            {
                return ThreatLevel.ResolutionAdvisory;
            }
        }

        // TrafficAdvisory
        double taTauMod = TauCalculator.CalculateModifiedTau(ownship, intruder, sensitivityParameters.RaDmodNM);
        bool isTaHorizontalBreach = taTauMod <= sensitivityParameters.TaTauSeconds;
        bool isTaVerticalBreach = deltaAltitude <= 850.0 || tauV <= sensitivityParameters.TaTauSeconds;

        if (isTaHorizontalBreach && isTaVerticalBreach)
        {
            return ThreatLevel.TrafficAdvisory;
        }

        // Proximate Traffic (Range <= 6.0 NM and Altitude Diff <= 1200 ft)
        double rx = intruder.CartesianX - ownship.CartesianX;
        double ry = intruder.CartesianY - ownship.CartesianY;
        double range = Math.Sqrt(rx * rx + ry * ry);

        if (range <= 6.0 && deltaAltitude <= 1200.0)
        {
            return ThreatLevel.Proximate;
        }

        // Clear Traffic
        return ThreatLevel.Clear;
    }
}