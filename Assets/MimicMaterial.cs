using UnityEngine;
using System.Collections;

public class MimicMaterial : MonoBehaviour {
	
	public GameObject target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if (target)
		{
            GetComponent<Renderer>().material = target.GetComponent<Renderer>().material;
		}
	
	}
}
