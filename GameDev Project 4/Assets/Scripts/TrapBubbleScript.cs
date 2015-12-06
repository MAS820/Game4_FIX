using UnityEngine;
using System.Collections;

public class TrapBubbleScript : MonoBehaviour {

	private MeshRenderer render;

	// Use this for initialization
	void Start () {
	
		render = GetComponent<MeshRenderer> ();
		render.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void enableTrap(){

		render.enabled = true;

	}

	public void disableTrap(){

		render.enabled = false;

	}
}
