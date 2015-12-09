using UnityEngine;
using System.Collections;

public class LightToggle : MonoBehaviour {

    //variables needed for light intensity interpolation
    public float fadeSpeed = 2f;
    public float highIntensity = 2.5f;
    public float lowIntensity = 1f;
    public float changeMargin = 0.2f;
    public float range = 20f;
    public Light currentLight;
    public bool playerInLight = false;

    //Local variables
    //Player and current light
    private GameObject playerObject;
    private PlayerController playerController;
    private GameObject cyclopsObject;
    private CyclopsSight cyclopsSight;
    private float targetIntensity;

    //Vectors and distance
    private Vector3 pos_player;
    private Vector3 pos_light;
    public float distance;

    private GameObject obj;
   
	void Start () {
        //Find reference to current light and player objects.
        currentLight = GetComponent<Light>();
        playerObject = GameObject.Find("Player");
        playerController = playerObject.GetComponent<PlayerController>();
        //and cyclops
        cyclopsObject = GameObject.Find("Cyclops");
        cyclopsSight = cyclopsObject.GetComponent<CyclopsSight>();
        //Init light values
        currentLight.intensity = 1.5f;
        currentLight.range = range;
        targetIntensity = highIntensity;
        obj = GameObject.FindGameObjectWithTag("MainCamera");
        
    }

    // Update is called once per frame
    void Update () {
        Camera fpcam = obj.GetComponent<Camera>();
        //get the positions of the player object and light object.
        pos_player = fpcam.transform.position;
        pos_light = currentLight.transform.position;

        //calculate the distance from player to light
        distance = Vector3.Distance(pos_player, pos_light);

        //Debug.Log("Distance between light and player: " + Vector3.Distance(pos_player, pos_light));
        //If the physical player is within 3 units of the light, then
        PlayerWithinRadius(distance);
        toggleLamp(distance);
        
        RaycastHit hit;
        //Vector3 rayDirection2 = (playerController.transform.position) - this.transform.position;
        Vector3 rayDirection = fpcam.transform.position - this.transform.position;
        if(currentLight.enabled)
        {
            if (Physics.Raycast(this.transform.position, rayDirection, out hit, range))
            {
                if (hit.transform.gameObject.tag == "Player")
                {
                    playerInLight = true;
                    cyclopsSight.previousSighting = hit.transform.gameObject.transform.position;
                    
                }
                else
                {
                    playerInLight = false;
                }
                cyclopsSight.playerInSight = playerInLight;
            }
            Debug.DrawLine(this.transform.position, hit.point, Color.cyan, 1.0f);
        }
        else
        {
            playerInLight = false;
        }
        
	}
    
    //Check player position if in range of lamp to toggle.
    void toggleLamp(float dist)
        {
            if (distance <= 10.0)
            {
                // If player presses "E", enable light;
                //Possible implementation: Light turns decreases in intensity over time and eventually turns off if untouched.
                if (Input.GetKeyUp(KeyCode.E))
                {
                    currentLight.enabled = !currentLight.enabled;
                }
            }
        }

    //Check if player in radius every update
    void PlayerWithinRadius(float dist)
    {
        //using  distance = Vector3.Distance(pos_player, pos_light);
        //it will allow us to flag the player to be hidden/non-hidden;

        //check the distance and light range.
        if (currentLight.enabled == true)
        {
            //flag player visible
            //playerController.isVisible = true;
            //Debug.Log(playerController.isVisible);
            currentLight.intensity = Mathf.Lerp(currentLight.intensity, targetIntensity, fadeSpeed * Time.deltaTime);
            CheckTargetIntensity();

        }
        else
        {
            //flag invisible
            currentLight.intensity = Mathf.Lerp(currentLight.intensity, 1f, fadeSpeed * Time.deltaTime);
            //playerController.isVisible = false;
            //Debug.Log(playerController.isVisible);
        }

    }
    //Check current intensity to lerp to. 
    void CheckTargetIntensity()
    {
        //"cooldown"
        if (Mathf.Abs(targetIntensity - currentLight.intensity) < changeMargin)
        {
            //change it back between high and low intensity
            if (targetIntensity == highIntensity)
            {
                targetIntensity = lowIntensity;
            }
            else
            {
                targetIntensity = highIntensity;
            }

        }
    }

}
