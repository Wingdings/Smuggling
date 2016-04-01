using UnityEngine;
using System.Collections;

[System.Serializable]
public struct CountryStats
{
    public double deathRate;
    public double birthRate;
    public double healRate;
    public double sickness; // between 0 and 1
    public double population; // between 0 and 1
    public double chaos; // between 0 and 1

    public CountryStats(double sickness, double population, double chaos)
    {
        deathRate = 0.05;
        birthRate = 0.05;
        healRate = 0.05;

        this.sickness = sickness;
        this.population = population;
        this.chaos = chaos;
    }

    public CountryStats TickStats(double groupSickness, double groupStats, int groupSize)
    {
        double newSickness = sickness;
        double newPopulation = population;
        double newChaos = chaos;

        if (groupSize > 0)
        {
            newSickness += (1 - sickness) * (groupSickness / 100.0);
            newChaos += (1 - chaos) * (groupStats / 1000.0);
        }

        newSickness -= newSickness * healRate;

        newPopulation += newPopulation * birthRate - newPopulation * newSickness * deathRate;

        newSickness = System.Math.Min(1, System.Math.Max(0, newSickness));
        newPopulation = System.Math.Min(1, System.Math.Max(0, newPopulation));
        newChaos = System.Math.Min(1, System.Math.Max(0, newChaos));

        Debug.LogFormat("Tick: groupSickness = {0}, groupStats = {1}, groupSize = {2}", groupSickness, groupStats, groupSize);
        Debug.LogFormat("Tick: sickness = {0}, population = {1}, chaos = {2}, newSickness = {3}, newPopulation = {4}, newChaos = {5}", sickness, population, chaos, newSickness, newPopulation, newChaos);

        var newStats = new CountryStats(newSickness, newPopulation, newChaos);
        Debug.LogFormat("Difficulty before: {0}, after {1}", GetDifficulty(), newStats.GetDifficulty());

        return newStats;
    }

    public double GetDifficulty()
    {
        return 1 + sickness * population * chaos * 5;
    }
}

[CreateAssetMenu(fileName = "Country", menuName = "Smuggling/Country", order = 1)]
public class Country : ScriptableObject
{
    public string countryName;
	public CountryStats stats;
	
	public static Country Create(double sickness = 0.1, double population = 0.5, double chaos = 0.0)
	{
		Country country = ScriptableObject.CreateInstance<Country>();
		country.stats = new CountryStats(sickness, population, chaos);
		return country;
	}

    public void TickCountry(SmugglingResult? lastResult)
    {
        if (!lastResult.HasValue)
            stats = stats.TickStats(0, 0, 0);
        else
            stats = stats.TickStats(lastResult.Value.stats.sickness,
                lastResult.Value.stats.desperation + lastResult.Value.stats.notoriety + lastResult.Value.stats.suspicion,
                lastResult.Value.group.clients.Count);
    }
}