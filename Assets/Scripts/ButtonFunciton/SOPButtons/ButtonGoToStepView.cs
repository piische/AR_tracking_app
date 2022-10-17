using HoloToolkit.Unity.InputModule;
using UnityEngine;

/// <summary>
/// Script for changing the application state to production 
/// <summary>
public class ButtonGoToStepView : MonoBehaviour, IInputClickHandler {

    private ConnectionHandler _ConectionHandler;

    void Start() {
        _ConectionHandler = GameObject.Find("Controller").GetComponent<ConnectionHandler>();
    }

    /// <summary>
    /// Button Function
    /// set application state to production and load new step from the sop
    /// <summary>
    public void OnInputClicked(InputClickedEventData e) {
        ApplicationModel.Instance.ApplicationState = ApplicationVariables.ApplicationState.PRODUCTION;
        _ConectionHandler.SetNewStep(true);
    }
}
