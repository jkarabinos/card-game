using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class MonsterZone : MonoBehaviour {

	public string monsterZoneName;

	public void initializeMonsterZone(GameLogic gameLogic, List<int> listOfMonsters){
		Debug.Log("MonsterPanel Operational!");
		
		for( int i = 0; i < listOfMonsters.Count; i++ ){
			int cardID = listOfMonsters[i];
			addCard(cardID, gameLogic);
		}
	}	


	//add the given card to the purchase panel and make it purchasable
	void addCard(int cardID, GameLogic gameLogic){

		GameObject card = gameLogic.createCardForId(cardID, gameLogic.globalDict);
		card.transform.SetParent( this.transform ); 

	}	
}
