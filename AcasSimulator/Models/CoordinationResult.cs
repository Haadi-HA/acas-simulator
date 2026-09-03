namespace AcasSimulator.Models;

public record CoordinationResult
(
    AdvisorySense ResolvedSense, 
    ModeSCoordinationMessage OutgoingMessage
);