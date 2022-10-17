using ApplicationVariables;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for handling the visualization of the warning and R&S senteces
/// <summary>
public class WarningSignController : MonoBehaviour {
    private ApplicationModel _model;
    private List<GameObject> _warnings;
    private Dictionary<String, WarningType> _warningPairs;
    private ConnectionHandler _ConnectionHandler;
    public TextMesh Titel;
    public GameObject WarningInformation;
    public GameObject RSInformation;
    public GameObject ParentSign;
    public GameObject ParentRPhrases;
    public GameObject ParentSPhrases;
    public float SpaceBetweenHorizontal = 0.11f;
    public float SpaceBetweenVertical = -0.1f;
    public float StartPositionHight = -0.069f;
    public float StartPositionWidth = -0.134f;
    public float StartSentencePositionHight = -0.04f;
    public float StartSentencePositionWidth = -0.05f;
    public float SpaceBetweenSentence = -0.07f;
    public int InfoPerRow = 3;

    void Start() {
        _ConnectionHandler = GameObject.Find("Controller").GetComponent<ConnectionHandler>();
        _model = ApplicationModel.Instance;
        _model.StepChanged += StepChanged;
        _warnings = new List<GameObject>();
        _warningPairs = new Dictionary<string, WarningType>();
        _warningPairs.Add("NONE", WarningType.NONE);
        _warningPairs.Add("R_PHRASES", WarningType.R_PHRASES);
        _warningPairs.Add("S_PHRASES", WarningType.S_PHRASES);
        _warningPairs.Add("WARNING", WarningType.WARNING);
    }

    /// <summary>
    /// Event, will be called when step changes, load and visualize new Warnings
    /// <summary>
    void StepChanged(System.Object sender, EventArgs e) {
        // destroy all warning gamobject
        foreach (GameObject warning in _warnings) {
            Destroy(warning);
        }
        _warnings.Clear();

        // gathering all warnings only once
        HashSet<WarningModel> set = new HashSet<WarningModel>(new WarningCompearer());
        foreach (WarningModel warning in _model.StepModel.warningList) {
            if (!set.Contains(warning)) {
                set.Add(warning);
            }
        }

        foreach (MaterialModel material in _model.StepModel.stepMaterials) {
            foreach (WarningModel warning in material.sopMaterial.warning) {
                if (!set.Contains(warning) ) {
                    set.Add(warning);
                }
            }
        }

        if (set.Count > 0) {
            gameObject.SetActive(true);
            // generate new warning object and place them
            int numberPlacedSign = 0;
            int numberPlacedR = 0;
            int numberPlacedS = 0;
            float xPosition, yPosition;
            float zPosition = 0.0f;
            foreach (WarningModel warning in set) {

                switch(getWarningType(warning.warningType)) {

                    case WarningType.WARNING:
                        int numberOfRow = numberPlacedSign / InfoPerRow;
                        xPosition = StartPositionWidth + ((numberPlacedSign - (numberOfRow * InfoPerRow)) * SpaceBetweenHorizontal);
                        yPosition = StartPositionHight + (numberOfRow * SpaceBetweenVertical);

                        GameObject panel = Instantiate(WarningInformation);
                        panel.transform.parent = ParentSign.transform;
                        panel.transform.rotation = ParentSign.transform.rotation;
                        panel.transform.localPosition = new Vector3(xPosition, yPosition, zPosition);
                        numberPlacedSign += 1;

                        Action<Texture2D> changePicture = new Action<Texture2D>((texture) => {
                            GameObject o = panel.transform.Find("UIButtonSquare").transform.Find("WarningSign").gameObject;
                            o.gameObject.GetComponent<Renderer>().material.mainTexture = texture;
                        });

                        _ConnectionHandler.SetNewImage(_model.StepModel.imageMetaDataList[0].imageName, changePicture);
                        _warnings.Add(panel);
                        break;

                    case WarningType.R_PHRASES:
                        xPosition = StartSentencePositionWidth;
                        yPosition = StartSentencePositionHight + (numberPlacedR * SpaceBetweenSentence);

                        GameObject rInfo = Instantiate(RSInformation);
                        rInfo.transform.parent = ParentRPhrases.transform;
                        rInfo.transform.rotation = ParentRPhrases.transform.rotation;
                        rInfo.transform.localPosition = new Vector3(xPosition, yPosition, zPosition);
                        numberPlacedR += 1;

                        _warnings.Add(rInfo);
                        rInfo.transform.Find("Titel").GetComponent<TextMesh>().text = warning.name + ":";
                        rInfo.transform.Find("Description").GetComponent<TextMesh>().text = warning.description;
                        break;

                    case WarningType.S_PHRASES:
                        xPosition = StartSentencePositionWidth;
                        yPosition = StartSentencePositionHight + (numberPlacedS * SpaceBetweenSentence);

                        GameObject sInfo = Instantiate(RSInformation);
                        sInfo.transform.parent = ParentSPhrases.transform;
                        sInfo.transform.rotation = ParentSPhrases.transform.rotation;
                        sInfo.transform.localPosition = new Vector3(xPosition, yPosition, zPosition);
                        numberPlacedS += 1;

                        _warnings.Add(sInfo);
                        sInfo.transform.Find("Titel").GetComponent<TextMesh>().text = warning.name + ":";
                        sInfo.transform.Find("Description").GetComponent<TextMesh>().text = warning.description;
                        break;
                }
            }
        }
    }

    public class WarningCompearer : IEqualityComparer<WarningModel> {
        public bool Equals(WarningModel x, WarningModel y) {
            if (x == null || y == null) {
                return false;
            } else {
                return x.id.Equals(y.id);
            }
        }

        public int GetHashCode(WarningModel obj) {
            return obj.id.GetHashCode();
        }
    }

    public  WarningType getWarningType(string s) {
        if (_warningPairs.ContainsKey(s)) {
            return _warningPairs[s];
        } else {
            return WarningType.NONE;
        }
    }
}
