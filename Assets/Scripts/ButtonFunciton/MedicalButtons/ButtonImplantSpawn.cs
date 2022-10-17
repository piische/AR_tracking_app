using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.UX;
using HoloToolkit.Unity.InputModule.Utilities.Interactions;
using UnityEngine;

/// <summary>
/// Script for creating and showing 3D Modell of a Implant
/// <summary>
public class ButtonImplantSpawn : MonoBehaviour, IInputClickHandler {

    private ImplantPartModel _ImplantPart;
    private const int maxLetterOnButton = 12;
    private ConnectionHandler _ConectionHandler;
    private bool _Activatet;
    private GameObject _Implant;

    private void Start() {
        _ConectionHandler = GameObject.Find("Controller").GetComponent<ConnectionHandler>();
        _Activatet = false;
    }

    /// <summary>
    /// Button Function
    /// Create and set metadata and functionality to the Gameobject 
    /// <summary>
    public void OnInputClicked(InputClickedEventData eventData) {
        if(!_Activatet) {
            GameObject o = new GameObject();
            o.AddComponent<TwoHandManipulatable>();
            o.GetComponent<TwoHandManipulatable>().ManipulationMode = ManipulationMode.MoveScaleAndRotate;
            o.GetComponent<TwoHandManipulatable>().EnableEnableOneHandedMovement = true;
            BoundingBox boundingBox = (BoundingBox)Resources.Load("BoundingBoxPref", typeof(BoundingBox));
            o.GetComponent<TwoHandManipulatable>().BoundingBoxPrefab = boundingBox;
            o.AddComponent<ImplantButton>();
            o.GetComponent<ImplantButton>().SetComponent(_ImplantPart);
            o.AddComponent<MeshCollider>();

            o.transform.position = transform.position + new Vector3(0f, 0.2f, 0f);

            //set material für metall 
            if(!_ImplantPart.material.Contains("Polyethylen")) { 
                _ConectionHandler.SetNewGLTF(_ImplantPart.gltfName, o, Color.gray);
            } else {
                _ConectionHandler.SetNewGLTF(_ImplantPart.gltfName, o);
            }
            _Implant = o;
            
            
        } else {
            Destroy(_Implant);
        }
        _Activatet = !_Activatet;
    }

    /// <summary>
    /// seting meta infomation for 3D Modell and set text to the button
    /// <summary>
    public void SetComponent(ImplantPartModel implantPart) {
        _ImplantPart = implantPart;
        int lenght = implantPart.name.Length;
        string name = implantPart.name.Substring(0, lenght > 12 ? 12 : lenght);
        if (lenght > maxLetterOnButton) {
            name = name + "...";
        }
        gameObject.transform.Find("UIButtonSquare").transform.Find("Text").GetComponent<TextMesh>().text = name;
    }
}
