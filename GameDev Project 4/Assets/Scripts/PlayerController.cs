using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {


	//Editable in editor
	public float speed = 3.0f;			//Adjust player walking speed
	public float rotateSpeed = 3.0f;    //Adjust camera rotation speed/sensitivity
    public float staminaMax = 50.0f;
	public float stamina;

    //visibility flag variable
    public bool isVisible = false;
    public float visibilityAmount = 0.0f;
    public float visibilityThreshold = 0.3f;
    public float distVisibility = 0.0f;

	//The game object that the player fires
	public GameObject projectile;
	public GameObject projectileLocation;

	//Local Variables
	private bool isSprinting;
	private bool isCrouching;
	public int numRats = 0;
	public float fireRate = 2.0f;
	private float fireRateConst;

    private GameObject[] lights;
    public bool refilling = false;

    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;

    //Might come back and initialize all variables in Start
    void Start() {
        stamina = staminaMax;
        lights = GameObject.FindGameObjectsWithTag("MainLights");
		fireRateConst = fireRate;
		fireRate = 0;
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


		//Sprinting															            //If left shift is held down 
		if (Input.GetButton ("Sprint") && !isCrouching && stamina > 0 && !refilling) {	//and the player is not crouching, they will move twice
			verticalSpeed = verticalSpeed * 2;								            //their normal speed and isSprinting = true
			horizontalSpeed = horizontalSpeed * 2;							            //otherwise isSprinting = false
			isSprinting = true;
		}
        /*
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            //moveDirection *= speed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
        */
        //Applying movement to the player controller
        controller.SimpleMove (forward * verticalSpeed);					//Foward and backward movement
		controller.SimpleMove (right * horizontalSpeed);					//Side to side movement

		//Fire Projectiles
		if (fireRate > 0.0) {
			fireRate -= Time.deltaTime;
		}

		if (Input.GetButtonDown ("Fire1") && fireRate <= 0.0) {
			//creates an instance of GameObject projectile based on the location and rotation of GameObject projectileLocation
			Instantiate(projectile, projectileLocation.transform.position , projectileLocation.transform.rotation);
			fireRate = fireRateConst;
		}

		//Stamina
        if(stamina <= 0 && !Input.GetButton("Sprint") || refilling)
        {
            refilling = true;
            RefillWait();
        }

        if(isSprinting)
        {
            DepleteStamina();
        }
        else if(!isSprinting && !Input.GetButton("Sprint") && !refilling)
        {
            RefillStamina();
        }

        //Visibility
        float tempVisAmount = 0.0f;
        //visibilityAmount = 0.0f;
        foreach (GameObject obj in lights)
        {
            LightToggle light = obj.GetComponent<LightToggle>();
            if(light.playerInLight)
            {
                if(isCrouching)
                {
                    tempVisAmount += (light.currentLight.intensity * 1 / (light.distance + 0.0000001f)) / 2.0f;
                }
                else
                {
                    tempVisAmount += light.currentLight.intensity * 1 / (light.distance + 0.0000001f);
                }
            }
        }

        visibilityAmount = tempVisAmount / 2.0f;

        /*
        if(visibilityAmount >= visibilityThreshold)
        {
            isVisible = true;
        }
        else
        {
            isVisible = false;
        }
        */

        if(distVisibility >= visibilityThreshold)
        {
            isVisible = true;
        }
        else
        {
            isVisible = false;
        }
    }

    float calculateVisibility(Vector3 otherPos)
    {
        float dist = Vector3.Distance(this.transform.position, otherPos);
        distVisibility = Mathf.Pow(visibilityAmount, dist);
        return distVisibility;
    }

    void DepleteStamina()
    {
        refilling = false;
        stamina -= 10.0f * Time.deltaTime;
    }
    
    void RefillStamina()
    {
        if(stamina < staminaMax)
        {
            stamina += 10.0f * Time.deltaTime;
        }
    }
    
    void RefillWait()
    {
        if(stamina < staminaMax / 2)
        {
            stamina += 10.0f * Time.deltaTime;
        }
        else
        {
            refilling = false;
        }
    }

	public void IncNumRats(){
		numRats += 1;
	}

}
