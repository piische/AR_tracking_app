using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class ToggleVuforia : MonoBehaviour {


    public void VuforiaONOFF()
    {
        bool onoffSwitch = GameObject.Find("GameObject/Cube/Canvas/ToggleMesh").GetComponent<Toggle>().isOn;
        
        if(onoffSwitch == false)
        {
            Debug.Log("Vuforia off");
            //Turn off Vuforia
            VuforiaBehaviour.Instance.enabled = false;
        }
        else 
        {
            Debug.Log("Vuforia on");
            //Turn on Vuforia
            VuforiaBehaviour.Instance.enabled = true;
   
        }
    }  
}

 

