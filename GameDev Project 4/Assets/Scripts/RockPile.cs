using UnityEngine;
using System.Collections;

public class RockPile : MonoBehaviour {

    public float uRRPS = 0.0f;
    public float numRatsDigging = 0.0f;
    public PlayerController playerController;
    public CyclopsAI cyclops;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        calculateURRPS();
	}

    void calculateURRPS()
    {
        uRRPS = Mathf.Log(numRatsDigging + 1f);
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.E))
        {
            print("placing rats");
            numRatsDigging += playerController.numRats;
            playerController.numRats = 0;
        }
        if(other.gameObject.tag == "Cyclops" && (cyclops.state == CyclopsAI.State.PATROLLING || cyclops.state == CyclopsAI.State.ENRAGED))
        {
            cyclops.state = CyclopsAI.State.EATING;
        }
    }
}
