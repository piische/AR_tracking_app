#if (UNITY_EDITOR)
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.Threading;
using ApplicationVariables;
#endif
public class BackendIntegrationTest {
#if (UNITY_EDITOR)
    private ApplicationModel _Model;
    private ConnectionHandler _ConnectionHandler;
    private GameObject _ConnectionHandler_gameObject;
    private bool SetUpDone = false;
    private static readonly string _URL = "http://localhost:8080/workbenchServer-0.1.0-SNAPSHOT/";
    private readonly string _GraphQLUrl = _URL + "graphql";
    private readonly string _FileURL = _URL + "fileStorage/";
    private readonly double _MaxWaitTime = 40;

    //Test Data
    private ArContentObjectModel _TestObject;
    private ImageModel _ImageModel;
    private List<ArContentObjectModel.Warning> _Warnings;
    private List<CycleComponentModel> _CycleComponents;
    private List<ArContentObjectModel.SopMaterial> _SopMaterials;
    private List<ImplantPartModel> _ImplantParts;
    
    public bool Setup() {
        if(SetUpDone) {
            return true;
        }
        string operationName;
        string gLTFName = "checkGLTF.gltf";
        QueryMessage message;
        _Warnings = new List<ArContentObjectModel.Warning>();
        _SopMaterials = new List<ArContentObjectModel.SopMaterial>();
        _CycleComponents = new List<CycleComponentModel>();
        _ImplantParts = new List<ImplantPartModel>();
        string sceneId = "5c484886bfe52300017391d8";
        string imageId = "5cd56ce9bfe52300017f8233";
        string warningId1 = "5cd56cfabfe52300017f8234";
        string warningId2 = "5cd56d02bfe52300017f8235";
        string matId1 = "5cd56d0cbfe52300017f8236";
        string matId2 = "5cd56d13bfe52300017f8237";
        string cycleComp1 = "5cd914abbfe5230001aaed1a";
        string cycleComp2 = "5cd914b3bfe5230001aaed1b";
        string medId1 = "5cd92c8abfe5230001aaed1c";
        string medId2 = "5cd92c8abfe5230001aaed1d";

        Dictionary<string, string> header = new Dictionary<string, string>() {
            { "Accept", "application/json" },
            { "Content-Type", "application/json"}
        };
        SetUpDone = true;
        _Model = ApplicationModel.Instance;

        // Setting ConectionHandler up
        _ConnectionHandler_gameObject = new GameObject("ConnectionHandler");
        _ConnectionHandler_gameObject.AddComponent<ConnectionHandler>();
        _ConnectionHandler = _ConnectionHandler_gameObject.GetComponent<ConnectionHandler>();
        _ConnectionHandler.SetUrl(_URL);


        //Creating Test Scene Data

        //saving 1 imageMetadata -> testimage is on server
        _ImageModel = new ImageModel {
            id = imageId,
            description = "some random image for testing",
            imageName = "checkOk.png"
        };
        operationName = "saveImageMetaData";
        string input = "id: \"" + _ImageModel.id + "\" description: \"" + _ImageModel.description + "\" imageName: \"" + _ImageModel.imageName + "\"";
        message = new QueryMessage(MutationBuilder(operationName, input), null, operationName);
        byte[] bytesImage = Encoding.ASCII.GetBytes(message.ToJSON());
        WWW wwwImg = new WWW(_GraphQLUrl, bytesImage, header);

        //saving 2 warnings
        ArContentObjectModel.Warning warningModel = new ArContentObjectModel.Warning {
            id = warningId1,
            name = "Explosive",
            warningType = WarningType.WARNING,
            description = "boom",
            pictureId = imageId
        };
        operationName = "saveWarning";
        input = "id: \"" + warningModel.id + "\", description: \"" + warningModel.description + "\", name: \"" + warningModel.name + 
            "\", warningType: " + warningModel.warningType + ", pictureId: \"" + warningModel.pictureId + "\"";
        message = new QueryMessage(MutationBuilder(operationName, input), null, operationName);
        byte[] bytesWarning1 = Encoding.ASCII.GetBytes(message.ToJSON());
        WWW wwwWarning1 = new WWW(_GraphQLUrl, bytesWarning1, header);
        _Warnings.Add(warningModel);

        warningModel = new ArContentObjectModel.Warning {
            id = warningId2,
            name = "S3",
            warningType = WarningType.S_PHRASES,
            description = "Protect yourself",
            pictureId = imageId
        };
        input = "id: \"" + warningModel.id + "\", description: \"" + warningModel.description + "\", name: \"" + warningModel.name +
             "\", warningType: " + warningModel.warningType + ", pictureId: \"" + warningModel.pictureId + "\"";
        message = new QueryMessage(MutationBuilder(operationName, input), null, operationName);
        byte[] bytesWarning2 = Encoding.ASCII.GetBytes(message.ToJSON());
        WWW wwwWarning2 = new WWW(_GraphQLUrl, bytesWarning2, header);
        _Warnings.Add(warningModel);

        //saving 2 materials and link it 
        ArContentObjectModel.SopMaterial sopMat = new ArContentObjectModel.SopMaterial {
            id = matId1,
            name = "Ethanol",
            placeId = "Unknown",
            pictureId = imageId,
            warningIds = new List<string> {
                warningId1
            }

        };
        operationName = "saveSOPMaterial";
        input = "id: \"" + sopMat.id + "\", name: \"" + sopMat.name + "\", placeId: \"" + sopMat.placeId +
            "\", pictureId: \"" + sopMat.pictureId + "\"" + ", warningIds: " + CreateInputList(sopMat.warningIds);
        message = new QueryMessage(MutationBuilder(operationName, input), null, operationName);
        byte[] bytesMat1 = Encoding.ASCII.GetBytes(message.ToJSON());
        WWW wwwwMat1 = new WWW(_GraphQLUrl, bytesMat1, header);
        _SopMaterials.Add(sopMat);

        sopMat = new ArContentObjectModel.SopMaterial {
            id = matId2,
            name = "Aspirin",
            placeId = "Unknown",
            pictureId = imageId,
            warningIds = new List<string> {
                warningId1, warningId2
            }
        };

        input = "id: \"" + sopMat.id + "\", name: \"" + sopMat.name + "\", placeId: \"" + sopMat.placeId +
            "\", pictureId: \"" + sopMat.pictureId + "\"" + ", warningIds: " + CreateInputList(sopMat.warningIds);
        message = new QueryMessage(MutationBuilder(operationName, input), null, operationName);
        byte[] bytesMat2 = Encoding.ASCII.GetBytes(message.ToJSON());
        WWW wwwwMat2 = new WWW(_GraphQLUrl, bytesMat2, header);
        _SopMaterials.Add(sopMat);

        //creating 2 cylcecomponents
        CycleComponentModel cm = new CycleComponentModel {
            id = cycleComp1,
            name = "Ethanol",
            description = "A clear, colorless liquid rapidly absorbed from the gastrointestinal tract and distributed throughout the body. ",
            type = "REACTION",
            cas = "702"
        };
        operationName = "saveCycleComponent";
        input = "id: \"" + cm.id + "\", name: \"" + cm.name + "\", description: \"" + cm.description +
            "\", type: " + cm.type  + ", cas: \"" + cm.cas + "\"";
        message = new QueryMessage(MutationBuilder(operationName, input), null, operationName);
        byte[] byteCm1 = Encoding.ASCII.GetBytes(message.ToJSON());
        WWW wwwwCm1 = new WWW(_GraphQLUrl, byteCm1, header);
        _CycleComponents.Add(cm);

        cm = new CycleComponentModel {
            id = cycleComp2,
            name = "L-Glucose",
            description = "Sweet soo sweet",
            type = "MOLECULE",
            cas = "	10954115"
        };
        operationName = "saveCycleComponent";
        input = "id: \"" + cm.id + "\", name: \"" + cm.name + "\", description: \"" + cm.description +
            "\", type: " + cm.type + ", cas: \"" + cm.cas + "\"";
        message = new QueryMessage(MutationBuilder(operationName, input), null, operationName);
        byte[] byteCm2 = Encoding.ASCII.GetBytes(message.ToJSON());
        WWW wwwwCm2 = new WWW(_GraphQLUrl, byteCm2, header);
        _CycleComponents.Add(cm);

        //medical test data
        ImplantPartModel threeDimModel = new ImplantPartModel {
            id = medId1,
            manufacturer = "FHNW",
            material = "Titan",
            operation = "rly nasty stuff",
            price = "100sFr, 20sFr aditional cost ",
            similarImplant = "none",
            gltfName = gLTFName,
            name = "bone 123",
            description = "some cracy comment"
            
        };

    operationName = "saveThreeDimensionalModel";
        input = "id: \"" + threeDimModel.id + "\", gltfName: \"" + threeDimModel.gltfName + "\", operation: \"" + threeDimModel.operation +
            "\", price: \"" + threeDimModel.price + "\", similarImplant: \"" + threeDimModel.similarImplant +
            "\", manufacturer: \"" + threeDimModel.manufacturer + "\", material: \"" + threeDimModel.material +
            "\", type: " + "NONE" + ", name: \"" + threeDimModel.name + "\", description: \"" + threeDimModel.description + "\"";

        message = new QueryMessage(MutationBuilder(operationName, input), null, operationName);
        byte[] byteMed = Encoding.ASCII.GetBytes(message.ToJSON());
        WWW wwwwMed = new WWW(_GraphQLUrl, byteMed, header);
        _ImplantParts.Add(threeDimModel);

        threeDimModel = new ImplantPartModel {
            id = medId2,
            manufacturer = "FHNW",
            material = "Titan",
            operation = "rly nasty stuff",
            price = "free",
            similarImplant = "i dont now",
            gltfName = gLTFName,
            name = "sadjio 09876",
            description = "comment No 990567"

        };
        input = "id: \"" + threeDimModel.id + "\", gltfName: \"" + threeDimModel.gltfName + "\", operation: \"" + threeDimModel.operation +
            "\", price: \"" + threeDimModel.price + "\", similarImplant: \"" + threeDimModel.similarImplant +
            "\", manufacturer: \"" + threeDimModel.manufacturer + "\", material: \"" + threeDimModel.material +
            "\", type: " + "NONE" + ", name: \"" + threeDimModel.name + "\", description: \"" + threeDimModel.description +  "\"";

        message = new QueryMessage(MutationBuilder(operationName, input), null, operationName);
        byte[] byteMed2 = Encoding.ASCII.GetBytes(message.ToJSON());
        WWW wwwwMed2 = new WWW(_GraphQLUrl, byteMed2, header);
        _ImplantParts.Add(threeDimModel);

        //main test object
        _TestObject = new ArContentObjectModel {
            id = sceneId,
            sceneId = sceneId,
            name = "Test Case",
            number = "12.233.421",
            version = "1.2",
            description = "this ArContentObject is for Testing the Hololens app",
            arContentType = ArContentType.PROCEDURAL,
            arObjectType = ArObjectType.STEP,
            stepNumber = 1,
            casNumber = "702", 
            cycleComponentLinks = new List<CycleModel.LinkedComponent> {
                new CycleModel.LinkedComponent {
                    destinationId = cycleComp1,
                    originId = cycleComp2
                },
                new CycleModel.LinkedComponent {
                    destinationId = cycleComp2,
                    originId = cycleComp1
                }
            },
            stepMaterials = new List<ArContentObjectModel.StepMaterial>() {
                new ArContentObjectModel.StepMaterial {
                    amount = 10,
                    unit = "g",
                    materialId = matId1
                },
                new ArContentObjectModel.StepMaterial {
                    amount = 1155,
                    unit = "ml",
                    materialId = matId2
                }
            },
            warningIds = new List<string> {
                warningId1, warningId2
            },
            pictureIds = new List<string> {
                imageId
            },
            implantPartIds = new List<string> {
                medId1, medId2
            },
            anatomyPartIds = new List<string> {
                medId2
            },
            operation = "Realy ugly stuff",
            price =  "500 Euro",
            manufacturer = "FHNW",
            similarImplant = "it is unique",
            material = "Titan"
        };

        string stepMat = "[ ";
        foreach(ArContentObjectModel.StepMaterial mat in _TestObject.stepMaterials) {
            stepMat += " { amount: " + mat.amount + ", unit: \"" + mat.unit +  "\", materialId: \"" + mat.materialId + "\" },";
        }
        stepMat.TrimEnd(',');
        stepMat += " ]";

        string cycleComponents = "[ ";
        foreach (CycleModel.LinkedComponent links in _TestObject.cycleComponentLinks) {
            cycleComponents += " { destinationId: \"" + links.destinationId + "\", originId: \""  + links.originId + "\" },";
        }
        cycleComponents.TrimEnd(',');
        cycleComponents += " ]";


        operationName = "saveArContentObject";
        input = "id: \"" + _TestObject.id + "\", sceneId: \"" + _TestObject.sceneId + "\", name: \"" + _TestObject.name +
            "\", number: \"" + _TestObject.number + "\", version: \"" + _TestObject.version + "\", description: \"" + _TestObject.description +
            "\", arContentType: " + _TestObject.arContentType + ", arObjectType: " + _TestObject.arObjectType + ", stepNumber: " + _TestObject.stepNumber +
            ", casNumber: \"" + _TestObject.casNumber + "\", cycleComponentLinks: " + cycleComponents + ", stepMaterials: " + stepMat +
            ", warningIds: " + CreateInputList(_TestObject.warningIds) + ", pictureIds: " + CreateInputList(_TestObject.pictureIds) +
            ", implantPartIds: " + CreateInputList(_TestObject.implantPartIds) + ", anatomyPartIds: " + CreateInputList(_TestObject.anatomyPartIds) +
            ", operation: \"" + _TestObject.operation + "\", price: \"" + _TestObject.price + "\", manufacturer: \"" + _TestObject.manufacturer +
            "\", similarImplant: \"" + _TestObject.similarImplant + "\", material: \"" + _TestObject.material + "\"";

        message = new QueryMessage(MutationBuilder(operationName, input), null, operationName);
        byte[] bytesScene = Encoding.ASCII.GetBytes(message.ToJSON());
        WWW wwwSecene = new WWW(_GraphQLUrl, bytesScene, header);

        //wait until everything is saved
        double t = 0;
        while ((!wwwSecene.isDone && !wwwImg.isDone && !wwwWarning1.isDone && !wwwWarning2.isDone && 
            !wwwwMat1.isDone && !wwwwMat2.isDone && !wwwwCm1.isDone && !wwwwCm2.isDone && !wwwwMed.isDone && !wwwwMed2.isDone)) {
            Thread.Sleep(200);
            t++;
            if (t < _MaxWaitTime)
                break;
        };
        return String.IsNullOrEmpty(wwwSecene.error) && String.IsNullOrEmpty(wwwImg.error) && String.IsNullOrEmpty(wwwWarning1.error) && 
            String.IsNullOrEmpty(wwwWarning2.error) && String.IsNullOrEmpty(wwwwMat1.error) && String.IsNullOrEmpty(wwwwMat2.error) && 
            String.IsNullOrEmpty(wwwwCm1.error) && String.IsNullOrEmpty(wwwwCm2.error) && String.IsNullOrEmpty(wwwwMed.error) && String.IsNullOrEmpty(wwwwMed2.error);
    }

