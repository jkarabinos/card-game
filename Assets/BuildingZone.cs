using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;


public class BuildingZone : MonoBehaviour {

	
	public bool isFriendly;

	public bool gainBuilding(GameObject building, GameLogic gameLogic){
		Debug.Log("add the building");
		BuildingSpot targetBuildingSpot = null;

		foreach(Transform child in this.transform){
			BuildingSpot bs = child.GetComponent<BuildingSpot>();
			if(!bs.isOccupied){
				targetBuildingSpot = bs;
				break;
			}
		}

		//if there is at least one free spot to build on
		if(targetBuildingSpot != null){
			CardObject c = building.GetComponent<CardObject>();
			gameLogic.updateMoneyCounter(-c.cost);
			gameLogic.totalBuys --;

			targetBuildingSpot.isOccupied = true;
			building.transform.SetParent(targetBuildingSpot.transform);
			return true;
			//gameLogic.didGainCard(building);
		}else{
			Destroy(building);
		}
		return false;
	}


	//handle the triggers for any start of turn buildings
	public void handleBuildingTriggers(GameLogic gameLogic){

		foreach(Transform child in this.transform){
			BuildingSpot bs = child.GetComponent<BuildingSpot>();
			if(bs.isOccupied){
				CardObject card = bs.GetComponentInChildren<CardObject>();
				handleBuildingTrigger(gameLogic, card);

			}
		}

	}

	void handleBuildingTrigger(GameLogic gameLogic, CardObject card){

		string cardName = card.cardName;

		if(String.Compare(cardName, "Blacksmith") == 0){
			GameObject newCard = gameLogic.createCardForId(4, gameLogic.globalDict);
			gameLogic.gainCard(newCard, "NeutralPurchasePanel");

		}else if(String.Compare(cardName, "TempleToTheHighGods") == 0){
			gameLogic.drawCard();
			gameLogic.updateMoneyCounter(1);
			gameLogic.totalBuys++;
			
		}

	}

}
