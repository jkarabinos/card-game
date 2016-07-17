using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

	public List<GameObject> deck;
	public string zoneName;
	public List<GameObject> discardPile;
	public CardObject.Type typeOfCard = CardObject.Type.ACTION;


	public void OnPointerEnter(PointerEventData eventData){
		//Debug.Log("OnPointerEnter");


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


		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(c != null){
			if(typeOfCard == c.typeOfCard){
				d.newParent = dropZoneForName("Tabletop").transform;

				//if the card is a treasure card
				if(c.typeOfCard == CardObject.Type.TREASURE){
					Text textBox = this.transform.parent.GetComponentInChildren<Text>();
					int currentValue = int.Parse(textBox.text);
					currentValue += c.value;
					textBox.text  = currentValue.ToString();

				}
				
			}
		}
		
	}

	//draws a card and checks to see if the deck is empty
	public void drawCard(){
		if (deck.Count <= 0){
			int cardCount = discardPile.Count;
			for( int i = 0; i <cardCount; i ++ ){
				deck.Add(discardPile[0]);
				discardPile.RemoveAt(0);
			}
			shuffleDeck();

		}
		GameObject card = deck[deck.Count - 1];
		deck.RemoveAt(deck.Count - 1);		

		card.transform.SetParent( this.transform );
	}


	public void startGame(){
		initializeDeck();
		initializeDiscard();
		shuffleDeck();
		drawHand();
	}

	//set the deck with 7 coppers and 3 arrows to start the game
	public void initializeDeck(){
		deck = new List<GameObject>();

		CardDictionary cardDictionary = new CardDictionary();
		cardDictionary.readFile();
		Dictionary< string, Dictionary<string, string> > globalDict = cardDictionary.globalDictionary;
		Debug.Log (globalDict.Count + " is the number of cards");
		
		for( int i = 0; i < 10; i++ ){

			
			GameObject card;

			CardObject cardScript;

			if( i < 7){
				card = createCardForId(0, globalDict);
				cardScript = card.GetComponent<CardObject>();
				cardScript.typeOfCard = CardObject.Type.TREASURE;

			}else{
				card = createCardForId(1, globalDict);
				cardScript = card.GetComponent<CardObject>();
				cardScript.typeOfCard = CardObject.Type.ATTACK;
			}

			cardScript.isDraggable = true;
			cardScript.isPurchasable = false;

			/*GameObject card = (GameObject)Instantiate(Resources.Load("Card"));
			var cardScript = card.GetComponent<CardObject>();
			cardScript.value = 1;*/

		

			/*Sprite spr = Resources.Load <Sprite> ("card_game/copper");

			//for arrows
			if(i>6){
				spr = Resources.Load <Sprite> ("card_game/arrow");
				cardScript.value = 0;
				cardScript.damage = 1;
			}

			Image cardImage = card.GetComponent<Image>();
			cardImage.sprite = spr;*/



			deck.Add(card); 
		}
			}

	//create a discard pile to store used cards
	public void initializeDiscard(){
		discardPile = new List<GameObject>();
	}

	public GameObject createCardForId(int id, Dictionary< string, Dictionary<string, string> > globalDict){
		//get the dictionary for the individual card, this will hold values like cost and damage
		string idString = id.ToString();
		Dictionary<string, string> individualCardDict;
		individualCardDict = globalDict[idString];

		//set the basic properties of the card
		GameObject card = (GameObject)Instantiate(Resources.Load("Card"));
		var cardScript = card.GetComponent<CardObject>();
		cardScript.value = int.Parse(individualCardDict["value"]);
		cardScript.damage = int.Parse(individualCardDict["damage"]);
		
		//set the appropriate image of the card
		Sprite spr = Resources.Load <Sprite> (individualCardDict["imagePath"]);
		Image cardImage = card.GetComponent<Image>();
		cardImage.sprite = spr;


		return card;
	}

	//arrange the cards in the deck array in a random order
	public void shuffleDeck(){
		List <GameObject> tempDeck = new List<GameObject>();
		int cardCount = deck.Count;

		for( int i = 0; i <cardCount; i ++ ){
			tempDeck.Add(deck[0]);
			deck.RemoveAt(0);
		}

		System.Random rnd = new System.Random();
		
		

		for( int i = 0; i < cardCount; i++ ){
			int randIndex = rnd.Next(0, tempDeck.Count); 
			GameObject card = tempDeck[randIndex];
			tempDeck.RemoveAt(randIndex);
			deck.Add(card);			

		}
		

	}



	public void drawHand(){
		//draw a hand of five cards at the end of turn or the beginning of the game
		for( int i = 0; i < 5; i++ ){
			drawCard();
		}


	}

	//returns a DropZone script for the given name
	public DropZone dropZoneForName(string name){
		Transform canvas = this.transform.parent;
		foreach(Transform child in canvas){
			DropZone dropZone = child.GetComponent<DropZone>();
			if (dropZone != null){
				if (String.Compare (name, dropZone.zoneName) == 0){
					return dropZone;
				}
			}
		}
		return null;
	}

	//clears a dropzone for the given dropZone
	public void clearDropZone(DropZone dropZone){
		var childList = new List<Transform>();
		foreach(Transform child in dropZone.transform){
			discardPile.Add(child.gameObject);
			childList.Add(child);
		}
		while(childList.Count > 0){
			childList[0].SetParent(null);
			childList.RemoveAt(0);			
		}
	}

	//Ends your turn.  Clears the tabletop and hand.  Draws a new hand.
	public void endTurn(){
		DropZone tabletop = dropZoneForName("Tabletop");
		clearDropZone(tabletop);
		clearDropZone(this);
		Debug.Log("the number of cards in the discard pile is " + discardPile.Count);
		drawHand();
	}


}
