using UnityEngine;
using System.Collections;

[System.Serializable]
public struct CountryStats
{
	public string name;
	public string currentNews;
	public int population;
	public int percentSick; // percent of people in country that are sick
		
	public CountryStats(string _name = "EMPTY", string _currentNews = "EMPTY", int _population = 0, int _percentSick = 0)
	{
		name = _name;
		currentNews = _currentNews;
		population = _population;
		percentSick = _percentSick;
	}
}

[CreateAssetMenu(fileName = "Country", menuName = "Smuggling/Country", order = 1)]
public class Country : ScriptableObject
{
	public CountryStats stats;
	
	public static Country Create(string _name = "EMPTY", string _currentNews = "EMPTY", int _population = 0, int _percentSick = 0)
	{
		Country country = ScriptableObject.CreateInstance<Country>();
		country.stats = new CountryStats(_name, _currentNews, _population, _percentSick);
		return country;
	}

	//handles people traveling between countries -- use negative if you want to subtract people
	public void changePopulation(int _num){
		this.stats.population += _num;
	}

	//method for handling news events after a certain smuggling group comes in


	public void printStats(){
		Debug.Log (this.stats.name);
		Debug.Log (this.stats.currentNews);
		Debug.Log (this.stats.population);
		Debug.Log (this.stats.percentSick);
	}
}