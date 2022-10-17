using System;
using System.Collections.Generic;
using UnityEngine;
using ApplicationVariables;
using System.Linq;

/// <summary>
/// Script for handling the choos panel (Molecular and Meducal case) visualization
/// <summary>
public class ChoosPanelContorller : MonoBehaviour {  
    private const float _XShift = 0.179f;
    private const float _YShift = -0.182f;
    private const float _ZShift = 0.0f;
    private const float _XStart = -0.265f;
    private const float _YStart = 0.351f;
    private const int _MaxNumberButtonPerRow = 4;
    private Dictionary<string, CycleComponentModel> _CycleModelComponents;
    private ApplicationModel _Model;
    private GameObject _ButtonPrefab;
    private GameObject _GenerallButton;

    void Awake() {
        _Model = ApplicationModel.Instance;
        _Model.ApplicationStateChanged += InitializeAllButotnsButtons;
        _ButtonPrefab = (GameObject)Resources.Load("ButtonSpawnMolecule", typeof(GameObject));
        _GenerallButton = (GameObject)Resources.Load("GenerallButton", typeof(GameObject));
    }

    /// <summary>
    /// Event will be calles when the application state changes
    /// Load all buttons on the panel depending on the state
    /// <summary>
    private void InitializeAllButotnsButtons(System.Object sender, EventArgs e) {
        int i = 0;
        int j = 0;
        float x, y, z;

        if (_Model.ApplicationState == ApplicationState.GAME) {
            _CycleModelComponents = _Model.CycleModelComponents;

            //shuffle list 
            List<CycleComponentModel> cycleComponents = _CycleModelComponents.Values.ToList();
            System.Random rng = new System.Random();
            int n = cycleComponents.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                CycleComponentModel value = cycleComponents[k];
                cycleComponents[k] = cycleComponents[n];
                cycleComponents[n] = value;
            }
            // adding for every component a Button
            foreach (CycleComponentModel cycleComponentModel in cycleComponents) {
                x = _XStart + (_XShift * i);
                y = _YStart + (_YShift * j);
                z = 0;

                GameObject button = Instantiate(_GenerallButton);
                button.transform.localScale = new Vector3(2, 2, 2);
                button.transform.parent = gameObject.transform;
                button.transform.rotation = transform.rotation;
                button.transform.localPosition = new Vector3(x, y, z);
                button.AddComponent<ButtonSpawnMolecule>();
                button.GetComponent<ButtonSpawnMolecule>().SetComponent(cycleComponentModel);

                if (i == _MaxNumberButtonPerRow - 1) {
                    i = 0;
                    j++;
                } else {
                    i++;
                }
            }
            gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        } else if(_Model.ApplicationState == ApplicationState.IMPLANTVIEW) {
            //create buttons for spawning implants
            foreach (ImplantPartModel implantPart in _Model.MedicalModel.implantParts) {
                x = _XStart + (_XShift * i);
                y = _YStart + (_YShift * j);
                z = 0;

                GameObject buttonI = Instantiate(_GenerallButton);
                buttonI.transform.localScale = new Vector3(2, 2, 2); 
                buttonI.transform.parent = gameObject.transform;
                buttonI.transform.rotation = transform.rotation;
                buttonI.transform.localPosition = new Vector3(x, y, z);
                buttonI.AddComponent<ButtonImplantSpawn>();
                buttonI.GetComponent<ButtonImplantSpawn>().SetComponent(implantPart);

                if (i == _MaxNumberButtonPerRow - 1) {
                    i = 0;
                    j++;
                } else {
                    i++;
                }
            }
            j++;
            i = 0;

            foreach (BodyPartModel implantPart in _Model.MedicalModel.anatomyParts) {
                x = _XStart + (_XShift * i);
                y = _YStart + (_YShift * j) + _YShift/3;
                z = 0;

                GameObject buttonI = Instantiate(_GenerallButton);
                buttonI.transform.localScale = new Vector3(2, 2, 2);
                buttonI.transform.parent = gameObject.transform;
                buttonI.transform.rotation = transform.rotation;
                buttonI.transform.localPosition = new Vector3(x, y, z);
                buttonI.transform.Find("UIButtonSquare").GetComponent<MeshRenderer>().material.color = Color.cyan;
                buttonI.AddComponent<ButtonSpawnAnatomy>();
                buttonI.GetComponent<ButtonSpawnAnatomy>().SetComponent(implantPart);

                if (i == _MaxNumberButtonPerRow - 1) {
                    i = 0;
                    j++;
                } else {
                    i++;
                }
            }
        }
    }
}
