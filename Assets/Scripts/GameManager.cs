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

public class GameManager : MonoBehaviour {

    public List<SmugglingGroup> smugglingGroups = new List<SmugglingGroup>();
    public List<Client> clientQueue = new List<Client>();

    public bool useRandomClients = false;
    public bool doSimulationTest = false;

    public System.Random rand = new System.Random();

    public static double CalculateChance(ClientStats stats, int groupSize)
    {
        return (100 - System.Math.Pow(((stats.suspicion + stats.noteriety + stats.sickness + stats.desperation) / groupSize), 2) / 20.0 - (System.Math.Pow(groupSize, 2) / 5.0))/100.0;
    }

    void Start()
    {
        if (doSimulationTest)
            Simulate();
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

    public Client GetNextClient()
    {
        if (useRandomClients)
        {
            throw new System.NotImplementedException("Random client generation is not yet implemented!");
        }

        if (clientQueue.Count == 0)
            return null;

        Client next = clientQueue[0];
        clientQueue.RemoveAt(0);
        return next;
    }
}
