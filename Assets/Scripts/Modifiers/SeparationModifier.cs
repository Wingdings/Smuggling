using UnityEngine;
using System.Collections;
using System;

// Use for this modifier is twofold: The first is to give a bonus if another client has a certain modifier
// id on them. The second is to give a bonus if there isn't a client in the group with a certain modifier
// id. Remember that bonuses can be negative!
[CreateAssetMenu(fileName = "MySeparationModifier", menuName = "Smuggling/Modifiers/Separation", order = 2)]
public class SeparationModifier : ClientModifier
{
    public string[] hintStrings;

    [Tooltip("The modifier id to search for. This will exclude this modifier.")]
    public string searchForId;

    [Tooltip("The bonus to stats when no other client in the group has the modifier id.")]
    public ClientStats separationBonus;

    [Tooltip("The bonus to stats when another client in the group has the modifier id.")]
    public ClientStats togetherBonus;

    public override ClientStats CalculateModifier(SmugglingGroup group, Client client, int clientId)
    {
        if (string.IsNullOrEmpty(searchForId))
        {
            Debug.LogWarning("Skipping SeparationModifier as searchForId is unset.");
            return new ClientStats();
        }

        for (var i = 0; i < group.clients.Count; ++i)
        {
            if (i == clientId) // skip self, we have to use indices because you can have multiple of the same client
                continue;

            var other = group.clients[i];

            foreach (var modifier in other.modifiers)
            {
                if (string.Equals(searchForId, modifier.modifierId))
                {
                    return togetherBonus;
                }
            }
        }

        return separationBonus;
    }

    public override string[] GetHintStrings()
    {
        return hintStrings;
    }
}
