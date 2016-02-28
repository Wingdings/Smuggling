﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public struct CountryStats
{
	public string name;
	public string currentNews;
	public int difficulty;
	public int population;
	public int percentSick; // percent of people in country that are sick
		
	public CountryStats(string _name = "EMPTY", string _currentNews = "EMPTY", int _difficulty = 0, int _population = 0, int _percentSick = 0)
	{
		name = _name;
		currentNews = _currentNews;
		difficulty = _difficulty;
		population = _population;
		percentSick = _percentSick;
	}
}

[CreateAssetMenu(fileName = "Country", menuName = "Smuggling/Country", order = 1)]
public class Country : ScriptableObject
{
	public CountryStats stats;
	
	public static Country Create(string _name = "EMPTY", string _currentNews = "EMPTY", int _difficulty = 0, int _population = 0, int _percentSick = 0)
	{
		Country country = ScriptableObject.CreateInstance<Country>();
		country.stats = new CountryStats(_name, _currentNews,_difficulty, _population, _percentSick);
		return country;
	}

	public void setCountryName(string _name){
		this.stats.name = _name;
	}

	public void setCurrentNews(string currNews){
		this.stats.currentNews = currNews;
	}

	public void setPopulation(int _num){
		this.stats.population = _num;
	}

	public void setPercentSick(int _num){
		this.stats.percentSick = _num;
	}

	public void setDifficulty(int _num){
		this.stats.difficulty = _num;
	}

	public int getDifficulty(){
		return this.stats.difficulty;
	}

	public string getName (){
		return this.stats.name;
	}

	public string getCurrentNews(){
		return this.stats.currentNews;
	}

	public int getPopulation(){
		return this.stats.population;
	}

	public int getPercentSick(){
		return this.stats.percentSick;
	}

	//handles people traveling between countries -- use negative if you want to subtract people
	public void changePopulation(int _num){
		this.stats.population += _num;
	}

	//not sure how sickness is working yet
	public void calcPercentSick(){

	}
	//method for handling news events after a certain smuggling group comes in


	public void printStats(){
		Debug.Log (this.stats.name);
		Debug.Log (this.stats.currentNews);
		Debug.Log (this.stats.population);
		Debug.Log (this.stats.percentSick);
	}
}