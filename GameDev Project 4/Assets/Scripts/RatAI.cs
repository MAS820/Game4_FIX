using System;
using UnityEngine;
using System.Collections;

public class RatAI : MonoBehaviour {

	public NavMeshAgent nav;
	public enum State{
		WONDER,
		FLEE
	}
	public State state;
	private bool alive;
	private CharacterController ratController;

	//WONDER variables
	public GameObject[] ratWaypoint;
	public float wonderSpeed = 0.2f;
	private int ratWaypointIndex = 0;


	//FLEE variables
	public float fleeSpeed = 0.5f;
	public GameObject target;


	// Use this for initialization
	void Start () {
		ratController = GetComponent<CharacterController>();
		nav = GetComponent<NavMeshAgent> ();
		nav.updatePosition = true;
		nav.updateRotation = true;
		alive = true;

		state = RatAI.State.WONDER;

		StartCoroutine ("FSM");

	}

	IEnumerator FSM(){
		while (alive) {
			switch (state){
				case State.WONDER:
					Wonder ();
					break;

				case State.FLEE:
					Flee ();
					break;

			}
			yield return null;
		} 
	}

	void Wonder(){
		nav.speed = wonderSpeed;
		if (Vector3.Distance (this.transform.position, ratWaypoint [ratWaypointIndex].transform.position) >= 2) {
			nav.SetDestination (ratWaypoint [ratWaypointIndex].transform.position);
			ratController.Move (nav.desiredVelocity);
		} else if (Vector3.Distance (this.transform.position, ratWaypoint [ratWaypointIndex].transform.position) <= 2) {
			ratWaypointIndex += 1;
			if (ratWaypointIndex >= ratWaypoint.Length) {
				ratWaypointIndex = 0;
			}
		} else {
			ratController.Move (Vector3.zero);
		}
	}

	void Flee(){
		nav.speed = fleeSpeed;
		nav.SetDestination (target.transform.position);
		ratController.Move (nav.desiredVelocity);
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player") {
			state = RatAI.State.FLEE;
			target = other.gameObject;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