    private string CreateInputList(List<string> datalist) {
        string inputList = "[ ";
        foreach (string s in datalist) {
            inputList += "\"" + s + "\"" + ",";
        }
        inputList.TrimEnd(',');
        inputList += " ]";
        return inputList;
    }

    private string MutationBuilder(string operationName, string input) {
        return "mutation " + operationName + "{" + operationName + "(" + input.Trim(new Char[] { '{', '}' }) + "){id}}";
    }

    [UnityTest]
    public IEnumerator Test_00_Connection() {
        bool s = Setup();
        Assert.True(s, "Connection error, saving Data not posible");
        yield return new WaitForFixedUpdate();
    }

    [UnityTest]
    public IEnumerator Test_01_SetSOP() {
        Setup();
        _Model = ApplicationModel.Instance;
        _ConnectionHandler.SetSop(_TestObject.id);
        double t = 0;
        while (_Model.ApplicationState != ApplicationState.DESCRIPTION && t < _MaxWaitTime) {
            yield return new WaitForFixedUpdate();
            Thread.Sleep(200);
            t++; 

        };
        yield return new WaitForFixedUpdate();

        Assert.True(_Model.SopModel.name == _TestObject.name);
        Assert.True(_Model.SopModel.description == _TestObject.description);
        Assert.True(_Model.SopModel.version == _TestObject.version);
        Assert.True(_Model.SopModel.id == _TestObject.id);
        Assert.True(_Model.SopModel.number == _TestObject.number);
        yield return new WaitForFixedUpdate();

    }

