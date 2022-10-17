using System;
using UnityEngine;
using UnityEngine.UI;
using ApplicationVariables;

/// <summary>
/// Script for handling the visualization of the HUD
/// <summary>
public class HudController : MonoBehaviour {
    public Text DescriptionPanel;
    public Text StepNumberPanel;
    public Text TitelPanel;
    public Image ImageField;
    private ApplicationModel _model;
    private ConnectionHandler _ConectionHandler;

    void Start() {
        _model = ApplicationModel.Instance;
        _model.StepChanged += TheStepChanged;
        _model.ApplicationStateChanged += TheStepChanged;
        _ConectionHandler = GameObject.Find("Controller").GetComponent<ConnectionHandler>();
        ImageField.enabled = false;
        _model.ApplicationStateChanged += ApplicationStateChanged;
    }

    /// <summary>
    /// Event, will be called when the step changes or application state (Procedural), loading all new information
    /// <summary>
    private void TheStepChanged(System.Object sender, EventArgs e) {
        if (_model.ApplicationState == ApplicationState.PRODUCTION) {
            TitelPanel.text = _model.StepModel.name;
            DescriptionPanel.text = _model.StepModel.description;
            StepNumberPanel.text = _model.StepModel.stepNumber.ToString();
            if (_model.StepModel.imageMetaDataList.Count > 0) {
                ImageField.enabled = true;

                Action<Texture2D> changePicture = new Action<Texture2D>((texture) => {
                    int targetWidth = 512;
                    int targetHeight = 512;
                    RenderTexture renderTexture = RenderTexture.GetTemporary(targetWidth, targetHeight, 0, RenderTextureFormat.ARGB32);
                    Graphics.Blit(texture, renderTexture);
                    texture.Resize(targetWidth, targetHeight);
                    RenderTexture active = RenderTexture.active;
                    RenderTexture.active = renderTexture;
                    texture.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
                    texture.Apply(false, true);
                    RenderTexture.active = active;
                    RenderTexture.ReleaseTemporary(renderTexture);
                    Sprite sprite = Sprite.Create(texture, ImageField.sprite.rect, ImageField.sprite.pivot);
                    ImageField.sprite = sprite;
                });

                _ConectionHandler.SetNewImage(_model.StepModel.imageMetaDataList[0].imageName, changePicture);
            } else {
                ImageField.enabled = false;
            }
        } else if (_model.ApplicationState == ApplicationState.MATERIALCHECK) {
            TitelPanel.text = "Material check";
            DescriptionPanel.text = "";
            StepNumberPanel.text = "";
        }
    }


    private void ApplicationStateChanged(System.Object sender, EventArgs e) {
        if (_model.ApplicationState == ApplicationState.LOADING) {
            TitelPanel.text = "Loading Data";
            DescriptionPanel.text = "Please be patient, Data are loading and applcation is starting";
        } else if(_model.ApplicationState == ApplicationState.QR_READING) {
            TitelPanel.text = "Looking for QR Code";
            DescriptionPanel.text = "";
        } else if (_model.ApplicationState == ApplicationState.MATERIALCHECK) {
            TitelPanel.text = "Material check";
            DescriptionPanel.text = "";
            StepNumberPanel.text = "";
        } else {
            TitelPanel.text = "";
            DescriptionPanel.text = "";
        }
    }
}



