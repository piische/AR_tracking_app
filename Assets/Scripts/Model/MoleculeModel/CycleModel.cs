using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data Model
/// Contains the hole information about the cycle
/// <summary>
[Serializable]
public class CycleModel {
    public string id;
    public string name;
    public string description;
    public List<LinkedComponent> cycleComponentLinks;

    public static CycleModel CreateFromJSON(string jsonString) {
        IcommingData data = IcommingData.CreateFromJSON(jsonString);
        return data.data.arContentObjectById;
    }


    [Serializable]
    private class CycleData {
        public CycleModel arContentObjectById;
        public static CycleData CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<CycleData>(jsonString);
        }
    }

    [Serializable]
    private class IcommingData {
        public CycleData data;
        public static IcommingData CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<IcommingData>(jsonString);
        }
    }

    /// <summary>
    /// Data Model
    /// Contains all linking information about all components
    /// <summary>
    [Serializable]
    public class LinkedComponent {
        public string originId;
        public string destinationId;

        public static LinkedComponent CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<LinkedComponent>(jsonString);
        }

        public static List<LinkedComponent> CreateListFromJSON(string jsonString) {
            LinkedList materialList = LinkedList.CreateListFromJSON(jsonString);
            return materialList.materialList;
        }

        [Serializable]
        private class LinkedList {
            public List<LinkedComponent> materialList;
            public static LinkedList CreateListFromJSON(string jsonString) {
                return JsonUtility.FromJson<LinkedList>(jsonString);
            }
        }
    }
}