    [UnityTest]
    public IEnumerator Test_02_SetNewStep() {
        Setup();
        _Model = ApplicationModel.Instance;
        double t = 0;
        _Model.StepModel.stepNumber = 0; 
        _Model.Steps = new Dictionary<int, string> {
            { 1, _TestObject.id},
        } ;

        _ConnectionHandler.SetNewStep(true);
        while (_Model.StepModel.description.Equals("Loading") && t <_MaxWaitTime) {
            yield return new WaitForFixedUpdate(); 
            Thread.Sleep(200);
            t++;
        };

        Assert.True(_Model.StepModel.name == _TestObject.name);
        Assert.True(_Model.StepModel.description == _TestObject.description);
        Assert.True(_Model.StepModel.sceneId == _TestObject.sceneId);
        Assert.True(_Model.StepModel.id == _TestObject.id);
        Assert.True(_Model.StepModel.stepNumber == _TestObject.stepNumber);

        if (_Model.StepModel.stepMaterials.Count == _TestObject.stepMaterials.Count) {
            Test_0X_Material(_Model.StepModel.stepMaterials);
        } else {
            Assert.False(true, "Wrong size of the step mateiral list");
        }

        if (_Model.StepModel.warningList.Count == _TestObject.warningIds.Count) {
            foreach(WarningModel warning in  _Model.StepModel.warningList) {
                foreach(ArContentObjectModel.Warning testWarning in _Warnings) {
                    if(warning.id.Equals(testWarning.id)) {
                        Assert.True(warning.imageMetaData.id == testWarning.pictureId);
                        Assert.True(warning.imageMetaData.imageName == _ImageModel.imageName);
                        Assert.True(warning.imageMetaData.description == _ImageModel.description);
                        Assert.True(warning.name == testWarning.name);
                        Assert.True(warning.warningType == testWarning.warningType.ToString());
                    }
                }
            }
        } else {
            Assert.False(true, "Wrong size of the warning list");
        }

        if(_Model.StepModel.imageMetaDataList.Count < 2) {
            Assert.True(_Model.StepModel.imageMetaDataList[0].imageName == _ImageModel.imageName, "Image name is wrong");
            Assert.True(_Model.StepModel.imageMetaDataList[0].description == _ImageModel.description, "Wrong image description");
            Assert.True(_Model.StepModel.imageMetaDataList[0].id == _ImageModel.id);
        } else {
            Assert.False(true, "ImageMetaData has to be 1");
        }

        yield return new WaitForFixedUpdate();
    }

