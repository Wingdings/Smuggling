using UnityEngine;
using System.Collections;

[System.Serializable]
public struct PlayerStats
{
	public string name;
	public int money;
	public int reputation;
	public int runsFailed; // if player fails too many runs they have a chance of being caught
		
	public PlayerStats(string _name = "EMPTY", int _money = 0, int _reputation = 0, int _runsFailed = 0)
	{
		name = _name;
		money = _money;
		reputation = _reputation;
		runsFailed = _runsFailed;
	}
}
	
[CreateAssetMenu(fileName = "Player", menuName = "Smuggling/Player", order = 1)]
public class Player : ScriptableObject
{
	public PlayerStats stats;

	public static Player Create(string _name = "EMPTY", int _money = 0, int _reputation = 0, int _runsFailed = 0)
	{
		Player player = ScriptableObject.CreateInstance<Player>();
		player.stats = new PlayerStats(_name, _money, _reputation, _runsFailed);
		return player;
	}

	//handles money changes -- use negative to subtract
	public void changeMoney(int _num){
		this.stats.money += _num;
	}
		
	//handles reputation changes -- use negative to subtract
	public void changeReputation(int _num){
		this.stats.reputation += _num;
	}
		
	public void printStats(){
		Debug.Log (this.stats.name);
		Debug.Log (this.stats.money);
		Debug.Log (this.stats.reputation);
		Debug.Log (this.stats.runsFailed);
	}
}