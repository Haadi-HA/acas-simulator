namespace AcasSimulator.Engine;

using System;
using AcasSimulator.Models;

public static class CoordinationProcessor
{
    public static CoordinationResult ResolveCoordinatedSense(
        Aircraft receivingOwnship, 
        Aircraft transmittingIntruder, 
        ModeSCoordinationMessage? incomingMessage)  //incoming message is allowed to be a null value
    {
        // Single aircraft calculation - ownships choosing prefered manouver
        AdvisorySense preferredSense = RaSenseSelector.SelectSense(receivingOwnship, transmittingIntruder);
        AdvisorySense finalSense = preferredSense;

        // If the transmitting aircraft is in a Active Vertical controll restriction
        if (incomingMessage != null && incomingMessage.VerticalControlRestricted)
        {
            // Check if both aircraft chose same sense
            if (preferredSense == incomingMessage.ActiveSense)
            {
                // For Tie Breaking compare ICAO addresses identifiers
                int addressComparison = string.Compare(receivingOwnship.IcaoAddress, transmittingIntruder.IcaoAddress, StringComparison.OrdinalIgnoreCase);

                // Higher ICAO address is less priority, therefore yield and invert from prefered sense
                if (addressComparison > 0) 
                {
                    finalSense = preferredSense == AdvisorySense.Climb ? AdvisorySense.Descend : AdvisorySense.Climb;
                }
                // Lower ICAO address skips invert, maintains prefered sense
            }
        } 

        // Make message for target aircraft
        ModeSCoordinationMessage outgoingMessage = new ModeSCoordinationMessage(
            senderIcao: receivingOwnship.IcaoAddress,
            targetIcao: transmittingIntruder.IcaoAddress,
            activeSense: finalSense
        );

        // Return the packaged CoordinationResult
        return new CoordinationResult(finalSense, outgoingMessage);   
    }
}