    [UnityTest]
    public IEnumerator Test_03_LoadNewSteps() {
        Setup();
        _Model = ApplicationModel.Instance;
        _Model.SopModel.id = _TestObject.sceneId;
        _Model.Steps = null;
        _ConnectionHandler.LoadNewSteps(null, null);
        double t = 0;
        while (_Model.ApplicationState != ApplicationState.MATERIALCHECK) {
            Thread.Sleep(200);
            yield return new WaitForFixedUpdate();
            t++;
            if(t > _MaxWaitTime) {
                break;
            }
        }
        if (t < _MaxWaitTime) {
            if (_Model.Steps.Count == 1) {
                Assert.True(_Model.Steps[_TestObject.stepNumber] == _TestObject.id);
            } else {
                Assert.True(false, "Worng number of steps loaded");
            }

            if (_Model.MaterialList.Count == _TestObject.stepMaterials.Count) {
                Test_0X_Material(_Model.MaterialList);
            } else {
                Assert.False(true, "Wrong size of the step mateiral list");
            }
        } else {
            Assert.True(false, "Connection Error");
        }

        yield return new WaitForFixedUpdate();
    }

    [UnityTest]
    public IEnumerator Test_04_SetNewMolecule() {
        Setup();
        _Model = ApplicationModel.Instance;
        GameObject gameObject = new GameObject();
        _ConnectionHandler.SetNewMolecule(_TestObject.casNumber, gameObject);

        double t = 0;
        while (gameObject.transform.childCount < 1 && t < _MaxWaitTime) {
            Thread.Sleep(200);
            yield return new WaitForFixedUpdate();
            t++;
        }

        if(t < _MaxWaitTime) {
            Assert.True(gameObject.transform.childCount == 9, "Wrong Atomnumber in gameobject");
        }else {
            Assert.True(false, "Connection Error");
        }


        yield return new WaitForFixedUpdate();
    }

