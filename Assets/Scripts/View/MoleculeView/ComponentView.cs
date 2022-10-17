using HoloToolkit.Unity.InputModule.Utilities.Interactions;
using UnityEngine;

/// <summary>
/// Descripes the functions of one Component in the cycle
/// <summary>
public class ComponentView : MonoBehaviour {
    private CycleComponentModel _CycleComponentModel;
    private CycleComponentModel _CycleComponentAssigned;
    private GameObject _AssignedMolecule;
    private string _ShallComponentenId;
    private bool _IsStaticMolecule = false;
    public string TriggerTag;
    private bool _IsCorrect;
    
    public void SetMetaData(string shallComponentenId, CycleComponentModel cycleComponentModel, bool isStaticMolecule) {
        _ShallComponentenId = shallComponentenId;
        _CycleComponentModel = cycleComponentModel;
        _CycleComponentAssigned = null;
        _IsStaticMolecule = isStaticMolecule;
    }
    /// <summary>
    /// Set a component to the place, the first one will be set by the application itself 
    /// change the color of the place to yellow if not placed by the application
    /// <summary>
    public void SetComponent(CycleComponentModel cycleComponentAssigned, GameObject assignedMolecule) {
        _CycleComponentAssigned = cycleComponentAssigned;
        if (assignedMolecule != null) {
            _AssignedMolecule = assignedMolecule;
            assignedMolecule.transform.parent = gameObject.transform;
            assignedMolecule.transform.position = gameObject.transform.position;
        }
        if(_IsStaticMolecule) {
            gameObject.transform.Find("Cube").GetComponent<Renderer>().material.color = Color.green;
            _IsCorrect = true;
        } else {
            gameObject.transform.Find("Cube").GetComponent<Renderer>().material.color = Color.yellow;
            _IsCorrect = false;
        }
    }

    /// <summary>
    /// Remove the assigned component 
    /// <summary>
    public void RemoveCompnend() {
        if (_IsStaticMolecule) {
            return;
        }
        _IsCorrect = false;
        Destroy(_AssignedMolecule);
        _CycleComponentAssigned = null;
        gameObject.transform.Find("Cube").GetComponent<Renderer>().material.color = Color.blue;
    }

    /// <summary>
    /// Check if the assigned component is correct and change color
    /// <summary>
    public bool Check() {
        if(_CycleComponentAssigned == null ) {
            gameObject.transform.Find("Cube").GetComponent<Renderer>().material.color = Color.blue;
        } else if(_CycleComponentAssigned.id.Equals(_ShallComponentenId)) {
            gameObject.transform.Find("Cube").GetComponent<Renderer>().material.color = Color.green;
            _IsCorrect = true;
        } else {
            gameObject.transform.Find("Cube").GetComponent<Renderer>().material.color = Color.red;
        }
        return _IsCorrect;
    }

    /// <summary>
    /// Event that descripes the behavior of the gameObject if another gameObject collide with it
    /// Assign component with gameObject if tags are correct
    /// <summary>
    public void OnTriggerEnter(Collider col) {
        //add or replace molecule
        if (_IsStaticMolecule) {
            return;
        }
        if ((col.gameObject.tag == TriggerTag) && !_IsCorrect) {
            if (_CycleComponentAssigned != null) {
                RemoveCompnend();
            }
            col.gameObject.GetComponent<TwoHandManipulatable>().EnableEnableOneHandedMovement = false;
            CycleComponentModel cycleComponentModel = col.gameObject.GetComponent<MovableComponent>().getCycleComponent();
            col.gameObject.GetComponent<MovableComponent>().SetInformationVisible(true);
            SetComponent(cycleComponentModel, col.gameObject);
        } 
    }

    public void OnTriggerExit(Collider col) {

    }
}
