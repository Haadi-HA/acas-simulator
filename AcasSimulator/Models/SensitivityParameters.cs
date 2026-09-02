namespace AcasSimulator.Models;

public class SensitivityParameters
{
    public int SensitivityLevel { get; set; }
    public double TaTauSeconds { get; set; }
    public double TaDmodNM { get; set; }
    public double RaTauSeconds { get; set; }
    public double RaDmodNM { get; set; }
    public double RaZthrFeet { get; set; }

    public static SensitivityParameters GetParametersForAltitude(double altitudeFeet)
    {
        // Standard TCAS II Sensitivity Levels
        if (altitudeFeet > 20000.0)
            return new SensitivityParameters { SensitivityLevel = 7, TaTauSeconds = 48.0, TaDmodNM = 1.30, RaTauSeconds = 35.0, RaDmodNM = 1.10, RaZthrFeet = 600.0 };
        if (altitudeFeet > 10000.0)
            return new SensitivityParameters { SensitivityLevel = 6, TaTauSeconds = 30.0, TaDmodNM = 1.00, RaTauSeconds = 25.0, RaDmodNM = 0.75, RaZthrFeet = 400.0 };
        if (altitudeFeet > 5000.0)
            return new SensitivityParameters { SensitivityLevel = 5, TaTauSeconds = 25.0, TaDmodNM = 0.75, RaTauSeconds = 20.0, RaDmodNM = 0.55, RaZthrFeet = 350.0 };
        if (altitudeFeet > 2350.0)
            return new SensitivityParameters { SensitivityLevel = 4, TaTauSeconds = 20.0, TaDmodNM = 0.48, RaTauSeconds = 15.0, RaDmodNM = 0.35, RaZthrFeet = 300.0 };

        // Below 2,350 ft TA only, no RAs
        return new SensitivityParameters { SensitivityLevel = 3, TaTauSeconds = 15.0, TaDmodNM = 0.30, RaTauSeconds = 0.0, RaDmodNM = 0.0, RaZthrFeet = 0.0 };
    }
}