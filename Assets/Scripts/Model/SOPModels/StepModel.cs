using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Data Model
/// Contains all information about one step
/// <summary>
[Serializable]
public class StepModel {
    public string id;
    public string sceneId;
    public string name;
    public int stepNumber;
    public string description;
    public List<MaterialModel> stepMaterials;
    public List<WarningModel> warningList;
    public List<ImageModel> imageMetaDataList;

    public StepModel(string id, string sceneId, string name, int stepNumber, string description, 
        List<MaterialModel> stepMaterials, List<WarningModel> warningList, List<ImageModel> imageMetaDataList)  {
        this.id = id;
        this.sceneId = sceneId;
        this.name = name;
        this.description = description;
        this.stepNumber = stepNumber;
        this.stepMaterials = stepMaterials;
        this.warningList = warningList;
        this.imageMetaDataList = imageMetaDataList;
    }

    public static StepModel getDefaultModel() {
        return new StepModel("", "", "", 0, "Loading",new List<MaterialModel>(), new List<WarningModel>(), new List<ImageModel>());
    }

    public static StepModel CreateFromJSON(string jsonString) {
        IcommingData data = IcommingData.CreateFromJSON(jsonString);
        return  data.data.arContentObjectById;
    }

    public static List<StepModel> CreateListFromJSON(string jsonString) {
        IcommingData data = IcommingData.CreateFromJSON(jsonString);
        return data.data.allStepBySceneId;
    }

    [Serializable]
    private class StepData {
        public StepModel arContentObjectById;
        public List<StepModel> allStepBySceneId;
        public static StepData CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<StepData>(jsonString);
        }
    }

    [Serializable]
    private class IcommingData {
        public StepData data;
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