    [UnityTest]
    public IEnumerator Test_05_SetNewImage() {
        Setup();
        _Model = ApplicationModel.Instance;
        GameObject imageFiled = GameObject.CreatePrimitive(PrimitiveType.Plane);
        imageFiled.AddComponent<MeshCollider>();
        imageFiled.gameObject.GetComponent<Renderer>().material.mainTexture = null;
        Action<Texture2D> changePicture = new Action<Texture2D>((texture) => {
            imageFiled.gameObject.GetComponent<Renderer>().material.mainTexture = texture;
        });

        _ConnectionHandler.SetNewImage(_ImageModel.imageName, changePicture);
        double t = 0;
        while (imageFiled.gameObject.GetComponent<Renderer>().material.mainTexture == null && t < _MaxWaitTime) {
            Thread.Sleep(200);
            yield return new WaitForFixedUpdate();
            t++;
        }

        if (t < _MaxWaitTime) {
            Assert.True(imageFiled.gameObject.GetComponent<Renderer>().material.mainTexture != null, "GameObject has no texture");
        } else {
            Assert.True(false, "Conection error");
        }

        yield return new WaitForFixedUpdate();
    }

    [UnityTest]
    public IEnumerator Test_06_SetNewCycle() {
        Setup();
        _Model = ApplicationModel.Instance;
        _ConnectionHandler.SetNewCycle(_TestObject.id);
        double t = 0;
        while (_Model.CycleModel == null) {
            Thread.Sleep(200);
            yield return new WaitForFixedUpdate();
            t++;
            if (t > _MaxWaitTime) {
                break;
            }
        }

        if (t < _MaxWaitTime) {
            Assert.True(_Model.CycleModel.id == _TestObject.id);
            Assert.True(_Model.CycleModel.name == _TestObject.name);
            Assert.True(_Model.CycleModel.description == _TestObject.description);

            if(_Model.CycleModel.cycleComponentLinks.Count == _TestObject.cycleComponentLinks.Count) {
                foreach(CycleModel.LinkedComponent links in _Model.CycleModel.cycleComponentLinks) {
                    foreach(CycleModel.LinkedComponent testLinks in _TestObject.cycleComponentLinks) {
                        if(links.originId == testLinks.originId) {
                            Assert.True(links.destinationId == testLinks.destinationId);
                        } else {
                            Assert.True(links.destinationId != testLinks.destinationId);
                        }
                    }
                }
            } else {
                Assert.True(false, "Wrong number of Cycle componnets");
            }
        } else {
            Assert.True(false, "Connection Error");
        }

        yield return new WaitForFixedUpdate();
    }

