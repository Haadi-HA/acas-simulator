namespace AcasSimulator.Engine;


using System;
using AcasSimulator.Models;

// Calculates Horizontal/Slant Range Tau (seconds) between two aircraft
public static class TauCalculator
{
    public static double CalculateSlantRangeTau(Aircraft ownship, Aircraft intruder)
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
}