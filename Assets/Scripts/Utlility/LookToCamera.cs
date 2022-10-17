using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script attaching to a gamobject -> gamobject will allways loock to the camera
/// <summary>
public class LookToCamera : MonoBehaviour {
    public float xTransofrm = 0;

	void FixedUpdate() {
        Vector3 target = Camera.main.transform.position;
        transform.LookAt(target);
        transform.Rotate(xTransofrm, 180, 0);
    }
}
