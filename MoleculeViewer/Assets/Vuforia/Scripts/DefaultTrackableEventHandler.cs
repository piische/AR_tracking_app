/*==============================================================================
Copyright (c) 2017 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using Image = UnityEngine.UI.Image;

/// <summary>
///     A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class DefaultTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    #region PROTECTED_MEMBER_VARIABLES

    protected TrackableBehaviour mTrackableBehaviour;

    #endregion // PROTECTED_MEMBER_VARIABLES

    #region UNITY_MONOBEHAVIOUR_METHODS

    protected virtual void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
    }

    protected virtual void OnDestroy()
    {
        if (mTrackableBehaviour)
            mTrackableBehaviour.UnregisterTrackableEventHandler(this);
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    /// 
   
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NOT_FOUND)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
            OnTrackingLost();
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }

    #endregion // PUBLIC_METHODS

    #region PROTECTED_METHODS

    public GameObject myPrefab;

    private bool foundObjectState;

    public float xScale = 0.5f;
    public float yScale = 0.5f;
    public float zScale = 0.5f;

    public string textTitle;
    public string textInfo;

    private string textTitleReset = "ready to analyze";
    private string textInfoReset = "\r\nWelcome to the MoleculeViewer\r\nScan a medication pack to see whats inside";

    public string wikiURL;

    public Sprite spriteImage;
    public Sprite spriteStructure;

    // Methods to set the information to the UI board
    public void SetTitle()
    {
        Text txtTitle = GameObject.Find("GameObject/Cube/Canvas/Title").GetComponent<Text>();
        txtTitle.text = textTitle;
    }

    public void SetInfo()
    {
        Text txtInfo = GameObject.Find("GameObject/Cube/Canvas/Info").GetComponent<Text>();
        txtInfo.text = textInfo;
    }

    public void SetWikiURL(string wikiURL)
    {
        Text url = GameObject.Find("GameObject/Cube/Canvas/WikiButton/URL").GetComponent<Text>();
        url.text = wikiURL;
    }

    public void SetImage()
    {
        GameObject image = GameObject.Find("GameObject/Cube/Canvas/Image");
        image.GetComponent<Image>().sprite = spriteImage;
    }
    public void SetStructure()
    {
        GameObject structure = GameObject.Find("GameObject/Cube/Canvas/Structure");
        structure.GetComponent<Image>().sprite = spriteStructure;
        structure.SetActive(true);
    }

    public void ResetBoolFound()
    {
        foundObjectState = false;
    }

    protected virtual void OnTrackingFound()
   {
        // Save the Camera Position in a variable to place the molecule in the center of the view
        Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, (Screen.height / 2) + 2f, Camera.main.nearClipPlane + 2f));

        // Place the UI Buttons when a medical pack is recognized
        GameObject resetButton = GameObject.Find("GameObject/Cube/Canvas/ResetButton");
        resetButton.SetActive(true);
        GameObject wikiButton = GameObject.Find("GameObject/Cube/Canvas/WikiButton");
        wikiButton.SetActive(true);
        GameObject centerButton = GameObject.Find("GameObject/Cube/Canvas/CenterButton");
        centerButton.SetActive(true);

        // The state is used, so that the object just gets recognized once in one position
        // If the object is recognized the information methods place the info on the UI board and instanciate the molecule
        if (foundObjectState == false)
        {
            GameObject bexin = GameObject.Find("Bexin(Clone)");
            GameObject earth = GameObject.Find("Erde(Clone)");
            GameObject motilium = GameObject.Find("Motilium(Clone)");
            GameObject zaldiar = GameObject.Find("Zaldiar(Clone)");

            if (bexin != null)
                bexin.SetActive(true);
            if (earth != null)
                earth.SetActive(true);
            if (motilium != null)
                motilium.SetActive(true);
            if (zaldiar != null)
                zaldiar.SetActive(true);

            myPrefab.SetActive(true);
            myPrefab.transform.localScale = new Vector3(xScale, yScale, zScale);
            Instantiate(myPrefab, cameraPosition, Quaternion.identity);
            SetTitle();
            SetInfo();
            SetWikiURL(wikiURL);
            SetImage();
            SetStructure();
            foundObjectState = true;
        }
    
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;
    }


    protected virtual void OnTrackingLost()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;
    }

    #endregion // PROTECTED_METHODS
}
