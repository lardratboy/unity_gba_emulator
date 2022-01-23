using UnityEngine;
using System.Collections;

public class TumbleScript : MonoBehaviour {

    public float speed = 0.01f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        this.transform.transform.RotateAroundLocal(Vector2.up, speed);
	
	}
}
