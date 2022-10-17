using ApplicationVariables;
using System;
using UnityEngine;

/// <summary>
/// Data Model
/// Contains the generall information about the AR Content to load
/// <summary>
[Serializable]
public class QRMetaData  {
    public string ArContentObjectId;
    public string ArContentType;

    public static QRMetaData CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<QRMetaData>(jsonString);
    }
}
