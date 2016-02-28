using UnityEngine;
using System.Collections;

[System.Serializable]
public struct PlayerStats
{
	public string name;
	public int money;
	public int reputation;
	public int totalRuns;
	public int runsFailed; // if player fails too many runs they have a chance of being caught
		
	public PlayerStats(string _name = "EMPTY", int _money = 0, int _reputation = 0, int _runsFailed = 0)
	{
		name = _name;
		money = _money;
		reputation = _reputation;
		runsFailed = _runsFailed;
		totalRuns = 0;
	}
}
	
[CreateAssetMenu(fileName = "Player", menuName = "Smuggling/Player", order = 1)]
public class Player : ScriptableObject
{
	public PlayerStats stats;

	//holds money data for different tranportation
	public int trainTransportCost = 8000;
	public int seaTransportCost = 3000;
	public int airTransportCost = 10000;
	public int bribeTransportCost = 2000;

	public static Player Create(string _name = "EMPTY", int _money = 0, int _reputation = 0, int _runsFailed = 0)
	{
		Player player = ScriptableObject.CreateInstance<Player>();
		player.stats = new PlayerStats(_name, _money, _reputation, _runsFailed);
		return player;
	}

	public void increaseRunsFailed(){
		this.stats.runsFailed++;
	}

	//handles money changes -- use negative to subtract
	public void changeMoney(int _num){
		this.stats.money += _num;
	}
		
	//handles reputation changes -- use negative to subtract
	public void changeReputation(int _num){
		this.stats.reputation += _num;
	}

	public int calculateTransportCosts (TransportType type, int numClients){
		if (type == TransportType.NONE) {
			return 0;
		} else if (type == TransportType.SEA) {
			return seaTransportCost;
		} else if (type == TransportType.TRAIN) {
			return trainTransportCost;
		} else if (type == TransportType.AIR) {
			return airTransportCost * 1;
		} else if (type == TransportType.BRIBE) {
			return (bribeTransportCost) + ((numClients - 1) * 1000);
		} else {
			return 0;
		}
	}

	public int getTotalRuns(){
		return this.stats.totalRuns;
	}

	public void updateTotalRuns(){
		this.stats.totalRuns++;
	}

	public void setStartingStats(){
		changeMoney (100000);
		changeReputation (50);
	}

	public void setTrainTransportCost(int _num){
		trainTransportCost = _num;
	}

	public void setSeaTransportCost(int _num){
		seaTransportCost = _num;
	}

	public void setBribeTransportCost(int _num){
		bribeTransportCost = _num;
	}

	public void setAirTransportCost(int _num){
		airTransportCost = _num;
	}
		
	public void printStats(){
		Debug.Log (this.stats.name);
		Debug.Log (this.stats.money);
		Debug.Log (this.stats.reputation);
		Debug.Log (this.stats.runsFailed);
	}
}