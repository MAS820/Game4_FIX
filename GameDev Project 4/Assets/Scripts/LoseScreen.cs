﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoseScreen : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void menuClick()
    {
        Application.LoadLevel("MainMenu");
    }
}
