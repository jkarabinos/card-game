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

	GameObject placeholder = null;
	CardObject cardScript;

	public void OnPointerEnter(PointerEventData eventData){
		//Debug.Log("OnPointerEnter");

		
		if(selectedCard != null){
			CardObject c = selectedCard.GetComponent<CardObject>();
			if(String.Compare(c.type , "hero") == 0){
				Draggable draggable = selectedCard.GetComponent<Draggable>();
				draggable.inHeroZone = true;
				draggable.createHeroPlaceholder();
			}
		}

		/*
		Debug.Log ("OnEndDrag");
		//if(String.Compare(c.type , "hero") == 0){
			placeholder = new GameObject();
			placeholder.transform.SetParent( this.transform );
			LayoutElement le = placeholder.AddComponent<LayoutElement>();
			le.preferredWidth = selectedCard.GetComponent<LayoutElement>().preferredWidth;
			le.preferredHeight = selectedCard.GetComponent<LayoutElement>().preferredHeight;
			le.flexibleWidth = 0;
			le.flexibleHeight = 0;
			
			//this.transform.SetParent( this.transform.parent.parent );


			int newSiblingIndex = this.transform.childCount;

			for (int i = 0; i < this.transform.childCount; i++) {
				if(selectedCard.transform.position.x < this.transform.GetChild(i).position.x){

					newSiblingIndex = i;

					if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
						newSiblingIndex--;

					break;
				}
			}
			placeholder.transform.SetSiblingIndex(newSiblingIndex);
		}

			//the original parent is the hand, the new parent might change if we move the card
			
		//}
		*/
		

	}

	public void OnPointerExit(PointerEventData eventData){
		//Debug.Log("OnPointerExit");
		if(selectedCard != null){
			Draggable draggable = selectedCard.GetComponent<Draggable>();
			if(draggable.inHeroZone == true){
				draggable.inHeroZone = false;
				Destroy(draggable.heroPlaceholder);
			}
		}
		
		Destroy(placeholder);
		
		
	}


	public void OnDrop(PointerEventData eventData){
		

		

		Debug.Log (eventData.pointerDrag.name + "OnDrop to " + gameObject.name);

		CardObject c = eventData.pointerDrag.GetComponent<CardObject>();
		if(!c.isDraggable){
			return;
		}

		Transform tabletop = this.transform.parent;
		DropZone dropZone = tabletop.GetComponent<DropZone>();


		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(c != null){
			
			if(String.Compare(c.type, "hero") == 0){
				if(isFriendly == true){
					if(numHeroes < 5){
						Destroy(placeholder);
						//Destroy(heroPlaceholder)
						d.newParent = this.transform;	
						numHeroes++;
					}
				}		
			}
		}
		
	}


}
