using UnityEngine;
using System.Collections;

public class PurchasePanel : MonoBehaviour {

	public void initializePurchasePanel(GameLogic gameLogic){
		Debug.Log("Purchase Panel Operational!");
		/*GameObject silver = gameLogic.createCardForId(1, gameLogic.globalDict);
		GameObject gold = gameLogic.createCardForId(2, gameLogic.globalDict);
		GameObject tactfulNegotiations = gameLogic.createCardForId(5, gameLogic.globalDict);
		setPurchasable(silver);
		setPurchasable(gold);
		setPurchasable(tactfulNegotiations);
		silver.transform.SetParent(this.transform);
		gold.transform.SetParent(this.transform);
		tactfulNegotiations.transform.SetParent(this.transform);*/


		addCard(0, gameLogic);
		addCard(1, gameLogic);
		addCard(2, gameLogic);
		addCard(3, gameLogic);
		addCard(4, gameLogic);
		addCard(6, gameLogic);
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
