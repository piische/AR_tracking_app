using HoloToolkit.Unity.InputModule;
using UnityEngine;

/// <summary>
/// Script for Hud on/off Button
/// <summary>
public class ButtonHud : MonoBehaviour, IInputClickHandler {

    private Canvas _Hud;

	void Start () {
        _Hud = GameObject.Find("Hud").GetComponent<Canvas>();
	}

    public void OnInputClicked(InputClickedEventData e) {
        if (_Hud.isActiveAndEnabled) {
            _Hud.enabled = false;
        } else {
            _Hud.enabled = true;
        }
    }
}
