using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCenterPlacement : MonoBehaviour {

    public void SetToCenter()
    {
        Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / 2), (Screen.height / 2), Camera.main.nearClipPlane + 3f));

        GameObject board = GameObject.Find("GameObject/Cube");

        if (board != null)
            board.transform.localPosition = cameraPosition;
    }
}
