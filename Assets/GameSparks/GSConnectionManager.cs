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

public class GSConnectionManager : MonoBehaviour {

	public void authenticateUser(){



		Debug.Log("Authorizing Player");
		new GameSparks.Api.Requests.AuthenticationRequest ()
			.SetUserName ("johnjohnjohn")
			.SetPassword ("password")
			.Send ((response) => {
				if(!response.HasErrors){
					GameLogic gl = this.transform.GetComponent<GameLogic>();
					Debug.Log("Player Authenticated");
					
					wasAuthenticated(gl);

				}else{
					Debug.Log("Error Authenticating Player");
					//for now, if the player is not authenticated, we will register a new one
					registerPlayer();
				}

		});

	}


	public void wasAuthenticated(GameLogic gl){
		//listenForMessages();
		gl.startTheGame();
		findMatch();
	}


	void listenForMessages(){
		//ScriptMessage.Listener = ((ScriptMessage message) => { Debug.Log("We just got a message yooooo!!!"); });
	}

	//register a new player to GameSparks
	public void registerPlayer(){

		Debug.Log ("Registering player");
		new GameSparks.Api.Requests.RegistrationRequest()
			.SetDisplayName ("king_john")
			.SetUserName ("johnjohnjohn")
			.SetPassword ("password")
			.Send ((response) => {

				if(!response.HasErrors){
					Debug.Log("Player Registered");
					GameLogic gl = this.transform.GetComponent<GameLogic>();
					wasAuthenticated(gl);
				}else{
					Debug.Log("Error Registering Player");
				}

		});
	}


	public void testSavePlayer(){
		new GameSparks.Api.Requests.LogEventRequest ()
			.SetEventKey ("SAVE_PLAYER")
			.SetEventAttribute ("POS", this.transform.position.ToString())
			.Send ((response) => {

				if(!response.HasErrors){
					Debug.Log("Player saved to GameSparks");
					loadPlayerData();
				}else{
					Debug.Log("Error Saving Player Data...");
				}
		});
	}


	//load the simple player data from GameSparks
	public void loadPlayerData(){

		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("LOAD_PLAYER").Send((response) => {
			if (!response.HasErrors) {
				Debug.Log("Received Player Data From GameSparks... " + response.ScriptData);

				GSData data = response.ScriptData.GetGSData("player_Data");
				//Debug.Log("Player ID: " + data.GetString("playerID"));
				//print("Player XP: " + data.GetString("playerXP"));
				//print("Player Gold: " + data.GetString("playerGold"));
				Debug.Log("Player Pos: " + data.GetString("playerPos"));
			} else {
				Debug.Log("Error Loading Player Data...");
			}
		});
	}

	//find a game for the authenticated player
	public void findMatch(){
		Debug.Log("attempting to find game");

		new GameSparks.Api.Requests.MatchmakingRequest()
			.SetMatchShortCode("MULTI_MCH")
			.SetSkill(0)
			.Send ((response) => {

				if(!response.HasErrors){
					Debug.Log("Started searching for a match successfully with response " + response);
				}else{
					Debug.Log("Error Matchmaking...");
				}
		});
	}

	void Awake() {
		GameSparks.Api.Messages.MatchNotFoundMessage.Listener += MatchNotFoundMessageHandler;
	}
	void MatchNotFoundMessageHandler(GameSparks.Api.Messages.MatchNotFoundMessage _message) {
		Debug.Log("Sorry, we could not find a match. Please try again later." );
	}


}
