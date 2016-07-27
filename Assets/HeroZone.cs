using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class HeroZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

	public bool isFriendly;
	public int numHeroes =0;
	public GameObject selectedCard = null;

	CardObject cardScript;


	public void OnPointerEnter(PointerEventData eventData){
		//Debug.Log("OnPointerEnter");

		Transform tabletop = this.transform.parent;
		Transform canvas = tabletop.parent;
		GameLogic gl = canvas.GetComponent<GameLogic>();
		/*if(!gl.interactionEnabled){
			return;
		}*/

		
		if(selectedCard != null && gl.interactionEnabled){
			CardObject c = selectedCard.GetComponent<CardObject>();
			if(String.Compare(c.type , "heroCards") == 0){
				Draggable draggable = selectedCard.GetComponent<Draggable>();
				draggable.inHeroZone = true;
				draggable.createHeroPlaceholder();
			}else{
				//if the user is attempting to play something other than a hero
				Debug.Log("the user is attempting to play something other than a hero");
				
			}
		}

	
		

	}

	public void OnPointerExit(PointerEventData eventData){
		//Debug.Log("OnPointerExit");

		Transform tabletop = this.transform.parent;
		Transform canvas = tabletop.parent;
		GameLogic gl = canvas.GetComponent<GameLogic>();
		/*if(!gl.interactionEnabled){
			return;
		}*/


		if(selectedCard != null && gl.interactionEnabled){
			Draggable draggable = selectedCard.GetComponent<Draggable>();
			if(draggable.inHeroZone == true){
				draggable.inHeroZone = false;
				Destroy(draggable.heroPlaceholder);
			}
		}
		

		//Debug.Log("the pointer has exited");

		
	}


	public void OnDrop(PointerEventData eventData){
		
		Debug.Log("Attempting to play a card on the hero zone");
		

		//Debug.Log (eventData.pointerDrag.name + "OnDrop to " + gameObject.name);

		CardObject c = eventData.pointerDrag.GetComponent<CardObject>();
		if(!c.isDraggable){
			return;
		}

		Transform tabletop = this.transform.parent;
		DropZone dropZone = tabletop.GetComponent<DropZone>();

		Transform canvas = tabletop.parent;
		GameLogic gl = canvas.GetComponent<GameLogic>();
		if(!gl.interactionEnabled){
			return;
		}

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(c != null){
			
			if(String.Compare(c.type, "heroCards") == 0){
				if(isFriendly == true){
					if(numHeroes < 5){

						gl.playHero(c, d.heroPlaceholder.transform.GetSiblingIndex());
						//Destroy(heroPlaceholder)
						//d.newParent = this.transform;
						//d.transform.SetSiblingIndex(siblingIndex);
						//c.attacks = 1; // if the hero has charge
						//numHeroes++;
					}
				}		
			}else{
				//if the user is attempting to drop something other than a hero
				dropZone.OnDrop(eventData);

			}
		}
		
	}


}
