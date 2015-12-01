using UnityEngine;
using System.Collections;

public class RockPile : MonoBehaviour {

    public float numRatsDigging = 0.0f;
    public float unitsRemovedPerSecond = 0.0f;
    public PlayerController playerController;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        CalcuateURPS();
	}

    void CalcuateURPS()
    {
        unitsRemovedPerSecond = Mathf.Log(numRatsDigging + 1.0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.Equals(playerController) && Input.GetKeyDown(KeyCode.E))
        {
            //remove rats from player and add to numRatsDigging
        }
        if(collision.Equals("Cyclops"))
        {
            //remove rats from numRatsDigging
        }
    }
}
