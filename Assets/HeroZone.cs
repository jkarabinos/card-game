using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class HeroZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

	public bool isFriendly;
	public int numHeroes =0;
	int siblingIndex = 0;

	GameObject placeholder = null;
	CardObject cardScript;

	public void OnPointerEnter(PointerEventData eventData){
		//Debug.Log("OnPointerEnter");
		 Debug.Log ("OnBeginDrag");

		

		Debug.Log ("OnEndDrag");
		//if(String.Compare(c.type , "hero") == 0){
			placeholder = new GameObject();
			placeholder.transform.SetParent( this.transform );
			LayoutElement le = placeholder.AddComponent<LayoutElement>();
			le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
			le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
			le.flexibleWidth = 0;
			le.flexibleHeight = 0;


			//the original parent is the hand, the new parent might change if we move the card
			
		//}
		

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

		Transform tabletop = this.transform.parent;
		DropZone dropZone = tabletop.GetComponent<DropZone>();


		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(c != null){
			
			if(String.Compare(c.type, "hero") == 0){
				if(isFriendly == true){
					if(numHeroes < 5){
						d.newParent = this.transform;	
						numHeroes++;
					}
				}		
			}
		}
		
	}


}
