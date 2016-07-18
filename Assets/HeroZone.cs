using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class HeroZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {



	public void OnPointerEnter(PointerEventData eventData){
		//Debug.Log("OnPointerEnter");


	}

	public void OnPointerExit(PointerEventData eventData){
		//Debug.Log("OnPointerExit");
	}


	public void OnDrop(PointerEventData eventData){
		

		

		/*Debug.Log (eventData.pointerDrag.name + "OnDrop to " + gameObject.name);

		CardObject c = eventData.pointerDrag.GetComponent<CardObject>();
		if(!c.isDraggable){
			return;
		}

		Transform canvas = this.transform.parent;
		GameLogic gameLogic = canvas.GetComponent<GameLogic>();


		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(c != null){
			if(typeOfCard == c.typeOfCard){
				d.newParent = dropZoneForName("PlayedThisTurn").transform;
				increaseTotalCoin(c);
				gameLogic.drawDuringTurn(c.draw);
				gameLogic.updateBuys(c.buys);				
			}
		}*/
		
	}


}
