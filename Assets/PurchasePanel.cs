﻿using UnityEngine;
using System.Collections;

public class PurchasePanel : MonoBehaviour {

	public void initializePurchasePanel(GameLogic gameLogic){
		Debug.Log("Purchase Panel Operational!");
		GameObject silver = gameLogic.createCardForId(1, gameLogic.globalDict);
		GameObject gold = gameLogic.createCardForId(2, gameLogic.globalDict);
		setPurchasable(silver);
		setPurchasable(gold);
		silver.transform.SetParent(this.transform);
		gold.transform.SetParent(this.transform);
	}	

	void setPurchasable(GameObject card){
		CardObject cardObject = card.GetComponent<CardObject>();
		cardObject.isPurchasable = true;
	}
}