using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class DamageHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler  {
	
	//note that this refers to the character of the user rather than the type of card hero
	public bool isCastle;

	public void OnPointerEnter(PointerEventData eventData){
		//Debug.Log("OnPointerEnter");


	}

	public void OnPointerExit(PointerEventData eventData){
		//Debug.Log("OnPointerExit");
	}


	public void OnDrop(PointerEventData eventData){

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		CardObject cardObject = d.gameObject.GetComponent<CardObject>();
		if(cardObject.damage > 0){

			//send the card to the played this turn zone
			Transform hand = d.originalParent;
			DropZone dropZone = hand.GetComponent<DropZone>();
			d.newParent = dropZone.dropZoneForName("PlayedThisTurn").transform;


			//if the attack is targeting a card, perfom the necessary operations
			if(!isCastle){
				didAttackCard(cardObject);
			}else{
				didAttackCastle(cardObject);
			}
			
			Transform canvas = getCanvas();
			GameLogic gameLogic = canvas.GetComponent<GameLogic>();
			gameLogic.playCard(cardObject);

		}
	
	}


	public void OnPointerClick(PointerEventData eventData){
		
		

		//use the card to get the canvas no matter where we are in the hierarchy
		Transform canvas = getCanvas();
		GameLogic gameLogic = canvas.GetComponent<GameLogic>();
		if(gameLogic.selectedHero != null){
			//if the user is attempting to attack with a hero

			CardObject c = this.transform.GetComponent<CardObject>();
			if(isCastle){
				didAttackCastle(gameLogic.selectedHero.GetComponent<CardObject>());
				heroDidAttack(gameLogic);
			}
			else if(String.Compare(c.type, "hero") == 0){
				if(isLegalHeroTarget(c)){
					//didAttackHero(gameLogic.selectedHero.GetComponent<CardObject>());
				}
			}else if(String.Compare(c.type, "monster") == 0){
				if(isLegalMonsterTarget(c)){
					didAttackCard(gameLogic.selectedHero.GetComponent<CardObject>());
					heroDidAttack(gameLogic);
				}
			}else if(String.Compare(c.type, "building") == 0){
				if(isLegalBuildingTarget(c)){
					didAttackCard(gameLogic.selectedHero.GetComponent<CardObject>());
					heroDidAttack(gameLogic);
				}
			}
		}
	}

	void heroDidAttack(GameLogic gameLogic){
		gameLogic.selectedHero.GetComponent<CardObject>().attacks --;
		gameLogic.selectedHero = null;
	}

	//if the building is in the enemy building zone
	bool isLegalBuildingTarget(CardObject c){
		Transform parent = c.transform.parent;
		BuildingSpot bs = parent.GetComponent<BuildingSpot>();
		if(bs == null){
			return false;
		}

		BuildingZone bz = bs.transform.parent.GetComponent<BuildingZone>();
		if(bz == null){
			return false;
		}

		if(bz.isFriendly){
			return false;
		}	

		//if the building is in the enemy building zone
		return true;

	}

	//if the monster is in the monster zone
	bool isLegalMonsterTarget(CardObject c)	{
		Transform parent = c.transform.parent;
		if(parent.GetComponent<MonsterZone>() == null){
			return false;
		}

		return true;
	}

	bool isLegalHeroTarget(CardObject c){
		return true;
	}


	public Transform getCanvas(){
		GameLogic gameLogic = null;
		Transform currentParent = this.transform;
		while(gameLogic == null){
			currentParent = currentParent.parent;
			gameLogic = currentParent.GetComponent<GameLogic>();
		}

		return currentParent;
	}

	//if the user targeted the hero with the attack
	public void didAttackCastle(CardObject attackCard){
		Debug.Log("did attack castle");

		int damage = 0;
		if(String.Compare(attackCard.type, "hero") == 0){
			damage = attackCard.power;
		}else{
			damage = attackCard.damage;
		}


		Transform canvas = this.transform.parent;
		GameLogic gameLogic = canvas.GetComponent<GameLogic>();
		gameLogic.enemyHealth -= damage;

		Text textBox = this.transform.GetComponentInChildren<Text>();
		textBox.text = gameLogic.enemyHealth.ToString();
	}

	public void didAttackCard(CardObject attackCard){
		int damage = 0;
		if(String.Compare(attackCard.type, "hero") == 0){
			CardObject attackedCard = this.transform.GetComponent<CardObject>();
			if(String.Compare(attackedCard.type, "hero") == 0
			|| String.Compare(attackedCard.type, "monster") == 0){
				attackCard.health -= attackedCard.power;
				Debug.Log("the attacking hero's health is " + attackCard.health);
				if(attackCard.health <= 0){
					Destroy(attackCard.gameObject);
				}
			}
			damage = attackCard.power;
		}else{
			damage = attackCard.damage;
		}

		Transform targetCard = this.transform;
		CardObject target = targetCard.GetComponent<CardObject>();
		target.health -= damage;
			
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
