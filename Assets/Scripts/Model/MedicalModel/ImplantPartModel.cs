using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data Model
/// Contains one implant part
/// <summary>
[Serializable]
public class ImplantPartModel {
    public string id;
    public string name;
    public string description;
    public string gltfName;
    public string operation; 
    public string price;
    public string similarImplant; 
    public string manufacturer; 
    public string material;

    public static ImplantPartModel CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<ImplantPartModel>(jsonString);
    }

    public static List<ImplantPartModel> CreateListFromJSON(string jsonString) {
        ImplantList implantList = ImplantList.CreateListFromJSON(jsonString);
        return implantList.implantList;
    }

    [Serializable]
    private class ImplantList {
        public List<ImplantPartModel> implantList;
        public static ImplantList CreateListFromJSON(string jsonString) {
            return JsonUtility.FromJson<ImplantList>(jsonString);
        }
    }
}
