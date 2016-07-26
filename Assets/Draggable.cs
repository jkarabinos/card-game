using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//test

public class Draggable : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {


	

	public Transform originalParent = null;
	public Transform newParent = null;
	public bool inHeroZone = false;

	public GameObject placeholder = null;
	public GameObject heroPlaceholder = null;

	CardObject cardScript;


	public void OnBeginDrag(PointerEventData eventData){
		Debug.Log ("OnBeginDrag");

		//if the card is not moveable, do nothing
		GameObject card = eventData.pointerPress;
		cardScript = card.GetComponent<CardObject>();
		if(!cardScript.isDraggable){
			return;
		}
		Transform canvas = this.transform.parent.parent;
		GameLogic gameLogic = canvas.GetComponent<GameLogic>();
		HeroZone friendlyHeroZone = gameLogic.getFriendlyHeroZone();
		HeroZone enemyHeroZone = gameLogic.getEnemyHeroZone();
		friendlyHeroZone.selectedCard = card;
		enemyHeroZone.selectedCard = card;

		
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

		this.transform.SetParent( this.transform.parent.parent );


		GetComponent<CanvasGroup>().blocksRaycasts = false;
	}

	public void OnDrag(PointerEventData eventData){
		//Debug.Log ("OnDrag");
		if(!cardScript.isDraggable){
				return;
			}
			
		this.transform.position = eventData.position;

		int newSiblingIndex = originalParent.childCount;
		if(inHeroZone == false){
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
		else if(inHeroZone == true){
			
			Transform canvas = originalParent.parent;
			GameLogic gameLogic = canvas.GetComponent<GameLogic>();
			HeroZone friendlyHeroZone = gameLogic.getFriendlyHeroZone();
			int heroSiblingIndex = friendlyHeroZone.transform.childCount;

			

			for (int i = 0; i < friendlyHeroZone.transform.childCount; i++) {
				if(this.transform.position.x < friendlyHeroZone.transform.GetChild(i).position.x){
					heroSiblingIndex = i;
					if (heroPlaceholder.transform.GetSiblingIndex() < heroSiblingIndex)
						heroSiblingIndex--;
						break;
					}
				}
			
			heroPlaceholder.transform.SetSiblingIndex(heroSiblingIndex);
		}
			
	}
	


	public void createHeroPlaceholder(){
			
			Transform canvas = originalParent.parent;
			GameLogic gameLogic = canvas.GetComponent<GameLogic>();
			HeroZone friendlyHeroZone = gameLogic.getFriendlyHeroZone();

			heroPlaceholder = new GameObject();
			heroPlaceholder.transform.SetParent( friendlyHeroZone.transform );
			LayoutElement le = heroPlaceholder.AddComponent<LayoutElement>();
			le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
			le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
			le.flexibleWidth = 0;
			le.flexibleHeight = 0;

	}

	public void OnEndDrag(PointerEventData eventData){
		Debug.Log ("EndDrag");

		Transform canvas = originalParent.parent;
		GameLogic gameLogic = canvas.GetComponent<GameLogic>();
		HeroZone friendlyHeroZone = gameLogic.getFriendlyHeroZone();
		HeroZone enemyHeroZone = gameLogic.getEnemyHeroZone();
		enemyHeroZone.selectedCard = null;
		friendlyHeroZone.selectedCard = null;

		if(!cardScript.isDraggable){
			return;
		}
		
		if(originalParent == newParent){
			//if the card will remain in the hand of the user, keep it at the same sibling index
			this.transform.SetParent( newParent );
			this.transform.SetSiblingIndex( placeholder.transform.GetSiblingIndex() );
			Destroy(placeholder);
		}

		

		/*if(inHeroZone){
			this.transform.SetSiblingIndex (heroPlaceholder.transform.GetSiblingIndex());
		}

		//to keep the card in the same place in the hand
		if(originalParent == newParent){
			//if the card will remain in the hand of the user, keep it at the same sibling index
			//this.transform.SetSiblingIndex( siblingIndex );
			this.transform.SetSiblingIndex( placeholder.transform.GetSiblingIndex() );

		}

		else{
			cardScript.isDraggable = false;
		}
		Destroy(placeholder);
		Destroy(heroPlaceholder);*/

		GetComponent<CanvasGroup>().blocksRaycasts = true;
	}



}
