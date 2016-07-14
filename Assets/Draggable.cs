using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//test

public class Draggable : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {


	

	Transform originalParent = null;
	public Transform newParent = null;
	int siblingIndex = 0;
	GameObject placeholder = null;

	//the type of card
	public enum Type { ACTION, ATTACK, MONSTER, HERO, BUILDING, TREASURE }
	public Type typeOfCard = Type.TREASURE;

	//the coin value of the card
	public int value;

	//the cost of the card
	public int cost = 0;

	//the damage that the card does when played
	public int damage;

	//initialize the properties of the card
	void Start(){
		//value = 1;
		typeOfCard = Type.TREASURE;
	}

	

	public void OnBeginDrag(PointerEventData eventData){
		Debug.Log ("OnBeginDrag");

		placeholder = new GameObject();
		placeholder.transform.SetParent( this.transform.parent );
		LayoutElement le = placeholder.AddComponent<LayoutElement>();
		le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
		le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
		le.flexibleWidth = 0;
		le.flexibleHeight = 0;


		//the original parent is the hand, the new parent might change if we move the card
		originalParent = this.transform.parent;
		newParent = originalParent;

		siblingIndex = this.transform.GetSiblingIndex();

		this.transform.SetParent( this.transform.parent.parent );


		GetComponent<CanvasGroup>().blocksRaycasts = false;
	}

	public void OnDrag(PointerEventData eventData){
		//Debug.Log ("OnDrag");

		this.transform.position = eventData.position;

		int newSiblingIndex = originalParent.childCount;

		for (int i = 0; i < originalParent.childCount; i++) {
			if(this.transform.position.x < originalParent.GetChild(i).position.x){

				newSiblingIndex = i;

				if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
					newSiblingIndex--;

				break;
			}
		}
		placeholder.transform.SetSiblingIndex(newSiblingIndex);
	}

	public void OnEndDrag(PointerEventData eventData){
		Debug.Log ("EndDrag");
		

		this.transform.SetParent( newParent );

		//to keep the card in the same place in the hand
		if(originalParent == newParent){
			//if the card will remain in the hand of the user, keep it at the same sibling index
			//this.transform.SetSiblingIndex( siblingIndex );
			this.transform.SetSiblingIndex( placeholder.transform.GetSiblingIndex() );


		}
		Destroy(placeholder);

		GetComponent<CanvasGroup>().blocksRaycasts = true;
	}

}
