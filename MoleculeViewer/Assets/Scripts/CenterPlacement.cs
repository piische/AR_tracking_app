using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UI;


public class CenterPlacement : MonoBehaviour {

    
    public void SetToCenter()
    {
        Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / 2) + 1f, (Screen.height / 2) + 2f, Camera.main.nearClipPlane + 2f));

        GameObject bexin = GameObject.Find("Bexin(Clone)");
        GameObject earth = GameObject.Find("Erde(Clone)");
        GameObject motilium = GameObject.Find("Motilium(Clone)");
        GameObject zaldiar = GameObject.Find("Zaldiar(Clone)");

        if (bexin != null)
            bexin.transform.localPosition = cameraPosition;
        if (earth != null)
            earth.transform.localPosition = cameraPosition;
        if (motilium != null)
            motilium.transform.localPosition = cameraPosition;
        if (zaldiar != null)
            zaldiar.transform.localPosition = cameraPosition;

    }

}
