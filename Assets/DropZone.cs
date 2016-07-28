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

		//if the user attempts to play the card on something other than the tabletop
		if(String.Compare(zoneName, "Tabletop") != 0){
			return;
		}

		CardObject c = eventData.pointerDrag.GetComponent<CardObject>();
		if(c == null){
			return;
		}

		if(!c.isDraggable || String.Compare(c.type, "heroCards") == 0 || String.Compare(c.canTarget, "nothing") != 0){
			return;
		}

		Transform canvas = this.transform.parent;
		GameLogic gameLogic = canvas.GetComponent<GameLogic>();

		//if user interaction is not enabled (other players turn or waiting for response)
		if(!gameLogic.interactionEnabled){
			return;
		}

		
		if(String.Compare(c.type, "actionCards") == 0){
			//if the user attempts to play an action card when they have no more actions
			if(gameLogic.totalActions <= 0){
				return;
			} 
		}

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		d.newParent = canvas;
		//if(c != null){
		
		//d.newParent = dropZoneForName("PlayedThisTurn").transform;
		gameLogic.playCard(c);
					
							

		//}
		
	}

	//remove any placeholders associated with the cards on the dropzone
	public void removeAllPlaceholders(){

		//destory everything that is not a card in the user's hand
		foreach(Transform child in this.transform){
			Draggable d = child.GetComponent<Draggable>();
			if(d == null){
				Destroy(child.gameObject);
			}
		}
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
