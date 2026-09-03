namespace AcasSimulator.Models;

public class ModeSCoordinationMessage
{
    // Unique ICAO aircraft identifiers
    public string SenderIcaoAddress { get; set; } = string.Empty;   
    public string TargetIcaoAddress { get; set; } = string.Empty;   // Prevent non Target aircrafts from recieving message

    public AdvisorySense ActiveSense { get; set; }      // Resolution direction chosen by the sender
    public bool VerticalControlRestricted { get; set; }     // Signal aircraft that sender aircraft has locked in direction, restrict recievers choice to complement their choice.

    public ModeSCoordinationMessage(string senderIcao, string targetIcao, AdvisorySense activeSense)
    {
        SenderIcaoAddress = senderIcao;
        TargetIcaoAddress = targetIcao;
        ActiveSense = activeSense;
        VerticalControlRestricted = activeSense != AdvisorySense.None;
    }
}