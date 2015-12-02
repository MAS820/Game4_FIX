using UnityEngine;
using System.Collections;

//Still working on this one, will clean up.
public class CyclopsSight : MonoBehaviour {
    public float fieldOfViewAngle = 120f;
    public bool playerInSight;
    public bool gameOver;
    Light currentLight;
    private NavMeshAgent nav;
    private CapsuleCollider col;
    private Animator anim;
    private GameObject player;
    private GameObject cyclops;
    private Animator playerAnim;
    public Vector3 previousSighting;

    private CharacterController cyclopsController;

    

    // Use this for initialization
    void Start () {
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
	void Update () {
        if (!gameOver)
        {
        }
        	
	}

    void OnTriggerStay (Collider other)
    {
        if (other.gameObject.tag == "Lamps")
        {
            //nav.speed = 3.0f;
            Light current = other.gameObject.GetComponentInChildren<Light>();
            if (current.enabled)
            {
                nav.destination = current.transform.position;
              //  cyclopsController.Move(nav.desiredVelocity);
                float dist = Vector3.Distance(transform.position, current.transform.position);
                if (dist < 1)
                {
                    current.enabled = false;
                }
            }
        }
        else if (other.gameObject == player)
        {
            playerInSight = false;
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);
           // Debug.Log(angle);
            if (angle < fieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius))
                {
                    if (hit.collider.gameObject == player)
                    {
                        //player is seen
                        playerInSight = true;
                        previousSighting = player.transform.position;
                        
                    }
                }
            }
        }

       /* else if (other.gameObject.CompareTag("Lamps") == true )
        {
            other.gameObject.GetComponent<Light>().enabled = false;

        }*/
    }
    void OnTriggerExit (Collider other)
    {
        if (other.gameObject == player)
        {
            playerInSight = false;
        }
    }
}
