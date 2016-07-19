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
			setPileCount(card);
		}

		card.transform.SetParent( this.transform ); 

		if(siblingIndex >= 0){
			Debug.Log("the sib index is " + siblingIndex);
			card.transform.SetSiblingIndex(siblingIndex);
		}

		

	}

	//return the card on the panel with the given id
	public CardObject cardForId(int id){
		foreach(Transform child in this.transform){
			CardObject c = child.GetComponent<CardObject>();
			if(c != null){
				if(c.id == id){
					return c;
				}
			}
		}
		return null;
	}

	void setPileCount(GameObject card){
		CardObject cardObject = card.GetComponent<CardObject>();
		cardObject.pileCount = numberForRarity(cardObject.rarity);
	}


	//the number of cards for the given rarity
	int numberForRarity(string rarity){
		if(String.Compare(rarity, "common") == 0){
			return 3;
		}
		else if(String.Compare(rarity, "rare") == 0){
			return 2;
		}
		else if(String.Compare(rarity, "epic") == 0){
			return 1;
		}
		else if(String.Compare(rarity, "legendary") == 0){
			return 1;
		}else{
			//for basic cards, this will act as our infinity
			return 100;
		}
	}

	void setPurchasable(GameObject card){
		CardObject cardObject = card.GetComponent<CardObject>();
		cardObject.isPurchasable = true;
	}
}
