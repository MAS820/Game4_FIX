using UnityEngine;
using System.Collections;

//Still working on this one, will clean up.
public class CyclopsSight : MonoBehaviour
{
    public float fieldOfViewAngle = 190f;
    public bool playerInSight;

    public bool gameOver;
    Light currentLight;
    private NavMeshAgent nav;
    private CapsuleCollider col;
    private Animator anim;
    private GameObject player;
    private GameObject cyclops;
    private Animator playerAnim;
    private Vector3 playerPos;
    public Vector3 previousSighting;
    public CyclopsAI cyclopsAI;
    private CharacterController cyclopsController;



    // Use this for initialization
    void Start()
    {
        gameOver = false;
        nav = GetComponent<NavMeshAgent>();
        col = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        cyclops = GameObject.Find("Cyclops");

        nav.updateRotation = true;
        nav.updatePosition = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver)
        {
            playerPos = player.transform.position;
        }

    }

    void OnTriggerStay(Collider other)
    {

        if (other.gameObject == player)
        {
            
            //playerInSight = false;
            //Debug.Log(playerInSight);
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);
            // Debug.Log(angle);
            //Check front cone of view
            if (angle < fieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius))
                {
                    if (hit.collider.gameObject == player)
                    {
                        playerInSight = true;
                        previousSighting = playerPos;

                    }
                }
            }
            else
            {
                // small radius check around the cyclops so to agro player.

                if (Vector3.Distance(transform.position, playerPos) < 45)
                {
                    playerInSight = true;
                    previousSighting = playerPos;
                }
                else
                {
                    playerInSight = false;
                }

            }

        }

        else if (other.gameObject.tag == "Lamps")
        {

            Light current = other.gameObject.GetComponentInChildren<Light>();
            LightToggle LTscript = current.GetComponent<LightToggle>();

            //Debug.Log(current.enabled);
            if (current.enabled)
            {
                nav.destination = current.transform.position;

                float dist = Vector3.Distance(transform.position, current.transform.position);
                if (dist < 20)
                {
                    current.enabled = false;
                    //cyclopsAI.state = CyclopsAI.State.PATROLLING;

                }
            }
        }
        else if (other.gameObject.tag == "Projectile")
        {
            //cyclopsAI.state = CyclopsAI.State.CHASING;
            playerInSight = true;
            nav.destination = playerPos;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInSight = false;

        }
    }
}
