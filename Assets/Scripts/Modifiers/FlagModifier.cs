using UnityEngine;
using System.Collections;
using System;

// A simple flag, doesn't do anything on its own.
[CreateAssetMenu(fileName = "MyFlagModifier", menuName = "Smuggling/Modifiers/Flag", order = 1)]
public class FlagModifier : ClientModifier
{
    public override ClientStats CalculateModifier(SmugglingGroup group, Client client, int clientId)
    {
        return new ClientStats();
    }
}
