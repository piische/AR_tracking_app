using System.Collections;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System;
using ApplicationVariables;
using System.Text.RegularExpressions;
using UnityGLTF;
using System.IO;

/// <summary>
/// This scripts handles all conection to the server and will create and store the data
/// <summary>
public class ConnectionHandler : MonoBehaviour {
    private ApplicationModel _Model;
    //private string _Url = "http://192.168.88.3:8080/workbenchServer-0.1.0-SNAPSHOT/";
    //private string _Url = "http://localhost:8080/workbenchServer-0.1.0-SNAPSHOT/";
    // private string _Url = "http://10.35.4.84:4800/workbenchServer-0.1.0-SNAPSHOT/";
    private string _Url = "http://172.20.10.7:8080/workbenchServer-0.1.0-SNAPSHOT/";
    private string _GraphQLUrl;
    private string _ImageURL;
    private string _GLTFURL;
    private string _MoleculeURLPart1;
    private string _MoleculeURLPart2;
    private string _MoleculeURLPart2_2D;
    private volatile int _NumberOfAsync;

    void Start () {
        _GraphQLUrl = _Url + "graphql";
        _ImageURL = _Url + "fileStorage";
        _GLTFURL = _Url + "fileStorage";
        _MoleculeURLPart1 = "https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/cid/";
        _MoleculeURLPart2 = "/record/SDF/?record_type=3d&response_type=save&response_basename=Structure3D_CID_";
        _MoleculeURLPart2_2D = "/record/SDF/?record_type=2d&response_type=save&response_basename=Structure2D_CID_";
        _Model = ApplicationModel.Instance;
    }

    // method to set a different url for testing
    // Start() is not executed in EditMode tests
    public void SetUrl(String url) {
        _Url = url;
        Start();
    }

    public void SetSop(string SOPId) { 
        string input = "id: " + "\"" + SOPId + "\"";
        string operationName = "arContentObjectById";
        
        string query = QueryBuilder(operationName, input, "id number name version description");
        QueryMessage message = new QueryMessage(query, null, operationName);
        string msg = message.ToJSON();
        StartCoroutine(PostDataAsyncSOP(_GraphQLUrl, msg));
    }

    public void SetNewStep(bool next) {
        string operationName = "arContentObjectById";
        int step = _Model.StepModel.stepNumber;
        if (next) {
            step += 1;
        } else {
            step -= 1;
        }
        step = Math.Max(1, Math.Min(step, _Model.Steps.Count)); 
        string stepId = _Model.Steps[step];
        string input = "id: \"" + stepId + "\"";
        string outputField = "id sceneId name stepNumber description, " +
            "stepMaterials { " +
                "sopMaterial { " +
                    "id name placeId warning { " +
                        "id name warningType description imageMetaData { " +
                            "id imageName description " +
                        "} " +
                    "} " +
                    "imageMetaData { " +
                        "id imageName description " +
                    "} " +
                "} " +
                "amount unit " +
             "} " +
            "imageMetaDataList { " +
                "id imageName description" +
            "} " +
            "warningList { " +
                "id name warningType description imageMetaData {" +
                    "id imageName description" +
                 "} " +
            "} ";
        string query = QueryBuilder(operationName, input, outputField);
        QueryMessage message = new QueryMessage(query, null, operationName);
        string msg = message.ToJSON();
        StartCoroutine(PostDataAsyncStep(_GraphQLUrl, msg));
    }

    public void LoadNewSteps(System.Object sender, EventArgs e) {
        //load all steps in dictionary and also load a list of materials
        string operationName = "allStepBySceneId";
        string SOPId = _Model.SopModel.id;
        string input = "sceneId: " + "\"" + SOPId + "\"";
        string outputFiled = "id stepNumber " +
             "stepMaterials { " +
                "sopMaterial { " +
                    "id name placeId warning { " +
                        "id name warningType description imageMetaData { " +
                            "id imageName description " +
                        "} " +
                    "} " +
                    "imageMetaData { " +
                        "id imageName description " +
                    "} " +
                "} " +
                "amount unit " +
             "} ";
        string query = QueryBuilder(operationName, input, outputFiled);
        QueryMessage message = new QueryMessage(query, null, operationName);
        string msg = message.ToJSON();
        StartCoroutine(PostDataAsyncStepList(_GraphQLUrl, msg));
    }

