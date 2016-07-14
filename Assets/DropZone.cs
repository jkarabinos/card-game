using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

	public GameObject[] deck;
	public int cardCount = 10;


	public Draggable.Type typeOfCard = Draggable.Type.ACTION;


	public void OnPointerEnter(PointerEventData eventData){
		//Debug.Log("OnPointerEnter");


	}

	public void OnPointerExit(PointerEventData eventData){
		//Debug.Log("OnPointerExit");
	}


	public void OnDrop(PointerEventData eventData){
		Debug.Log (eventData.pointerDrag.name + "OnDrop to " + gameObject.name);

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(d != null){
			if(typeOfCard == d.typeOfCard){
				d.newParent = this.transform;

				//if the card is a treasure card
				if(d.typeOfCard == Draggable.Type.TREASURE){
					Text textBox = this.transform.parent.GetComponentInChildren<Text>();
					int currentValue = int.Parse(textBox.text);
					currentValue += d.value;
					textBox.text  = currentValue.ToString();
				}
			}
		}
		
	}

	public void drawCard(){
		GameObject card = deck[cardCount-1];
		cardCount --; 

		card.transform.SetParent( this.transform );
	}


	public void startGame(){
		initializeDeck();
		shuffleDeck();
		drawHand();
	}

	//set the deck with 7 coppers and 3 arrows to start the game
	public void initializeDeck(){
		deck = new GameObject[cardCount];
		
		for( int i = 0; i < cardCount; i++ ){

			GameObject card = (GameObject)Instantiate(Resources.Load("Card"));
			var draggable = card.GetComponent<Draggable>();
			draggable.value = 1;

			Sprite spr = Resources.Load <Sprite> ("card_game/copper");

			//for arrows
			if(i>6){
				spr = Resources.Load <Sprite> ("card_game/arrow");
				draggable.value = 0;
				draggable.damage = 1;
			}

			Image cardImage = card.GetComponent<Image>();
			cardImage.sprite = spr;

			deck[i] = card; 
		}
	}

	//arrange the cards in the deck array in a random order
	public void shuffleDeck(){
		//GameObject[] tempDeck1 = new GameObject[cardCount];
		var tempDeck = new List<GameObject>(deck);

		System.Random rnd = new System.Random();
		

		for( int i = 0; i < cardCount; i++ ){
			int randIndex = rnd.Next(0, tempDeck.Count); 
			GameObject card = tempDeck[randIndex];
			tempDeck.RemoveAt(randIndex);
			deck[i] = card;
			

		}
		/*

		for(int i = 0; i < cardCount; i++ ){
			int index = i;
			GameObject[] tempDeck2 = new GameObject[cardCount-i];
			Array.Copy(tempDeck1, 0, tempDeck2, 0, i);
			//Array.Copy(tempDeck1, index + 1, tempDeck2, index, tempDeck2.Length - index - 1);
			//GameObject card = tempDeck1[index];
			//tempDeck1 = tempDeck2;

			//deck[i] = card;
		}
*/


		//Random rnd = new Random();
		//int randIndex = rnd.Next(0, cardCount); 
	}



	public void drawHand(){
		//draw a hand of five cards at the end of turn or the beginning of the game
		for( int i = 0; i < 5; i++ ){
			drawCard();
		}


	}

}
