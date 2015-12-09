using UnityEngine;
using System.Collections;

public class GeneratorRotation : MonoBehaviour {
    public int SPEED = 10;
    public int numRats;
    public GameObject rockPile;
	// Use this for initialization
	void Start () {
        rockPile = GameObject.Find("RockPile");

	    
	}
	
	// Update is called once per frame
	void Update () {

        this.transform.Rotate(new Vector3(0, (this.transform.rotation.z + SPEED) * Time.deltaTime),1 );

    }
}