    public void SetNewMolecule(String cas, GameObject moleculeField) {
        string url = _MoleculeURLPart1 + cas + _MoleculeURLPart2 + cas;
        StartCoroutine(PostMoleculeAsync(url, moleculeField, cas));
    }

    public void SetNewImage(String imageName, Action<Texture2D> setImage) {
        StartCoroutine(PostImageAsunc(imageName, setImage));
    }

    public void SetNewCycle(string cycleId) {
        string operationName = "arContentObjectById";
        string id = cycleId;
        string input = "id: " + "\"" + id + "\"";
        string outputFiled = "id name description " +
            "cycleComponentLinks {originId destinationId}";
        string query = QueryBuilder(operationName, input, outputFiled);
        QueryMessage message = new QueryMessage(query, null, operationName);
        string msg = message.ToJSON();
        StartCoroutine(PostCycleAsync(_GraphQLUrl, msg));
    }

    public void LoadComponent(string ComponentId) {
        //load Component
        string operationName = "componentById";
        string id = ComponentId;
        string input = "id: " + "\"" + id + "\"";
        string outputFiled = "id cas name type description";
        string query = QueryBuilder(operationName, input, outputFiled);
        QueryMessage message = new QueryMessage(query, null, operationName);
        string msg = message.ToJSON();
        StartCoroutine(PostCycleComponentAsync(_GraphQLUrl, msg));

    }

    public void SetNewGLTF(string modelName, GameObject parent, Nullable<Color> color = null) {

        StartCoroutine(PostGTLFAsync(modelName, parent, color));
    }

    public void SetNewMedicalData(String id) {
        //todo change -> when graphql interface is known
        string input = "id: " + "\"" + id + "\"";
        string operationName = "arContentObjectById";
        string query = QueryBuilder(operationName, input, "id description name price similarImplant manufacturer material operation " +
            "implantParts { id name description gltfName operation price similarImplant manufacturer material }" +
            "anatomyParts { id description gltfName name }");
        QueryMessage message = new QueryMessage(query, null, operationName);
        string msg = message.ToJSON();
        StartCoroutine(PostImplantAsync(_GraphQLUrl, msg));
    }

    IEnumerator PostDataAsyncStep(string url, string msg) {
        byte[] bytes = Encoding.ASCII.GetBytes(msg);
        WWW www = new WWW(url, bytes, getHeaders());
        yield return www;

        var jsonResults = www.text; 
        if (String.IsNullOrEmpty(www.error)) {
            StepModel stepModel = StepModel.CreateFromJSON(jsonResults);
            _Model.StepModel = stepModel;
            yield return "success";
        } else {
            _Model.ApplicationState = ApplicationState.ERROR;
            yield return -1;

        }
    }

    IEnumerator PostDataAsyncSOP(string url, string msg) {
        byte[] bytes = Encoding.ASCII.GetBytes(msg);
        WWW www = new WWW(url, bytes, getHeaders());
        yield return www;

        var jsonResults = www.text; 
        if (String.IsNullOrEmpty(www.error)) {
            SopModel sopModel = SopModel.CreateFromJSON(jsonResults);
            _Model.StepModel = StepModel.getDefaultModel();
            _Model.ApplicationState = ApplicationState.DESCRIPTION;
            _Model.SopModel = sopModel;
            yield return "success";
        } else {
            _Model.ApplicationState = ApplicationState.ERROR;
            yield return -1;

        }
    }

    IEnumerator PostDataAsyncStepList(string url, string msg) {
        byte[] bytes = Encoding.ASCII.GetBytes(msg);
        WWW www = new WWW(url, bytes, getHeaders());
        yield return www;

        var jsonResults = www.text; 

        if (String.IsNullOrEmpty(www.error)) {
            List<StepModel> stepList = StepModel.CreateListFromJSON(jsonResults);
            List<MaterialModel> materialList = new List<MaterialModel>();
            Dictionary<int, string> dic = new Dictionary<int, string>();
            foreach (StepModel model in stepList) {
                dic.Add(model.stepNumber, model.id);
                foreach (MaterialModel mat in model.stepMaterials) {
                    materialList.Add(mat);
                }
            }
            _Model.Steps = dic;
            _Model.MaterialList = materialList;
            _Model.ApplicationState = ApplicationState.MATERIALCHECK;
   
            yield return "success";
        } else {
            yield return -1;
            _Model.ApplicationState = ApplicationState.ERROR;
        }
    }

