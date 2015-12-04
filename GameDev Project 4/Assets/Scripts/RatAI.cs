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
	public float trappedTimer = 5.0f;

	Light lightComp;
	
	// Use this for initialization
	void Start () {
		GS = GameObject.FindGameObjectWithTag ("GameState");
		GSscript = GS.GetComponent<GameState> ();

		Player = GameObject.FindGameObjectWithTag ("Player");
		PlayerCon = Player.GetComponent<PlayerController> ();
		PlayerTransform = Player.GetComponent<Transform> ();

		ratController = GetComponent<CharacterController>();
		nav = GetComponent<NavMeshAgent> ();
		nav.updatePosition = true;
		//use nav map for rotation, should be changed to use an animator at somepoint
		nav.updateRotation = true;
		nav.angularSpeed = 100;
		alive = true;
		ratWaypoint = GameObject.FindGameObjectsWithTag ("Lamps");
		ratWaypointIndex = Random.Range (0, ratWaypoint.Length);
		fleeLocation = this.transform.position;

		lightComp = ratWaypoint [ratWaypointIndex].GetComponentInChildren<Light> ();

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

		nav.speed = wonderSpeed;

		//if the light is active then wonder to it
		//else randomly choose a new active light to walk to
		if (lightComp.enabled) {

			if (Vector3.Distance (this.transform.position, ratWaypoint [ratWaypointIndex].transform.position) >= (lightComp.range / 2)) {
				nav.SetDestination (ratWaypoint [ratWaypointIndex].transform.position);
				nav.Move (nav.desiredVelocity);
			} else if (Vector3.Distance (this.transform.position, ratWaypoint [ratWaypointIndex].transform.position) <= (lightComp.range / 2)) {
				state = RatAI.State.IDLE;	//If the rat is close to it's target destination, switch to IDLE
			} else {
				nav.Move (Vector3.zero);
			}
		} else {
			ratWaypointIndex = Random.Range (0, ratWaypoint.Length);
			lightComp = ratWaypoint [ratWaypointIndex].GetComponentInChildren <Light> ();
		}
	}

	void Idle(){

		nav.Move (Vector3.zero);

		idleTimer -= Time.deltaTime;
		if (idleTimer <= 0.0f) {
			state = RatAI.State.FLEE;
		}
	}

	void Trapped(){

		nav.Move (Vector3.zero);

		trappedTimer -= Time.deltaTime;
		if (trappedTimer <= 0.0f) {
			state = RatAI.State.FLEE;
		}
	}

	void Flee(){

		nav.speed = fleeSpeed;
		nav.SetDestination (fleeLocation);
		nav.Move (nav.desiredVelocity);

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
