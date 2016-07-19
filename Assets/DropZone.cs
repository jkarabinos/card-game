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
		Debug.Log (eventData.pointerDrag.name + "OnDrop to " + gameObject.name);

		CardObject c = eventData.pointerDrag.GetComponent<CardObject>();
		if(!c.isDraggable){
			return;
		}

		Transform canvas = this.transform.parent;
		GameLogic gameLogic = canvas.GetComponent<GameLogic>();


		if(gameLogic.totalActions <= 0 && String.Compare(c.type, "action") == 0){
			return;
		}

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(c != null){
			if(typeOfCard == c.typeOfCard){
				d.newParent = dropZoneForName("PlayedThisTurn").transform;
				increaseTotalCoin(c);
				gameLogic.drawDuringTurn(c.draw);
				gameLogic.updateBuys(c.buys);
				if(String.Compare(c.type, "action") == 0){
					gameLogic.updateActionCounter(-1);
				}	
							
			}
		}
		
	}

	public void increaseTotalCoin(CardObject c){
		Transform canvas = this.transform.parent;
		GameLogic gameLogic = canvas.GetComponent<GameLogic>();
		gameLogic.updateMoneyCounter(c.value);
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
