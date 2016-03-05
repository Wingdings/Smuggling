﻿using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct SmugglingResult
{
    public SmugglingGroup group;
    public bool success;
    public double chance;
    public double roll;
    public ClientStats stats;
    public List<string> summary;
}

public class GameManager : MonoBehaviour
{
    public NameGenerator NameGen
    {
        get;
        private set;
    }

    public HintGenerator HintGen
    {
        get;
        private set;
    }

    public ClientGenerator ClientGen
    {
        get;
        private set;
    }

	public float timeBetweenClients = 5;

	public List<Client> clientsWaiting;

    public Client referencedClient;
    public SmugglingGroup referencedGroup;

    public Player player;
	public Country country1;
	public Country country2;

    public List<SmugglingGroup> smugglingGroups = new List<SmugglingGroup>();

    public TextAsset namesAsset;
    public TextAsset hintsAsset;
    public TextAsset peopleAsset;

    public ClientModifier[] modifierList;

    public static System.Random rand = new System.Random();

    public static double CalculateChance(ClientStats stats, int groupSize)
    {
        return (100 - System.Math.Pow(((stats.suspicion + stats.notoriety + stats.sickness + stats.desperation) / groupSize), 2) / 20.0 - (System.Math.Pow(groupSize, 2) / 5.0))/100.0;
    }

    void Start()
    {
        timeBetweenClients = Time.time + 5;
		clientsWaiting = new List<Client> ();
        NameGen = new NameGenerator(namesAsset);
        HintGen = new HintGenerator(hintsAsset);
        ClientGen = new ClientGenerator(peopleAsset, modifierList);

        player = ScriptableObject.CreateInstance<Player>();
		country1 = ScriptableObject.CreateInstance<Country> ();
		country2 = ScriptableObject.CreateInstance<Country> ();

		//setting country stats
		country1.setPopulation (1000000);
		country2.setPopulation (1000000);
		country1.setCountryName ("Country A"); //for now until names are assigned
		country2.setCountryName ("Country B");
		country1.setPercentSick(0);
		country2.setPercentSick (0);

		//setting starting values manually for testing purposes, will move to method
		player.changeMoney (100000);
		player.changeReputation(50);

        //TODO default groups
		SmugglingGroup seaTransportGroup = new SmugglingGroup();
		SmugglingGroup trainTransportGroup = new SmugglingGroup();
		SmugglingGroup airTransportGroup = new SmugglingGroup();
		SmugglingGroup bribeTransportGroup = new SmugglingGroup();
		seaTransportGroup.SetTransportType (TransportType.SEA);
        seaTransportGroup.name = "Sea Transport Group";
		airTransportGroup.SetTransportType (TransportType.AIR);
        airTransportGroup.name = "Air Transport Group";
		bribeTransportGroup.SetTransportType (TransportType.BRIBE);
        bribeTransportGroup.name = "Bribe Transport Group";
		trainTransportGroup.SetTransportType (TransportType.TRAIN);
        trainTransportGroup.name = "Train Transport Group";

		//need to add train, havent implemented yet
        
        smugglingGroups.Add(seaTransportGroup);
        smugglingGroups.Add(airTransportGroup);
        smugglingGroups.Add(bribeTransportGroup);
    }

	void Update(){
		if (Input.GetKeyDown ("g")) {
			clientsWaiting.Add(GenerateNextClient());
		}
		if (Time.time > timeBetweenClients) {
			timeBetweenClients += Random.Range(10,25);
			clientsWaiting.Add(GenerateNextClient());
			//Debug.Log(clientsWaiting.Count);
		}
	}

	public List<Client> GetClientsWaiting(){
		return clientsWaiting;
	}

    public SmugglingResult SimulateGroup(SmugglingGroup group)
    {
        SmugglingResult result = new SmugglingResult();
        ClientStats stats = group.CalculateStats();
        result.chance = CalculateChance(stats, group.clients.Count);
        result.stats = stats;
        result.group = group;
        result.summary = new List<string>();

        result.roll = rand.NextDouble();
        if (result.roll > result.chance)
        {
            foreach (var client in group.clients)
            {
                result.summary.AddRange(client.failureSummaries);
            }

            ChangePlayerStatsFailedRun(result);
            result.success = false;
        }
        else
        {
            foreach (var client in group.clients)
            {
                result.summary.AddRange(client.successSummaries);
            }

            ChangeCountryStatsSucceededRun(group);
            ChangePlayerStatsSucceededRun(result);
            result.success = true;
        }

		player.updateTotalRuns();

        group.clients.Clear();

        return result;
    }

   
    public int GetClientGroup(Client c)
    {
        for (int i = 0; i < smugglingGroups.Count; i++)
        {
            if (smugglingGroups[i].ContainsClient(c))
            {
                return i;
            }
        }
        return -1;
    }

    public Client GenerateNextClient()
    {
        var client = ClientGen.GenerateClient(NameGen, HintGen);
        Debug.LogFormat("Name: {0} {1} ({2})", client.nameData.first, client.nameData.last, client.nameData.gender);
        Debug.LogFormat("Bio: {0}", client.bio);
        Debug.LogFormat("Stats: suspicion={0}, notoriety={1}, sickness={2}, desperation={3}, money={4}, success={5}, fail={6}, deny={7}", client.stats.suspicion, client.stats.notoriety,
            client.stats.sickness, client.stats.desperation, client.stats.money, client.stats.successRep, client.stats.failRep, client.stats.denyRep);
        foreach (var hint in client.hints)
        {
            Debug.LogFormat("Hint: {0}", hint);
        }

        foreach (var text in client.successSummaries)
        {
            Debug.LogFormat("Success: {0}", text);
        }

        foreach (var text in client.failureSummaries)
        {
            Debug.LogFormat("Failure: {0}", text);
        }

        return client;
    }

	public void ChangePlayerStatsFailedRun(SmugglingResult result){
		player.changeMoney(-player.calculateTransportCosts(result.group.GetTransportType(), result.group.clients.Count));
		player.increaseRunsFailed();
        
        player.changeReputation((int)System.Math.Round(-result.stats.failRep));
	}

	public void ChangePlayerStatsSucceededRun(SmugglingResult result){
		//update costs of travel
		player.changeMoney(-player.calculateTransportCosts(result.group.GetTransportType(), result.group.clients.Count));

        player.changeReputation((int)System.Math.Round(result.stats.successRep));
        player.changeMoney((int)System.Math.Round(result.stats.money));
	}

	public void ChangeCountryStatsSucceededRun(SmugglingGroup group)
    {
        foreach (var client in group.clients)
        {
            country1.changePopulation(-1);
            country2.changePopulation(1);
        }
	}
}
