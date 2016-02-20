﻿using UnityEngine;
using System.Collections.Generic;

public class ClientStats : ScriptableObject
{
    public double suspicion = 0.0;
    public double noteriety = 0.0;
    public double sickness = 0.0;
    public double desparation = 0.0;

    public static ClientStats Create(double suspicion = 0.0, double noteriety = 0.0, double sickness = 0.0, double desparation = 0.0)
    {
        ClientStats stats = ScriptableObject.CreateInstance<ClientStats>();
        stats.suspicion = suspicion;
        stats.noteriety = noteriety;
        stats.sickness = sickness;
        stats.desparation = desparation;
        return stats;
    }

    public static ClientStats operator +(ClientStats a, ClientStats b)
    {
        return Create(a.suspicion + b.suspicion, a.noteriety + b.noteriety, a.sickness + b.sickness, a.desparation + b.desparation);
    }
}

public abstract class ClientModifier : ScriptableObject
{
    public abstract ClientStats CalculateModifier(SmugglingGroup group, Client client);
}

[CreateAssetMenu(fileName = "client", menuName = "Game/Client", order = 1)]
public class Client : ScriptableObject
{
    public ClientStats stats;
    public ClientModifier[] modifiers;

    public static Client Create(double suspicion = 0.0, double noteriety = 0.0, double sickness = 0.0, double desparation = 0.0)
    {
        Client client = ScriptableObject.CreateInstance<Client>();
        client.stats = ClientStats.Create(suspicion, noteriety, sickness, desparation);
        return client;
    }

    public ClientStats CalculateStats(SmugglingGroup group)
    {
        ClientStats result = stats;
        if (modifiers != null)
        {
            foreach (var modifier in modifiers)
            {
                result = result + modifier.CalculateModifier(group, this);
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
        ClientStats result = ClientStats.Create(0, 0, 0, 0);
        foreach (var client in clients)
        {
            result = result + client.CalculateStats(this);
        }

        return result;
    }
}