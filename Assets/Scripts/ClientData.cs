using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ClientStats
{
    public double suspicion;
    public double noteriety;
    public double sickness;
    public double desperation;

    public ClientStats(double suspicion = 0.0, double noteriety = 0.0, double sickness = 0.0, double desperation = 0.0)
    {
        this.suspicion = suspicion;
        this.noteriety = noteriety;
        this.sickness = sickness;
        this.desperation = desperation;
    }

    public static ClientStats operator +(ClientStats a, ClientStats b)
    {
        return new ClientStats(a.suspicion + b.suspicion, a.noteriety + b.noteriety, a.sickness + b.sickness, a.desperation + b.desperation);
    }
}

public abstract class ClientModifier : ScriptableObject
{
    public string modifierId;

    public abstract ClientStats CalculateModifier(SmugglingGroup group, Client client, int clientId);
    public abstract string[] GetHintStrings();
}

[CreateAssetMenu(fileName = "MyClient", menuName = "Smuggling/Client", order = 1)]
public class Client : ScriptableObject
{
    public ClientStats stats;
    public ClientModifier[] modifiers;

    public static Client Create(double suspicion = 0.0, double noteriety = 0.0, double sickness = 0.0, double desperation = 0.0)
    {
        Client client = ScriptableObject.CreateInstance<Client>();
        client.stats = new ClientStats(suspicion, noteriety, sickness, desperation);
        return client;
    }

    public ClientStats CalculateStats(SmugglingGroup group, int clientId)
    {
        ClientStats result = stats;
        if (modifiers != null)
        {
            foreach (var modifier in modifiers)
            {
                result = result + modifier.CalculateModifier(group, this, clientId);
            }
        }

        return result;
    }
}

[CreateAssetMenu(fileName = "MySmugglingGroup", menuName = "Smuggling/Group", order = 2)]
public class SmugglingGroup : ScriptableObject
{
    public List<Client> clients = new List<Client>();

    //TODO: Some representation of destination

    public ClientStats CalculateStats()
    {
        ClientStats result = new ClientStats(0, 0, 0, 0);
        for (var i = 0; i < clients.Count; ++i)
        {
            var client = clients[i];
            result = result + client.CalculateStats(this, i);
        }

        return result;
    }
}