    [UnityTest]
    public IEnumerator Test_07_LoadComponent() {
        Setup();
        _ConnectionHandler.SetNewCycle(_TestObject.id);

        _Model.CycleModelChanged += (s, e) => {
            Dictionary<string, CycleComponentModel> component = new Dictionary<string, CycleComponentModel>();
            foreach (CycleModel.LinkedComponent componentModel in _Model.CycleModel.cycleComponentLinks) {
                if (!component.ContainsKey(componentModel.originId)) {
                    component.Add(componentModel.originId, null);
                }
                if (!component.ContainsKey(componentModel.destinationId)) {
                    component.Add(componentModel.destinationId, null);
                }
            }
            _Model.CycleModelComponents = component;
            foreach (string key in _Model.CycleModelComponents.Keys) {
                _ConnectionHandler.LoadComponent(key);
            }
        };

        double t = 0;
        while (_Model.ApplicationState != ApplicationState.GAME) {
            Thread.Sleep(200);
            yield return new WaitForFixedUpdate();
            t++;
            if (t > _MaxWaitTime) {
                break;
            }
        }
        if (t < _MaxWaitTime) {
            if(_Model.CycleModelComponents.Count == _CycleComponents.Count) {
               foreach(CycleComponentModel component in _CycleComponents) {
                    Assert.True(_Model.CycleModelComponents.ContainsKey(component.id), "Missing component");
                    Assert.True(_Model.CycleModelComponents[component.id].id == component.id);
                    Assert.True(_Model.CycleModelComponents[component.id].name == component.name);
                    Assert.True(_Model.CycleModelComponents[component.id].cas == component.cas);
                    Assert.True(_Model.CycleModelComponents[component.id].type == component.type);
                    Assert.True(_Model.CycleModelComponents[component.id].description == component.description);

                }

            } else {
                Assert.True(_Model.CycleModelComponents.Count == _CycleComponents.Count, "number of cyclecomponents are wrong");
            }
   
        } else {
            Assert.True(false, "Connection Error");
        }
        yield return new WaitForFixedUpdate();
    }

