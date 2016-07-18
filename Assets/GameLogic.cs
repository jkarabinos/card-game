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
	public int totalBuys;

	//the list of cards that the user has chosen to include in their deck
	public List<int> userBuild;
	
	public void purchaseCard (int cardID){
		GameObject card = createCardForId(cardID, globalDict);
		CardObject cardScript = card.GetComponent<CardObject>();
		int costOfCard = cardScript.cost;
		if(totalBuys > 0){
			if(totalCoin >= costOfCard){

				//build a building
				if(String.Compare(cardScript.type, "building")==0){
					gainBuilding(card);
				}
				//gain any other type of card
				else{
					gainCard(card);
					updateMoneyCounter(-costOfCard);
					totalBuys --;
				}
				 
			}else{
				Destroy(card);
			}
		}else{
			Destroy(card);
		}
	}

	//place the new building in the earliest available buildingzone
	public void gainBuilding(GameObject building){
		BuildingZone friendlyBuildingZone = getFriendlyBuildingZone();
		friendlyBuildingZone.gainBuilding(building, this);
	}


	//add the hand to the just played zone and later the player's deck
	public void gainCard(GameObject card){
		DropZone playedThisTurn = dropZoneForName("PlayedThisTurn");
		card.transform.SetParent(playedThisTurn.transform);

	}

	//draws the correct number of cards if a player plays a card that draws cards
	public void drawDuringTurn(int drawNumber){
		for (int i = 0; i < drawNumber; i++ ){
		drawCard();
		}
	}

	//updates totalBuys.  Is called from dropzone like drawDuringTurn()
	public void updateBuys(int buyNumber){
		totalBuys += buyNumber;
	}

	//is called in the DropZone script when a player drops a treasure onto the tabletop.  Updates the total coin and textual representation
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
		initializeBuild();
		initializeDeck();
		initializeDiscard();
		shuffleDeck();
		setAllPurchasePanels();
		totalCoin = 0;
		totalBuys = 1;
		initializeMonsters();

		drawHand();
	}

	//initializes monsters
	void initializeMonsters(){
		List<int> listOfMonsters = new List<int>(new int[] {7, 8, 9});
		MonsterZone monsterZone = this.transform.GetComponentInChildren<MonsterZone>();
		monsterZone.initializeMonsterZone(this ,listOfMonsters);
	}

	//set the global build variable that defines the list of cards that the user has chosen
	void initializeBuild(){
		userBuild = new List<int>();

		//always add copper, silver, gold and both types of arrows
		for( int i = 0; i < 5; i++ ){
			userBuild.Add(i);
		}

		//temporary, for testing purposes
		userBuild.Add(6);
		userBuild.Add(5);

	}

	void setPurchase(List<int> cardsChosen, string purchasePanelName){

		PurchasePanel purchasePanel = purchasePanelForName(purchasePanelName);
		purchasePanel.initializePurchasePanel(this, cardsChosen);
	}

	void setAllPurchasePanels(){
		List<int> friendlyList = new List<int>(new int[] {6, 10});
		List<int> neutralList = new List<int>(new int[] {0,1,2,3,4});
		List<int> enemyList = new List<int>(new int[] {5});
		setPurchase( friendlyList , "FriendlyPurchasePanel");
		setPurchase( neutralList, "NeutralPurchasePanel");
		setPurchase( enemyList, "EnemyPurchasePanel");
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
		cardScript.buys = int.Parse(individualCardDict["buys"]);
		cardScript.type = individualCardDict["type"];
		cardScript.id = id;

		if(String.Compare(cardScript.type, "monster") == 0){
			cardScript.health = int.Parse(individualCardDict["health"]);
			cardScript.power = int.Parse(individualCardDict["power"]);
		}
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

	//returns a purchase panel for the given name
	public PurchasePanel purchasePanelForName(string name){
		foreach(Transform child in this.transform){
			PurchasePanel purchasePanel = child.GetComponent<PurchasePanel>();
			if (purchasePanel != null){
				if (String.Compare (name, purchasePanel.purchasePanelName) == 0){
					return purchasePanel;
				}
			}
		}
		return null;
	}

	//returns a purchase panel for the given name
	public BuildingZone getFriendlyBuildingZone(){
		DropZone tabletopDropZone = dropZoneForName("Tabletop");
		foreach(Transform child in tabletopDropZone.transform){
			BuildingZone buildingZone = child.GetComponent<BuildingZone>();
			if (buildingZone != null){
				if (buildingZone.isFriendly){
					return buildingZone;
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
		DropZone playedThisTurn = dropZoneForName("PlayedThisTurn");
		clearDropZone(playedThisTurn);
		clearDropZone(hand);
		Debug.Log("the number of cards in the discard pile is " + discardPile.Count);
		updateMoneyCounter(-totalCoin);
		totalBuys = 1;
		drawHand();
	}
}
