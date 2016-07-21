using UnityEngine;
using System.Collections;

public class GSConnectionManager : MonoBehaviour {

	public void authenticateUser(){

		Debug.Log("Authorizing Player");
		new GameSparks.Api.Requests.AuthenticationRequest ()
			.SetUserName ("johnjohn")
			.SetPassword ("password")
			.Send ((response) => {
				if(!response.HasErrors){
					GameLogic gl = this.transform.GetComponent<GameLogic>();
					Debug.Log("Player Authenticated");
					gl.startTheGame();
					testSavePlayer();


				}else{
					Debug.Log("Error Authenticating Player");
					//for now, if the player is not authenticated, we will register a new one
					registerPlayer();
				}

			});

	}


	//register a new player to GameSparks
	public void registerPlayer(){

		Debug.Log ("Registering player");
		new GameSparks.Api.Requests.RegistrationRequest()
			.SetDisplayName ("king_john")
			.SetUserName ("johnjohn")
			.SetPassword ("password")
			.Send ((response) => {

				if(!response.HasErrors){
					Debug.Log("Player Registered");
					GameLogic gl = this.transform.GetComponent<GameLogic>();
					gl.startTheGame();
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
				}else{
					Debug.Log("Error Saving Player Data...");
				}
			});
	}

}
