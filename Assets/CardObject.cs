using UnityEngine;
using System.Collections;

public class CardObject : MonoBehaviour {


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


}
