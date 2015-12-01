using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {


	//Editable in editor
	public float speed = 3.0f;			//Adjust player walking speed
	public float rotateSpeed = 3.0f;    //Adjust camera rotation speed/sensitivity
    public float staminaMax = 50.0f;
	public float stamina;

    //visibility flag variable
    public bool isVisible;

	//The game object that the player fires
	public GameObject projectile;
	public GameObject projectileLocation;

	//Local Variables
	private bool isSprinting;
	private bool isCrouching;
	public int numRats = 0;

	//Might come back and initialize all variables in Start
	void Start() {
        stamina = staminaMax;
	}

	// Update is called once per frame
	void Update () {
	
		CharacterController controller = GetComponent<CharacterController>();

		//Camera controlls
		transform.Rotate(0, Input.GetAxis("Mouse X") * rotateSpeed, 0);		//Moving the mouse left and right turns the whole object
		Camera.main.transform.Rotate (-1*Input.GetAxis ("Mouse Y"), 0, 0);	//Moving the mouse up and down only tilts the camera

		//Movement
		Vector3 forward = transform.TransformDirection(Vector3.forward);	//Used to set movement relative to current orientation
		Vector3 right = transform.TransformDirection (Vector3.right);		//Used to set movement relative to current orientation
		float verticalSpeed = Input.GetAxis ("Vertical") * speed;
		float horizontalSpeed = Input.GetAxis ("Horizontal") * speed;
		isSprinting = false;


		//Crouching
		if (Input.GetButtonUp ("Crouch")) {									//If Q is pressed the player will crouch
			if(isCrouching){												//and if it is pressed again the player will uncrouch
				isCrouching = false;
				this.transform.localScale = this.transform.localScale * 2;	//If uncrouched set Player scale back to normal
			}else{
				isCrouching = true;
				this.transform.localScale = this.transform.localScale / 2;	//If crouched set Player scale to be half it's normal size
			}	
		}

		if (isCrouching) {													//If crouching movement is half it's normal speed
			horizontalSpeed = horizontalSpeed / 2;
			verticalSpeed = verticalSpeed / 2;
		}


		//Sprinting															//If left shift is held down 
		if (Input.GetButton ("Sprint") && !isCrouching && stamina > 0) {	//and the player is not crouching, they will move twice
			verticalSpeed = verticalSpeed * 2;								//their normal speed and isSprinting = true
			horizontalSpeed = horizontalSpeed * 2;							//otherwise isSprinting = false
			isSprinting = true;
		}

		//Applying movement to the player controller
		controller.SimpleMove (forward * verticalSpeed);					//Foward and backward movement
		controller.SimpleMove (right * horizontalSpeed);					//Side to side movement

		//Fire Projectiles
		if (Input.GetButtonDown ("Fire1")) {
			//creates an instance of GameObject projectile based on the location and rotation of GameObject projectileLocation
			Instantiate(projectile, projectileLocation.transform.position , projectileLocation.transform.rotation);
		}

        if(isSprinting)
        {
            DepleteStamina();
        }
        else if(!isSprinting && !Input.GetButton("Sprint"))
        {
            RefillStamina();
        }

	}

    void DepleteStamina()
    {
        stamina -= 10.0f * Time.deltaTime;
    }
    
    void RefillStamina()
    {
        if(stamina < staminaMax)
        stamina += 10.0f * Time.deltaTime;
    }

	public void IncNumRats(){
		numRats += 1;
	}

}
