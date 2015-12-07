﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameState : MonoBehaviour {

	private float TimeRemaining;
	public Text countDown;
    public Image staminaBorder;
    public Image staminaBackground;
    public Image staminaBar;
	public Text NumRatsHeld;
    public Text uRRPS;
    public Text numRatsDigging;
    public Text playerVisible;
    public Text visibilityAmount;
    public PlayerController playerController;

	//Spawning rats variables
	public GameObject[] ratSpawnLocations;
	public float ratSpawnRate = 3.0f;
	public int maxActiveRats = 3;
	public GameObject Rat;
	public int numActiveRats;
	private int ratSpawnInd;
	private float staticRSR;

    public RockPile rockPile;
    private float stamScale = 4.0f;

	// Use this for initialization
	void Start () {
		TimeRemaining = 100.0f;
		countDown.text = TimeRemaining.ToString ();
        staminaBorder.rectTransform.sizeDelta = new Vector2(playerController.staminaMax + (10 / stamScale), 30);
        staminaBorder.rectTransform.localScale = staminaBackground.rectTransform.localScale = staminaBar.rectTransform.localScale = new Vector3(stamScale, 1, 1);
        staminaBackground.rectTransform.sizeDelta = new Vector2(playerController.staminaMax, 20);
        staminaBar.rectTransform.sizeDelta = new Vector2(playerController.stamina, 20);
		NumRatsHeld.text = playerController.numRats.ToString ();
        uRRPS.text = "Units of Rock Removed Per Second: " + rockPile.uRRPS.ToString();
        numRatsDigging.text = "Number of Rats Digging: " + rockPile.numRatsDigging.ToString();
        playerVisible.text = "Player visible: " + playerController.isVisible.ToString();
        visibilityAmount.text = "Visibility Amount: " + playerController.visibilityAmount.ToString();
		//Spawning rats initialization
		ratSpawnLocations = GameObject.FindGameObjectsWithTag ("RatSpawn");
		ratSpawnInd = Random.Range(0, ratSpawnLocations.Length);
		numActiveRats = 0;
		staticRSR = ratSpawnRate;
    }
	
	// Update is called once per frame
	void Update () {
		ratSpawn ();

		TimeRemaining -= Time.deltaTime; 
		countDown.text = ((int)TimeRemaining).ToString ();
        staminaBar.rectTransform.sizeDelta = new Vector2(playerController.stamina, 20);
		NumRatsHeld.text = playerController.numRats.ToString ();
        uRRPS.text = "Units of Rock Removed Per Second: " + rockPile.uRRPS.ToString();
        numRatsDigging.text = "Number of Rats Digging: " + rockPile.numRatsDigging.ToString();
        playerVisible.text = "Player visible: " + playerController.isVisible.ToString();
        visibilityAmount.text = "Visibility Amount: " + playerController.visibilityAmount.ToString();
        if(playerController.refilling)
        {
            staminaBar.color = Color.red;
        }
        else
        {
            staminaBar.color = Color.blue;
        }
    }

	void ratSpawn(){
		ratSpawnRate -= Time.deltaTime;
		if (ratSpawnRate <= 0.0f && numActiveRats < maxActiveRats) {
			ratSpawnRate = staticRSR;
			ratSpawnInd = Random.Range(0, ratSpawnLocations.Length);
			Instantiate (Rat, ratSpawnLocations[ratSpawnInd].transform.position, Quaternion.identity);
			numActiveRats += 1;
		}
	}

	public void subRat(){
		numActiveRats -= 1;
	}
}
