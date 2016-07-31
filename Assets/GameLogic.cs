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
	public Dictionary< string, Dictionary<string, object> > currentFriendlyPurchasePanel = new Dictionary< string, Dictionary<string, object> >();
	public Dictionary< string, Dictionary<string, object> > currentFriendlyHeroZone = new Dictionary< string, Dictionary<string, object> >();
	public Dictionary< string, Dictionary<string, object> > currentEnemyHeroZone = new Dictionary< string, Dictionary<string, object> >();

	public Dictionary<string, Dictionary<string, object> > currentMonsterZones;


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

	}

	//play a hero onto the hero zone at the given sibling index
	public void playHero(CardObject card, int siblingIndex){
		GSChallengeHandler ch = this.transform.GetComponent<GSChallengeHandler>();
		Dictionary<string, object> target = new Dictionary<string, object>();
		target.Add("target", "isFalse");
		ch.playCard(card, target);
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


	//perform the necessary steps at the beginning of the game
	public void startGame(){
		GSConnectionManager cm = this.transform.GetComponent<GSConnectionManager>();
		cm.authenticateUser();
		//startTheGame();
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

	public GameObject createCardForStats(Dictionary< string, object > stats, string key){
		Debug.Log("the type of value is " + stats["value"].GetType());

		//set the basic properties of the card
		GameObject card = (GameObject)Instantiate(Resources.Load("Card3"));

		var cardScript = card.GetComponent<CardObject>();
		cardScript.value = Convert.ToInt32( stats["value"] );
		cardScript.damage = Convert.ToInt32( stats["damage"] );
		cardScript.cost = Convert.ToInt32( stats["cost"] );
		cardScript.draw = Convert.ToInt32( stats["draw"] );
		cardScript.buys = Convert.ToInt32( stats["buys"] );
		cardScript.type = (string) stats["cardType"];
		cardScript.name = (string) stats["name"];
		//cardScript.cardName = individualCardDict["name"];
		cardScript.rarity = (string) stats["rarity"];
		cardScript.cardId = key;
		//cardScript.id = id;

		cardScript.hasAbility = (bool) stats["hasAbility"];
		cardScript.activeTrigger = (bool) stats["activeTrigger"];
		cardScript.canTarget = (string) stats["canTarget"];

		setCardText(card, "Cost", cardScript.cost.ToString());

		if(String.Compare(cardScript.type, "monsterCards") == 0 
		|| String.Compare(cardScript.type, "heroCards") == 0){
			cardScript.health = Convert.ToInt32( stats["health"] );
			cardScript.power = Convert.ToInt32( stats["power"] );
			setCardText(card, "Power", cardScript.power.ToString());
			setCardText(card, "Health", cardScript.health.ToString());

			if(String.Compare(cardScript.type, "heroCards") == 0){
				cardScript.attacks = Convert.ToInt32( stats["attacks"] );
			}else{
				//monsters don't have a cost
				setCardText(card, "Cost", "");
			}
		}else{
			//for cards that don't have power and health
			setCardText(card, "Power", "");
			setCardText(card, "Health", "");
		}

		

		//set the appropriate image of the card
		string imagePath = (string) stats["imagePath"];
		Sprite spr = Resources.Load <Sprite> (imagePath);
		Image cardImage = card.GetComponent<Image>();
		cardImage.sprite = spr;

		return card;
	}

	//set a changable value of text on the card
	public void setCardText(GameObject card, string textName, string value){
		foreach(Transform child in card.transform){
			Text textBox = child.GetComponent<Text>();
			if(textBox){
				if(String.Compare(textBox.name, textName) == 0){
					textBox.text = value;
				}
			}
		}
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

			border.transform.SetParent(card.transform, false);
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

	//returns a MonsterZone script for the given name
	public MonsterZone monsterZoneForName(string name){
		foreach(Transform child in this.transform){
			MonsterZone monsterZone = child.GetComponent<MonsterZone>();
			if (monsterZone != null){
				if (String.Compare (name, monsterZone.monsterZoneName) == 0){
					return monsterZone;
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

	}

	//add the default card effects to the game scene
	public void playCard(CardObject c){

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
		card.transform.SetParent(hand.transform, false);

	}

	//playing a card on the user's played this turn zone
	void animatePlay(GameObject card, DropZone playField){
		//DropZone played = dropZoneForName("FriendlyPlayField");
		card.GetComponent<CardObject>().isDraggable = false;
		card.transform.SetParent(playField.transform, false);
		Destroy(card.GetComponent<Draggable>().placeholder);
		card.GetComponent<CanvasGroup>().blocksRaycasts = true;
	}

	void animatePurchasePanelPlacement(GameObject card, PurchasePanel purchasePanel){
		card.transform.SetParent(purchasePanel.transform, false);
		CardObject cardObject = card.GetComponent<CardObject>();
		cardObject.isPurchasable = true;
		cardObject.pileCount = 100;
	}

	//play a hero from the user's hand to the hero zone
	void animateHeroPlay(GameObject card, HeroZone heroZone){
		card.GetComponent<CardObject>().isDraggable = false;
		Destroy(card.GetComponent<Draggable>().heroPlaceholder);
		Destroy(card.GetComponent<Draggable>().placeholder);
		card.transform.SetParent(heroZone.transform, false);
		card.GetComponent<CanvasGroup>().blocksRaycasts = true;

	}

	//animate the enemy drawing the cards 
	void animateEnemyDraw(int numCards){
		for(int i = 0; i < numCards; i++){
			GameObject card = (GameObject)Instantiate(Resources.Load("Card"));
			string imagePath = "card_game/card_back";
			Sprite spr = Resources.Load <Sprite> (imagePath);
			Image cardImage = card.GetComponent<Image>();
			cardImage.sprite = spr;

			Transform enemyHand = dropZoneForName("EnemyHand").transform;
			card.transform.SetParent(enemyHand, false);
		}
		
	}

	//spawn the given monster on the given zone
	void animateMonsterToZone(GameObject card, MonsterZone monsterZone){
		card.transform.SetParent(monsterZone.transform, false);
	}

	//remove a given amount of cards from the enemy's hand (most likey a temporary method)
	void removeEnemyCardsFromHand(int numCards){
		Transform enemyHand = dropZoneForName("EnemyHand").transform;
		int count = 0;
		foreach(Transform child in enemyHand){
			if(count < numCards){
				Destroy(child.gameObject);
				count++;
			}else{
				break;
			}
		}
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
		updateMonsterZones(challenge);

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

	//update all of the monster zones with their new monsters and updated stats of existing monsters
	public void updateMonsterZones(GSData challenge){

		if(currentMonsterZones == null){
			//initialize the local monster zone data with nonexistent monsters to simplify the next steps
			currentMonsterZones = new Dictionary < string, Dictionary<string, object> >();
			currentMonsterZones["neutral1"] = getInitialMonsterDict();
			currentMonsterZones["neutral2"] = getInitialMonsterDict();
			currentMonsterZones[this.transform.GetComponent<GSConnectionManager>().playerId] = getInitialMonsterDict();
			currentMonsterZones[this.transform.GetComponent<GSChallengeHandler>().enemyId] = getInitialMonsterDict();
		}

		updateMonsterZone(challenge, "NeutralMonsterZone1", "neutral1");
		updateMonsterZone(challenge, "NeutralMonsterZone2", "neutral2");
		updateMonsterZone(challenge, "FriendlyMonsterZone", this.transform.GetComponent<GSConnectionManager>().playerId);
		updateMonsterZone(challenge, "EnemyMonsterZone", this.transform.GetComponent<GSChallengeHandler>().enemyId);
	}

	public Dictionary<string, object> getInitialMonsterDict(){
		Dictionary <string, object> monsterDict = new Dictionary<string, object>();
		monsterDict.Add("name", "nonexistent");
		return monsterDict;
	}

	//update an individual monster zone
	public void updateMonsterZone(GSData challenge, string localZoneName, string serverZoneName){
		GSDataHandler dataHandler = this.transform.GetComponent<GSDataHandler>();
		MonsterZone monsterZone = monsterZoneForName(localZoneName);

		Dictionary<string, object> monster = dataHandler.getMonster(challenge, serverZoneName);


		if(String.Compare((string) monster["name"], "nonexistent") == 0){
			if(String.Compare((string) currentMonsterZones[serverZoneName]["name"], "nonexistent") != 0){
				//if we need to remove a slain monster from the zone without adding a new one
				removeMonster(monsterZone);
			}
		}

		else if(String.Compare((string) currentMonsterZones[serverZoneName]["name"], (string) monster["name"]) != 0){
			//if one monster has died and we need to replace it with another
			if(String.Compare((string) currentMonsterZones[serverZoneName]["name"], "nonexistent") !=0){
				removeMonster(monsterZone);
			}
			addMonsterToZone(monster, monsterZone, serverZoneName);
		}

		else{
			//update the living monster
			foreach(Transform child in monsterZone.transform){
				CardObject c = child.GetComponent<CardObject>();
				if(c != null){
					updateMonster(c, monster);
				}
			}
			
		}

		currentMonsterZones[serverZoneName] = monster;
	}

	public void updateMonster(CardObject card, Dictionary<string, object> monsterStats){
		card.health =  Convert.ToInt32( monsterStats["health"] ); 
		//setCardText(card, "Power", cardScript.power.ToString());
		setCardText(card.gameObject, "Health", card.health.ToString());
	}

	//remove the current monster from the zone
	public void removeMonster(MonsterZone monsterZone){
		foreach(Transform child in monsterZone.transform){
			CardObject c = child.GetComponent<CardObject>();
			if(c != null){
				Destroy(c.gameObject);
			}
		}
	}

	//add a monster to the zone by creating a new card
	public void addMonsterToZone(Dictionary<string, object> monster, MonsterZone monsterZone, string serverZoneName){
		GameObject card = createCardForStats(monster, serverZoneName);
		animateMonsterToZone(card, monsterZone);
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
		//Debug.Log("update the hero with the new number of attacks");
		card.attacks = Convert.ToInt32( heroZoneStats[card.cardId]["attacks"] );
		card.health = Convert.ToInt32( heroZoneStats[card.cardId]["health"] );
		card.power = Convert.ToInt32( heroZoneStats[card.cardId]["power"] );

		setCardText(card.gameObject, "Health", card.health.ToString());
		setCardText(card.gameObject, "Power", card.power.ToString());
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
		Dictionary< string, Dictionary<string, object> > friendlyPurchasePanelStats = dataHandler.getPurchasePanel(challenge, 1);

		PurchasePanel neutralPurchasePanel = purchasePanelForName("NeutralPurchasePanel");
		PurchasePanel friendlyPurchasePanel = purchasePanelForName("FriendlyPurchasePanel");

		updatePurchasePanel(purchasePanelStats, neutralPurchasePanel, currentNeutralPurchasePanel);
		updatePurchasePanel(friendlyPurchasePanelStats, friendlyPurchasePanel, currentFriendlyPurchasePanel);
		
		//store the server play field locally
		currentNeutralPurchasePanel = purchasePanelStats;
		currentFriendlyPurchasePanel = friendlyPurchasePanelStats;
	}

	//update one of the purchase panels
	public void updatePurchasePanel(Dictionary< string, Dictionary<string, object> > purchasePanelStats, PurchasePanel purchasePanel,
	Dictionary< string, Dictionary<string, object> > localPurchasePanel){


		//remove all the cards that are no longer on the purchase panel
		foreach(Transform child in purchasePanel.transform){
			CardObject card = child.GetComponent<CardObject>();
			if(card != null){
				if(!purchasePanelStats.ContainsKey(card.cardId)){
					//Debug.Log("animate a card in the hand to the discard");
					Destroy(card.gameObject);
				}else{
					updatePurchasableCard(card, purchasePanelStats[card.cardId]);
				}
			}
		}


		foreach(string cardKey in purchasePanelStats.Keys){
			//if the card is has not yet been rendered in the user's play field
			if(!localPurchasePanel.ContainsKey(cardKey)){
				//find the card, at this point it will be a direct child of the canvas
				Dictionary<string, object> cardStats = purchasePanelStats[cardKey];
				GameObject card = createCardForStats(cardStats, cardKey);
				animatePurchasePanelPlacement(card, purchasePanel);
			}

			
		}
	}

	//update the cost of the card if it has changed
	public void updatePurchasableCard(CardObject card, Dictionary<string, object> cardStats){
		card.cost =  Convert.ToInt32( cardStats["cost"] ); 
		setCardText(card.gameObject, "Cost", card.cost.ToString());
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

		updateEnemyHand(challenge);
	}

	//display the number of cards in the enemy player's hand without revealing them
	public void updateEnemyHand(GSData challenge){
		GSDataHandler dataHandler = this.transform.GetComponent<GSDataHandler>();
		int cardCount = dataHandler.getEnemyCardCount(challenge);
		Transform enemyHand = dropZoneForName("EnemyHand").transform;
		if(cardCount < enemyHand.childCount){
			//remove cards from the hand
			removeEnemyCardsFromHand(enemyHand.childCount - cardCount);
		}else if(cardCount > enemyHand.childCount){
			//add more cards to the hand
			animateEnemyDraw(cardCount - enemyHand.childCount);
		}
	}



}
