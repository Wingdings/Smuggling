using UnityEngine;
using System.Collections;
using System;

// Use for this modifier is twofold: The first is to give a bonus if another client has a certain modifier
// id on them. The second is to give a bonus if there isn't a client in the group with a certain modifier
// id. Remember that bonuses can be negative!
[CreateAssetMenu(fileName = "MyTransportModifier", menuName = "Smuggling/Modifiers/Transport", order = 3)]
public class TransportModifier : ClientModifier
{
    public string[] hintStrings;

    public TransportType transport = TransportType.LAND;

    [Tooltip("The bonus to stats when without the specified transport.")]
    public ClientStats withBonus;

    [Tooltip("The bonus to stats when with the specified transport.")]
    public ClientStats withoutBonus;

    public override ClientStats CalculateModifier(SmugglingGroup group, Client client, int clientId)
    {
        if (group.transport == transport)
        {
            return withBonus;
        }
        else
        {
            return withoutBonus;
        }
    }
}
