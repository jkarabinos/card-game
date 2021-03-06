﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class DamageHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler  {
	
	//note that this refers to the character of the user rather than the type of card hero
	public bool isCastle;

	//if a castle is friendly or enemy
	public bool isFriendly;

	public void OnPointerEnter(PointerEventData eventData){
		//Debug.Log("OnPointerEnter");


	}

	public void OnPointerExit(PointerEventData eventData){
		//Debug.Log("OnPointerExit");
	}


	public void OnDrop(PointerEventData eventData){
		
		Transform canvas = getCanvas();
		GameLogic gameLogic = canvas.GetComponent<GameLogic>();
		if(!gameLogic.interactionEnabled){
			return;
		}

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		CardObject cardObject = d.gameObject.GetComponent<CardObject>();
		if(String.Compare(cardObject.canTarget, "nothing") != 0 && String.Compare(cardObject.type, "heroCards") != 0){
			//if the card can target something

			


			//if the attack is targeting a card, perfom the necessary operations
			if(!isCastle ){
				if(String.Compare(cardObject.canTarget, this.transform.GetComponent<CardObject>().type) == 0 || 
					String.Compare(cardObject.canTarget, "everything") == 0){
					//if the card can target the type of card it is attempting to target

					//check for odd card rules (think finishing blow)
					if(!isLegalTarget(cardObject)){
						return;
					}

					//send the card to the played this turn zone
					Transform hand = d.originalParent;
					DropZone dropZone = hand.GetComponent<DropZone>();
					d.newParent = dropZone.dropZoneForName("FriendlyPlayField").transform;

					didAttackCard(cardObject);
				}
			}else if(isCastle && String.Compare(cardObject.canTarget, "everything") == 0){
				Transform hand = d.originalParent;
				DropZone dropZone = hand.GetComponent<DropZone>();
				d.newParent = dropZone.dropZoneForName("FriendlyPlayField").transform;

				didAttackCastle(cardObject);
			}
			
		

		}else{
			//for a card that does not do damage but is attempting to be played
			//yield the drop to the tabletop
			//Transform canvas = getCanvas();
			DropZone tabletop = canvas.GetComponent<GameLogic>().dropZoneForName("Tabletop");
			tabletop.OnDrop(eventData);
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
				heroAttackedCastle(gameLogic.selectedHero.GetComponent<CardObject>());
				//didAttackCastle(gameLogic.selectedHero.GetComponent<CardObject>());
				//heroDidAttack(gameLogic);
			}
			else if(String.Compare(c.type, "heroCards") == 0){
				if(isLegalHeroTarget(c)){
					didAttackCard(gameLogic.selectedHero.GetComponent<CardObject>());
					//heroDidAttack(gameLogic);
				}
			}else if(String.Compare(c.type, "monsterCards") == 0){
				if(isLegalMonsterTarget(c)){
					didAttackCard(gameLogic.selectedHero.GetComponent<CardObject>());
					//heroDidAttack(gameLogic);
				}
			}
		}
	}


	bool isLegalTarget(CardObject attackCard){
		CardObject attackedCard = this.transform.GetComponent<CardObject>();

		//for finishing blow
		if(String.Compare("FinishingBlow", attackCard.name) == 0){
			Debug.Log("playing a fin blow");
			if(String.Compare(attackedCard.type, "monsterCards") == 0){
				Debug.Log("on a monster");
				if(attackedCard.health <= 4){
					Debug.Log("the health looks good");
					return true;
				}
			}
			return false;
		}
		return true;
	}

	void heroAttackedCastle(CardObject hero){
		Dictionary<string, object> target = new Dictionary<string, object>();
		target.Add("target", "isPlayer");
		target.Add("isFriendly", isFriendly);

		Transform canvas = this.getCanvas();
		GSChallengeHandler ch = canvas.GetComponent<GSChallengeHandler>();
		ch.attackWithHero(hero, target);
	}

	void heroDidAttack(GameLogic gameLogic){
		//gameLogic.selectedHero.GetComponent<CardObject>().attacks --;
		//gameLogic.removeSelected(gameLogic.selectedHero.GetComponent<CardObject>());
		//gameLogic.selectedHero = null;
		//gameLogic.
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
		Transform parent = c.transform.parent;
		HeroZone heroZone = parent.GetComponent<HeroZone>();
		if(heroZone == null){
			return false;
		}

		if(heroZone.isFriendly){
			return false;
		}

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

		/*int damage = 0;
		if(String.Compare(attackCard.type, "hero") == 0){
			damage = attackCard.power;
		}else{
			damage = attackCard.damage;
		}*/

		Dictionary<string, object> target = new Dictionary<string, object>();
		target.Add("target", "isPlayer");
		target.Add("isFriendly", isFriendly);
		
		//this is where we will send the damage to gamesparks

		Transform canvas = this.getCanvas();
		GSChallengeHandler ch = canvas.GetComponent<GSChallengeHandler>();
		ch.playCard(attackCard, target);

/*
		Transform canvas = this.transform.parent;
		GameLogic gameLogic = canvas.GetComponent<GameLogic>();
		gameLogic.enemyHealth -= damage;

		*/
	}

	public void displayCastleHealth(int health){
		Text textBox = this.transform.GetComponentInChildren<Text>();
		textBox.text = health.ToString();
	}



	public void didAttackCard(CardObject attackCard){
		Debug.Log("DID ATTACK CARD");
		/*
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

		//remove a hero when its health reaches zero
		if(String.Compare(target.type, "hero") == 0){
			Destroy(targetCard.gameObject);
		}*/

		//the user has targeted a hero or monster with an attack coming from something other than a hero
		CardObject c = this.transform.GetComponent<CardObject>();
		Debug.Log("the card is of type " + c.type);

		Dictionary<string, object> target = new Dictionary<string, object>();

		Transform canvas = getCanvas();
		GameLogic gameLogic = canvas.GetComponent<GameLogic>();

		if(String.Compare(c.type, "heroCards") == 0){
			
			HeroZone hz = this.transform.parent.GetComponent<HeroZone>();
			if(hz.isFriendly){
				target = gameLogic.currentFriendlyHeroZone[c.cardId];
			}else{
				target = gameLogic.currentEnemyHeroZone[c.cardId];
			}
			target.Add("isFriendly", hz.isFriendly);
		}
		else if(String.Compare(c.type, "monsterCards") == 0){

			target = gameLogic.currentMonsterZones[c.cardId];
		}
		target.Add("target", "isCard");
		//target.Add("isFriendly", isFriendly);
		
		//this is where we will send the damage to gamesparks

		//if we are attacking a hero or monster with a hero
		if(String.Compare(attackCard.type, "heroCards") == 0){
			GSChallengeHandler ch = canvas.GetComponent<GSChallengeHandler>();
			ch.attackWithHero(attackCard, target);
		}else{
			//if we are attacking a hero or monster with some type of damage spell
			GSChallengeHandler ch = canvas.GetComponent<GSChallengeHandler>();
			ch.playCard(attackCard, target);
		}

		

	}



	

}
