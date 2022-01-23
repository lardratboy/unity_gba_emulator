using UnityEngine;
using System.Collections;

public class LiveButton : MonoBehaviour {

	static Object siblingActive;
	public bool isActive;
	public bool isDown;
	public bool isOver;
	public KeyCode key;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		isActive = isDown || (isOver && ((null != siblingActive)&&(siblingActive != this))) || Input.GetKey( key );

		if ( isActive ) {

			GetComponent<Renderer>().material.color = Color.green;

		} else if ( isOver ) {

			GetComponent<Renderer>().material.color = Color.yellow;

		} else {

			GetComponent<Renderer>().material.color = Color.white;

		}

	}

	void OnMouseDown() {
		isDown = true;
		siblingActive = this;
	}

	void OnMouseUp() {
		isDown = false;
		if ((null != siblingActive) && (siblingActive == this)) {
			siblingActive = null;
		}
	}

	void OnMouseEnter() {
		isOver = true;
	}

	void OnMouseExit() {
		isOver = false;
	}

}
