using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using GameSparks.Core;
//Roman has no dick

//This script handles most of the basic logic of the game

public class GameLogic : MonoBehaviour {

	public Dictionary< string, Dictionary<string, object> > currentHand = new Dictionary< string, Dictionary<string, object> >();
	public Dictionary< string, Dictionary<string, object> > currentFriendlyPlayField = new Dictionary< string, Dictionary<string, object> >();
	public Dictionary< string, Dictionary<string, object> > currentEnemyPlayField = new Dictionary< string, Dictionary<string, object> >();
	public Dictionary< string, Dictionary<string, object> > currentNeutralPurchasePanel = new Dictionary< string, Dictionary<string, object> >();
	public Dictionary< string, Dictionary<string, object> > currentFriendlyHeroZone = new Dictionary< string, Dictionary<string, object> >();
	public Dictionary< string, Dictionary<string, object> > currentEnemyHeroZone = new Dictionary< string, Dictionary<string, object> >();


	public bool isMyTurn;
	public bool interactionEnabled = false;

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
	
	public void purchaseCard (string cardId, string purchasePanelName){
		//attempt to purchase the card with a server call
		Debug.Log("I want to buy the card " + cardId);
		GSChallengeHandler ch = this.transform.GetComponent<GSChallengeHandler>();
		ch.buyCard(cardId, purchasePanelName);
		/*
		GameObject card = createCardForId(cardID, globalDict);
		CardObject cardScript = card.GetComponent<CardObject>();
		int costOfCard = cardScript.cost;
		if(totalBuys > 0){
			if(totalCoin >= costOfCard){
				
				gainCard(card, purchasePanelName);
				updateMoneyCounter(-costOfCard);
				updateBuys(-1);
			
				 
			}else{
				Destroy(card);
			}
		}else{
			Destroy(card);
		}*/
	}

