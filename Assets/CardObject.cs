using UnityEngine;
using System.Collections;

public class CardObject : MonoBehaviour {

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

	//the number of buys a player has
	public int buys;

}
