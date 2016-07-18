using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class PurchasePanel : MonoBehaviour {

	public string purchasePanelName;

	public void initializePurchasePanel(GameLogic gameLogic, List<int> userBuild){
		Debug.Log("Purchase Panel Operational!");
		
		for( int i = 0; i < userBuild.Count; i++ ){
			int cardID = userBuild[i];
			addCard(cardID, gameLogic, -1);
		}
	}	


	//add the given card to the purchase panel and make it purchasable
	public void addCard(int cardID, GameLogic gameLogic, int siblingIndex){

		GameObject card = gameLogic.createCardForId(cardID, gameLogic.globalDict);
		if(String.Compare(purchasePanelName, "EnemyPurchasePanel") != 0 ){
			setPurchasable(card);
		}

		card.transform.SetParent( this.transform ); 
		
		if(siblingIndex >= 0){
			Debug.Log("the sib index is " + siblingIndex);
			card.transform.SetSiblingIndex(siblingIndex);
		}

		

	}

	void setPurchasable(GameObject card){
		CardObject cardObject = card.GetComponent<CardObject>();
		cardObject.isPurchasable = true;
	}
}
