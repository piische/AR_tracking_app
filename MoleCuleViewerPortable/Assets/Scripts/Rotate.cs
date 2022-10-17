using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * 15f);
        transform.Rotate(Vector3.left * Time.deltaTime * 30f);
        transform.Rotate(Vector3.forward * Time.deltaTime *15f);
	}
}
