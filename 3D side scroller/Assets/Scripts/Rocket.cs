using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

    public float hitForce;
    public Vector3 startingPos;
    public Vector3 endPos;

	// Use this for initialization
	void Start () {
        startingPos = transform.position;
	}

    void OnCollisionEnter(Collision col)
    {
        //if collided object is floor, destroy
        if (col.gameObject.tag == "Floor" || col.gameObject.tag == "DeathPlane")
        {
            Destroy(this.gameObject);
        }

        if (col.gameObject.tag == "Player")
        {
            endPos = transform.position;
            Debug.Log("Hit the player");
        }
    }

    // Update is called once per frame
    void Update () {
		if(transform.position.y < -10)
        {
            Destroy(this.gameObject);
        }
	} 
}
