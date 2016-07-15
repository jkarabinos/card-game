using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

	public GameObject[] deck;
	public int cardCount = 10;


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
				d.newParent = this.transform;

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

		CardDictionary cardDictionary = new CardDictionary();
		cardDictionary.readFile();
		Dictionary< string, Dictionary<string, string> > globalDict = cardDictionary.globalDictionary;
		Debug.Log (globalDict.Count + " is the number of cards");
		
		for( int i = 0; i < cardCount; i++ ){

			
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



			deck[i] = card; 
		}
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
		var tempDeck = new List<GameObject>(deck);

		System.Random rnd = new System.Random();
		

		for( int i = 0; i < cardCount; i++ ){
			int randIndex = rnd.Next(0, tempDeck.Count); 
			GameObject card = tempDeck[randIndex];
			tempDeck.RemoveAt(randIndex);
			deck[i] = card;
			

		}

	}



	public void drawHand(){
		//draw a hand of five cards at the end of turn or the beginning of the game
		for( int i = 0; i < 5; i++ ){
			drawCard();
		}


	}

}
