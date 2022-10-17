using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data Model
/// Contains the information about one warning
/// <summary>
[Serializable]
public class WarningModel : IEqualityComparer<WarningModel> {
    public string id;
    public string name;
    public String warningType;
    public string description;
    public ImageModel imageMetaData;

    public static WarningModel CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<WarningModel>(jsonString);
    }

    public static List<WarningModel> CreateListFromJSON(string jsonString) {
        WarningList warning = WarningList.CreateListFromJSON(jsonString);
        return warning.warning;
    }

    public bool Equals(WarningModel x, WarningModel y) {
        if (x == null || y == null) {
            return false;
        } else {
            return x.id.Equals(y.id);
        }
    }

    public int GetHashCode(WarningModel obj) {
        return obj.id.GetHashCode();
    }

    [Serializable]
    private class WarningList {
        public List<WarningModel> warning;
        public static WarningList CreateListFromJSON(string jsonString) {
            return JsonUtility.FromJson<WarningList>(jsonString);
        }
    }
#if !WINDOWS_UWP
    //only for testing (works not on the Hololens)
    public string ToJSON() {
        return JsonUtility.ToJson(this);
    }
#endif
}
