using UnityEngine;
using System.Collections;

public class ProjectilePhysics : MonoBehaviour {

	//speed of the projectile
	public float speed;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		//Sets a vector3 to our current "foward" orientation.
		Vector3 forward = transform.TransformDirection(Vector3.forward);
		//Moves this projectile's possition every frame based on public speed variable and adjusted by foward
		this.transform.position = this.transform.position + (speed * forward);
	}

	//When this collides with anything it sets its active state to false and destroys its self
	void OnTriggerEnter(Collider other){
		if(other.tag == "Rat"){
			RatAI ratai;
			ratai = other.GetComponent<RatAI>();
			ratai.state = RatAI.State.TRAPPED;
		}

		this.gameObject.SetActive (false);
		Destroy (this.gameObject);
	}
}
