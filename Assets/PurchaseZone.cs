using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;


//John Karabinos

public class PurchaseZone : MonoBehaviour, IPointerClickHandler  {


	public void OnPointerClick(PointerEventData eventData){
		Debug.Log("clicked a card to buy");


		GameObject card = eventData.pointerEnter;
		CardObject cardScript = card.GetComponent<CardObject>();


		if(cardScript.isPurchasable){
			if ( cardScript.cost > 3 ){
				//for testing purposes
				purchaseCard(card);
				
			}else{
				Debug.Log( "this card costs 3 or less" ); 
			}
		}else{
			Debug.Log( "this card is not purchasable" );
		}
		

	}


	//subtract the cost from the user's coins add a copy of the card to the discard pile
	public void purchaseCard(GameObject card){
		Debug.Log( "Purchase the card" ); 


		GameObject card1 = (GameObject)Instantiate(Resources.Load("Card"));


	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
