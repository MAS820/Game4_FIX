using UnityEngine;
using System.Collections;

public class CyclopsAI : MonoBehaviour
{
    //Editables //some variables will be used later for animation and navigational mesh
    public float patrolSpeed = 10f;
    public float chaseSpeed = 30f;
    public float patrolWaitTime = 1f;
    public float acceleration = 0.01f;
    private float patrolTimer;

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
        INVESTIGATING,
        PATROLLING,
        EATING,
        ENRAGED
    }
    public State state;
    public Renderer r;

    //Patrolling var
    public Material MatCyclopsPatrolling;

    //Use this to eat rats
    public float eatRate = 1.0f;
    public RockPile rockPile;
    private float playerLastSeen;

    public AudioClip[] audioClips;
    private AudioSource[] audioSources;

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

        audioSources = new AudioSource[audioClips.Length];
        int i = 0;
        while (i < audioClips.Length)
        {
            audioSources[i] = this.gameObject.AddComponent<AudioSource>() as AudioSource;
            audioSources[i].clip = audioClips[i];
            i++;
        }
        audioSources[0].playOnAwake = false;
        audioSources[0].loop = false;
        audioSources[1].playOnAwake = false;
        audioSources[1].loop = true;
        audioSources[2].playOnAwake = false;
        audioSources[2].loop = false;
        audioSources[3].playOnAwake = false;
        audioSources[3].loop = true;
        audioSources[4].playOnAwake = false;
        audioSources[4].loop = true;
        audioSources[5].playOnAwake = false;
        audioSources[5].loop = true;
        audioSources[6].playOnAwake = false;
        audioSources[6].loop = false;

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
                case State.INVESTIGATING:
                    Investigating();
                    if(!audioSources[6].isPlaying)
                    {
                        audioSources[6].Play();
                    }
                    break;
                case State.CHASING:
                    Chasing();
                    break;
                case State.EATING:
                    Eating();
                    if(!audioSources[1].isPlaying) {
                        audioSources[1].Play();
                    }
                    break;
                case State.ENRAGED:
                    Enraged();
                    break;
            }
            yield return null;
        }
    }
    // Update is called once per frame
    void Investigating()
    {
        nav.destination = cyclopsSight.previousSighting;
        if (nav.remainingDistance < nav.stoppingDistance)
        {
            state = CyclopsAI.State.PATROLLING;
        }
    }

    void GoToRockPile()
    {
        nav.speed = chaseSpeed;
        nav.destination = rockPile.transform.position; 
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
        if (dist <= 50 && cyclopsSight.playerInSight == true)
        {
            if (!audioSources[0].isPlaying)
            {
                audioSources[0].Play();
            }
            if (!audioSources[3].isPlaying)
            {
                audioSources[3].Play();
            }
            //Tell enemy to walk to player location
            nav.destination = player.transform.position;
        }
        else 
        {
            nav.destination = cyclopsSight.previousSighting;
            cyclopsSight.playerInSight = false;
        }
        //If we're nearing the destination, add to chase timer.
        /*if (nav.remainingDistance < nav.stoppingDistance)
        {
            chaseds= Time.deltaTime;
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
            playerLastSeen = Time.time;
        }
        else
        {

            //Get the position of the first waypoint
            waypoint = wayPoints[wayPointIndex].transform.position;
            if (nav.speed != patrolSpeed && playerLastSeen < 0)
            {
                Debug.Log("hello");
                nav.speed = patrolSpeed;

            }
            else
            {
                nav.speed += acceleration;

            }
            // Debug.Log(waypoint);
            //float dist = Vector3.Distance(waypoint, transform.position);
            if (nav.remainingDistance < nav.stoppingDistance)
            {
                nav.destination = waypoint;
                Debug.Log(Time.time);

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
                //basic investigation state. checks the spot of player last seen if its been a while.
                if (Time.time - playerLastSeen > 20.0f && cyclopsSight.playerInSight == false)
                {
                    //state = CyclopsAI.State.ENRAGED;
                    if (rockPile != null)
                    {
                        if (rockPile.numRatsDigging > 5)
                        {
                            state = CyclopsAI.State.ENRAGED;
                        }
                        else
                        {
                            state = CyclopsAI.State.INVESTIGATING;
                        }
                    }
                }


            }

        }
    }

    void Enraged()
    {
        GoToRockPile();
    }

    void Eating()
    {
        if (rockPile.numRatsDigging > 0)
        {
            rockPile.numRatsDigging -= eatRate * Time.deltaTime;
        }
        else
        {
           state = CyclopsAI.State.PATROLLING;

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

