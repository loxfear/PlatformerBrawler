using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoRespawner : MonoBehaviour {

    public float respawnTime;

    private bool counterStarted;
    private Renderer ammoRenderer;
    private Collider ammoCollider;


        //Respawning
        IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        ammoCollider.enabled = !ammoCollider.enabled;
        ammoRenderer.enabled = !ammoRenderer.enabled;
        counterStarted = false;
    }

    // Use this for initialization
    void Start () {
        counterStarted = false;
        ammoCollider = GetComponent<Collider>();
        ammoRenderer = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if(!ammoRenderer.enabled & !counterStarted)
        {
            counterStarted = true;
            StartCoroutine(Respawn());
        }
	}

}
