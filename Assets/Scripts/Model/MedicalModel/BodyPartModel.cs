using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data Model
/// Contains one anatomy part
/// <summary>
[Serializable]
public class BodyPartModel  {
    public string id;
    public string gltfName;
    public string name;
    public string description;

    public static BodyPartModel CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<BodyPartModel>(jsonString);
    }


    public static List<BodyPartModel> CreateListFromJSON(string jsonString) {
        BodyList bodyList = BodyList.CreateListFromJSON(jsonString);
        return bodyList.bodyList;
    }

    [Serializable]
    private class BodyList {
        public List<BodyPartModel> bodyList;
        public static BodyList CreateListFromJSON(string jsonString) {
            return JsonUtility.FromJson<BodyList>(jsonString);
        }
    }

}
