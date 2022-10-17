using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleMesh : MonoBehaviour {


    public void SpatialMapping()
    {
        bool onoffSwitch = GameObject.Find("GameObject/Cube/Canvas/ToggleMesh").GetComponent<Toggle>().isOn;
        
        if(onoffSwitch == true)
        {
            Debug.Log("Mesh enabled");
            SpatialMappingManager.Instance.StartObserver();
            SpatialMappingManager.Instance.CastShadows = true;
            SpatialMappingManager.Instance.DrawVisualMeshes = true;
        }
        else 
        {
            Debug.Log("Mesh disabled");
            SpatialMappingManager.Instance.CastShadows = false;
            SpatialMappingManager.Instance.DrawVisualMeshes = false;
        }
    }

        
    }

 