    [UnityTest]
    public IEnumerator Test_08_SetNewGLTF() {
        Setup();
        _Model = ApplicationModel.Instance;
        GameObject gltfObject = new GameObject();
        _ConnectionHandler.SetNewGLTF(_ImplantParts[0].gltfName, gltfObject);
        double t = 0;
        while (gltfObject.transform.childCount < 1 && t < _MaxWaitTime) {
            Thread.Sleep(200);
            yield return new WaitForFixedUpdate();
            t++;
        }

        if (t < _MaxWaitTime) {
            Assert.True(gltfObject.transform.childCount == 1, "Error by creating the gltf object");
        } else {
            Assert.True(false, "Conection error");
        }
        yield return new WaitForFixedUpdate();
    }

    [UnityTest]
    public IEnumerator Test_09_SetNewMedicalData(){ 
        Setup();
        _Model = ApplicationModel.Instance;
        _ConnectionHandler.SetNewMedicalData(_TestObject.id);

        double t = 0;
        while (_Model.ApplicationState != ApplicationState.IMPLANTVIEW) {
            Thread.Sleep(200);
            yield return new WaitForFixedUpdate();
            t++;
            if (t > _MaxWaitTime) {
                break;
            }
        }
        
        if (t < _MaxWaitTime) {
            Assert.True(_Model.MedicalModel.id == _TestObject.id);
            Assert.True(_Model.MedicalModel.name == _TestObject.name);
            Assert.True(_Model.MedicalModel.operation == _TestObject.operation);
            Assert.True(_Model.MedicalModel.manufacturer == _TestObject.manufacturer);
            Assert.True(_Model.MedicalModel.price == _TestObject.price);
            Assert.True(_Model.MedicalModel.material == _TestObject.material);
            Assert.True(_Model.MedicalModel.similarImplant == _TestObject.similarImplant);

            if (_Model.MedicalModel.implantParts.Count == _TestObject.implantPartIds.Count) {
                foreach (ImplantPartModel implant in _Model.MedicalModel.implantParts) {
                    foreach (ImplantPartModel testImplant in _ImplantParts) {
                        if (implant.id == testImplant.id) {
                            Assert.True(implant.gltfName == testImplant.gltfName);
                            Assert.True(implant.manufacturer == testImplant.manufacturer);
                            Assert.True(implant.material == testImplant.material);
                            Assert.True(implant.price == testImplant.price);
                            Assert.True(implant.operation == testImplant.operation);
                            Assert.True(implant.name == testImplant.name);
                            Assert.True(implant.description == testImplant.description);
                            Assert.True(implant.similarImplant == testImplant.similarImplant);
                        }
                    }
                }
            } else {
                Assert.True(false, "Wrong number of implant loaded");
            }

            if (_Model.MedicalModel.anatomyParts.Count == _TestObject.anatomyPartIds.Count) {
                foreach (BodyPartModel anatomy in _Model.MedicalModel.anatomyParts) {
                    foreach (ImplantPartModel testImplant in _ImplantParts) {
                        if (anatomy.id == testImplant.id) {
                            Assert.True(anatomy.description == testImplant.description);
                            Assert.True(anatomy.name == testImplant.name);
                            Assert.True(anatomy.gltfName == testImplant.gltfName);
                        }
                    }
                }
            } else {
                Assert.True(false, "Wrong number of anatomy loaded");
            }
        } else {
            Assert.True(false, "Connection error");
        }
        yield return new WaitForFixedUpdate();
    }

