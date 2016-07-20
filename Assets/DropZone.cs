using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

	public List<GameObject> deck;
	public string zoneName;
	public List<GameObject> discardPile;
	public CardObject.Type typeOfCard = CardObject.Type.ACTION;
	

	public void OnPointerEnter(PointerEventData eventData){
		//Debug.Log("OnPointerEnter");


	}

	public void OnPointerExit(PointerEventData eventData){
		//Debug.Log("OnPointerExit");
	}


	public void OnDrop(PointerEventData eventData){

		CardObject c = eventData.pointerDrag.GetComponent<CardObject>();
		if(c == null){
			return;
		}

		if(!c.isDraggable){
			return;
		}

		//the user is not allowed to play cards onto the played this turn zone
		if(String.Compare(zoneName, "PlayedThisTurn") == 0){
			return;
		}

		Transform canvas = this.transform.parent;
		GameLogic gameLogic = canvas.GetComponent<GameLogic>();

		
		if(String.Compare(c.type, "action") == 0){
			//if the user attempts to play an action card when they have no more actions
			if(gameLogic.totalActions <= 0){
				return;
			} 
			//the user must target a legal target if the action can do damage
			if(c.damage > 0){
				return;
			}
		}

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		//if(c != null){
		
		d.newParent = dropZoneForName("PlayedThisTurn").transform;
		gameLogic.playCard(c);
					
							

		//}
		
	}


	
	//returns a DropZone script for the given name
	public DropZone dropZoneForName(string name){
		Transform canvas = this.transform.parent;
		foreach(Transform child in canvas){
			DropZone dropZone = child.GetComponent<DropZone>();
			if (dropZone != null){
				if (String.Compare (name, dropZone.zoneName) == 0){
					return dropZone;
				}
			}
		}
		return null;
	}


}
