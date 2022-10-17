using HoloToolkit.Unity.InputModule;
using UnityEngine;

/// <summary>
/// Script for chaning step backwards
/// <summary>
public class ButtonPrevious : MonoBehaviour, IInputClickHandler{

    private ConnectionHandler _ConectionHandler;

    void Start() {
        _ConectionHandler = GameObject.Find("Controller").GetComponent<ConnectionHandler>();
    }

    public void OnInputClicked(InputClickedEventData e) {
        _ConectionHandler.SetNewStep(false);
    }
}
