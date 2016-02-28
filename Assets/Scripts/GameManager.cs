using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct SmugglingResult
{
    public SmugglingGroup group;
    public bool success;
    public double chance;
    public double roll;
    public double money;
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

	public int timeBetweenClients = 5;

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
		clientsWaiting = new List<Client> ();
        NameGen = new NameGenerator(namesAsset);
        HintGen = new HintGenerator(hintsAsset);
        ClientGen = new ClientGenerator(peopleAsset, modifierList);

        Client testClient = ClientGen.GenerateClient(NameGen, HintGen);
        Debug.LogFormat("Name: {0} {1} ({2})", testClient.nameData.first, testClient.nameData.last, testClient.nameData.gender);
        Debug.LogFormat("Bio: {0}", testClient.bio);
        Debug.LogFormat("Stats: suspicion={0}, notoriety={1}, sickness={2}, desperation={3}, money={4}", testClient.stats.suspicion, testClient.stats.notoriety, testClient.stats.sickness, testClient.stats.desperation, testClient.stats.money);
        foreach (var hint in testClient.hints)
        {
            Debug.LogFormat("Hint: {0}", hint);
        }

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
        smugglingGroups.Add(bribeTransportGroup);
        smugglingGroups.Add(seaTransportGroup);
        smugglingGroups.Add(airTransportGroup);
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

    public SmugglingResult SimulateGroup(SmugglingGroup g)
    {
        var group = g;
        SmugglingResult result = new SmugglingResult();
        ClientStats stats = group.CalculateStats();
        result.chance = CalculateChance(stats, group.clients.Count);

        result.roll = rand.NextDouble();
        if (result.roll > result.chance)
        {
            ChangePlayerStatsFailedRun(group);
            result.success = false;
        }
        else
        {
            ChangeCountryStatsSucceededRun(group);
            ChangePlayerStatsSucceededRun(group);
            result.success = true;
        }
		/*
        Debug.Log(country1.getName() + "");
        Debug.Log(country1.getPopulation() + "");
        Debug.Log(country2.getName() + "");
        Debug.Log(country2.getPopulation() + "");
		*/
        result.money = stats.money;

        //results[i] = result;
        //Debug.Log(string.Format("Group #{0}: {1:F2}% chance, rolled {2:F2}, result is {3}", i, result.chance * 100, result.roll * 100, result.success ? "success" : "failure"));

        group.clients.Clear();

        return result;
    }

    public SmugglingResult[] Simulate()
    {
        SmugglingResult[] results = new SmugglingResult[smugglingGroups.Count];
        Debug.Log("Simulating smuggling operation...");

        for (var i = 0; i < smugglingGroups.Count; ++i)
        {
            var group = smugglingGroups[i];
            SmugglingResult result = new SmugglingResult();
            ClientStats stats = group.CalculateStats();
            result.chance = CalculateChance(stats, group.clients.Count);

            result.roll = rand.NextDouble();
            if (result.roll > result.chance)
            {
				ChangePlayerStatsFailedRun(group);
                result.success = false;
            }
            else
            {
				ChangeCountryStatsSucceededRun(group);
				ChangePlayerStatsSucceededRun(group);
                result.success = true;
            }

			/*
			Debug.Log(country1.getName() + "");
			Debug.Log(country1.getPopulation() + "");
			Debug.Log(country2.getName() + "");
			Debug.Log(country2.getPopulation() + "");
			*/
            result.money = stats.money;

            results[i] = result;
            Debug.Log(string.Format("Group #{0}: {1:F2}% chance, rolled {2:F2}, result is {3}", i, result.chance * 100, result.roll * 100, result.success ? "success" : "failure"));

            group.clients.Clear();
        }



        return results;
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
        return ClientGen.GenerateClient(NameGen, HintGen);
    }

	public void ChangePlayerStatsFailedRun(SmugglingGroup group){
		player.changeMoney(-player.calculateTransportCosts(group.GetTransportType(), group.clients.Count));
		player.increaseRunsFailed ();
		for (int i = 0; i < group.clients.Count; i++) {
			Client tempClient = group.clients[i];

			TransportType wantedTransportType = TransportType.NONE;

			if(tempClient.stats.transportTypeNum == 0){
				wantedTransportType = TransportType.NONE;
			}else if(tempClient.stats.transportTypeNum == 1){
				wantedTransportType = TransportType.SEA;
			}else if(tempClient.stats.transportTypeNum == 2){
				wantedTransportType = TransportType.TRAIN;
			}else if(tempClient.stats.transportTypeNum == 3){
				wantedTransportType = TransportType.AIR;
			}else if(tempClient.stats.transportTypeNum == 4){
				wantedTransportType = TransportType.BRIBE;
			}
			/*
			Debug.Log(group.GetTransportType() + "");
			Debug.Log(wantedTransportType + "");
			Debug.Log(-player.calculateTransportCosts(group.GetTransportType(), group.clients.Count) + "");
			*/

			if(group.GetTransportType() != wantedTransportType && wantedTransportType != TransportType.NONE){
				player.changeReputation(-2);
			}else if(wantedTransportType == TransportType.NONE || wantedTransportType == group.GetTransportType()){
				player.changeReputation(-1);
			}
		}
	}

	public void ChangePlayerStatsSucceededRun(SmugglingGroup group){
		//update costs of travel
		player.changeMoney(-player.calculateTransportCosts(group.GetTransportType(), group.clients.Count));

		for (int i = 0; i < group.clients.Count; i++) {
			Client tempClient = group.clients[i];
			
			TransportType wantedTransportType = TransportType.NONE;
			
			if(tempClient.stats.transportTypeNum == 0){
				wantedTransportType = TransportType.NONE;
			}else if(tempClient.stats.transportTypeNum == 1){
				wantedTransportType = TransportType.SEA;
			}else if(tempClient.stats.transportTypeNum == 2){
				wantedTransportType = TransportType.TRAIN;
			}else if(tempClient.stats.transportTypeNum == 3){
				wantedTransportType = TransportType.AIR;
			}else if(tempClient.stats.transportTypeNum == 4){
				wantedTransportType = TransportType.BRIBE;
			}

			/*
			Debug.Log(group.GetTransportType() + "");
			Debug.Log(wantedTransportType + "");
			Debug.Log(-player.calculateTransportCosts(group.GetTransportType(), group.clients.Count) + "");

			*/
			if(group.GetTransportType() != wantedTransportType && wantedTransportType != TransportType.NONE){
				player.changeReputation(+1);
			}else if(wantedTransportType == TransportType.NONE || wantedTransportType == group.GetTransportType()){
				player.changeReputation(+2);
			}

			player.changeMoney((int)tempClient.stats.money);
		}
	}

	public void ChangeCountryStatsSucceededRun(SmugglingGroup group){
		for (int i = 0; i < group.clients.Count; i++) {
			Client tempClient = group.clients [i];

			if(tempClient.stats.countryOrigin == 0){
				country1.changePopulation(-1);
				country2.changePopulation(1);
			}else if(tempClient.stats.countryOrigin == 1){
				country1.changePopulation(1);
				country2.changePopulation(-1);
			}
		}
	}
}
