using HoloToolkit.Unity.InputModule;
using UnityEngine;

/// <summary>
/// Script for spawning molecule
/// <summary>
public class ButtonSpawnMolecule : MonoBehaviour, IInputClickHandler {
    private CycleComponentModel _Component;
    private const int maxLetterOnButton = 12;
    private GameObject _ComponentSpawn;

    void Start() {
        _ComponentSpawn = (GameObject)Resources.Load("Molecule-Specific/ComponentSpawn", typeof(GameObject));
    }

    /// <summary>
    /// Button Function
    /// creates a Gameobject and asing the molecule and metadata that are stored inside the button
    /// <summary>
    public void OnInputClicked(InputClickedEventData eventData) {
        GameObject o = Instantiate(_ComponentSpawn);
        o.transform.position = transform.position + new Vector3(0f, 0.2f, 0f);
        o.transform.GetComponent<MovableComponent>().SetData(_Component);
        if(_Component.type.Equals("MOLECULE")) {
            GameObject.Find("Controller").GetComponent<ConnectionHandler>().SetNewMolecule(_Component.cas, o);
            o.tag = "Molecule";
            o.transform.Find("Name").GetComponent<TextMesh>().fontSize = 96;
        } else {
            o.transform.Find("Name").GetComponent<TextMesh>().fontSize = 48;
            o.transform.Find("Name").transform.localPosition = new Vector3(0, 0, 0);
            o.tag = "Reaction";
            o.transform.GetComponent<MovableComponent>().SetInformationVisible(true);
        }
    }

    /// <summary>
    /// Set cycle component meta data and set the text for the button
    /// <summary>
    public void SetComponent(CycleComponentModel component) {
        _Component = component;
        int lenght = component.name.Length;
        string name = component.name.Substring(0, lenght>12?12:lenght);
        if(lenght > maxLetterOnButton) {
            name = name + "...";
        }
        gameObject.transform.Find("UIButtonSquare").transform.Find("Text").GetComponent<TextMesh>().text = name;
    }
}
