using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class PurchasePanel : MonoBehaviour {

	public void initializePurchasePanel(GameLogic gameLogic, List<int> userBuild){
		Debug.Log("Purchase Panel Operational!");
		
		for( int i = 0; i < userBuild.Count; i++ ){
			int cardID = userBuild[i];
			addCard(cardID, gameLogic);
		}
	}	


	//add the given card to the purchase panel and make it purchasable
	void addCard(int cardID, GameLogic gameLogic){

		GameObject card = gameLogic.createCardForId(cardID, gameLogic.globalDict);
		setPurchasable(card);
		card.transform.SetParent( this.transform ); 

	}

	void setPurchasable(GameObject card){
		CardObject cardObject = card.GetComponent<CardObject>();
		cardObject.isPurchasable = true;
	}
}
