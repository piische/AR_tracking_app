using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data Model
/// Contains the information about one image
/// <summary>
[Serializable]
public class ImageModel {
    public string id;
    public string imageName;
    public string description;
    //public List<string> tags;

    public static ImageModel CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<ImageModel>(jsonString);
    }

    public static List<ImageModel> CreateListFromJSON(string jsonString) {
        ImageList imageList = ImageList.CreateListFromJSON(jsonString);
        return imageList.imageList;
    }

    [Serializable]
    private class ImageList {
        public List<ImageModel> imageList;
        public static ImageList CreateListFromJSON(string jsonString) {
            return JsonUtility.FromJson<ImageList>(jsonString);
        }
    }
#if !WINDOWS_UWP
    //only for testing (works not on the Hololens)
    public string ToJSON() {
        return JsonUtility.ToJson(this);
    }
#endif
}
