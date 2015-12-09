using UnityEngine;
using System.Collections;

public class CogRotation : MonoBehaviour {
    int SPEED = 10;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.Rotate(new Vector3(((this.transform.rotation.z + SPEED) * Time.deltaTime), 0, 0));


    }
}
