using System;
using UnityEngine;

/// <summary>
/// Data Model
/// Contains the generall information about the SOP
/// <summary>
[Serializable]
public class SopModel {
    public string id;
    public string number;
    public string name;
    public string version;
    public string description;

    public SopModel(string id, string number, string name, string version, string description) {
        this.id = id;
        this.name = name;
        this.number = number;
        this.version = version;
        this.description = description;
    }

    public static SopModel getDefaultModel() {
        return new SopModel("", "", "", "","");
    }

    public static SopModel CreateFromJSON(string jsonString) {
        IcommingData data = IcommingData.CreateFromJSON(jsonString);
        return data.data.arContentObjectById;
    }

    [Serializable]
    private class SOPData{
        public SopModel arContentObjectById;
        public static SOPData CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<SOPData>(jsonString);
        }
    }

    [Serializable]
    private class IcommingData {
        public SOPData data;
        public static IcommingData CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<IcommingData>(jsonString);
        }
    }

#if !WINDOWS_UWP
    //only for testing (works not on the Hololens)
    public string ToJSON() {
        return JsonUtility.ToJson(this); 
    }
#endif
}
