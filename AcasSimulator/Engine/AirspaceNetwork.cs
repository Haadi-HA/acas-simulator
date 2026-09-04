namespace AcasSimulator.Engine;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AcasSimulator.Models;

public class AirspaceNetwork
{
    // Dictionary for fast lookup using ICAO address
    // Concurent to prevent errors
    private readonly ConcurrentDictionary<string, Aircraft> _activeAircraft = new();

    // A queue for sent messages.
    // Ensures messages processed in the order they arrived.
    private readonly ConcurrentQueue<ModeSCoordinationMessage> _messageBuffer = new();

    // Add plane to _activeAircraft Dictionary
    public void RegisterAircraft(Aircraft aircraft)
    {
        _activeAircraft[aircraft.IcaoAddress] = aircraft;
    }

    // Removes a plane from _activeAircraft Dictionary
    public void DeregisterAircraft(string icaoAddress)
    {
        _activeAircraft.TryRemove(icaoAddress, out _);  //Discard variable with out_
    }

    // Puts a sent message into the _messageBuffer Queue
    public void BroadcastMessage(ModeSCoordinationMessage message)
    {
        _messageBuffer.Enqueue(message);
    }

    // Checks the queue, grabs messages meant for a specific plane ID,
    // and puts all other messages back in queue for other planes to read.
    public List<ModeSCoordinationMessage> FetchIncomingMessageFor(string targetIcao)
    {
        List<ModeSCoordinationMessage> targetMessages = new List<ModeSCoordinationMessage>();
        List<ModeSCoordinationMessage> remainingMessages = new List<ModeSCoordinationMessage>();

        // If target aircraft exist, Find the target aircraft to check its location
        if (!_activeAircraft.TryGetValue(targetIcao, out Aircraft? targetAircraft))
        {
            return targetMessages; // Target plane does not exist on network
        }

        // Go through all messages currently in line
        while (_messageBuffer.TryDequeue(out ModeSCoordinationMessage? msg))
        {
            if (msg.TargetIcaoAddress.Equals(targetIcao, StringComparison.OrdinalIgnoreCase))
            {
                // If sender aircraft exist, Find the sender aircraft to check its location
                if (_activeAircraft.TryGetValue(msg.SenderIcaoAddress, out Aircraft? senderAircraft))
                {
                    // Calculate distance between sender and target Aircrafts (NM)
                    double Rx = targetAircraft.CartesianX - senderAircraft.CartesianX;
                    double Ry = targetAircraft.CartesianY - senderAircraft.CartesianY;
                    double Range = Math.Sqrt(Rx * Rx + Ry * Ry);

                    if (Range <= 15)
                    {
                        targetMessages.Add(msg); // Within 15 NM, so message can be received
                    }
                }
            }
            else
            {
                remainingMessages.Add(msg); // Message is for another plane save it, or is from too far away
            }
        }

        // Put unread mesages back to queue
        foreach (ModeSCoordinationMessage remaining in remainingMessages)
        {
            _messageBuffer.Enqueue(remaining);
        }

        return targetMessages;
    }

    // Clears out any left-over messages so old data doesn't spill into the next second.
    public void ClearBuffer()
    {
        _messageBuffer.Clear();
    }
}