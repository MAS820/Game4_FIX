using UnityEngine;
using System.Collections;

public class RatAI : MonoBehaviour {

	public NavMeshAgent nav;
	public enum State{
		WONDER,
		FLEE,
		IDLE,
		TRAPPED
	}
	public State state;
	private bool alive;
	private CharacterController ratController;
	private GameObject GS;
	private GameState GSscript;
	private GameObject Player;
	private Transform PlayerTransform;
	private PlayerController PlayerCon;

	//WONDER variables
	public Material MatRatWonder;
	public GameObject[] ratWaypoint;
	public float wonderSpeed = 0.2f;
	private int ratWaypointIndex;

	//IDLE variables
	public Material MatRatIdle;
	public float idleTimer = 5.0f;
	private bool shouldIdle = true;	//set to false once they start fleeing so they don't bounc between states when in range of the target location
	
	//FLEE variables
	public Material MatRatFlee;
	public float fleeSpeed = 0.5f;
	public GameObject target;
	private Vector3 fleeLocation;

	//TRAPPED variables
	public Material MatRatTrapped;
	public Renderer rend;
	public float trappedTimer = 5.0f;
	
	// Use this for initialization
	void Start () {
		GS = GameObject.FindGameObjectWithTag ("GameState");
		GSscript = GS.GetComponent<GameState> ();

		Player = GameObject.FindGameObjectWithTag ("Player");
		PlayerCon = Player.GetComponent<PlayerController> ();
		PlayerTransform = Player.GetComponent<Transform> ();

		rend = GetComponent<Renderer> ();

		ratController = GetComponent<CharacterController>();
		nav = GetComponent<NavMeshAgent> ();
		nav.updatePosition = true;
		//use nav map for rotation, should be changed to use an animator at somepoint
		nav.updateRotation = true;
		alive = true;
		ratWaypoint = GameObject.FindGameObjectsWithTag ("RatWaypoint");
		ratWaypointIndex = Random.Range (0, ratWaypoint.Length);
		fleeLocation = this.transform.position;

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

				case State.IDLE:
					Idle ();
					break;

				case State.TRAPPED:
					Trapped ();
					break;
			}
			yield return null;
		} 
	}

	void Wonder(){

		rend.material = MatRatWonder;

		nav.speed = wonderSpeed;
		if (Vector3.Distance (this.transform.position, ratWaypoint [ratWaypointIndex].transform.position) >= 2) {
			nav.SetDestination (ratWaypoint [ratWaypointIndex].transform.position);
			ratController.Move (nav.desiredVelocity);
		} else if (Vector3.Distance (this.transform.position, ratWaypoint [ratWaypointIndex].transform.position) <= 2) {
			state = RatAI.State.IDLE;	//If the rat is close to it's target destination, switch to IDLE
		} else {
			ratController.Move (Vector3.zero);
		}
	}

	void Idle(){

		rend.material = MatRatIdle;

		idleTimer -= Time.deltaTime;
		if (idleTimer <= 0.0f) {
			state = RatAI.State.FLEE;
		}
	}

	void Trapped(){

		rend.material = MatRatTrapped;

		trappedTimer -= Time.deltaTime;
		if (trappedTimer <= 0.0f) {
			state = RatAI.State.FLEE;
		}
	}

	void Flee(){

		rend.material = MatRatFlee;

		nav.speed = fleeSpeed;
		nav.SetDestination (fleeLocation);
		ratController.Move (nav.desiredVelocity);

		if (Vector3.Distance (this.transform.position, fleeLocation) <= 2) {
			GSscript.subRat();
			this.gameObject.SetActive (false);
			Destroy (this);
		}
	}
	
	// Update is called once per frame
	void Update () {
		float distPlayer = Vector3.Distance (this.transform.position, PlayerTransform.transform.position);
		if (distPlayer <= 3.0) {
			if (Input.GetKeyUp (KeyCode.E) && state == RatAI.State.TRAPPED){
				PlayerCon.IncNumRats ();
				GSscript.subRat ();
				this.gameObject.SetActive (false);
				Destroy (this.gameObject);
			}
		}
	}
}
