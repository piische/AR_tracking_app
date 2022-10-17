using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate : MonoBehaviour
{

    Animator anim;

    public void StartAnimation()
    {
        anim = gameObject.GetComponent<Animator>();
        anim.SetTrigger("Active");

    }

}

