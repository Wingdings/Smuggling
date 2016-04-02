using UnityEngine;
using System.Collections;

public enum CountryAttribute
{
    Sickness,
    Population,
    Chaos,
    DeltaSickness,
    DeltaPopulation,
    DeltaChaos
}

[System.Serializable]
public struct CountryStats
{
    public double deathRate;
    public double birthRate;
    public double healRate;
    public double sickness; // between 0 and 1
    public double population; // between 0 and 1
    public double chaos; // between 0 and 1

    public double deltaSickness;
    public double deltaPopulation;
    public double deltaChaos;

    public CountryStats(double sickness, double population, double chaos)
    {
        deathRate = 0.05;
        birthRate = 0.09;
        healRate = 0.05;

        this.sickness = sickness;
        this.population = population;
        this.chaos = chaos;

        this.deltaSickness = 0;
        this.deltaPopulation = 0;
        this.deltaChaos = 0;
    }

    public static CountryAttribute? StringToAttribute(string attr)
    {
        switch (attr.ToLower())
        {
            case "sickness":
                return CountryAttribute.Sickness;

            case "population":
                return CountryAttribute.Population;

            case "chaos":
                return CountryAttribute.Chaos;

            case "delta-sickness":
                return CountryAttribute.DeltaSickness;

            case "delta-population":
                return CountryAttribute.DeltaPopulation;

            case "delta-chaos":
                return CountryAttribute.DeltaChaos;
        }

        return null;
    }

    public double GetAttribute(CountryAttribute attr)
    {
        switch (attr)
        {
            case CountryAttribute.Sickness:
                return sickness;

            case CountryAttribute.Population:
                return population;

            case CountryAttribute.Chaos:
                return chaos;

            case CountryAttribute.DeltaSickness:
                return deltaSickness;

            case CountryAttribute.DeltaPopulation:
                return deltaPopulation;

            case CountryAttribute.DeltaChaos:
                return deltaChaos;
        }

        return double.MinValue;
    }

    public void SetAttribute(CountryAttribute attr, double value)
    {
        switch (attr)
        {
            case CountryAttribute.Sickness:
                sickness = value;
                break;

            case CountryAttribute.Population:
                population = value;
                break;

            case CountryAttribute.Chaos:
                chaos = value;
                break;

            case CountryAttribute.DeltaSickness:
                deltaSickness = value;
                break;

            case CountryAttribute.DeltaPopulation:
                deltaPopulation = value;
                break;

            case CountryAttribute.DeltaChaos:
                deltaChaos = value;
                break;
        }
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

        double clampSickness = System.Math.Min(1, System.Math.Max(0, newSickness));
        double clampPopulation = System.Math.Min(1, System.Math.Max(0, newPopulation));
        double clampChaos = System.Math.Min(1, System.Math.Max(0, newChaos));

        Debug.LogFormat("Tick: groupSickness = {0}, groupStats = {1}, groupSize = {2}", groupSickness, groupStats, groupSize);
        Debug.LogFormat("Tick: sickness = {0}, population = {1}, chaos = {2}, newSickness = {3}, newPopulation = {4}, newChaos = {5}", sickness, population, chaos, clampSickness, clampPopulation, clampChaos);

        var newStats = new CountryStats(clampSickness, clampPopulation, clampChaos);
        Debug.LogFormat("Difficulty before: {0}, after {1}", GetDifficulty(), newStats.GetDifficulty());

        newStats.deltaSickness = newSickness - sickness;
        newStats.deltaPopulation = newPopulation - population;
        newStats.deltaChaos = newChaos - chaos;

        Debug.LogFormat("Tick delta: sickness = {0}, population = {1}, chaos = {2}", newStats.deltaSickness, newStats.deltaPopulation, newStats.deltaChaos);

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
	
	public static Country Create(double sickness = 0.1, double population = 0.4, double chaos = 0.0)
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

    public string ReplaceStringData(string input)
    {
        string result = input.Replace("%country%", countryName);
        return result;
    }
}