	//play a hero onto the hero zone at the given sibling index
	public void playHero(CardObject card, int siblingIndex){
		GSChallengeHandler ch = this.transform.GetComponent<GSChallengeHandler>();
		Dictionary<string, object> target = new Dictionary<string, object>();
		target.Add("target", "isFalse");
		ch.playCard(card, target);
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


	//add the hand to the just played zone and later the player's deck
	public void gainCard(GameObject card, string purchasePanelName){
	
		DropZone playedThisTurn = dropZoneForName("FriendlyPlayField");
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
		totalBuys = buyNumber;
		textBox.text  = "Buys: " + totalBuys.ToString();
	}

	//is called in the DropZone script when a player drops a treasure onto the tabletop.  Updates the total coin and textual representation
	public void updateMoneyCounter(int money){
		TextHandler th = textHandlerForName("MoneyCounter");
		Text textBox = th.GetComponent<Text>();
		totalCoin = money;
		textBox.text  = "Coin: " + totalCoin.ToString();
	}


	//updates the number of actions for the user and displays it in the appropriate text box
	public void updateActionCounter(int actions){
		TextHandler th = textHandlerForName("ActionCounter");
		Text textBox = th.GetComponent<Text>();
		totalActions = actions;
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
		GSConnectionManager cm = this.transform.GetComponent<GSConnectionManager>();
		cm.authenticateUser();
		//startTheGame();
	}



	public void startTheGame(){
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


	/*void authenticateDevice(){

		new GameSparks.Api.Requests.DeviceAuthenticationRequest().SetDisplayName("Randy").Send((response) => {
			if (!response.HasErrors) {
				Debug.Log("Device Authenticated...");
				startTheGame();
				testSavePlayer();
			} else {
				Debug.Log("Error Authenticating Device...");
				
			}
		});
	}*/



	

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

	public GameObject createCardForStats(Dictionary< string, object > stats, string key){
		Debug.Log("the type of value is " + stats["value"].GetType());

		//set the basic properties of the card
		GameObject card = (GameObject)Instantiate(Resources.Load("Card"));

		var cardScript = card.GetComponent<CardObject>();
		cardScript.value = Convert.ToInt32( stats["value"] );
		cardScript.damage = Convert.ToInt32( stats["damage"] );
		cardScript.cost = Convert.ToInt32( stats["cost"] );
		cardScript.draw = Convert.ToInt32( stats["draw"] );
		cardScript.buys = Convert.ToInt32( stats["buys"] );
		cardScript.type = (string) stats["cardType"];
		//cardScript.cardName = individualCardDict["name"];
		cardScript.rarity = (string) stats["rarity"];
		cardScript.cardId = key;
		//cardScript.id = id;

		if(String.Compare(cardScript.type, "monsterCards") == 0 
		|| String.Compare(cardScript.type, "heroCards") == 0){
			cardScript.health = Convert.ToInt32( stats["health"] );
			cardScript.power = Convert.ToInt32( stats["power"] );
			if(String.Compare(cardScript.type, "heroCards") == 0){
				cardScript.attacks = Convert.ToInt32( stats["attacks"] );
			}
		}

		//set the appropriate image of the card
		string imagePath = (string) stats["imagePath"];
		Sprite spr = Resources.Load <Sprite> (imagePath);
		Image cardImage = card.GetComponent<Image>();
		cardImage.sprite = spr;


		return card;
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

	public GameObject getBorderForCard(CardObject card){
		foreach(Transform child in card.transform){
			HighlightHandler h = child.GetComponent<HighlightHandler>();
			if(h != null){
				return child.gameObject;
			}
			
		}
		return null;
	}

	public void setSelected(CardObject card){
		
		GameObject border = getBorderForCard(card);

		Sprite spr = Resources.Load <Sprite> ("card_game/selected_border2");
		Image cardBorder = border.GetComponent<Image>();
		cardBorder.sprite = spr;

		//border.transform.SetParent(card.transform);
		//selectedHero = card;
	}

	public void setAttackable(CardObject card){
		Debug.Log("attempting to set the card selected");

		GameObject border = getBorderForCard(card);
		if(border == null){
			border = (GameObject)Instantiate(Resources.Load("Border"));

			Sprite spr = Resources.Load <Sprite> ("card_game/selected_border");
			Image cardImage = border.GetComponent<Image>();
			cardImage.sprite = spr;

			border.transform.SetParent(card.transform);
		}
		
		//selectedHero = card;
	}

	public void removeSelected(CardObject card){
		foreach(Transform child in card.transform){
			HighlightHandler h = child.GetComponent<HighlightHandler>();
			if(h != null){
				Destroy(child.gameObject);
			}
			
		}
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

	//remove all of the selected borders on the hero card
	public void removedAllSelected(){
		HeroZone hz = getFriendlyHeroZone();
		foreach(Transform child in hz.transform){
			CardObject c = child.GetComponent<CardObject>();
			if(c != null){
				removeSelected(c);
			}
		}
	}

	//Ends your turn.  Clears the tabletop and hand.  Draws a new hand.
	public void endTurn(){

		GSChallengeHandler ch = this.transform.GetComponent<GSChallengeHandler>();
		ch.endTurnRequest();

		/*
		DropZone playedThisTurn = dropZoneForName("PlayedThisTurn");
		clearDropZone(playedThisTurn);
		clearDropZone(hand);
		
		updateMoneyCounter(-totalCoin);
		//totalBuys = 1;
		updateBuys(-totalBuys + 1);
		updateActionCounter(-totalActions + 1);
		removedAllSelected();
		drawHand();

		updateDiscard();


		//temperory, normally this would be called by input from the server
		startTurn();*/
	}

	
	public void startTurn(){
	

		//set the number of attacks that a hero can make at the beginning of the turn
		setHeroAttacks();

	}

	void setHeroAttacks(){
		HeroZone heroZone = getFriendlyHeroZone();
		foreach(Transform child in heroZone.transform){
			CardObject c = child.GetComponent<CardObject>();
			if(c != null){
				setAttackable(c);
				c.attacks = 1;
			}
		}
	}

	//add the default card effects to the game scene
	public void playCard(CardObject c){

		/*updateMoneyCounter(c.value);
		drawDuringTurn(c.draw);
		updateBuys(c.buys);
		if(String.Compare(c.type, "action") == 0){
			updateActionCounter(-1);
		}*/

		//play a card that does not require a target
		GSChallengeHandler ch = this.transform.GetComponent<GSChallengeHandler>();
		Dictionary<string, object> target = new Dictionary<string, object>();
		target.Add("target", "isFalse");
		ch.playCard(c, target);
	}


	//set the appropriate player to this health visually
	void setPlayerHealth(int health, bool isFriendly){

		foreach(Transform child in this.transform){
			DamageHandler dh = child.GetComponent<DamageHandler>();
			if(dh!= null){
				if(dh.isCastle && dh.isFriendly == isFriendly){
					dh.displayCastleHealth(health);
				}
			}
		}
	}

	//get the end turn button 
	Transform getEndTurnButton(){
		foreach(Transform child in this.transform){
			EndTurnButton eb = child.GetComponent<EndTurnButton>();
			if(eb != null){
				return child;
			}
		}
		return null;
	}

	//----------------------------------------//
	//Updated gamelogic that deals with server//
	//----------------------------------------//

	//where we will animate the draw card method
	void animateDraw(GameObject card){
		DropZone hand = dropZoneForName("Hand");
		card.GetComponent<CardObject>().isDraggable = true;
		card.transform.SetParent(hand.transform);

	}

	//playing a card on the user's played this turn zone
	void animatePlay(GameObject card, DropZone playField){
		//DropZone played = dropZoneForName("FriendlyPlayField");
		card.GetComponent<CardObject>().isDraggable = false;
		card.transform.SetParent(playField.transform);
		Destroy(card.GetComponent<Draggable>().placeholder);
		card.GetComponent<CanvasGroup>().blocksRaycasts = true;
	}

	void animatePurchasePanelPlacement(GameObject card, PurchasePanel purchasePanel){
		card.transform.SetParent(purchasePanel.transform);
		CardObject cardObject = card.GetComponent<CardObject>();
		cardObject.isPurchasable = true;
		cardObject.pileCount = 100;
	}

	//play a hero from the user's hand to the hero zone
	void animateHeroPlay(GameObject card, HeroZone heroZone){
		card.GetComponent<CardObject>().isDraggable = false;
		Destroy(card.GetComponent<Draggable>().heroPlaceholder);
		Destroy(card.GetComponent<Draggable>().placeholder);
		card.transform.SetParent(heroZone.transform);
		card.GetComponent<CanvasGroup>().blocksRaycasts = true;

	}

	//find the card with the given id on the canvas
	GameObject cardOnCanvas(string cardId){
		foreach(Transform child in this.transform){
			CardObject c = child.GetComponent<CardObject>();
			if(c != null ){
				if(String.Compare(c.cardId, cardId) == 0){
					return c.gameObject;
				}
			}
		}
		return null;
	}

	//right now only takes hand as parameter, but will eventually take all of the challenge data
	public void startChallenge(GSData challenge, string activeUser){
		Debug.Log("sync the board with the server");

		//remove any placeholders from the hand or user zone that may still exist
		//removePlaceholders();

		//allow the user to interact with the cards
		setUserInteraction(activeUser);

		//set the health values of the players to the starting health
		updateHealth(challenge);

		//initialize the actions, buys, and money for the player
		updateCounters(challenge);

		//update the hero zones for both players
		updateHeroZones(challenge);

		//initialize the monster zone for the player

		//initialize the purchase panels for the player
		updatePurchasePanels(challenge);

		//update the play fields (cards the players have played this turn)
		updatePlayFields(challenge);

		//initialize the deck and discard count icons for both players
		updateDeckCounts(challenge);

		//show whose turn it currently is
		updateEndTurnButton(activeUser);

		//draw the hand for the player
		updateHand(challenge);

		
		
	}

	//sync the hero zones with the server's
	public void updateHeroZones(GSData challenge){
		GSDataHandler dataHandler = this.transform.GetComponent<GSDataHandler>();

		Dictionary< string, Dictionary<string, object> > friendlyHeroZoneStats = dataHandler.getHeroZone(challenge, true);
		HeroZone friendlyHeroZone = getFriendlyHeroZone();

		Dictionary< string, Dictionary<string, object> > enemyHeroZoneStats = dataHandler.getHeroZone(challenge, false);
		HeroZone enemyHeroZone = getEnemyHeroZone();
		
		updateHeroZone(friendlyHeroZone, friendlyHeroZoneStats, currentFriendlyHeroZone);
		updateHeroZone(enemyHeroZone, enemyHeroZoneStats, currentEnemyHeroZone);

		currentFriendlyHeroZone = friendlyHeroZoneStats;
		currentEnemyHeroZone = enemyHeroZoneStats;

		if(interactionEnabled){
			setHeroesAttackable(friendlyHeroZone);
		}else{
			removeHeroesSelected(friendlyHeroZone);
		}
		
	}

	//update the individual hero zone 
	public void updateHeroZone(HeroZone heroZone, Dictionary< string, Dictionary<string, object> > heroZoneStats, 
	Dictionary< string, Dictionary<string, object> > localHeroZone){
		//remove all the cards that are no longer on the purchase panel
		foreach(Transform child in heroZone.transform){
			CardObject card = child.GetComponent<CardObject>();
			if(card != null){
				if(!heroZoneStats.ContainsKey(card.cardId)){
					//Debug.Log("animate a card in the hand to the discard");
					Destroy(card.gameObject);
				}else{
					//simply update the properties of the hero in the zone (power, health, number of attacks)
					updateHero(card, heroZoneStats);
				}
			}
		}


		foreach(string cardKey in heroZoneStats.Keys){
			//if the card is has not yet been rendered in the user's hero zone
			if(!localHeroZone.ContainsKey(cardKey)){
				//find the card, at this point it will be a direct child of the canvas
				Dictionary<string, object> cardStats = heroZoneStats[cardKey];

				GameObject card;
				if(heroZone.isFriendly){
					card = cardOnCanvas(cardKey);
				}else{
					card = createCardForStats(cardStats, cardKey);
				} 
				animateHeroPlay(card, heroZone);
			}
		}
		//store the server play field locally
		//localHeroZone = heroZoneStats;
		
	}

	//update the modifiable properties of a hero card
	public void updateHero(CardObject card, Dictionary< string, Dictionary<string, object> > heroZoneStats){
		Debug.Log("update the hero with the new number of attacks");
		card.attacks = Convert.ToInt32( heroZoneStats[card.cardId]["attacks"] );
		card.health = Convert.ToInt32( heroZoneStats[card.cardId]["health"] );
	}

	//remove a border from a card if it did not attack
	public void removeHeroesSelected(HeroZone heroZone){
		foreach(Transform child in heroZone.transform){
			CardObject card = child.GetComponent<CardObject>();
			if(card != null){
				if(card.attacks > 0){
					removeSelected(card);
				}
			}
		}
	}

	//set the green attackable borders on a card at the start of a user's turn
	public void setHeroesAttackable(HeroZone heroZone){
		foreach(Transform child in heroZone.transform){
			CardObject card = child.GetComponent<CardObject>();
			if(card != null){
				if(card.attacks > 0){
					setAttackable(card);
				}else{
					//if the hero has no more attacks, remove the border
					removeSelected(card);
				}
			}
		}
	}

	//update the two viewable purchase panels for the user
	public void updatePurchasePanels(GSData challenge){
		GSDataHandler dataHandler = this.transform.GetComponent<GSDataHandler>();
		Dictionary< string, Dictionary<string, object> > purchasePanelStats = dataHandler.getPurchasePanel(challenge, 0);

		PurchasePanel purchasePanel = purchasePanelForName("NeutralPurchasePanel");
		//remove all the cards that are no longer on the purchase panel
		foreach(Transform child in purchasePanel.transform){
			CardObject card = child.GetComponent<CardObject>();
			if(card != null){
				if(!purchasePanelStats.ContainsKey(card.cardId)){
					//Debug.Log("animate a card in the hand to the discard");
					Destroy(card.gameObject);
				}
			}
		}


		foreach(string cardKey in purchasePanelStats.Keys){
			//if the card is has not yet been rendered in the user's play field
			if(!currentNeutralPurchasePanel.ContainsKey(cardKey)){
				//find the card, at this point it will be a direct child of the canvas
				Dictionary<string, object> cardStats = purchasePanelStats[cardKey];
				GameObject card = createCardForStats(cardStats, cardKey);
				animatePurchasePanelPlacement(card, purchasePanel);
			}

			
		}
		//store the server play field locally
		currentNeutralPurchasePanel = purchasePanelStats;
	}

	//allow the user to interact with the cards if it is his turn
	public void setUserInteraction(string activeUser){
		string localPlayer = this.transform.GetComponent<GSConnectionManager>().playerId;
		if(String.Compare(activeUser, localPlayer) == 0){
			interactionEnabled = true;
		}else{
			interactionEnabled = false;
		}
	}

	public void removePlaceholders(){
		//HeroZone heroZone = getFriendlyHeroZone();
		//heroZone.removeAllPlaceholders();

		DropZone hand = dropZoneForName("Hand");
		hand.removeAllPlaceholders();

	}



	//update the button and set the boolean to show if it is the user or opponent turn
	public void updateEndTurnButton(string activeUser){
		string localPlayer = this.transform.GetComponent<GSConnectionManager>().playerId;
		Transform endButton = getEndTurnButton();
		Text textBox = endButton.GetComponentInChildren<Text>();
		if(String.Compare(activeUser, localPlayer) == 0){
			//if it is the local player's turn
			textBox.text = "End Turn";
			isMyTurn = true;
		}else{
			//if it is the opponent's turn
			textBox.text = "Enemy Turn";
			isMyTurn = false;
		}
	}


	//visually set the health of both players 
	public void updateHealth(GSData challenge){
		Debug.Log("updating the health for both players");
		GSDataHandler dataHandler = this.transform.GetComponent<GSDataHandler>();

		double enemyHealth = (double) dataHandler.getPlayerStat(challenge, "playerHealth", false);
		double friendlyHealth = (double) dataHandler.getPlayerStat(challenge, "playerHealth", true);

		int eh = Convert.ToInt32(enemyHealth);
		int fh = Convert.ToInt32(friendlyHealth);
		setPlayerHealth(eh, false);
		setPlayerHealth(fh, true);
	}

	//set the number of cards in the deck and 
	public void updateDeckCounts(GSData challenge){
		GSDataHandler dataHandler = this.transform.GetComponent<GSDataHandler>();

		int myDeckCount = dataHandler.getCardStackCount(challenge, "currentDeck", true);
		int myDiscardCount = dataHandler.getCardStackCount(challenge, "currentDiscard", true);

		CardStack cardDeck = cardStackForName("FriendlyDeck");
		cardDeck.updateCount(myDeckCount);

		CardStack cardDiscard = cardStackForName("FriendlyDiscard");
		cardDiscard.updateCount(myDiscardCount);

		int enemyDeckCount = dataHandler.getCardStackCount(challenge, "currentDeck", false);
		int enemyDiscardCount = dataHandler.getCardStackCount(challenge, "currentDiscard", false);

		CardStack enemyCardDeck = cardStackForName("EnemyDeck");
		enemyCardDeck.updateCount(enemyDeckCount);

		CardStack enemyCardDiscard = cardStackForName("EnemyDiscard");
		enemyCardDiscard.updateCount(enemyDiscardCount);
	}

	//update the counters to match the server side
	public void updateCounters(GSData challenge){
		GSDataHandler dataHandler = this.transform.GetComponent<GSDataHandler>();
		//Debug.Log("about to get the stat");
		//List< Dictionary<string, object> > hand = dataHandler.convertHand(challenge);
		double actionDouble = (double) dataHandler.getPlayerStat(challenge, "actions", true);
		double buysDouble = (double) dataHandler.getPlayerStat(challenge, "buys", true);
		double coinDouble = (double) dataHandler.getPlayerStat(challenge, "coin", true);

		int actions = Convert.ToInt32( actionDouble );
		int buys = Convert.ToInt32( buysDouble );
		int coin = Convert.ToInt32( coinDouble );

		updateMoneyCounter(coin);
		updateActionCounter(actions);
		updateBuys(buys);
	}

		//update the play fields of both players
	public void updatePlayFields(GSData challenge){
		GSDataHandler dataHandler = this.transform.GetComponent<GSDataHandler>();

		Dictionary< string, Dictionary<string, object> > friendlyPlayField = dataHandler.getPlayField(challenge, true);
		Dictionary< string, Dictionary<string, object> > enemyPlayField = dataHandler.getPlayField(challenge, false);

		DropZone friendlyPlayFieldZone = dropZoneForName("FriendlyPlayField");
		DropZone enemyPlayFieldZone = dropZoneForName("EnemyPlayField");

		updatePlayField(friendlyPlayFieldZone, friendlyPlayField, currentFriendlyPlayField);
		updatePlayField(enemyPlayFieldZone, enemyPlayField, currentEnemyPlayField);

		
		//store the server play field locally
		currentFriendlyPlayField = friendlyPlayField;
		currentEnemyPlayField = enemyPlayField;
	}

	public void updatePlayField(DropZone playField, Dictionary< string, Dictionary<string, object> > playFieldStats, 
	Dictionary< string, Dictionary<string, object> > localPlayField){
		//remove all the cards that are no longer in the user's hand
		foreach(Transform child in playField.transform){
			CardObject card = child.GetComponent<CardObject>();
			if(card != null){
				if(!playFieldStats.ContainsKey(card.cardId)){
					//Debug.Log("animate a card in the hand to the discard");
					Destroy(card.gameObject);
				}
			}
		}


		foreach(string cardKey in playFieldStats.Keys){
			//if the card is has not yet been rendered in the user's play field
			if(!localPlayField.ContainsKey(cardKey)){
				//find the card, at this point it will be a direct child of the canvas
				string a = cardKey.Substring(0, 1);

				//if we have played the card from our hand
				if(String.Compare(a, "c") == 0){
					GameObject card;
					if(String.Compare(playField.zoneName, "FriendlyPlayField") == 0){
						card = cardOnCanvas(cardKey);
					}else{
						card = createCardForStats(playFieldStats[cardKey], cardKey);
					} 
					//GameObject card = cardOnCanvas((string)playFieldStats[cardKey]["cardId"]);
					animatePlay(card, playField);
				}

				//if we have bought the card from a purchase panel
				else{
					GameObject card = createCardForStats(playFieldStats[cardKey], cardKey);
					animatePlay(card, playField);
				}
			}
		}
	}

	//draw until you have the correct number of cards in hand
	public void updateHand(GSData challenge){
		//draw any cards that are not already in the user's hand

		GSDataHandler dataHandler = this.transform.GetComponent<GSDataHandler>();
		Dictionary< string, Dictionary<string, object> > hand = dataHandler.convertHand(challenge);


		DropZone myHand = dropZoneForName("Hand");
		//remove all the cards that are no longer in the user's hand
		foreach(Transform child in myHand.transform){
			CardObject card = child.GetComponent<CardObject>();
			if(card != null){
				if(!hand.ContainsKey(card.cardId)){
					//Debug.Log("animate a card in the hand to the discard");
					Destroy(card.gameObject);
				}
			}
		}


		foreach(string cardKey in hand.Keys){
			//create the card objects and animate them into the user's hand
			//Dictionary<string, object> cardStats = hand[i];


			//if the card is has not yet been rendered in the user's hand
			if(!currentHand.ContainsKey(cardKey)){
				Dictionary<string, object> cardStats = hand[cardKey];
				GameObject card = createCardForStats(cardStats, cardKey);
				animateDraw(card);
			}

			
		}
		currentHand = hand;
	}



}
