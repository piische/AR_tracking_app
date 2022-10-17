using System;
using UnityEngine;

/// <summary>
/// Data Model
/// Contains information about one cycle component
/// <summary>
[Serializable]
public class CycleComponentModel {
    public string id;
    public string cas;
    public string name;
    public string description;
    public string type;

    public static CycleComponentModel CreateFromJSON(string jsonString) {
        IcommingData data = IcommingData.CreateFromJSON(jsonString);
        return data.data.componentById;
    }

    [Serializable]
    private class CycleComponentData {
        public CycleComponentModel componentById;
        public static CycleComponentData CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<CycleComponentData>(jsonString);
        }
    }
    [Serializable]
    private class IcommingData {
        public CycleComponentData data;
        public static IcommingData CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<IcommingData>(jsonString);
        }
    }
}
