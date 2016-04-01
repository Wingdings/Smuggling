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

    public CountryStats(double sickness, double population)
    {
        deathRate = 0.1;
        birthRate = 0.1;
        healRate = 0.05;

        this.sickness = sickness;
        this.population = population;
    }

    public CountryStats TickStats(double groupSickness, int groupSize)
    {
        double newSickness = sickness;
        double newPopulation = population;

        if (groupSize > 0)
        {
            newSickness += (1 - sickness) * (groupSickness / (groupSize * 10));

        }

        newSickness -= newSickness * healRate;

        newPopulation += newPopulation * birthRate - newPopulation * newSickness * deathRate;

        Debug.LogFormat("Tick: groupSickness = {0}, groupSize = {1}", groupSickness, groupSize);
        Debug.LogFormat("Tick: sickness = {0}, population = {1}, newSickness = {2}, newPopulation = {3}", sickness, population, newSickness, newPopulation);

        return new CountryStats(newSickness, newPopulation);
    }

    public double GetDifficulty()
    {
        return 1 + sickness * population * 2.5;
    }
}

[CreateAssetMenu(fileName = "Country", menuName = "Smuggling/Country", order = 1)]
public class Country : ScriptableObject
{
    public string countryName;
	public CountryStats stats;
	
	public static Country Create(double sickness = 0.1, double population = 0.7)
	{
		Country country = ScriptableObject.CreateInstance<Country>();
		country.stats = new CountryStats(sickness, population);
		return country;
	}

    public void TickCountry(SmugglingResult? lastResult)
    {
        if (!lastResult.HasValue)
            stats.TickStats(0, 0);
        else
            stats = stats.TickStats(lastResult.Value.stats.sickness, lastResult.Value.group.clients.Count);
    }
}