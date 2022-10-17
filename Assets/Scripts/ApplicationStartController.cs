using UnityEngine;
using ApplicationVariables;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// This scripts handle the start of the application
/// <summary>
public class ApplicationStartController : MonoBehaviour {
    private readonly float SIZE_OF_CYCLE = 0.5f;
    private ApplicationModel _Model;
    private ConnectionHandler _ConnectionHandler;
    public GameObject SOPMainWindow;
    public GameObject MaterialList;
    public GameObject ChoosPlatform;
    public GameObject CyclePrefab;
    public GameObject CycleCompenentPrefab;
    public GameObject ArrowPref;
    public GameObject ComponentSpawn;

    void Start() {
        _Model = ApplicationModel.Instance;
        _ConnectionHandler = GameObject.Find("Controller").GetComponent<ConnectionHandler>();
        _Model.ApplicationState = ApplicationState.QR_READING;
    }

    /// <summary>
    /// This Function takes the information from the QRReader and starts the coresponding application 
    /// <summary>
    public void StartWithQRCode(String message) {
        QRMetaData qrData = QRMetaData.CreateFromJSON(message);
        ApplicationMode mode = ApplicationMode.NONE;
        _Model.ApplicationState = ApplicationState.LOADING;
        switch (qrData.ArContentType) {
            case "PROCEDURAL":
                mode = ApplicationMode.SOP;
                break;
            case "MEDICAL":
                mode = ApplicationMode.MEDICAL;
                break;
            case "MOLECULAR":
                mode = ApplicationMode.MOLECULE;
                break;
        }
        StartApplication(qrData.ArContentObjectId, mode);
    }

    /// <summary>
    /// switch who will set up the application
    /// <summary>
    void StartApplication(string id, ApplicationMode mode) {
        _Model.ApplicationMode = mode;
        switch (mode) {
            case ApplicationMode.SOP:
                InstantiateGameObject(MaterialList);
                InstantiateGameObject(SOPMainWindow);
                _Model.SopChanged += _ConnectionHandler.LoadNewSteps;
                _ConnectionHandler.SetSop(id);
                break;
            case ApplicationMode.MOLECULE:
                InstantiateGameObject(ChoosPlatform);
                _Model.CycleModelChanged += SetCycleComponents;
                _Model.ApplicationStateChanged += CreateCycle;
                _ConnectionHandler.SetNewCycle(id);
                break;
            case ApplicationMode.MEDICAL:
                InstantiateGameObject(ChoosPlatform);
                _ConnectionHandler.SetNewMedicalData(id);
                break;
            default:
                break;
        }
    }

    private void InstantiateGameObject(GameObject o) {
        GameObject ob = Instantiate(o);
        ob.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2;
    }

    /// <summary>
    /// Function for listener, should be called whenever the cycle model changed 
    /// <summary>
    void SetCycleComponents(System.Object sender, EventArgs e) {
        Dictionary<string, CycleComponentModel> component = new Dictionary<string, CycleComponentModel>();
        foreach (CycleModel.LinkedComponent componentModel in _Model.CycleModel.cycleComponentLinks) {
            if (!component.ContainsKey(componentModel.originId)) {
                component.Add(componentModel.originId, null);
            }
            if (!component.ContainsKey(componentModel.destinationId)) {
                component.Add(componentModel.destinationId, null);
            }
        }
        _Model.CycleModelComponents = component;

        foreach (string key in _Model.CycleModelComponents.Keys) {
            _ConnectionHandler.LoadComponent(key);
        } 
    }

