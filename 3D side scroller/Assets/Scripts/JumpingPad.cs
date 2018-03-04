using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JumpingPad : MonoBehaviour {

    public float boostPower;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.rigidbody.AddForce(transform.up * boostPower);
        }

    }
}
