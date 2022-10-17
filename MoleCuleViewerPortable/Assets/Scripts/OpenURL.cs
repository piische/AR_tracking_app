using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenURL : MonoBehaviour
{
    string wikiURL;
	public void OpenLink()
    {
        Text url = GameObject.Find("Canvas/WikiButton/URL").GetComponent<Text>();
        wikiURL = url.text;
        Debug.Log(wikiURL);
        Application.OpenURL(wikiURL);
    }
}


