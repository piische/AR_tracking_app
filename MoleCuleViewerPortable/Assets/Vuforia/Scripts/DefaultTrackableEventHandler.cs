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

    public string textTitle;
    public string textInfo;
    private bool foundObjectState;
    private int infoStateCounter;

    public string wikiURL;

    public void SetTitle()
    {
        Text title = GameObject.Find("GameObject/Canvas/Title").GetComponent<Text>();
        title.text = textTitle;
    }
    public void SetInfo()
    {
        GameObject info = GameObject.Find("GameObject/Canvas/Info");
        GameObject background = GameObject.Find("GameObject/Canvas/Background");

        if (infoStateCounter % 2 == 0)
        {
            info.SetActive(true);
            background.SetActive(true);
        }
        else
        {
            info.SetActive(false);
            background.SetActive(false);
;        }
    }
    public void IterateInfoStateCounter()
    {
        infoStateCounter++;
    }

    public void ResetTitle()
    {
        Text title = GameObject.Find("GameObject/Canvas/Title").GetComponent<Text>();
        title.text = "ready to analyze";
    }
   
    public void SetWikiURL(string wikiURL)
    {
        Text url = GameObject.Find("GameObject/Canvas/WikiButton/URL").GetComponent<Text>();
        url.text = wikiURL;
    }

    protected virtual void OnTrackingFound()
    {
        GameObject welcomeText = GameObject.Find("GameObject/Canvas/WelcomeText");
        welcomeText.SetActive(false);
        GameObject chemistry = GameObject.Find("GameObject/Canvas/Chemistry");
        chemistry.SetActive(false);

        SetTitle();
        SetWikiURL(wikiURL);

        Text info = GameObject.Find("GameObject/Canvas/Info").GetComponent<Text>();
        info.text = textInfo;

        GameObject infoButton = GameObject.Find("GameObject/Canvas/InfoButton");
        infoButton.SetActive(true);
        GameObject wikiButton = GameObject.Find("GameObject/Canvas/WikiButton");
        wikiButton.SetActive(true);


        if (foundObjectState == false)
        {
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
