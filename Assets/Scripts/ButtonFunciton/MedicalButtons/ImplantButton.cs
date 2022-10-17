using HoloToolkit.Unity.InputModule;
using UnityEngine;

/// <summary>
/// Script for showing and hiding the wheel for an Implant
/// <summary>
public class ImplantButton: MonoBehaviour, IInputClickHandler {

    private GameObject _WheelPrefab;
    private ImplantPartModel _ImplantPart;
    private bool _WheelISActive;

    private void Start() {
        _WheelPrefab = Instantiate((GameObject)Resources.Load("Medical-Specific/WeelMenu", typeof(GameObject)));
        _WheelPrefab.transform.parent = gameObject.transform;
        _WheelISActive = false;
        _WheelPrefab.SetActive(false);
    }

    public void SetComponent(ImplantPartModel implantPart) {
        _ImplantPart = implantPart;
    }

    public ImplantPartModel GetImplantPart() {
        return _ImplantPart;
    }

    /// <summary>
    /// Button Function
    /// Show or Hide the wheel over the implant
    /// <summary>
    public void OnInputClicked(InputClickedEventData eventData) {
        if(!_WheelISActive) {
            Bounds bounds = gameObject.transform.Find("AuxScene").transform.Find("GLTFNode").transform.Find("Primitive").GetComponent<MeshRenderer>().bounds;
            float y = bounds.center.y + bounds.extents.y + 0.21f;
            _WheelPrefab.transform.position = new Vector3(bounds.center.x, y, bounds.center.z);
            _WheelPrefab.transform.localRotation = Camera.main.transform.rotation;
            _WheelPrefab.SetActive(true);
        } else {
            _WheelPrefab.SetActive(false);
        }
        _WheelISActive = !_WheelISActive;
    }

    void FixedUpdate() {
        if (_WheelISActive) {
            Bounds bounds = gameObject.transform.Find("AuxScene").transform.Find("GLTFNode").transform.Find("Primitive").GetComponent<MeshRenderer>().bounds;
            float y = bounds.center.y + bounds.extents.y + 0.21f;
            _WheelPrefab.transform.position = new Vector3(bounds.center.x, y, bounds.center.z);
            _WheelPrefab.transform.localRotation = Camera.main.transform.rotation;
            Vector3 target = Camera.main.transform.position;
            _WheelPrefab.transform.LookAt(target);
            _WheelPrefab.transform.Rotate(0, 180, 0);
        }
    }
}
