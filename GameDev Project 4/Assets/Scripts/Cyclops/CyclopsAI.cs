using UnityEngine;
using System.Collections;

public class CyclopsAI : MonoBehaviour
{
    //Editables //some variables will be used later for animation and navigational mesh
    public float patrolSpeed = 4f;
    public float chaseSpeed = 2.9f;
    public float chaseWaitTime = 4f;
    public float patrolWaitTime = 1f;
    private float patrolTimer;
    private float chaseTimer;

    private NavMeshAgent nav;
    public Vector3[] patrolWayPoints;
    public GameObject[] wayPoints;
    public Vector3 waypoint;
    private int wayPointIndex;

    private GameObject cyclops;
    private CyclopsSight cyclopsSight;
    private CharacterController cyclopsController;

    private GameObject player;
    private Transform playerTransform;
    //  private Renderer enemyRenderer;
    // private GameObject GS;
    //  private GameState GSscript;
    private bool alive;
    // Use this for initialization
    public enum State
    {
        CHASING,
        NEUTRAL,
        PATROLLING
    }
    public State state;
    public Renderer r;

    //Patrolling var
    public Material MatCyclopsPatrolling;

    //Use this to eat rats
    public bool eat;
    public float eatRate = 1.0f;

    void Start()
    {
        //  GS = GameObject.FindGameObjectWithTag("GameState");
        // GSscript = GS.GetComponent<GameState>();
        // r = GetComponent<Renderer>();
        // enemyRenderer = enemy.GetComponent<Renderer>();
        cyclopsController = GetComponent<CharacterController>();

        //Field of View Script / Player Detection
        cyclopsSight = GetComponent<CyclopsSight>();

        //Nav Mesh agent sets up the mesh that our characters can move on.
        nav = GetComponent<NavMeshAgent>();
        nav.updatePosition = true;
        nav.updateRotation = true;
        alive = true;
        //autobraking causes the character to pause before each waypoint
         //nav.autoBraking = false;

        //char references
        player = GameObject.Find("Player");
        playerTransform = player.transform;
        state = CyclopsAI.State.PATROLLING;
        cyclops = GameObject.Find("Cyclops");
        //  enemyTransform = enemy.transform;

        //Set the first waypoint
        wayPointIndex = 0;
        //Get the list of waypoint objects.
        wayPoints = GameObject.FindGameObjectsWithTag("CyclopsWaypoint");

        //cyclops eating rats stuff
        eat = false;

        StartCoroutine("FSM");
    }
    IEnumerator FSM()
    {
        while (alive)
        {
            switch (state)
            {
                case State.PATROLLING:
                    Patrolling();
                    break;
                case State.NEUTRAL:
                    Neutral();
                    break;
                case State.CHASING:
                    Chasing();
                    break;
            }
            yield return null;
        }
    }
    // Update is called once per frame
    void Neutral()
    {

    }
    void GoToRockPile()
    {
        nav.speed = chaseSpeed;
        //nav.destination = rockpile.transform.position; 
    }
    void Chasing()
    {
        //set speed;
        //enemyRenderer.material.color = new Color(0.5f, 0.3f, 0.3f);
        nav.speed = chaseSpeed;
        //enemyRenderer.material.color = Color.red;
      //  cyclops.transform.Rotate();
        //Get the position of the player. Get the delta vector between player / enemy
        Vector3 sightingDeltaPos = cyclopsSight.previousSighting - transform.position;
        float dist = Vector3.Distance(cyclopsSight.previousSighting, transform.position);
        //Get the magnitude of the vector (distance)
        if (dist <= 6)
        {
            //Tell enemy to walk to player location
            nav.destination = cyclopsSight.previousSighting;
        }
        else 
        {
            cyclopsSight.playerInSight = false;
        }
        //If we're nearing the destination, add to chase timer.
        /*if (nav.remainingDistance < nav.stoppingDistance)
        {
            chaseTimer += Time.deltaTime;
            //Chasing cooldown, ensures that monster continues moving
            if (chaseTimer >= chaseWaitTime)
            {
                chaseTimer = 0f;
            }
        }
        else
        {
            chaseTimer = 0f;
        }*/
    }

    void Patrolling()
    {
        //enemyRenderer.material.color = Color.green;
        if (cyclopsSight.playerInSight)
        {
            state = CyclopsAI.State.CHASING;
        }
        else
        {

            //Get the position of the first waypoint
            waypoint = wayPoints[wayPointIndex].transform.position;
            nav.speed = patrolSpeed;
            //float dist = Vector3.Distance(waypoint, transform.position);
            if (nav.remainingDistance < nav.stoppingDistance)
            {
                nav.destination = waypoint;

                //If we reach the end of the list, start over
                if (wayPointIndex == wayPoints.Length - 1)
                {
                    wayPointIndex = 0;
                }
                //Otherwise, we go through the list.
                else
                {
                    wayPointIndex++;
                }
            }
        }
    }

    void Update()
    {
        if (alive)
        {
            if (!cyclopsSight.playerInSight)
            {
                state = CyclopsAI.State.PATROLLING;
            }
        }
    }
}

