using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;


//John Karabinos

public class PurchaseZone : MonoBehaviour, IPointerClickHandler  {


	public void OnPointerClick(PointerEventData eventData){


		GameObject card = eventData.pointerEnter;
		purchaseCard(card);
	}


	
	public void purchaseCard(GameObject card){
		
		Transform canvas = this.transform.parent.parent;
		GameLogic gameLogic = canvas.GetComponent<GameLogic>();
		CardObject cardObject = card.GetComponent<CardObject>();
		if(cardObject.isPurchasable == true && cardObject.pileCount > 0){
			Transform panel = this.transform.parent;
			PurchasePanel pp = panel.GetComponent<PurchasePanel>();
			gameLogic.purchaseCard(cardObject.id, pp.purchasePanelName);
		}	

	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
