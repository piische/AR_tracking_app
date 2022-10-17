using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.InputModule.Utilities.Interactions;
using HoloToolkit.Unity.UX;
using UnityEngine;

/// <summary>
/// Script for creating and showing 3D Modell of a human anatomy part
/// <summary>
public class ButtonSpawnAnatomy : MonoBehaviour, IInputClickHandler {
    private BodyPartModel _BodyPart;
    private const int maxLetterOnButton = 12;
    private ConnectionHandler _ConectionHandler;
    private bool _Activated;
    private GameObject _Anatomy;

    private void Start() {
        _ConectionHandler = GameObject.Find("Controller").GetComponent<ConnectionHandler>();
        _Activated = false;
    }

    /// <summary>
    /// Button Function
    /// Create and set metadata and functionality to the Gameobject 
    /// <summary>
    public void OnInputClicked(InputClickedEventData eventData) {
        if(!_Activated) {
            GameObject o = new GameObject();
            o.AddComponent<TwoHandManipulatable>();
            o.GetComponent<TwoHandManipulatable>().ManipulationMode = ManipulationMode.MoveScaleAndRotate;
            o.GetComponent<TwoHandManipulatable>().EnableEnableOneHandedMovement = true;
            BoundingBox boundingBox = (BoundingBox)Resources.Load("BoundingBoxPref", typeof(BoundingBox));
            o.GetComponent<TwoHandManipulatable>().BoundingBoxPrefab = boundingBox;
            o.AddComponent<MeshCollider>();
            _ConectionHandler.SetNewGLTF(_BodyPart.gltfName, o);
            o.transform.position = transform.position + new Vector3(0f, 0.2f, 0f);
            _Anatomy = o;
        } else {
            Destroy(_Anatomy);
        }
        _Activated = !_Activated;
    }


    /// <summary>
    /// seting meta infomation for 3D Modell and set text to the button
    /// <summary>
    public void SetComponent(BodyPartModel BodyPart) {
        _BodyPart = BodyPart;
        int lenght = _BodyPart.name.Length;
        string name = _BodyPart.name.Substring(0, lenght > 12 ? 12 : lenght);
        if (lenght > maxLetterOnButton) {
            name = name + "...";
        }
        gameObject.transform.Find("UIButtonSquare").transform.Find("Text").GetComponent<TextMesh>().text = name;
    }
}
