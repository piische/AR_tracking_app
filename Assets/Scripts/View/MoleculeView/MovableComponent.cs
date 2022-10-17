using HoloToolkit.Unity.InputModule;
using UnityEngine;

/// <summary>
/// Script for the behaivior of the spawnd component and setting the data in the Gameobject
/// <summary>
public class MovableComponent : MonoBehaviour, IFocusable {
    private CycleComponentModel _CycleComponent;
    private bool _InformationVisible = false;

    public void SetData(CycleComponentModel cycleComponent) {
        _CycleComponent = cycleComponent;
        transform.Find("Name").GetComponent<TextMesh>().text = _CycleComponent.name;
        transform.Find("Description").GetComponent<TextMesh>().text = _CycleComponent.description;
    }

    public void OnFocusEnter() {
        if(!_InformationVisible) {
            gameObject.transform.Find("Name").GetComponent<MeshRenderer>().enabled = true;
            gameObject.transform.Find("Description").GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void OnFocusExit() {
        if (!_InformationVisible) {
            gameObject.transform.Find("Name").GetComponent<MeshRenderer>().enabled = false;
            gameObject.transform.Find("Description").GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void SetInformationVisible(bool isVisible){
        _InformationVisible = isVisible;
        if(isVisible) {
            gameObject.transform.Find("Name").GetComponent<MeshRenderer>().enabled = true;
            gameObject.transform.Find("Description").GetComponent<MeshRenderer>().enabled = true;
        } else {
            gameObject.transform.Find("Name").GetComponent<MeshRenderer>().enabled = false;
            gameObject.transform.Find("Description").GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public CycleComponentModel getCycleComponent() {
        return _CycleComponent;
    }
}
