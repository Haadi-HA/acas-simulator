using System;

namespace AcasSimulator.Models;

public enum TcasState
{
    Clear,
    Proximate,
    TrafficAdvisory,
    ResolutionAdvisory
}

public class Aircraft
{
    // Identification
    public string CallSign { get; set; }

    // Position (Cartesian coordinates in Nautical Miles relative to Manchester airport EGCC)
    public double CartesianX { get; set; }
    public double CartesianY { get; set; }
    public double PressureAltitudeFeet { get; set; }

    // Kinematics
    public double GroundSpeedKnots { get; set; }
    public double HeadingDegrees { get; set; }
    public double VerticalSpeedFpm { get; set; }

    // Threat State (default to the Clear TcasState untill threatengine updates)
    public TcasState State { get; set; } = TcasState.Clear;

    // Constructor
    public Aircraft(
        string callSign, 
        double cartesianX, 
        double cartesianY, 
        double pressureAltitudeFeet, 
        double groundSpeedKnots, 
        double headingDegrees, 
        double verticalSpeedFpm)
    {
        CallSign = callSign;
        CartesianX = cartesianX;
        CartesianY = cartesianY;
        PressureAltitudeFeet = pressureAltitudeFeet;
        GroundSpeedKnots = groundSpeedKnots;
        HeadingDegrees = headingDegrees;
        VerticalSpeedFpm = verticalSpeedFpm;
    }

    //Position Update using kinematics attribiutes
    public void UpdatePosition(double deltaTimeSeconds)
    {
        //Convert heading to radians, c# Maths functions accept radians
        double headingRad = (90- HeadingDegrees) * (Math.PI /180);
        //Groundspeed in nautiucal miles per second, 1 knot is 1 Nm/h
        double speedNmPerSec = GroundSpeedKnots / 3600.0;

        //Position displacement
        CartesianX += speedNmPerSec * Math.Cos(headingRad) * deltaTimeSeconds;
        CartesianY += speedNmPerSec * Math.Sin(headingRad) * deltaTimeSeconds;

        // Altitude displacement (Feet per second)
        PressureAltitudeFeet += (VerticalSpeedFpm / 60.0) * deltaTimeSeconds;
    }

}