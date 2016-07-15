using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CardDictionary{




	public Dictionary <GameObject, int> globalDictionary;

	//read in the text file of cards and 
	public void readFile(){

		Dictionary<string, string> cardDictionary = new Dictionary<string, string>();

		using (System.IO.StreamReader sr = new System.IO.StreamReader("Assets/Resources/card_list.txt"))
	    {
	        while (!sr.EndOfStream) // Keep reading until we get to the end
	        {
	            string splitMe = sr.ReadLine();
	            string[] bananaSplits = splitMe.Split(new char[] { ':' }); //Split at the colons

	            if (bananaSplits.Length < 2) // If we get less than 2 results, discard them
	                continue; 
	            else if (bananaSplits.Length == 2) // Easy part. If there are 2 results, add them to the dictionary
	                cardDictionary.Add(bananaSplits[0].Trim(), bananaSplits[1].Trim());
	        }
	    }

	    Debug.Log(cardDictionary);

	}
}
