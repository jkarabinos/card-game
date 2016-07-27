using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

//handles heroes attacking monsters, castles, buildings, and other heroes

public class HeroAttackHandler : MonoBehaviour, IPointerClickHandler {

	//public GameObject selectedHero;

	public void OnPointerClick(PointerEventData eventData){
		Debug.Log("hero attack handler pointer click");
		
		CardObject c = this.transform.GetComponent<CardObject>();


		HeroZone possibleHeroZone = this.transform.parent.GetComponent<HeroZone>();

		//if we click on a hero in the friendly hero zone
		if(String.Compare(c.type, "heroCards") == 0 && possibleHeroZone != null){
			Debug.Log("attempt to attack with hero");

			//if the hero is out of attacks or the hero zone is not friendly
			if(c.attacks <= 0 || !possibleHeroZone.isFriendly){
				return;
			}

			Debug.Log("clicked a card to attack with");

			//we know this hero is in the friendly herozone, so we can now access the canvas
			Transform tabletop = this.transform.parent.parent;
			Transform canvas = tabletop.parent;
			GameLogic gameLogic = canvas.GetComponent<GameLogic>();
			gameLogic.selectedHero = c.gameObject;
			gameLogic.setSelected(c);
		}

		//GameObject card = eventData.pointerEnter;
		//purchaseCard(card);
	}



}
