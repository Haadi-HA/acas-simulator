namespace AcasSimulator.Engine;


using System;
using AcasSimulator.Models;

public static class TauCalculator
{
    // 2D Horizontal tau in seconds between two aircraft
    public static double CalculateHorizontalTau(Aircraft ownship, Aircraft intruder)
    {
        // Relative Position Vector components (NM)
        double rx = intruder.CartesianX - ownship.CartesianX;
        double ry = intruder.CartesianY - ownship.CartesianY;

        // Velocity vectors (NM/s)
        double ownHeadingRad = ownship.HeadingDegrees * (Math.PI / 180.0);
        double ownVx = (ownship.GroundSpeedKnots / 3600.0) * Math.Sin(ownHeadingRad);
        double ownVy = (ownship.GroundSpeedKnots / 3600.0) * Math.Cos(ownHeadingRad);

        double intHeadingRad = intruder.HeadingDegrees * (Math.PI / 180.0);
        double intVx = (intruder.GroundSpeedKnots / 3600.0) * Math.Sin(intHeadingRad);
        double intVy = (intruder.GroundSpeedKnots / 3600.0) * Math.Cos(intHeadingRad);

        // Relative velocity vector (NM/s) how fast intruder is intruder is closing in relative
        double rvx = intVx - ownVx;
        double rvy = intVy - ownVy;

        // Dot product and squared relative speed
        double dotProduct = (rx * rvx) + (ry * rvy);
        double relSpeedSquared = (rvx * rvx) + (rvy * rvy);

        // Divergence & Stationary Check
        // If dotProduct >= 0, aircraft are parallel or diverging.
        // If relSpeedSquared < 1e-9, stationary.
        if (dotProduct >= 0 || relSpeedSquared < 1e-9)
        {
            return double.PositiveInfinity;
        }

        // Calculate Tau
        return -dotProduct / relSpeedSquared;
    }

    // 2D Vertical in seconds between two aircraft
    public static double CalculateVerticalTau(Aircraft ownship, Aircraft intruder)
    {
        //Altitude difference (feet)
        double deltaAltitude = intruder.PressureAltitudeFeet - ownship.PressureAltitudeFeet;

        // Relative vertical closure rate (ft/s)
        // Postive rVCR if converging, negative if diverging
        double relativeVerticalClosureRate = (deltaAltitude > 0) ? ownship.VerticalSpeedFpm - intruder.VerticalSpeedFpm : intruder.VerticalSpeedFpm - ownship.VerticalSpeedFpm;
        
        // if aircrafts are level or moving appart vertically
        // using 1e-5 as epsilon threshold to avoid floating point rounding errors
        if (Math.Abs(deltaAltitude) < 1e-5 || relativeVerticalClosureRate <= 0)
        {
            return double.PositiveInfinity;
        }

        return Math.Abs(deltaAltitude) / relativeVerticalClosureRate;
    }
}