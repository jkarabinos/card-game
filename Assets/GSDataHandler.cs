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

	//get a playfield for one of the players
	public Dictionary<string, Dictionary<string, object> > getPlayField(GSData challenge, bool isFriendly){
		string playerId = this.transform.GetComponent<GSConnectionManager>().playerId;
		if(!isFriendly){
			playerId = this.transform.GetComponent<GSChallengeHandler>().enemyId;
		}

		GSData playField = challenge.GetGSData("playField").GetGSData(playerId);

		Dictionary< string, Dictionary<string, object> > fieldDict = new Dictionary< string, Dictionary<string, object> >();

		foreach(KeyValuePair<string, object> cardPair in playField.BaseData){
			GSData cardData = (GSData) cardPair.Value;
			Dictionary<string, object> card =  (Dictionary<string, object>) cardData.BaseData;
			string key = cardPair.Key;
			fieldDict.Add(key, card);
		}

		return fieldDict;
	}

	public Dictionary<string, Dictionary<string, object> > getHeroZone(GSData challenge, bool isFriendly){
		string playerId = this.transform.GetComponent<GSConnectionManager>().playerId;
		if(!isFriendly){
			playerId = this.transform.GetComponent<GSChallengeHandler>().enemyId;
		}

		GSData heroZone = challenge.GetGSData("heroZone").GetGSData(playerId);

		Dictionary< string, Dictionary<string, object> > heroDict = new Dictionary< string, Dictionary<string, object> >();

		foreach(KeyValuePair<string, object> cardPair in heroZone.BaseData){
			GSData cardData = (GSData) cardPair.Value;
			Dictionary<string, object> card =  (Dictionary<string, object>) cardData.BaseData;
			string key = cardPair.Key;
			heroDict.Add(key, card);
		}

		return heroDict;
	}


	//get a purchase panel, -1 = enemy, 0 = neutral, 1 = friendly
	public Dictionary<string, Dictionary<string, object> > getPurchasePanel(GSData challenge, int panelOwner){
		string panelId = this.transform.GetComponent<GSConnectionManager>().playerId;
		if(panelOwner == -1){
			panelId = this.transform.GetComponent<GSChallengeHandler>().enemyId;
		}else if(panelOwner == 0){
			panelId = "neutral";
		}

		GSData purchasePanel = challenge.GetGSData("purchasePanel").GetGSData(panelId);

		Dictionary< string, Dictionary<string, object> > panelDict = new Dictionary< string, Dictionary<string, object> >();

		foreach(KeyValuePair<string, object> cardPair in purchasePanel.BaseData){
			GSData cardData = (GSData) cardPair.Value;
			Dictionary<string, object> card =  (Dictionary<string, object>) cardData.BaseData;
			string key = cardPair.Key;
			panelDict.Add(key, card);
		}

		return panelDict;
	}

	
	//get the stat for the player for the particular key and challenge info
	public object getPlayerStat(GSData challenge, string key, bool isFriendly){
		string playerId = this.transform.GetComponent<GSConnectionManager>().playerId;
		if(!isFriendly){
			//if we want an enemy stat
			playerId = this.transform.GetComponent<GSChallengeHandler>().enemyId;
		}

		GSData playerStats = challenge.GetGSData("playerStats").GetGSData(playerId);

		Dictionary<string, object> statsDict = (Dictionary<string, object>) playerStats.BaseData;

		object value = statsDict[key];
		return value;

	}

	public int getEnemyCardCount(GSData challenge){
		string playerId = this.transform.GetComponent<GSChallengeHandler>().enemyId;

		GSData ch = challenge.GetGSData("currentHand").GetGSData(playerId);
		Dictionary<string, object> handDict = (Dictionary<string, object>) ch.BaseData;
		
		return handDict.Count;
	}


	//get the number of cards in the player's deck
	public int getCardStackCount(GSData challenge, string stackName, bool isFriendly){
		string playerId = this.transform.GetComponent<GSConnectionManager>().playerId;
		if(!isFriendly){
			playerId = this.transform.GetComponent<GSChallengeHandler>().enemyId;
		}

		GSData cd = challenge.GetGSData(stackName);
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
