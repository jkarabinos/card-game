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
		var draggable = card.GetComponent<Draggable>();
		
		if ( draggable.cost > 3 ){
			//for testing purposes
			Debug.Log( "I can buy this card" );
		}else{
			Debug.Log( "this card costs 3 or less" ); 
		}

	}



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
