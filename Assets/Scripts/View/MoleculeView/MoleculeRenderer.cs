using System.Collections.Generic;
using UnityEngine;
using ApplicationVariables;

/// <summary>
/// Script for handling the visualization of the molecules
/// <summary>
public class MoleculeRenderer {
    private GameObject _Place;
    private readonly float _Scaling = 0.05f;
    private readonly float _MoleculeSize = 0.1f;

    /// <summary>
    /// adjusting size and color depending on the atom type accroding to
    /// http://jmol.sourceforge.net/jscolors/#Atoms%20
    /// <summary>
    private void SetColor(SimpleAtom atom, GameObject sphere) {
        switch(atom.AtomType) {
            case AtomType.H:
                sphere.GetComponent<Renderer>().material.color = Color.white;
                float sizeH = _MoleculeSize / 2;
                sphere.transform.localScale = new Vector3(sizeH, sizeH, sizeH);
                break;
            case AtomType.N:
                sphere.GetComponent<Renderer>().material.color = new Color32(48,80,248,255);
                break;
            case AtomType.C:
                sphere.GetComponent<Renderer>().material.color = new Color32(144, 144, 144,255);
                break;
            case AtomType.O:
                sphere.GetComponent<Renderer>().material.color = new Color32(255,13,13,255);
                break;
            case AtomType.S:
                sphere.GetComponent<Renderer>().material.color = new Color32(255,255,48,255);
                break;
            case AtomType.P:
                sphere.GetComponent<Renderer>().material.color = new Color32(255,128,0,255);
                break;
            default:
                sphere.GetComponent<Renderer>().material.color = Color.green;
                break;
        }
    }

    /// <summary>
    /// creating a gameObject that represents one atom
    /// <summary>
    private GameObject CreateAtom(SimpleAtom atom) {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(_MoleculeSize, _MoleculeSize, _MoleculeSize);
        sphere.AddComponent<MeshFilter>();
        sphere.AddComponent<MeshRenderer>();
        SetColor(atom,sphere);
        return sphere;
    }

    /// <summary>
    /// creating the hole molecule and assing it to parentGameObject for visualization
    /// <summary>
    public void CreateMolecule(List<SimpleAtom> atoms, List<SimpleBond> bonds, GameObject parentGameObject) {
        _Place = parentGameObject;
        List<GameObject> spheres = new List<GameObject>();

        foreach(SimpleAtom atom in atoms) {
            GameObject sphere = CreateAtom(atom); 
            sphere.transform.parent = _Place.transform;
            sphere.transform.rotation = _Place.transform.rotation;
            sphere.transform.localPosition = new Vector3(atom.X * _Scaling, atom.Y * _Scaling, atom.Z * _Scaling);
            spheres.Add(sphere);
        }
        parentGameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
}
