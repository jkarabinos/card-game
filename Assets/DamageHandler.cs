using UnityEngine;
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
		deadMonster.SetParent(null);
	}

}
