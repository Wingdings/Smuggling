using UnityEngine;
using System.Collections.Generic;

public enum ClientAttribute
{
    Suspicion,
    Notoriety,
    Sickness,
    Desperation
}

[System.Serializable]
public struct ClientStats
{
    public double suspicion;
    public double notoriety;
    public double sickness;
    public double desperation;

    public ClientStats(double suspicion = 0.0, double notoriety = 0.0, double sickness = 0.0, double desperation = 0.0)
    {
        this.suspicion = suspicion;
        this.notoriety = notoriety;
        this.sickness = sickness;
        this.desperation = desperation;
    }

    public double GetAttribute(ClientAttribute attr)
    {
        switch (attr)
        {
            case ClientAttribute.Suspicion:
                return suspicion;

            case ClientAttribute.Notoriety:
                return notoriety;

            case ClientAttribute.Sickness:
                return sickness;

            case ClientAttribute.Desperation:
                return desperation;
        }

        return double.MinValue;
    }

    public void SetAttribute(ClientAttribute attr, double value)
    {
        switch (attr)
        {
            case ClientAttribute.Suspicion:
                suspicion = value;
                break;

            case ClientAttribute.Notoriety:
                notoriety = value;
                break;

            case ClientAttribute.Sickness:
                sickness = value;
                break;

            case ClientAttribute.Desperation:
                desperation = value;
                break;
        }
    }

    public static ClientStats operator +(ClientStats a, ClientStats b)
    {
        return new ClientStats(a.suspicion + b.suspicion, a.notoriety + b.notoriety, a.sickness + b.sickness, a.desperation + b.desperation);
    }

    public static ClientAttribute? StringToAttribute(string name)
    {
        switch (name.ToLower())
        {
            case "suspicion":
                return ClientAttribute.Suspicion;

            case "notoriety":
                return ClientAttribute.Notoriety;

            case "sickness":
                return ClientAttribute.Sickness;

            case "desperation":
                return ClientAttribute.Desperation;

            default:
                return null;
        }
    }
}

public abstract class ClientModifier : ScriptableObject
{
    public string modifierId;

    public abstract ClientStats CalculateModifier(SmugglingGroup group, Client client, int clientId);
    public abstract string[] GetHintStrings();
}

public class Client : ScriptableObject
{
    public NameData nameData;
    public string bio;
    public List<string> hints;

    public ClientStats stats;
    public ClientModifier[] modifiers;

    public static Client Create(double suspicion = 0.0, double notoriety = 0.0, double sickness = 0.0, double desperation = 0.0)
    {
        Client client = ScriptableObject.CreateInstance<Client>();
        client.stats = new ClientStats(suspicion, notoriety, sickness, desperation);
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