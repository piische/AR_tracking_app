using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Data Model
/// Contains the information about one material from the step
/// <summary>
[Serializable]
public class MaterialModel {
    public MaterialInfo sopMaterial;
    public double amount;
    public string unit;

    public static MaterialModel CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<MaterialModel>(jsonString);
    }

    public static List<MaterialModel> CreateListFromJSON(string jsonString) {
        MaterialList materialList = MaterialList.CreateListFromJSON(jsonString);
        return materialList.materialList;
    }

    [Serializable]
    private class MaterialList{
        public List<MaterialModel> materialList;
        public static MaterialList CreateListFromJSON(string jsonString) {
            return JsonUtility.FromJson<MaterialList>(jsonString);
        }
    }

    /// <summary>
    /// Data Model
    /// Contains the generall information from the material
    /// <summary>
    [Serializable]
    public class MaterialInfo {
        public string id;
        public string name;
        public string placeId;
        public List<WarningModel> warning;
        public ImageModel imageMetaData;

        public static MaterialInfo CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<MaterialInfo>(jsonString);
        }
#if !WINDOWS_UWP
        //only for testing (works not on the Hololens)
        public string ToJSON() {
            return JsonUtility.ToJson(this);
        }
#endif
    }

#if !WINDOWS_UWP
    //only for testing (works not on the Hololens)
    public string ToJSON() {
        return JsonUtility.ToJson(this);
    }
#endif
}
