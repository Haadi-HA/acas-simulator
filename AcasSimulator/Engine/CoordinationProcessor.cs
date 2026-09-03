namespace AcasSimulator.Engine;

using System;
using AcasSimulator.Models;

public static class CoordinationProcessor
{
    public static AdvisorySense ResolveCoordinatedSense(
        Aircraft receivingOwnship, 
        Aircraft transmittingIntruder, 
        ModeSCoordinationMessage? incomingMessage)  //incoming message is allowed to be a null value
    {
        // Single aircraft calculation - ownships choosing prefered manouver
        AdvisorySense initialSense = RaSenseSelector.SelectSense(receivingOwnship, transmittingIntruder);

        // If no incoming Mode S coordination frame use local sense selection
        if (incomingMessage == null || incomingMessage.ActiveSense == AdvisorySense.None)
        {
            return initialSense;
        }

        // Check if both aircraft chose same sense
        if (initialSense == incomingMessage.ActiveSense)
        {
            // For Tie Breaking compare ICAO addresses identifiers
            int addressComparison = string.Compare(receivingOwnship.IcaoAddress, transmittingIntruder.IcaoAddress, StringComparison.OrdinalIgnoreCase);

            // Lower ICAO address has priority in sense selection
            if (addressComparison > 0) 
            {
                // receivingOwnship has the LOWER ICAO address, swap its desired sense for both senses to be complementary
                return initialSense == AdvisorySense.Climb ? AdvisorySense.Descend : AdvisorySense.Climb;
            }
            // receivingOwnship has the HIGHER address, thefore gets priority in its sense
            return initialSense;
        }

        // Senses are not the same, they are complementary
        return initialSense;
    }
}