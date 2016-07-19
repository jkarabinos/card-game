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
	public int totalActions;
	public List<int> neutralCardList;

	public GameObject selectedHero = null;
	
	//the health of the two players
	public int friendlyHealth;
	public int enemyHealth;


	//the list of cards that the user has chosen to include in their deck
	public List<int> userBuild;
	
	public void purchaseCard (int cardID, string purchasePanelName){
		GameObject card = createCardForId(cardID, globalDict);
		CardObject cardScript = card.GetComponent<CardObject>();
		int costOfCard = cardScript.cost;
		if(totalBuys > 0){
			if(totalCoin >= costOfCard){

				//build a building
				if(String.Compare(cardScript.type, "building")==0){
					gainBuilding(card, purchasePanelName);
				}
				//gain any other type of card
				else{
					gainCard(card, purchasePanelName);
					updateMoneyCounter(-costOfCard);
					updateBuys(-1);
				}
				 
			}else{
				Destroy(card);
			}
		}else{
			Destroy(card);
		}
	}

	//if the card was gained by the user, check to see if we need to replace the neutral 
	//zone with a new random card
	public void didGainCard(GameObject card, string purchasePanelName){

		//if the user gained the card without buying it
		if(purchasePanelName == null){
			return;
		}

		updatePileCount(card, purchasePanelName);

		//if the card was bought from the neutral purchase panel, replace it with a new one
		if(String.Compare(purchasePanelName, "NeutralPurchasePanel") == 0){
			CardObject c = card.GetComponent<CardObject>();
			if( String.Compare(c.rarity, "basic") != 0 ){
				//if the card is not basic
				replaceNeutralCard(c.id);
			}
		}

	}

	//subtract 1 from the pile count of the card
	void updatePileCount(GameObject card, string purchasePanelName){
		if(String.Compare(purchasePanelName, "FriendlyPurchasePanel") == 0 || 
			String.Compare(purchasePanelName, "EnemyPurchasePanel") == 0){

			//if the user has bought from the friendly panel or the enemy panel using some card
			PurchasePanel pp = purchasePanelForName(purchasePanelName);
			CardObject cardCopy = card.GetComponent<CardObject>();
			CardObject cardOnPanel = pp.cardForId(cardCopy.id);
			cardOnPanel.pileCount --;

			if(cardOnPanel.pileCount == 0){
				Destroy(cardOnPanel.gameObject);
				//TODO: grey out the card here
			}
			
		}	
	}

	

	//replace the card with a new one
	void replaceNeutralCard(int cardId){

		PurchasePanel pp = purchasePanelForName("NeutralPurchasePanel");
		foreach(Transform child in pp.transform){
			CardObject c = child.GetComponent<CardObject>();
			if(c.id == cardId){
				//if the cards match
				int siblingIndex = child.GetSiblingIndex();
				Destroy(child.gameObject);
				neutralCardList.Remove(cardId);
				int newCardId = getRandomCard(neutralCardList);
				neutralCardList.Add(newCardId);
				pp.addCard(newCardId, this, siblingIndex);
				break;
			}
		}
	}



	//place the new building in the earliest available buildingzone
	public void gainBuilding(GameObject building, string purchasePanelName){
		BuildingZone friendlyBuildingZone = getFriendlyBuildingZone();
		bool gainedBuilding = friendlyBuildingZone.gainBuilding(building, this);

		if(gainedBuilding){
			didGainCard(building, purchasePanelName);
		}
	}


	//add the hand to the just played zone and later the player's deck
	public void gainCard(GameObject card, string purchasePanelName){
	
		DropZone playedThisTurn = dropZoneForName("PlayedThisTurn");
		card.transform.SetParent(playedThisTurn.transform);
		didGainCard(card, purchasePanelName);
		
	}

	//draws the correct number of cards if a player plays a card that draws cards
	public void drawDuringTurn(int drawNumber){
		for (int i = 0; i < drawNumber; i++ ){
			drawCard();
		}
	}

	//updates totalBuys.  Is called from dropzone like drawDuringTurn()
	public void updateBuys(int buyNumber){
		TextHandler th = textHandlerForName("BuyCounter");
		Text textBox = th.GetComponent<Text>();
		totalBuys += buyNumber;
		textBox.text  = "Buys: " + totalBuys.ToString();
	}

	//is called in the DropZone script when a player drops a treasure onto the tabletop.  Updates the total coin and textual representation
	public void updateMoneyCounter(int money){
		TextHandler th = textHandlerForName("MoneyCounter");
		Text textBox = th.GetComponent<Text>();
		totalCoin += money;
		textBox.text  = "Coin: " + totalCoin.ToString();
	}


	//updates the number of actions for the user and displays it in the appropriate text box
	public void updateActionCounter(int actions){
		TextHandler th = textHandlerForName("ActionCounter");
		Text textBox = th.GetComponent<Text>();
		totalActions += actions;
		textBox.text  = "Actions: " + totalActions.ToString();

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


		CardStack cardDeck = cardStackForName("FriendlyDeck");
		cardDeck.updateCount(deck.Count);
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
		totalBuys = 0;
		totalActions = 0;
		friendlyHealth = 30;
		enemyHealth = 30;


		updateMoneyCounter(0);
		updateBuys(-totalBuys + 1);
		updateActionCounter(-totalActions + 1);
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

		List<int> friendlyList = new List<int>(new int[] {6, 10, 12, 13, 25, 26, 11, 21} );
		neutralCardList = setNeutralList();
		List<int> enemyList = new List<int>(new int[] {5});
		setPurchase( friendlyList , "FriendlyPurchasePanel");
		setPurchase( neutralCardList, "NeutralPurchasePanel");
		setPurchase( enemyList, "EnemyPurchasePanel");
	}


	//randomly set five cards along with the basic 5 cards
	List<int> setNeutralList(){
		List<int> neutralList = new List<int> (new int [] {0,1,2,3,4} );
		for(int i=0; i<5; i++){
			int cardId = getRandomCard(neutralList);
			neutralList.Add(cardId);
		}

		return neutralList;
	}


	//return a random card for the neutral list so far
	//note that for this to work we must begin id-ing the cards at 0 and not skip any numbers
	int getRandomCard(List<int> neutralList){
		string targetRarity = decideTargetRarity();

		//holds all card ids of the target rarity
		List<int> possibleCardIds = new List<int>();

		for(int i = 0; i < globalDict.Count; i ++){
			string idString = i.ToString();
			Dictionary<string, string> individualCardDict = globalDict[idString];
			if(individualCardDict != null){
				if(String.Compare(individualCardDict["rarity"], targetRarity) == 0){
					//if the card is the target rarity, and not a monster, add its id to the possible card list
					if(String.Compare(individualCardDict["type"], "monster") != 0){
						if(!listCotainsInt(neutralList, i)){
							possibleCardIds.Add(i);
						}
					}
				}
			}
		}

		//Debug.Log("the number of possible cards " + possibleCardIds.Count +  " for rarity " + targetRarity);

		//now randomly select a card from the possible card list
		CryptoRandom rnd = new CryptoRandom();
		int randIndex = rnd.Next(0, possibleCardIds.Count); 
		return possibleCardIds[randIndex];

	}

	//returns if an int is contained in a list
	bool listCotainsInt(List<int> list, int a){
		
		for(int i = 0; i < list.Count; i++){
			if(list[i] == a){
				return true;
			}
		}
		return false;
	}


	//randomly determines the rarity of the card for the neutral zone
	string decideTargetRarity(){

		CryptoRandom rnd = new CryptoRandom();
		int randNum = rnd.Next(0, 100); 

		if(randNum < 50){
			return "common";
		}else if(randNum < 75){
			return "rare";
		}else if(randNum < 90){
			return "epic";
		}else {
			return "epic";
		}
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

		CardStack cardDeck = cardStackForName("FriendlyDeck");
		cardDeck.updateCount(deck.Count);
	}


	public CardStack cardStackForName(string name){
		foreach(Transform child in this.transform){
			CardStack cs = child.GetComponent<CardStack>();
			if(cs != null){
				if(String.Compare(name, cs.stackName) == 0){
					return cs;
				}
			}
		}

		return null;
	}

	public TextHandler textHandlerForName(string name){
		foreach(Transform child in this.transform){
			TextHandler t = child.GetComponent<TextHandler>();
			if(t != null){
				if(String.Compare(name, t.textBoxName) == 0){
					return t;
				}
			}
		}

		return null;
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
		cardScript.cardName = individualCardDict["name"];
		cardScript.rarity = individualCardDict["rarity"];
		cardScript.id = id;

		if(String.Compare(cardScript.type, "monster") == 0 
		|| String.Compare(cardScript.type, "hero") == 0
		|| String.Compare(cardScript.type, "building") == 0){
			cardScript.health = int.Parse(individualCardDict["health"]);
		}

		if(String.Compare(cardScript.type, "monster") == 0 
		|| String.Compare(cardScript.type, "hero") == 0 ) {
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

		CryptoRandom rnd = new CryptoRandom();
		
		

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


	public HeroZone getFriendlyHeroZone(){
		DropZone tabletopDropZone = dropZoneForName("Tabletop");
		foreach(Transform child in tabletopDropZone.transform){
			HeroZone heroZone = child.GetComponent<HeroZone>();
			if (heroZone != null){
				if (heroZone.isFriendly){
					return heroZone;
				}
			}
		}
		return null;
	}

	public HeroZone getEnemyHeroZone(){
		DropZone tabletopDropZone = dropZoneForName("Tabletop");
		foreach(Transform child in tabletopDropZone.transform){
			HeroZone heroZone = child.GetComponent<HeroZone>();
			if (heroZone != null){
				if (!heroZone.isFriendly){
					return heroZone;
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

	void updateDiscard(){
		CardStack cardStackDiscard = cardStackForName("FriendlyDiscard");
		cardStackDiscard.updateCount(discardPile.Count);
	}

	//Ends your turn.  Clears the tabletop and hand.  Draws a new hand.
	public void endTurn(){
		DropZone playedThisTurn = dropZoneForName("PlayedThisTurn");
		clearDropZone(playedThisTurn);
		clearDropZone(hand);
		
		updateMoneyCounter(-totalCoin);
		//totalBuys = 1;
		updateBuys(-totalBuys + 1);
		updateActionCounter(-totalActions + 1);
		drawHand();

		updateDiscard();


		//temperory, normally this would be called by input from the server
		startTurn();
	}

	//check for start of turn triggers, note that the hand was already drawn in end turn
	public void startTurn(){
		BuildingZone bz = getFriendlyBuildingZone();
		bz.handleBuildingTriggers(this);

	}



}
