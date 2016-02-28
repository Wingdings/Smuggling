using UnityEngine;
using System.Collections.Generic;

public enum ClientAttribute
{
    Suspicion,
    Notoriety,
    Sickness,
    Desperation,
    Money,
	TransportTypeNum,
	MinRepRequired
}

[System.Serializable]
public struct ClientStats
{
    public double suspicion;
    public double notoriety;
    public double sickness;
    public double desperation;
    public double money;
	public int minRepRequired;
	public TransportType wantedTransportType;
	public double transportTypeNum;

    public ClientStats(double suspicion = 0.0, double notoriety = 0.0, double sickness = 0.0, double desperation = 0.0, double money = 0.0, TransportType wantedTransportType = TransportType.NONE,int minRepRequired = 0)
    {
        this.suspicion = suspicion;
        this.notoriety = notoriety;
        this.sickness = sickness;
        this.desperation = desperation;
        this.money = money;
		this.wantedTransportType = wantedTransportType;
		this.minRepRequired = minRepRequired;
		transportTypeNum = 0;
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

			case ClientAttribute.TransportTypeNum:
				return transportTypeNum;

			case ClientAttribute.MinRepRequired:
				return minRepRequired;
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

			case ClientAttribute.TransportTypeNum:
				if(value == 0){
					wantedTransportType = TransportType.NONE;
					transportTypeNum = 0;
				}
				else if(value == 1){
					wantedTransportType = TransportType.SEA;
					transportTypeNum = 1;
				}
				else if(value == 2){
					wantedTransportType = TransportType.TRAIN;
					transportTypeNum = 2;	
				}
				else if(value == 3){
					wantedTransportType = TransportType.AIR;
					transportTypeNum = 3;
				}
				else if(value == 4){
					wantedTransportType = TransportType.BRIBE;
					transportTypeNum = 4;
				}
				break;

			case ClientAttribute.MinRepRequired:
				minRepRequired = (int)Mathf.Floor((float)value);
				break;
        }
    }

    public static ClientStats operator +(ClientStats a, ClientStats b)
    {
        return new ClientStats(a.suspicion + b.suspicion, a.notoriety + b.notoriety, a.sickness + b.sickness, a.desperation + b.desperation, a.money + b.money);
    }

    public static ClientStats operator *(ClientStats a, double b)
    {
        return new ClientStats(a.suspicion * b, a.notoriety * b, a.sickness * b, a.desperation * b, a.money);
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

			case "transportTypeNum":
				return ClientAttribute.TransportTypeNum;

			case "minRepRequired":
				return ClientAttribute.MinRepRequired;

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
    public List<string> hints;

    public ClientStats stats;
    public ClientModifier[] modifiers;

	public static Client Create(double suspicion = 0.0, double notoriety = 0.0, double sickness = 0.0, double desperation = 0.0, double money = 0.0, TransportType wantedTransportType = TransportType.NONE,int minRepRequired = 0)
    {
        Client client = ScriptableObject.CreateInstance<Client>();
        client.stats = new ClientStats(suspicion, notoriety, sickness, desperation, money, wantedTransportType, minRepRequired);
        return client;
    }

    public static string ReplaceStringData(Client client, string input)
    {
        string result = input.Replace("%name%", client.nameData.first + " " + client.nameData.last);
        result = result.Replace("%first%", client.nameData.first);
        result = result.Replace("%last%", client.nameData.last);
        result = result.Replace("%destination%", "Gastesal");
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