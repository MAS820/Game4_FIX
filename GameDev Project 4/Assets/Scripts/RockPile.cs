using UnityEngine;
using System.Collections;

public class RockPile : MonoBehaviour {

    public float uRRPS = 0.0f;
    public float numRatsDigging = 0.0f;
    public PlayerController playerController;
    public CyclopsAI cyclops;
    public AudioClip[] audioClips;
    private AudioSource[] audioSources;
    private bool stopPlayed = false;

	// Use this for initialization
	void Start () {
        audioSources = new AudioSource[audioClips.Length];
        int i = 0;
        while(i < audioClips.Length)
        {
            audioSources[i] = this.gameObject.AddComponent<AudioSource>() as AudioSource;
            audioSources[i].clip = audioClips[i];
            i++;
        }
        audioSources[0].playOnAwake = false;
        audioSources[0].loop = true;
        audioSources[1].playOnAwake = false;
        audioSources[1].loop = false;
        audioSources[2].playOnAwake = false;
        audioSources[2].loop = true;
        audioSources[2].spatialBlend = 1.0f;
        audioSources[2].maxDistance = 20;
    }
	
	// Update is called once per frame
	void Update () {
        calculateURRPS();
        if(numRatsDigging > 0.0f)
        {
            if(!audioSources[0].isPlaying)
            {
                audioSources[0].Play();
            }
            audioSources[1].Stop();
            stopPlayed = false;
            if (!audioSources[2].isPlaying)
            {
                audioSources[2].Play();
            }
        }
        if(numRatsDigging <= 0.0f)
        {
            audioSources[0].Stop();
            if(!audioSources[1].isPlaying && !stopPlayed)
            {
                audioSources[1].Play();
                stopPlayed = true;
            }
            audioSources[2].Stop();
        }
	}

    void calculateURRPS()
    {
        uRRPS = Mathf.Log(numRatsDigging + 1f);
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.E))
        {
			playerController = other.GetComponent <PlayerController> ();
            print("placing rats");
            numRatsDigging += playerController.numRats;
            playerController.numRats = 0;
        }
        if(other.gameObject.tag == "Cyclops" && (cyclops.state == CyclopsAI.State.PATROLLING || cyclops.state == CyclopsAI.State.ENRAGED))
        {
            cyclops.state = CyclopsAI.State.EATING;
        }
    }
}
