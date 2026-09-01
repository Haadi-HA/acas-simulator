using System.Security.AccessControl;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using SkiaSharp;

public class Aircraft
{
    //Attribiutes
    private string _callSign;

    //Position
    private double _cartisieanX, _cartisieanY;
    private double _preasureAltitudeFeet;

    //Kinematics
    private double _groundSpeedKnots;
    private double _headingDegrees;
    private double _verticalClimbRateFpm;

    //State
    private string _tcasState;


    //Methods
    //Constructor
    public Aircraft(string callSign, double cartisieanX, double cartisieanY,double preasureAltitudeFeet, double groundSpeedKnots, double headingDegrees, double verticalClimbRateFpm)
    {
        _callSign = callSign;
        _cartisieanX = cartisieanX;
        _cartisieanY = cartisieanY;
        _preasureAltitudeFeet = preasureAltitudeFeet;
        _groundSpeedKnots = groundSpeedKnots;
        _headingDegrees = headingDegrees;
        _verticalClimbRateFpm = verticalClimbRateFpm;
    }

    //Getters & Setters
    public string getCallSign() { return _callSign;}
    public void setCallSign(string callSign) { _callSign = callSign;}

    //Position
    public double getCartisieanX() { return _cartisieanX;}
    public void setCartisieanX(double cartisieanX) { _cartisieanX = cartisieanX;}

    public double getCartisieanY() { return _cartisieanY;}
    public void setCartisieanY(double cartisieanY) { _cartisieanY = cartisieanY;}

    public double getPreasureAltitudeFeet() { return _preasureAltitudeFeet;}
    public void setPreasureAltitudeFeet(double preasureAltitudeFeet) { _preasureAltitudeFeet = preasureAltitudeFeet;}

    //Kinematics
    public double getGroundSpeedKnots() { return _groundSpeedKnots;}
    public void setGroundSpeedKnots(double groundSpeedKnots) { _groundSpeedKnots = groundSpeedKnots;}

    public double getHeadingDegrees() { return _headingDegrees;}
    public void setHeadingDegrees(double headingDegrees) { _headingDegrees = headingDegrees;}
    
    public double getVerticalClimbRateFpm() { return _verticalClimbRateFpm;}
    public void setVerticalClimbRateFpm(double verticalClimbRateFpm) { _verticalClimbRateFpm = verticalClimbRateFpm;}
}   