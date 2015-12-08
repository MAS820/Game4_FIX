using UnityEngine;
using System.Collections;

public class GeneratorRotation : MonoBehaviour {
    public int SPEED = 10;
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.Rotate(new Vector3(0, (this.transform.rotation.z + SPEED) * Time.deltaTime),1 );

    }
}
