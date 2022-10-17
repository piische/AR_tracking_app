using System;
using System.Collections.Generic;
using UnityEngine;
using ApplicationVariables;

/// <summary>
/// Script for handling the view of the materialchecklist
/// <summary>
public class MaterialListController : MonoBehaviour{
    private ApplicationModel _model;
    private List<GameObject> _materialInfos;
    public GameObject MaterialInformation;
    public GameObject ParentObject;
    public float SpaceBetween = 0.05f;
    public float SpaceToTitle = 0.05f;
    public float Indent = 0.22f;

    void Start () {
        _model = ApplicationModel.Instance;
        _model.ApplicationStateChanged += StateChanged;
        _model.MaterialListChanged += SetNewMaterialList;
        _materialInfos = new List<GameObject>();
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 4;
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Event function who will be called when application state, only visible in the state materialcheck
    /// <summary>
    private void StateChanged(System.Object sender, EventArgs e) {
        if (_model.ApplicationState == ApplicationState.MATERIALCHECK) {
            Show();
        } else {
            Hide();
        }
    }

    /// <summary>
    /// Event set data new when new materiallist is loaded (only for checking)
    /// <summary>
    private void SetNewMaterialList(System.Object sender, EventArgs e) {
        // destroy all gameobject were material is written
        foreach (GameObject material in _materialInfos) {
            Destroy(material);
        }
        _materialInfos.Clear();

        // generate new material tabs and placing them 
        foreach (MaterialModel material in _model.MaterialList) {
            int numOfMaterialPlaced = _materialInfos.Count;
            float xShift = -Indent;
            float yShift = (-SpaceBetween * (numOfMaterialPlaced + 1)) - SpaceToTitle;
            float zShift = 0f;

            GameObject panel = Instantiate(MaterialInformation);
            panel.transform.parent = ParentObject.transform;
            panel.transform.rotation = ParentObject.transform.rotation;
            panel.transform.localPosition = new Vector3(xShift, yShift, zShift);

            String amount = material.amount.ToString() + " " + material.unit;
            Transform innerPanel = panel.transform.Find("MaterialInfoPanel");
            innerPanel.transform.Find("Amount").GetComponent<TextMesh>().text = amount;
            innerPanel.transform.Find("Name").GetComponent<TextMesh>().text = material.sopMaterial.name;

            List<ImageModel> imageList = new List<ImageModel>();
            foreach (WarningModel warning in material.sopMaterial.warning) {
                imageList.Add(warning.imageMetaData);
            }

            innerPanel.transform.Find("WarningButton").GetComponent<ButtonImageDetail>().ImageModel = imageList;
            imageList.Clear();
            imageList.Add(material.sopMaterial.imageMetaData);
            innerPanel.transform.Find("MaterialButton").GetComponent<ButtonImageDetail>().ImageModel = imageList;
            _materialInfos.Add(panel);
        }
    }
}
