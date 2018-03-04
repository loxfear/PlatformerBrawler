using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointGizmo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "Light Gizmo.tiff", true);
    }
    // Update is called once per frame
    void Update () {
		
	}
}