    /// <summary>
    /// Set up the cylce, should be called whenever a new cycle is seted or the appication state changes
    /// it will load all the necesary data and set up the gameObjects
    /// <summary>
    void CreateCycle(System.Object sender, EventArgs e) {
        if (_Model.ApplicationState != ApplicationState.GAME) {
            return;
        }

        _Model = ApplicationModel.Instance;
        GameObject panel = Instantiate(CyclePrefab);
        panel.transform.localPosition = Camera.main.gameObject.transform.position + new Vector3(1, 0, 2);
        panel.GetComponentInChildren<Text>().text = _Model.CycleModel.description;

        List<string> origins = new List<string>();
        List<string> arrowOrigins = new List<string>();

        //counting number of molecules
        foreach (String key in _Model.CycleModelComponents.Keys) {
            if (_Model.CycleModelComponents[key].type.Equals("MOLECULE")) {
                origins.Add(key);
            } else {
                arrowOrigins.Add(key);
            }
        }
        float radiantShift = (2 * Mathf.PI) / origins.Count;
        int i = 0;

        //creating for every molecule a place to be stored
        Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>();
        Dictionary<string, string> originToDestination = new Dictionary<string, string>();
        foreach (CycleModel.LinkedComponent linkedComponent in _Model.CycleModel.cycleComponentLinks) {
            if (!originToDestination.ContainsKey(linkedComponent.originId)) {
                originToDestination.Add(linkedComponent.originId, linkedComponent.destinationId);
            }
        }

        string destination = origins[0];
        do {
            GameObject componentObject = Instantiate(CycleCompenentPrefab);
            componentObject.transform.parent = panel.transform;
            componentObject.transform.rotation = panel.transform.rotation;
            float x = SIZE_OF_CYCLE * Mathf.Cos(Mathf.PI / 2 - radiantShift * i);
            float y = SIZE_OF_CYCLE * Mathf.Sin(Mathf.PI / 2 - radiantShift * i);
            float z = 0;
            componentObject.transform.localPosition = new Vector3(x, y, z);

            CycleComponentModel cycleComponent = _Model.CycleModelComponents[destination];
            if (i == 0) {
                GameObject o = Instantiate(ComponentSpawn);
                o.transform.GetComponent<MovableComponent>().SetData(cycleComponent);
                if (cycleComponent.type.Equals("MOLECULE")) {
                    GameObject.Find("Controller").GetComponent<ConnectionHandler>().SetNewMolecule(cycleComponent.cas, o);
                    o.tag = "Molecule";
                    o.transform.Find("Name").GetComponent<TextMesh>().fontSize = 96;
                } else {
                    o.transform.Find("Name").GetComponent<TextMesh>().fontSize = 96;
                    o.tag = "Reaction";
                    o.transform.GetComponent<MovableComponent>().SetInformationVisible(true);
                }
                componentObject.GetComponent<ComponentView>().SetMetaData(cycleComponent.id, cycleComponent, true);
                componentObject.GetComponent<ComponentView>().SetComponent(cycleComponent, o);
            } else {
                componentObject.GetComponent<ComponentView>().SetMetaData(cycleComponent.id, cycleComponent, false);
            }
            gameObjects.Add(destination, componentObject);
            destination = originToDestination[destination];
            i++;
        } while (!origins[0].Equals(destination));

        destination = null;

        //creating an arrow for every reaction
        foreach (String key in arrowOrigins) {
            CycleComponentModel cycleComponent = _Model.CycleModelComponents[key];
            destination = originToDestination[key];
            
            GameObject o1 = gameObjects[originToDestination.FirstOrDefault(x => x.Value == destination).Key];
            GameObject o2 = gameObjects[destination];
            GameObject arrow = Instantiate(ArrowPref);

            Vector3 direction = o1.transform.position - o2.transform.position;
            float distance = direction.magnitude - 0.5f;
            arrow.transform.parent = panel.transform;
            arrow.transform.position = o1.transform.position - direction / 2;
            arrow.transform.localScale = new Vector3(distance, distance, distance);

            arrow.transform.LookAt(o2.transform.position);
            arrow.GetComponent<ComponentView>().SetMetaData(cycleComponent.id, cycleComponent, false);
        }
    }
}
