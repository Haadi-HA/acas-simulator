namespace AcasSimulator.Tests;

using Xunit;
using AcasSimulator.Engine;
using AcasSimulator.Models;

public class CoordinationProcessorTests
{
    [Fact]
    public void  CoordinationProcessorn_NoIncomingMessage_ReturnsLocalPreferenceAndBroadcastsIt()
    {
        // Ownship at 30,000 ft, Intruder below at 29,700 ft. Prefered sense is Climb.

        // ARRANGE:
        Aircraft ownship = new Aircraft("OWN1", "400001", 0.0, 0.0, 30000.0, 360.0, 90.0, 0.0);
        Aircraft intruder = new Aircraft("INT1", "400002", 3.0, 0.0, 29700.0, 360.0, 270.0, 0.0);

        // ACT:
        CoordinationResult result = CoordinationProcessor.ResolveCoordinatedSense(ownship, intruder, null); // No Mode S message received from intruder

        // Should stick to Prefered sense (Climb) and broadcast it outbound

        // ASSERT:
        Assert.Equal(AdvisorySense.Climb, result.ResolvedSense);
        Assert.Equal(AdvisorySense.Climb, result.OutgoingMessage.ActiveSense);
        Assert.Equal("400001", result.OutgoingMessage.SenderIcaoAddress);
        Assert.Equal("400002", result.OutgoingMessage.TargetIcaoAddress);
    }

    [Fact]
    public void  CoordinationProcessor_AlreadyComplementary_MaintainsLocalSense()
    {
        // Ownship prefers Climb. Intruder transmits Descend.

        // ARRANGE:
        Aircraft ownship = new Aircraft("OWN1", "400001", 0.0, 0.0, 30000.0, 360.0, 90.0, 0.0);
        Aircraft intruder = new Aircraft("INT1", "400002", 3.0, 0.0, 29700.0, 360.0, 270.0, 0.0);
        ModeSCoordinationMessage incomingMsg = new ModeSCoordinationMessage("400002", "400001", AdvisorySense.Descend);

        // ACT:
        CoordinationResult result = CoordinationProcessor.ResolveCoordinatedSense(ownship, intruder, incomingMsg);

        // Senses do not conflict, should keep Climb

        // ASSERT:
        Assert.Equal(AdvisorySense.Climb, result.ResolvedSense);
        Assert.Equal(AdvisorySense.Climb, result.OutgoingMessage.ActiveSense);
    }

    [Fact]
    public void  CoordinationProcessor_TieBreak_HigherIcaoAddressYieldsAndInvertsSense()
    {
        // Both prefer Climb. Ownship ("400002") has a HIGHER ICAO address than Intruder ("400001")

        // ARRANGE:
        Aircraft ownship = new Aircraft("OWN1", "400002", 0.0, 0.0, 30000.0, 360.0, 90.0, 0.0);
        Aircraft intruder = new Aircraft("INT1", "400001", 3.0, 0.0, 29700.0, 360.0, 270.0, 0.0);
        ModeSCoordinationMessage incomingMsg = new ModeSCoordinationMessage("400001", "400002", AdvisorySense.Climb);

        // ACT:
        CoordinationResult result = CoordinationProcessor.ResolveCoordinatedSense(ownship, intruder, incomingMsg);

        // Higher ICAO address must yield and invert choice to Descend

        // ASSERT: 
        Assert.Equal(AdvisorySense.Descend, result.ResolvedSense);
        Assert.Equal(AdvisorySense.Descend, result.OutgoingMessage.ActiveSense);
        Assert.Equal("400002", result.OutgoingMessage.SenderIcaoAddress);
    }

    [Fact]
    public void  CoordinationProcessor_TieBreak_LowerIcaoAddressRetainsSense()
    {
        // Both prefer Climb. Ownship ("400001") has a LOWER ICAO address than Intruder ("400002")

        // ARRANGE:
        Aircraft ownship = new Aircraft("OWN1", "400001", 0.0, 0.0, 30000.0, 360.0, 90.0, 0.0);
        Aircraft intruder = new Aircraft("INT1", "400002", 3.0, 0.0, 29700.0, 360.0, 270.0, 0.0);
        ModeSCoordinationMessage incomingMsg = new ModeSCoordinationMessage("400002", "400001", AdvisorySense.Climb);

        // ACT:
        CoordinationResult result = CoordinationProcessor.ResolveCoordinatedSense(ownship, intruder, incomingMsg);

        // Lower ICAO address wins tie-break and keeps Climb

        // ASSERT:
        Assert.Equal(AdvisorySense.Climb, result.ResolvedSense);
        Assert.Equal(AdvisorySense.Climb, result.OutgoingMessage.ActiveSense);
        Assert.Equal("400001", result.OutgoingMessage.SenderIcaoAddress);
    }
}