    IEnumerator PostImageAsunc(string imageName, Action<Texture2D> setImage) {
        Dictionary<string, string> headers = new Dictionary<string, string>() {
            { "Accept", "multipart/form-data" },
            { "Content-Type", "multipart/form-data"},
            { "fileName", imageName}
        };
        WWW www = new WWW(_ImageURL, null, headers);
        yield return www;

 
        if (String.IsNullOrEmpty(www.error)) {
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(www.bytes);
            setImage(tex);
            yield return "success";
        } else {
            _Model.ApplicationState = ApplicationState.ERROR;
            yield return -1;
        }
    }

    IEnumerator PostMoleculeAsync(string url, GameObject moleculeField, string cas) {
        Dictionary<string, string> headers = new Dictionary<string, string>() {
            { "Accept", "multipart/form-data" },
            { "Content-Type", "multipart/form-data"}
        };
        WWW www = new WWW(url, null, headers);
        yield return www;

        //load 2D modell if no 3D Modell is aviable
        if (!string.IsNullOrEmpty(www.error)) {
            url = _MoleculeURLPart1 + cas + _MoleculeURLPart2_2D + cas;
            www = new WWW(url, null, headers);
            yield return www;
        }
        if (String.IsNullOrEmpty(www.error)) {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            string[] messageArray = www.text.Split('\n');
            string[] infoLine = regex.Replace(messageArray[3].Trim(), " ").Split(' ');

            int numberOfAtoms = Int32.Parse(infoLine[0]);
            int numberOfBonds = Int32.Parse(infoLine[1]);
            string[] atomInformation = new List<string>(messageArray).GetRange(4, numberOfAtoms).ToArray();
            string[] bondInformation = new List<string>(messageArray).GetRange(4 + numberOfAtoms, numberOfBonds).ToArray();
            List<SimpleAtom> atoms = FileReader.GetSimpleAtoms(atomInformation);
            List<SimpleBond> bonds = FileReader.GetSimpleBonds(bondInformation);
            MoleculeRenderer moleculeRenderer = new MoleculeRenderer();
            moleculeRenderer.CreateMolecule(atoms, bonds, moleculeField);
            yield return "success";
        } else {
            _Model.ApplicationState = ApplicationState.ERROR;
            yield return -1;
        }

    }

    IEnumerator PostCycleAsync(string url, string msg) {
        byte[] bytes = Encoding.ASCII.GetBytes(msg);
        WWW www = new WWW(url, bytes, getHeaders());
        yield return www;
        if (String.IsNullOrEmpty(www.error)) {
            var jsonResults = www.text;
            CycleModel cycleModel = CycleModel.CreateFromJSON(jsonResults);
            _Model.CycleModel = cycleModel;

            yield return "success";
        } else {
            _Model.ApplicationState = ApplicationState.ERROR;
            yield return -1;
        }
    }

    IEnumerator PostCycleComponentAsync(string url, string msg) {
        _NumberOfAsync++;
        byte[] bytes = Encoding.ASCII.GetBytes(msg);
        WWW www = new WWW(url, bytes, getHeaders());
        yield return www;
        if (String.IsNullOrEmpty(www.error)) {
            CycleComponentModel cycleComponent = CycleComponentModel.CreateFromJSON(www.text);
            _Model.CycleModelComponents[cycleComponent.id] = cycleComponent;
            _NumberOfAsync--;
            if (_NumberOfAsync == 0) {
                _Model.ApplicationState = ApplicationState.GAME;
            }
            yield return "success";
        } else {
            _Model.ApplicationState = ApplicationState.ERROR;
            yield return -1;
        }
    }


