using HoloToolkit.Unity.InputModule;
using UnityEngine;

/// <summary>
/// script for material checklist (checkbox)
/// <summary>
public class ButtonCheck : MonoBehaviour, IInputClickHandler {

    private bool _activated;
    public Material checkedMaterial;
    public Material notCheckedMaterial;
    public GameObject placeForSymbol;

    void Start() {
        _activated = false;
    }

    /// <summary>
    /// Button Function
    /// set marker or unset marker to the checkbox
    /// <summary>
    public void OnInputClicked(InputClickedEventData e) {
        _activated = !_activated;
        if (_activated) {
            placeForSymbol.GetComponent<Renderer>().material = checkedMaterial;
        } else {
            placeForSymbol.GetComponent<Renderer>().material = notCheckedMaterial;
        }
    }
}
