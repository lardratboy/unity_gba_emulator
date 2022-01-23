using UnityEngine;
using System.Collections;
using System;

public class CartHolder : MonoBehaviour {

	public TextAsset _romAsset;
	public GameObject target;

	void OnMouseDown() {

		GetComponent<Renderer>().material.color = Color.green;

	}

	void OnMouseUp() {
		GetComponent<Renderer>().material.color = Color.white;

		TopLevelObjScript to = target.GetComponent<TopLevelObjScript>();
		
		if ( to ) {
			
			to.LoadROM( (TextAsset)_romAsset );
			
		}
		
	}

	void OnMouseEnter() {
		GetComponent<Renderer>().material.color = Color.red;
	}
	
	void OnMouseExit() {
		GetComponent<Renderer>().material.color = Color.white;
	}
	
}
