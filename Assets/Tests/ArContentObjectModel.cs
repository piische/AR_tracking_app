using System;
using UnityEngine;
using System.Collections.Generic;
using static CycleModel;
using ApplicationVariables;

/// <summary>
/// Data Model
/// Contains the generall information about the ArContent for Testing the Interface (Saving data)
/// <summary>
[Serializable]
public class ArContentObjectModel {
    public string id;
    public string sceneId;
    public string name;
    public string number;
    public string description;
    public string version;
    public ArContentType arContentType;
    public ArObjectType arObjectType;
    public int stepNumber;
    public string casNumber;
    public List<LinkedComponent> cycleComponentLinks;
    public List<StepMaterial> stepMaterials; 
    public List<string> warningIds;
    public List<string> anatomyPartIds;
    public List<string> implantPartIds;
    public List<string> pictureIds;
    public string operation;
    public string price;
    public string similarImplant;
    public string manufacturer;
    public string material;


    public static ArContentObjectModel CreateFromJSON(string jsonString) {
        IcommingData data = IcommingData.CreateFromJSON(jsonString);
        return data.data.arContentObjectById;
    }

    [Serializable]
    private class ArContentData{
        public ArContentObjectModel arContentObjectById;
        public static ArContentObjectModel CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<ArContentObjectModel>(jsonString);
        }
    }

    [Serializable]
    private class IcommingData {
        public ArContentData data;
        public static IcommingData CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<IcommingData>(jsonString);
        }
    }

    [Serializable]
    public class StepMaterial
    {
        public string materialId;
        public float amount;
        public string unit;

        public static StepMaterial CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<StepMaterial>(jsonString);
        }
#if !WINDOWS_UWP
        //only for testing (works not on the Hololens)
        public string ToJSON() {
            return JsonUtility.ToJson(this);
        }
#endif
    }

    [Serializable]
    public class SopMaterial
    {
        public string id;
        public string name;
        public List<string> warningIds;
        public string placeId;
        public string pictureId;

        public static SopMaterial CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<SopMaterial>(jsonString);
        }
#if !WINDOWS_UWP
        //only for testing (works not on the Hololens)
        public string ToJSON() {
            return JsonUtility.ToJson(this);
        }
#endif
    }

    [Serializable]
    public class Warning
    {
        public string id;
        public string name;
        public string description;
        public WarningType warningType;
        public string pictureId;

        public static Warning CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<Warning>(jsonString);
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
