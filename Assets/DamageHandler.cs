using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class DamageHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler  {
	
	//note that this refers to the character of the user rather than the type of card hero
	public bool isHero;

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

			//send the card to the played this turn zone
			Transform hand = d.originalParent;
			DropZone dropZone = hand.GetComponent<DropZone>();
			d.newParent = dropZone.dropZoneForName("PlayedThisTurn").transform;


			//if the attack is targeting a card, perfom the necessary operations
			if(!isHero){
				didAttackCard(cardObject);
			}else{
				didAttackHero(cardObject);
			}
			
		}
	
	}

	//if the user targeted the hero with the attack
	void didAttackHero(CardObject attackCard){
		Transform canvas = this.transform.parent;
		GameLogic gameLogic = canvas.GetComponent<GameLogic>();
		gameLogic.enemyHealth -= attackCard.damage;

		Text textBox = this.transform.GetComponentInChildren<Text>();
		textBox.text = gameLogic.enemyHealth.ToString();
	}

	void didAttackCard(CardObject attackCard){
		Transform targetCard = this.transform;
		CardObject target = targetCard.GetComponent<CardObject>();
		target.health -= attackCard.damage;
			
		//remove a monster if it dies
		if(target.health <= 0 && String.Compare(target.type, "monster") == 0){
			removeMonster(targetCard);
		}

		//remove a building when its health reaches zero
		if(target.health <= 0 && (String.Compare(target.type, "building") == 0
								|| String.Compare(target.type, "hero") == 0)){
			Destroy(targetCard.gameObject);
		}
	}


	public void removeMonster(Transform deadMonster){
		gainReward(deadMonster);
		//deadMonster.SetParent(null);
		Destroy(deadMonster.gameObject);
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
			gameLogic.gainCard(card, "NeutralPurchasePanel");
		}
		//for a rabid wolves
		else if(String.Compare(cardObject.cardName, "RabidWolves") == 0){
			//gain an arrow
			GameObject card = gameLogic.createCardForId(3, gameLogic.globalDict);
			gameLogic.gainCard(card, "NeutralPurchasePanel");
		}
		//for an entrepreneurial ogre
		else if(String.Compare(cardObject.cardName, "EntrepreneurialOgre") == 0){
			//add two coins to the user's total coin for the turn
			gameLogic.updateMoneyCounter(2);
		}


	}

}
