namespace AcasSimulator.Engine;

using System;
using AcasSimulator.Models;

public static class TauCalculator
{
    // Hold caluclated values needed for other methods
    // Stored seperately to follow DRY principles.
    private readonly struct RelativeKinematics
    {
        public readonly double Rx;
        public readonly double Ry;
        public readonly double Range;
        public readonly double Rvx;
        public readonly double Rvy;
        public readonly double DotProduct;
        public readonly double RelSpeedSquared;

        public RelativeKinematics(Aircraft ownship, Aircraft intruder)
        {
            // Relative Position Vector components (NM)
            Rx = intruder.CartesianX - ownship.CartesianX;
            Ry = intruder.CartesianY - ownship.CartesianY;
            Range = Math.Sqrt(Rx * Rx + Ry * Ry);

            // Velocity vectors (NM/s)
            double ownRad = ownship.HeadingDegrees * (Math.PI / 180.0);
            double ownVx = (ownship.GroundSpeedKnots / 3600.0) * Math.Sin(ownRad);
            double ownVy = (ownship.GroundSpeedKnots / 3600.0) * Math.Cos(ownRad);

            double intRad = intruder.HeadingDegrees * (Math.PI / 180.0);
            double intVx = (intruder.GroundSpeedKnots / 3600.0) * Math.Sin(intRad);
            double intVy = (intruder.GroundSpeedKnots / 3600.0) * Math.Cos(intRad);

            // Relative velocity vector (NM/s) how fast intruder is intruder is closing in relative
            Rvx = intVx - ownVx;
            Rvy = intVy - ownVy;

            // Dot product and squared relative speed
            DotProduct = (Rx * Rvx) + (Ry * Rvy);
            RelSpeedSquared = (Rvx * Rvx) + (Rvy * Rvy);
        }
    }

    // 2D Horizontal tau in seconds between two aircraft
    public static double CalculateHorizontalTau(Aircraft ownship, Aircraft intruder)
    {
        RelativeKinematics k = new RelativeKinematics(ownship, intruder);

        // Divergence & Stationary Check
        // If dotProduct >= 0, aircraft are parallel or diverging.
        // If relSpeedSquared < 1e-9, stationary.
        if (k.DotProduct >= 0 || k.RelSpeedSquared < 1e-9)
        {
            return double.PositiveInfinity;
        }

        // Calculate Tau
        return -k.DotProduct / k.RelSpeedSquared;
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

    
    public static double CalculateModifiedTau(Aircraft ownship, Aircraft intruder, double dmodNM)
    {

    }
}