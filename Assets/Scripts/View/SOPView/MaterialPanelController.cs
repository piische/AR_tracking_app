using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for handling the visualization of the material of one step
/// <summary>
public class MaterialPanelController : MonoBehaviour {
    private ApplicationModel _model;
    private List<GameObject> _materialInfos;
    public GameObject materialInformationPanel;
    public GameObject ParentObject;
    public float spaceBetween = 0.04f;
    public float spaceToTitle = 0.078f;
    public float indent = 0.58f;

	void Start () {
        _model = ApplicationModel.Instance;
        _materialInfos = new List<GameObject>();
        _model.StepChanged += setNewMaterialForStep;
    }

    /// <summary>
    /// Event, will be called when step changes, load and visualize new material
    /// <summary>
    void setNewMaterialForStep(System.Object sender, EventArgs e) {
        // destroy all gameobject were material is written
        foreach (GameObject material in _materialInfos) {
            Destroy(material);
        }
        _materialInfos.Clear();

        // generate new material tabs and placing them 
        foreach (MaterialModel material in _model.StepModel.stepMaterials) {
            int numOfMaterialPlaced = _materialInfos.Count;
            float xShift = -indent;
            float yShift = (-spaceBetween * (numOfMaterialPlaced+1)) + spaceToTitle;
            float zShift = 0f;            

            GameObject panel = Instantiate(materialInformationPanel);
            panel.transform.parent = ParentObject.transform;
            panel.transform.rotation = ParentObject.transform.rotation;
            panel.transform.localPosition = new Vector3(xShift, yShift, zShift);

            List<ImageModel> imageList = new List<ImageModel>();
            foreach (WarningModel warning in material.sopMaterial.warning) {
                imageList.Add(warning.imageMetaData);
            }

            panel.transform.Find("WarningButton").GetComponent<ButtonImageDetail>().ImageModel = imageList;
            imageList.Clear();
            imageList.Add(material.sopMaterial.imageMetaData);
            panel.transform.Find("MaterialButton").GetComponent<ButtonImageDetail>().ImageModel = imageList;

            String amount = material.amount.ToString() + " " + material.unit;
            panel.transform.Find("Amount").GetComponent<TextMesh>().text = amount;
            panel.transform.Find("Name").GetComponent<TextMesh>().text = material.sopMaterial.name;
            _materialInfos.Add(panel);
        }
    }
}
