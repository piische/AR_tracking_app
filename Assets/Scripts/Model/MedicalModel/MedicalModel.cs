using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data Model
/// Contains the hole data about the medical case
/// <summary>
[Serializable]
public class MedicalModel  {
    public string id;
    public string name;
    public List<ImplantPartModel> implantParts;
    public List<BodyPartModel> anatomyParts;
    public string operation;
    public string price;
    public string similarImplant;
    public string manufacturer;
    public string material;

    public static MedicalModel CreateFromJSON(string jsonString) {
        IcommingData data = IcommingData.CreateFromJSON(jsonString);
        return data.data.arContentObjectById;
    }

    [Serializable]
    private class ImplantData {
        public MedicalModel arContentObjectById;
        public static ImplantData CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<ImplantData>(jsonString);
        }
    }

    [Serializable]
    private class IcommingData {
        public ImplantData data;
        public static IcommingData CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<IcommingData>(jsonString);
        }
    }
}
