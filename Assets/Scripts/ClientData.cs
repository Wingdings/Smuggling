using UnityEngine;
using System.Collections.Generic;

public enum ClientAttribute
{
    Suspicion,
    Notoriety,
    Sickness,
    Desperation,
    Money,
    SuccessRep,
    FailRep,
    DenyRep
}

[System.Serializable]
public struct ClientStats
{
    public double suspicion;
    public double notoriety;
    public double sickness;
    public double desperation;
    public double money;
    public double successRep;
    public double failRep;
    public double denyRep;

    public ClientStats(double suspicion = 0.0, double notoriety = 0.0, double sickness = 0.0, double desperation = 0.0, double money = 0.0,
        double successRep = 0.0, double failRep = 0.0, double denyRep = 0.0)
    {
        this.suspicion = suspicion;
        this.notoriety = notoriety;
        this.sickness = sickness;
        this.desperation = desperation;
        this.money = money;
        this.successRep = successRep;
        this.failRep = failRep;
        this.denyRep = denyRep;
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

            case ClientAttribute.Money:
                return money;

            case ClientAttribute.SuccessRep:
                return successRep;

            case ClientAttribute.FailRep:
                return failRep;

            case ClientAttribute.DenyRep:
                return denyRep;
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

            case ClientAttribute.Money:
                money = value;
                break;

            case ClientAttribute.SuccessRep:
                successRep = value;
                break;

            case ClientAttribute.FailRep:
                failRep = value;
                break;

            case ClientAttribute.DenyRep:
                denyRep = value;
                break;
        }
    }

    public static ClientStats operator +(ClientStats a, ClientStats b)
    {
        return new ClientStats(a.suspicion + b.suspicion, a.notoriety + b.notoriety, a.sickness + b.sickness, a.desperation + b.desperation, a.money + b.money,
            a.successRep + b.successRep, a.failRep + b.failRep, a.denyRep + b.denyRep);
    }

    public static ClientStats operator *(ClientStats a, double b)
    {
        return new ClientStats(a.suspicion * b, a.notoriety * b, a.sickness * b, a.desperation * b, a.money, a.successRep, a.failRep, a.denyRep);
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

            case "money":
                return ClientAttribute.Money;

            case "success-rep":
                return ClientAttribute.SuccessRep;

            case "fail-rep":
                return ClientAttribute.FailRep;

            case "deny-rep":
                return ClientAttribute.DenyRep;

            default:
                return null;
        }
    }
}

public abstract class ClientModifier : ScriptableObject
{
    public string modifierId;

    public abstract ClientStats CalculateModifier(SmugglingGroup group, Client client, int clientId);
}

public class Client : ScriptableObject
{
    public NameData nameData;
    public string bio;
    public List<int> portraitIndices = new List<int>();
    public List<string> hints = new List<string>();
    public List<string> successSummaries = new List<string>();
    public List<string> failureSummaries = new List<string>();

    public ClientStats stats;
    public ClientModifier[] modifiers;

	public static Client Create(double suspicion = 0.0, double notoriety = 0.0, double sickness = 0.0, double desperation = 0.0, double money = 0.0,
        double successRep = 0.0, double failRep = 0.0, double denyRep = 0.0)
    {
        Client client = ScriptableObject.CreateInstance<Client>();
        client.stats = new ClientStats(suspicion, notoriety, sickness, desperation, money, successRep, failRep, denyRep);
        return client;
    }

    public static string ReplaceStringData(Client client, string input)
    {
        string result = input.Replace("%name%", client.nameData.first + " " + client.nameData.last);
        result = result.Replace("%first%", client.nameData.first);
        result = result.Replace("%last%", client.nameData.last);
        result = result.Replace("%destination%", "Gastesal");
        result = result.Replace("%money%", System.Math.Round(client.stats.money).ToString());
        return result;
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

public enum TransportType
{
    TRAIN,
	BRIBE,
    AIR,
    SEA,
	NONE
}

public class SmugglingGroup
{
    public List<Client> clients = new List<Client>();
    public double difficulty = 1; // should be close to 1
    public double cost = 3000;
    public TransportType transport = TransportType.NONE;
    public string name;

    public SmugglingGroup()
    {
    }

    public ClientStats CalculateStats()
    {
        ClientStats result = new ClientStats(0, 0, 0, 0);
        for (var i = 0; i < clients.Count; ++i)
        {
            var client = clients[i];
            result = result + client.CalculateStats(this, i);
        }

        return result * difficulty;
    }

    public void AddClient(Client c)
    {
        clients.Add(c);
    }

    public bool ContainsClient(Client c)
    {
        return clients.Contains(c);
    }

    public void SetTransportType(TransportType t)
    {
        transport = t;
    }

	public TransportType GetTransportType()
	{
		return transport;
	}
}