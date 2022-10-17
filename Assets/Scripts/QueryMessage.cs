
using UnityEngine;
#if WINDOWS_UWP
using Windows.Data.Json;
#endif

/// <summary>
/// This script creats query in json format 
/// <summary>
public class QueryMessage {

    public string query ;
    public object variables;
    public string operationName;

    public QueryMessage(string query, object variables, string operationName) {
        this.query = query;
        this.variables = variables;
        this.operationName = operationName;
    }

    public static QueryMessage CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<QueryMessage>(jsonString);
    }

    public string ToJSON() {
       
#if WINDOWS_UWP
        JsonObject json = new JsonObject();
        json["query"] = JsonValue.CreateStringValue(this.query);
        json["variables"] = JsonValue.CreateNullValue(); // variables is allways null
        json["operationName"] = JsonValue.CreateStringValue(this.operationName);
        return json.ToString();
#else 
        return JsonUtility.ToJson(this); //only works in unity
#endif
    }
}