    IEnumerator PostGTLFAsync(string imageName, GameObject parent, Nullable<Color> color) {
        Dictionary<string, string> headers = new Dictionary<string, string>() {
            { "Accept", "multipart/form-data" },
            { "Content-Type", "multipart/form-data"},
            { "fileName", imageName}
        };

        WWW www = new WWW(_GLTFURL, null, headers);
        yield return www;

        MemoryStream dataStream =  new MemoryStream(www.bytes, 0, www.bytes.Length, false, true);
        GLTFSceneImporter loader = new GLTFSceneImporter(imageName, dataStream, parent.transform, true) {
           MaximumLod = 300
        };
     
        yield return loader.Load(-1);

        parent.transform.Find("AuxScene").transform.Find("GLTFNode").localScale = new Vector3(0.002f, 0.002f, 0.002f);
        if (color != null) {
           while(parent.transform.Find("AuxScene").transform.Find("GLTFNode").transform.Find("Primitive") == null) {
                //wait until the GLTFSceneImporter is finished
            } 
            parent.transform.Find("AuxScene").transform.Find("GLTFNode").transform.Find("Primitive").gameObject.GetComponent<MeshRenderer>().materials[0].color = color.Value;
        }
        yield return 1;
    }

    IEnumerator PostImplantAsync(string url, string msg) {
        byte[] bytes = Encoding.ASCII.GetBytes(msg);
        WWW www = new WWW(url, bytes, getHeaders());
        yield return www;
        if (String.IsNullOrEmpty(www.error)) {
            var jsonResults = www.text;
            MedicalModel medicalModel = MedicalModel.CreateFromJSON(jsonResults);
            _Model.MedicalModel = medicalModel;
            _Model.ApplicationState = ApplicationState.IMPLANTVIEW;
            yield return "success";
        } else {
            _Model.ApplicationState = ApplicationState.ERROR;
            yield return -1;
        }

    }

    private string QueryBuilder(string operationName, string inputPairs, string output) {
        return "query " + operationName + "{" + operationName + "(" + inputPairs + ")" + "{" + output + "} }";
    }

    private Dictionary<string, string> getHeaders() {
        return new Dictionary<string, string>() {
            { "Accept", "application/json" },
            { "Content-Type", "application/json"}
        };
    }

    public void SetTestMedical(string id) {
        MedicalModel medicalModel = new MedicalModel {
            id = "12345",
            price = "125.- sFr",
            material = "Titan",
            manufacturer = "FHNW - IMM",
            operation = "Uff Das willst du nicht sehen",
            similarImplant = "Odio aliquam hendrerit sem pulvinar malesuada sit sollicitudin adipiscing accumsan, sollicitudin consequat dui ultricies nisi neque porta ornare, purus senectus tempus nisi massa sit vitae primis fames tincidunt morbi nam mauris ut tristique tellus vestibulum dictumst."
        };

        List<ImplantPartModel> implantParts = new List<ImplantPartModel>();
        for(int i = 0; i < 3; i++) {
            ImplantPartModel modelX = new ImplantPartModel();
            modelX.id = "Id_ " + i;
            modelX.gltfName = id;
            modelX.id = "12345";
            modelX.price = "125.- sFr";
            modelX.material = "Titan";
            modelX.manufacturer = "FHNW - IMM";
            modelX.operation = "Uff Das willst du nicht sehen";
            modelX.similarImplant = "Odio aliquam hendrerit sem pulvinar malesuada sit sollicitudin adipiscing accumsan, sollicitudin consequat dui ultricies nisi neque porta ornare, purus senectus tempus nisi massa sit vitae primis fames tincidunt morbi nam mauris ut tristique tellus vestibulum dictumst.";
            implantParts.Add(modelX);

        }
        medicalModel.implantParts = implantParts;
        List<BodyPartModel> bodyParts = new List<BodyPartModel>();
        for (int i = 0; i < 2; i++) {
            BodyPartModel modelX = new BodyPartModel();
            modelX.id = "Id_ " + i;
            modelX.gltfName = id;
            modelX.id = "12345";
            modelX.description = "blaaablaaa";

            bodyParts.Add(modelX);
        }
        medicalModel.anatomyParts = bodyParts;

        _Model.MedicalModel = medicalModel;
        _Model.ApplicationState = ApplicationState.IMPLANTVIEW;
    }
}
