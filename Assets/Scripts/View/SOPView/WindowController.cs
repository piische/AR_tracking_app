using System;
using UnityEngine;
using ApplicationVariables;
using UnityEngine.UI;

/// <summary>
/// Script for handling the general visualization of the main window in the Procedural case
/// <summary>
public class WindowController : MonoBehaviour {
    public Text DescriptionPanel;
    public Text StepNumberPanel;
    public Text TitelPanel;
    public Text SopTitel;
    public GameObject ImageField;
    private ApplicationModel _model;
    private ConnectionHandler _ConectionHandler;

    void Start() {
        _ConectionHandler = GameObject.Find("Controller").GetComponent<ConnectionHandler>();
        _model = ApplicationModel.Instance;
        _model.StepChanged += TheStepChanged;
        _model.SopChanged += TheSOPChanged;
        _model.ApplicationStateChanged += StateChanged;
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
    /// Event, will be called when state changes
    /// windwo is only visible during the state production
    /// <summary>
    private void StateChanged(System.Object sender, EventArgs e) {
        if (_model.ApplicationState == ApplicationState.PRODUCTION) {
            Show();
        } else {
            Hide();
        }
    }

    /// <summary>
    /// Event, will be called when step changes, load and visualize images and step information
    /// <summary>
    private void TheStepChanged(System.Object sender, EventArgs e) {
        TitelPanel.text = _model.StepModel.name;
        DescriptionPanel.text = _model.StepModel.description;
        StepNumberPanel.text = _model.StepModel.stepNumber.ToString() + "/" + _model.Steps.Count.ToString();
        if(_model.StepModel.imageMetaDataList.Count > 0 ) {
            ImageField.SetActive(true);

            Action< Texture2D > changePicture = new Action<Texture2D>((texture) => {
                ImageField.gameObject.GetComponent<Renderer>().material.mainTexture = texture;
            });

            _ConectionHandler.SetNewImage(_model.StepModel.imageMetaDataList[0].imageName, changePicture);
        } else {
            ImageField.SetActive(false);
        }
    }

    private void TheSOPChanged(System.Object sender, EventArgs e) {
        SopTitel.text = _model.SopModel.name;
    }
}
