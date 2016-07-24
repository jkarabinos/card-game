﻿using UnityEngine;
using System.Collections.Generic;
using System;
using GameSparks;
using GameSparks.Core;
using GameSparks.Platforms;
using GameSparks.Platforms.IOS;
using GameSparks.Platforms.WebGL;
using GameSparks.Api.Responses;
using GameSparks.Api.Requests;

public class GSChallengeHandler : MonoBehaviour {

	public string enemyId;
	public string challengeId;

	void Awake() {
		GameSparks.Api.Messages.ChallengeStartedMessage.Listener += ChallengeStartedMessageHandler;
		GameSparks.Api.Messages.ChallengeTurnTakenMessage.Listener += PerformedActionHandler;
	}

	//handle loading the data when some type of action has been played by the user
	void PerformedActionHandler(GameSparks.Api.Messages.ChallengeTurnTakenMessage _message){
		loadChallengeData(challengeId, "health");
	}

	// handle the start of game stuff
	void ChallengeStartedMessageHandler(GameSparks.Api.Messages.ChallengeStartedMessage _message){
		Debug.Log("a challenge has been started");
		/*GSData d  = _message.Challenge.ScriptData.GetGSData("currentHand");
		string f = _message.Challenge.ScriptData.GetString("testData");
		Debug.Log("the string: " + f);
		//JSON j = d.JSON;
		//Debug.Log("the raw raw data: " + data);
		Debug.Log("the test " + d);*/

		//get the data for the challenge, note that both players will have access to the same data
		string thisChallengeId = _message.Challenge.ChallengeId; 
		//Debug.Log ("challenge id is " + challengeId);
		challengeId = thisChallengeId;
		loadChallengeData(challengeId, "everything");

	}
	

	public void attackPlayer(int damage, int isFriendly){
		Debug.Log("did attack player " + isFriendly);
		new GameSparks.Api.Requests.LogChallengeEventRequest ()
			.SetChallengeInstanceId (challengeId)
			.SetEventKey ("action_attackPlayer")
			.SetEventAttribute ("damage", damage)
			.SetEventAttribute ("isFriendly", isFriendly)
			.Send ((response) => {

				if(!response.HasErrors){
					Debug.Log("attacked player");
					//loadChallengeData(challengeId, "health");
				}else{
					Debug.Log("Error attacking player...");
				}
		});
	}

	//get the data for the current challenge of which the user is a part
	void loadChallengeData(string challengeId, string info){

		new GameSparks.Api.Requests.GetChallengeRequest ()
			.SetChallengeInstanceId (challengeId)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log ("Received Chal Data From GameSparks... ");
					/*GSData currentHand = response.Challenge.ScriptData.GetGSData("currentHand").GetGSData(playerId);
					
					List< Dictionary<string, object> > handList = new List< Dictionary<string, object> >();

					foreach(KeyValuePair<string, object> cardPair in currentHand.BaseData){
						GSData cardData = (GSData) cardPair.Value;
						Dictionary<string, object> card =  (Dictionary<string, object>) cardData.BaseData;
						handList.Add(card);
						//string type = (string) card["cardType"];
					}

					//now we have the cards in a reasonable format. we can now pass them to the game logic script*/

					setEnenmyId(response.Challenge.Accepted);
					//Debug.Log("num players is " + response.Challenge.Accepted.Count);

					GameLogic gl = this.transform.GetComponent<GameLogic>();

					if(String.Compare(info, "everything") == 0){
						gl.startChallenge(response.Challenge.ScriptData);
					}
					else if(String.Compare(info, "health") == 0){
						gl.updateHealth(response.Challenge.ScriptData);
					}
					
					
				} else {
					Debug.Log ("Error Loading Challenge Data...");
				}
		});
	}



	//set the global enemy id property when a new challenge starts, this will be used to get enemy deck count
	void setEnenmyId(GSEnumerable<GetChallengeResponse._Challenge._PlayerDetail> acceptedPlayers){
		
		string localId = this.transform.GetComponent<GSConnectionManager>().playerId;

		foreach(GetChallengeResponse._Challenge._PlayerDetail playerDetail in acceptedPlayers){
			GSData playerData = (GSData) playerDetail.BaseData;
			if(String.Compare(playerData.GetString("id"), localId) != 0){
				enemyId = playerData.GetString("id");
			}
		}

		//Debug.Log("we set the enemy id to " + enemyId);
		
	}



}