    private void Test_0X_Material(List<MaterialModel> matieralL) {
        foreach (MaterialModel material in matieralL) {
            foreach (ArContentObjectModel.StepMaterial testMaterial in _TestObject.stepMaterials) {
                if (material.sopMaterial.id == testMaterial.materialId) {
                    Assert.True(material.amount == testMaterial.amount, "Wrong amount of material");
                    Assert.True(material.unit == testMaterial.unit, "Wrong unit");
                    foreach (ArContentObjectModel.SopMaterial sopMaterial in _SopMaterials) {
                        if (material.sopMaterial.id == sopMaterial.id) {
                            Assert.True(material.sopMaterial.name == sopMaterial.name, "Wrong unit");
                            Assert.True(material.sopMaterial.imageMetaData.id == sopMaterial.pictureId, "Wrong image id");
                            Assert.True(material.sopMaterial.placeId == sopMaterial.placeId, "Wrong place");
                            Assert.True(material.sopMaterial.imageMetaData.imageName == _ImageModel.imageName, "Image name is wrong");
                            Assert.True(material.sopMaterial.imageMetaData.description == _ImageModel.description, "Wrong image description");

                            if (material.sopMaterial.warning.Count == sopMaterial.warningIds.Count) {
                                foreach (WarningModel warning in material.sopMaterial.warning) {
                                    foreach (ArContentObjectModel.Warning testWarning in _Warnings) {
                                        if (warning.id == testWarning.id) {
                                            Assert.True(warning.imageMetaData.id == testWarning.pictureId);
                                            Assert.True(warning.imageMetaData.imageName == _ImageModel.imageName);
                                            Assert.True(warning.imageMetaData.description == _ImageModel.description);
                                            Assert.True(warning.name == testWarning.name);
                                            Assert.True(warning.warningType == testWarning.warningType.ToString());
                                        }
                                    }
                                }
                            } else {
                                Assert.True(false, "Wrong number of warnings");
                            }
                        }
                    }
                }
            }
        }
    }
#endif
}