﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class DamageHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler  {
	

	public void OnPointerEnter(PointerEventData eventData){
		//Debug.Log("OnPointerEnter");


	}

	public void OnPointerExit(PointerEventData eventData){
		//Debug.Log("OnPointerExit");
	}


	public void OnDrop(PointerEventData eventData){

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		CardObject cardObject = d.gameObject.GetComponent<CardObject>();
		if(String.Compare(cardObject.type, "attack") == 0){
			Transform hand = d.originalParent;
			DropZone dropZone = hand.GetComponent<DropZone>();
			d.newParent = dropZone.dropZoneForName("PlayedThisTurn").transform;
			Transform targetCard = this.transform;
			CardObject target = targetCard.GetComponent<CardObject>();
			target.health -= cardObject.damage;
			Debug.Log("the health of the monster is " + target.health);
			if(target.health <= 0){
				removeMonster(targetCard);
			}
		}
	
	}

	public void removeMonster(Transform deadMonster){
		gainReward(deadMonster);
		deadMonster.SetParent(null);
	}


	//gain the necessary reward for the monster that has been killed
	void gainReward(Transform deadMonster){

		Transform canvas = this.transform.parent.parent;
		GameLogic gameLogic = canvas.GetComponent<GameLogic>();
		CardObject cardObject = deadMonster.GetComponent<CardObject>();

		//for a silverback gorilla
		if(String.Compare(cardObject.cardName, "SilverbackGorilla") == 0){
			//gain a silver
			GameObject card = gameLogic.createCardForId(1, gameLogic.globalDict);
			gameLogic.gainCard(card);
		}
		//for a rabid wolves
		else if(String.Compare(cardObject.cardName, "RabidWolves") == 0){
			//gain an arrow
			GameObject card = gameLogic.createCardForId(3, gameLogic.globalDict);
			gameLogic.gainCard(card);
		}
		//for an entrepreneurial ogre
		else if(String.Compare(cardObject.cardName, "EntrepreneurialOgre") == 0){
			//add two coins to the user's total coin for the turn
			gameLogic.updateMoneyCounter(2);
		}


	}

}