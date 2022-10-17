using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Linq;

namespace PosterAlignment
{ 
[RequireComponent(typeof(ImageQRScanner))]
public class QRScannerController : MonoBehaviour 
    {

        public GameObject Poster = null;

        [Tooltip("Shows rays/spheres/cubes denoting the various positions of alignment.")]
        public bool ShowDebugObjects = false;
        //public Text qrCodeDetectionPanel;
        public ImageQRScanner qrScanner;

        private GameObject[] cornerObjects = new GameObject[4];
        private GameObject[] debugRayObjects = new GameObject[4];
        //public GameObject attachableGameObject;
        private GameObject[] debugPositionObjects = new GameObject[4];

        private int updateVersion = -1;

        private bool isInitialized = false;
        void Initialize()
        {
            if (isInitialized)
                return;

            if (Poster != null &&
                 Poster.GetComponent<Renderer>() != null &&
                 Poster.GetComponent<Renderer>().material.mainTexture != null &&
                 Poster.GetComponent<MeshFilter>() != null &&
                 Poster.GetComponent<MeshFilter>().mesh.vertexCount == 4)
            {
                Texture2D posterTexture = (Texture2D)Poster.GetComponent<Renderer>().material.mainTexture;
                qrScanner.SetPosterTexture(posterTexture);

                Mesh mesh = Poster.GetComponent<MeshFilter>().mesh;
                for (int i = 0; i < 4; i++)
                {
                    Vector3 vertexPosW = Poster.transform.TransformPoint(mesh.vertices[i]);
                    var posterVert = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    posterVert.transform.localScale = new Vector3(.005f, .005f, .005f);
                    posterVert.transform.position = vertexPosW;
                    posterVert.transform.SetParent(Poster.transform);
                    cornerObjects[i] = posterVert;
                }
                isInitialized = true;
            }
            else
            {
                Debug.LogError("Poster not valid - should be a quad with a read/write enabled texture.");
            }
        }

        void OnDisable()
        {
            StopProcessing();
        }

        public void StartProcessing()
        {
            Initialize();
            qrScanner.enabled = true;
            qrScanner.StartProcessing();
            DisplayCorners(true);
        }

        public void StopProcessing()
        {
            foreach (var ob in debugRayObjects)
            {
                GameObject.Destroy(ob);
            }

            foreach (var ob in debugPositionObjects)
            {
                GameObject.Destroy(ob);
            }

            //attachableGameObject.SetActive(false);

            DisplayCorners(false);
            if (qrScanner != null)
            {
                qrScanner.StopProcessing();
                qrScanner.enabled = false;
            } 
        }

        private void DisplayCorners(bool show)
        {
            foreach (var ob in cornerObjects)
            {
                if (ob != null)
                    ob.SetActive(show);
            }
        }

        private void Update()
        {
            if (this.updateVersion != this.qrScanner.UpdateVersion)
            {
                this.updateVersion = this.qrScanner.UpdateVersion;
                Debug.Log("Update version updated.");

                UpdateLocationFromPoster();
                if(qrScanner.qrResult != null)
                {
                    string qrInfo = qrScanner.qrResult.Text.Replace("-", "\"");
                    GameObject.Find("Controller").GetComponent<ApplicationStartController>().StartWithQRCode(qrInfo);

                    qrScanner.StopProcessing();
                    StopProcessing(); //bug window is not going away
                }

                if (ShowDebugObjects)
                {
                    UpdateDebugObjects();
                }
            } 
        }

        private void AlignCorners(GameObject[] objectsToMove, Vector3 newCenter)
        {
            var oldCenter = objectsToMove.Select(k => k.transform.position)
                .Aggregate((a, b) => (a + b)) / objectsToMove.Length;
            var delta = newCenter - oldCenter;
            transform.position += delta;
        }

        public void UpdateLocationFromPoster()
        {
            if (qrScanner.DetectedPositions == null)
                return;

            List<Vector3> corners = new List<Vector3>();
           foreach (WorldPosFromRays worldPos in qrScanner.DetectedPositions)
            {
                if (worldPos != null)
                {
                    corners.Add(worldPos.EstimatedWorldPos);
                } 
            }

            // var corners = qrScanner.DetectedPositions.Select(k => k.EstimatedWorldPos).ToList();
            if (corners.Count > 0)
            {
                Debug.Log("Applying placement...");

                // First recenter the object:
                var toCenter = corners.Aggregate((a, b) => (a + b))
                    / corners.Count;
                AlignCorners(cornerObjects, toCenter);

                // Now for rotation:
                Quaternion avgRot = Quaternion.identity;
                float oneOverI = 1.0f / ((float)corners.Count);
                for (int i = 0; i < corners.Count; i++)
                {
                    var from = cornerObjects[i].transform.position - toCenter;
                    var to = corners[i] - toCenter;

                    from.y = 0.0f;
                    to.y = 0.0f;

                    var rot = Quaternion.FromToRotation(from, to);
                    avgRot = avgRot * Quaternion.Slerp(Quaternion.identity, rot, oneOverI);
                    //avgRot = rot;
                }
                transform.rotation *= avgRot;

                // And do a final re-center:
                AlignCorners(cornerObjects, toCenter);
            }
        }

        private void UpdateDebugObjects()
        {
            if (this.qrScanner == null || this.qrScanner.DetectedPositions == null)
                return;
            for (var i = 0; i < 4; i++)
            {
                var corner = this.qrScanner.DetectedPositions[i];
                if (corner != null && corner.Rays != null)
                {
                    if (debugRayObjects[i] == null)
                    {
                        var rayObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        var sphereCol = rayObj.GetComponent<Collider>();
                        if (sphereCol != null)
                        {
                            rayObj.GetComponent<Collider>().enabled = false;
                        }
                        rayObj.transform.forward = corner.LatestRay.direction;
                        rayObj.transform.localScale = new Vector3(0.001f, 0.001f, 3.0f);
                        debugRayObjects[i] = rayObj;
                    }

                    debugRayObjects[i].transform.position = corner.LatestRay.origin + (corner.LatestRay.direction * 1.5f);
                    debugRayObjects[i].transform.forward = corner.LatestRay.direction;

                    //attachableGameObject.SetActive(true);
                    //attachableGameObject.transform.position = corner.LatestRay.origin;


                    if (corner.HasWorldPos)
                    {
                        if (debugPositionObjects[i] == null)
                        {
                            var posObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            posObj.transform.localScale = Vector3.one * 0.1f;

                            var posObjCol = posObj.GetComponent<Collider>();
                            if (posObjCol != null)
                            {
                                //sphereCol.enabled = false;
                                posObjCol.bounds.Expand(.1f);
                            }

                            debugPositionObjects[i] = posObj;
                        }
                        debugPositionObjects[i].transform.position = corner.EstimatedWorldPos;
                    }
                    else
                    {
                        // no world location, make sure there isn't an object for it:
                        if (debugPositionObjects[i] != null)
                        {
                            GameObject.Destroy(debugPositionObjects[i]);
                            debugPositionObjects[i] = null;
                        }
                    }
                }
            } 
        }

    }
}