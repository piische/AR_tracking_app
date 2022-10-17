using HoloToolkit.Unity.InputModule;
using UnityEngine;

/// <summary>
/// Script for Destroying a gameobject on click
/// <summary>
public class ButtonClosePrefab : MonoBehaviour, IInputClickHandler {

    public void OnInputClicked(InputClickedEventData eventData) {
        Destroy(gameObject.transform.parent.gameObject);
    }
}
