using UnityEngine;
using System.Collections;

public class CardObject : MonoBehaviour {

	//to identify unique cards in the user's hand
	public string cardId;

	//the id of the card
	public int id;

	//the type of card
	public enum Type { ACTION, ATTACK, MONSTER, HERO, BUILDING, TREASURE }
	
	public Type typeOfCard = Type.TREASURE;
	
	//the coin value of the card
	public int value;

	//the cost of the card
	public int cost;

	//the damage that the card does when played
	public int damage;

	//if the card if purchasable
	public bool isPurchasable;

	//if we want the card to be able to be dragged by the user
	public bool isDraggable;

	//the number of cards we draw when this card is played
	public int draw;

	//the number of buys a card gives a player
	public int buys;

	//the type of a card
	public string type;

	//how much damage a permanent does
	public int power;

	//how much damage a permanent can take
	public int health;

	//the name of the card
	public string cardName;

	//the rarity of the card
	public string rarity;

	//if the cards are purchasable, this will store the number that can still be purchased from this pile
	public int pileCount;

	//the number of attacks a hero card can make
	public int attacks;

	//if the card has a trigger ability (something that triggers when some other action takes place)
	public bool activeTrigger;

	//the kind of cards that the card can target (monsterCards, heroCards, nothing, everything)
	public string canTarget;

	//if the card has a special ability when played (note this is a one time effect, different than a lasting trigger)
	public bool hasAbility;


}
