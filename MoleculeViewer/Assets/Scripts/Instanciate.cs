using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instanciate : MonoBehaviour
{
    public GameObject myPrefab;
    private bool objectState = false;

    public float xScale = 0.5f;
    public float yScale = 0.5f;
    public float zScale = 0.5f;

    public void InstanciatePrefab()
    {
        Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane + 2f));

        if (objectState == false)
        {
            myPrefab.transform.localScale = new Vector3(xScale, yScale, zScale);
            Instantiate(myPrefab, cameraPosition, Quaternion.identity);
            objectState = true;
        }
    }
}
