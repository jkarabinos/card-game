using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;


//This script handles most of the basic logic of the game

public class GameLogic : MonoBehaviour {

	public List<GameObject> deck;
	public List<GameObject> discardPile;
	public DropZone hand;
	public Dictionary< string, Dictionary<string, string> > globalDict;
	public int totalCoin;
	
	public void purchaseCard (int cardID){
		GameObject card = createCardForId(cardID, globalDict);
		CardObject cardScript = card.GetComponent<CardObject>();
		int costOfCard = cardScript.cost;
		if(totalCoin >= costOfCard){
			DropZone tabletop = dropZoneForName("Tabletop");
			card.transform.SetParent(tabletop.transform);
			updateMoneyCounter(-costOfCard);
		}
		Debug.Log("The card ID is " +cardScript.id);
	}

	public void drawDuringTurn(int drawNumber){
		for (int i = 0; i < drawNumber; i++ ){
		drawCard();
		}
	}

	public void updateMoneyCounter(int money){
		Text textBox = this.transform.GetComponentInChildren<Text>();
		totalCoin += money;
		textBox.text  = totalCoin.ToString();
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
		CardObject cardObject = card.GetComponent<CardObject>();		
		cardObject.isDraggable = true;
		//move a card into the hand once it is drawn
		card.transform.SetParent( hand.transform );
	}

	//set the global hand variable at the start of the game
	void setHand(){
		hand = dropZoneForName("Hand");
	}

	//perform the necessary steps at the beginning of the game
	public void startGame(){
		setHand();
		initializeDeck();
		initializeDiscard();
		shuffleDeck();
		setPurchase();
		totalCoin = 0;
		drawHand();
	}

	void setPurchase(){
		PurchasePanel purchasePanel = this.transform.GetComponentInChildren<PurchasePanel>();
		purchasePanel.initializePurchasePanel(this);
	}

	//set the deck with 7 coppers and 3 arrows to start the game
	public void initializeDeck(){
		deck = new List<GameObject>();

		CardDictionary cardDictionary = new CardDictionary();
		cardDictionary.readFile();
		globalDict = cardDictionary.globalDictionary;
		
		for( int i = 0; i < 10; i++ ){

			
			GameObject card;

			CardObject cardScript;

			if( i < 7){
				card = createCardForId(0, globalDict);
				cardScript = card.GetComponent<CardObject>();
				cardScript.typeOfCard = CardObject.Type.TREASURE;

			}else{
				card = createCardForId(3, globalDict);
				cardScript = card.GetComponent<CardObject>();
				cardScript.typeOfCard = CardObject.Type.ATTACK;
			}

			cardScript.isPurchasable = false;

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
		cardScript.cost = int.Parse(individualCardDict["cost"]);
		cardScript.draw = int.Parse(individualCardDict["draw"]);
		cardScript.id = id;
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
		foreach(Transform child in this.transform){
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
		clearDropZone(hand);
		Debug.Log("the number of cards in the discard pile is " + discardPile.Count);
		updateMoneyCounter(-totalCoin);
		drawHand();
	}
}
