using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct SmugglingResult
{
    public SmugglingGroup group;
    public bool success;
    public double chance;
    public double roll;
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

    public Client referencedClient;

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
        NameGen = new NameGenerator(namesAsset);
        HintGen = new HintGenerator(hintsAsset);
        ClientGen = new ClientGenerator(peopleAsset, modifierList);

        Client testClient = ClientGen.GenerateClient(NameGen, HintGen);
        Debug.LogFormat("Name: {0} {1} ({2})", testClient.nameData.first, testClient.nameData.last, testClient.nameData.gender);
        Debug.LogFormat("Bio: {0}", testClient.bio);
        Debug.LogFormat("Stats: suspicion={0}, notoriety={1}, sickness={2}, desperation={3}", testClient.stats.suspicion, testClient.stats.notoriety, testClient.stats.sickness, testClient.stats.desperation);
        foreach (var hint in testClient.hints)
        {
            Debug.LogFormat("Hint: {0}", hint);
        }
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
                result.success = false;
            }
            else
            {
                result.success = true;
            }

            results[i] = result;
            Debug.Log(string.Format("Group #{0}: {1:F2}% chance, rolled {2:F2}, result is {3}", i, result.chance * 100, result.roll * 100, result.success ? "success" : "failure"));
        }

        return results;
    }

    public Client GenerateNextClient()
    {
        return ClientGen.GenerateClient(NameGen, HintGen);
    }
}
