using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour {
    //Editables //some variables will be used later for animation and navigational mesh
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public float chaseWaitTime = 4f;
    public float patrolWaitTime = 1f;

    public Vector3[] patrolWayPoints;
    public GameObject[] wayPoints;

    public Vector3 waypoint;
    private EnemySight enemySight;
    private NavMeshAgent nav;
    private GameObject player;
    private GameObject enemy;

    private float patrolTimer;
    private float chaseTimer;

    private int wayPointIndex;
	// Use this for initialization
	void Start () {
        //Field of View Script / Player Detection
        enemySight = GetComponent<EnemySight>();
        
        //Nav Mesh agent sets up the mesh that our characters can move on.
        nav = GetComponent<NavMeshAgent>();
        
        //autobraking causes the character to pause before each waypoint
        nav.autoBraking = false;
        
        //char references
        player = GameObject.Find("Player");
        enemy = GameObject.Find("Enemy");

        //Set the first waypoint
        wayPointIndex = 0;
        //Get the list of waypoint objects.
        wayPoints = GameObject.FindGameObjectsWithTag("CyclopsWaypoint");


    }

    // Update is called once per frame
    void Chasing()
    {
        //set speed;
        nav.speed = chaseSpeed;

        //Get the position of the player. Get the delta vector between player / enemy
        Vector3 sightingDeltaPos = enemySight.previousSighting - transform.position;

        //Get the magnitude of the vector (distance)
        if (sightingDeltaPos.sqrMagnitude > 4f)
        {
            //Tell enemy to walk to player location
            nav.destination = player.transform.position;//enemySight.previousSighting;
        }

        //If we're nearing the destination, add to chase timer.
        if (nav.remainingDistance < nav.stoppingDistance)
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
        }
    }

    void Patrolling()
    {
        //Get the position of the first waypoint
        waypoint = wayPoints[wayPointIndex].transform.position;
        nav.speed = patrolSpeed;
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
   
    void Update()
    {
        if (!enemySight.playerInSight)
        {
            Patrolling();
            //Debug.Log("Not Seen");
        }
        else
        {
            Chasing();
            //Debug.Log("seen");
        }
    
    }

}

