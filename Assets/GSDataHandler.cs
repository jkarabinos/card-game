using UnityEngine;
using System.Collections.Generic;
using System;
using GameSparks;
using GameSparks.Core;
using GameSparks.Platforms;
using GameSparks.Platforms.IOS;
using GameSparks.Platforms.WebGL;
using GameSparks.Api.Responses;
using GameSparks.Api.Requests;

//Handles changing data from gsdata to more appropriate types

public class GSDataHandler : MonoBehaviour {

	
	public Dictionary < string, Dictionary <string, object> > convertHand(GSData challenge){
		Debug.Log("convert hand");
		string playerId = this.transform.GetComponent<GSConnectionManager>().playerId;

		GSData currentHand = challenge.GetGSData("currentHand").GetGSData(playerId);
					
		Dictionary< string, Dictionary<string, object> > handDict = new Dictionary< string, Dictionary<string, object> >();

		foreach(KeyValuePair<string, object> cardPair in currentHand.BaseData){
			GSData cardData = (GSData) cardPair.Value;
			Dictionary<string, object> card =  (Dictionary<string, object>) cardData.BaseData;
			string key = cardPair.Key;
			handDict.Add(key, card);
		}

		return handDict;
	}

	
	//get the stat for the player for the particular key and challenge info
	public object getPlayerStat(GSData challenge, string key, bool isFriendly){
		string playerId = this.transform.GetComponent<GSConnectionManager>().playerId;
		if(isFriendly){
			//if we want an enemy stat
			playerId = this.transform.GetComponent<GSChallengeHandler>().enemyId;
		}

		GSData playerStats = challenge.GetGSData("playerStats").GetGSData(playerId);

		Dictionary<string, object> statsDict = (Dictionary<string, object>) playerStats.BaseData;

		object value = statsDict[key];
		return value;

	}


	//get the number of cards in the player's deck
	public int getFriendlyDeckCount(GSData challenge){
		string playerId = this.transform.GetComponent<GSConnectionManager>().playerId;

		GSData cd = challenge.GetGSData("currentDeck");
		//GSData deckData = cd.GetGSData(playerId);
		List< GSData > deckList = cd.GetGSDataList(playerId);
		return deckList.Count;
	}

	//get the number of cards in the opponent's deck
	public int getEnemyDeckCount(GSData challenge){
		return 1;
	}
	/*
	//get the stat of the enemy player for the particular key and challenge info
	public object getEnemyPlayerStat(GSData challenge, string key){

	}*/

}
