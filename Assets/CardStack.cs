using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class CardStack : MonoBehaviour {

	public string stackName;

	public void updateCount(int count){

		Text textBox = this.transform.GetComponentInChildren<Text>();
		textBox.text = count.ToString();

	}

}
