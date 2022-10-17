using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using ZXing;

namespace PosterAlignment
{
    /// <summary>
    /// Finds QR codes in the world using the PV Camera and reports them.
    /// 
    /// </summary>
    public class ImageQRScanner : MonoBehaviour
    {
        [Tooltip("An optional object on which to render the camera image (useful for debugging or just showing the user feedback).")]
        public MeshRenderer OptionalShowCamTextureOn = null;
        [Tooltip("The maximum width of the poster mip to be used if mipmaps are anabled.  Larger posters/mips can slow down processing.")]
        public int MaxPosterMipWidth = 512;
        [HideInInspector]

        public int UpdateVersion = 0;

        private volatile bool isProcessingImage;

        private PosterLocationHandler qrCodeLocationHandler;

        private Texture2D previewTexture = null;
        private Color32[] imageTextureData;
        private Texture2D posterTexture;
        private Color32[] posterTextureData;
        private int posterTextureDataWidth = 0;
        private int posterTextureDataHeight = 0;
        public Result qrResult = null;


        private static byte[] cameraImageBuffer = null; // color webcam data buffer
        private static List<byte> cameraImageListBuffer = null; // color webcam data buffer as list
        private static GCHandle cameraImageGCHandle;
        private static IntPtr cameraImagePtrToData;

        public void StartProcessing()
        {
            Debug.Log("ImageQrScanner:StartProcessing: " + gameObject.name);
            qrCodeLocationHandler = null;
            PVCamManager.Instance.RegisterForFrames(this.OnCaptureCompleted);
        }

        void OnDisable()
        {
            StopProcessing();
        }

        public void StopProcessing()
        {
            Debug.Log("ImageQRScanner:StopProcessing: " + gameObject.name);
            if (PVCamManager.Instance != null)
            {
                PVCamManager.Instance.UnregisterForFrames(this.OnCaptureCompleted);
            }
            this.isProcessingImage = false;
        }

        public void SetPosterTexture(Texture2D posterTexture)
        {
            this.posterTexture = posterTexture;     

            int whichMip = 0;
            Debug.Log("ImagePosterLocator:SetPosterTexture Mips:" + this.posterTexture.mipmapCount);

            for (int i = 0; i < this.posterTexture.mipmapCount; i++)
            {
                whichMip = i;
                var mipWidth = Math.Max(1, this.posterTexture.width >> i);

                if (mipWidth <= MaxPosterMipWidth)
                    break;
            }

            posterTextureData = this.posterTexture.GetPixels32(whichMip);
            posterTextureDataWidth = this.posterTexture.width >> whichMip;
            posterTextureDataHeight = this.posterTexture.height >> whichMip;

            Debug.Log("ImagePosterLocator:SetPosterTexture - using mip #" + whichMip + ":" + posterTextureDataWidth + "x" + posterTextureDataHeight);
        }

       

        private void DoOnSeperateThread(Action act)
        {
#if UNITY_WSA && !UNITY_EDITOR
            System.Threading.Tasks.Task.Run(act);
#else
            var thrd = new System.Threading.Thread(() => act());
            thrd.Start();
#endif
        }

        private void UpdatePreviewTexture(byte[] buffer)
        {
            if ((OptionalShowCamTextureOn != null && OptionalShowCamTextureOn.gameObject.activeSelf))
            {
                if (previewTexture == null)
                {
                    var res = PVCamManager.Instance.PhotoCapCamResolution;
                    previewTexture = new Texture2D(res.width, res.height, TextureFormat.BGRA32, false);
                }

                if (this.OptionalShowCamTextureOn != null && OptionalShowCamTextureOn.material.mainTexture != previewTexture)
                {
                    OptionalShowCamTextureOn.material.mainTexture = previewTexture;
                }

                if (cameraImageBuffer != null && cameraImageBuffer.Length > 0)
                {
                    previewTexture.LoadRawTextureData(cameraImagePtrToData, cameraImageBuffer.Length);
                    previewTexture.Apply();
                    imageTextureData = previewTexture.GetPixels32();
                }
            }
        }

        private static Matrix4x4 TuneProjectionMatrix(UnityEngine.XR.WSA.WebCam.PhotoCaptureFrame frame)
        {
            Matrix4x4 camProj;
            if (!frame.TryGetProjectionMatrix(0.0f, 1.0f, out camProj))
            {
#if UNITY_EDITOR
                camProj = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, -1, 0), new Vector4(0, 0, 0, 1));
                //camProj = Matrix4x4.identity;
#else
                Debug.LogError("Unable to get the camera projection for this frame!");
#endif
            }
            return camProj;
        }

        private bool OnCaptureCompleted(UnityEngine.XR.WSA.WebCam.PhotoCaptureFrame frame)
        {
            if (this.isProcessingImage)
            {
                return true;
            }

            this.isProcessingImage = true;

            Matrix4x4 cameraToWorldMatrix;
            frame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix);
            cameraToWorldMatrix = cameraToWorldMatrix * Matrix4x4.Scale(new Vector3(1, 1, -1));

            // Update the preview image with the last image processed
            UpdatePreviewTexture(cameraImageBuffer);
            var res = PVCamManager.Instance.PhotoCapCamResolution;
            var w = res.width;
            var h = res.height;


            if (qrCodeLocationHandler == null)
            {
                qrCodeLocationHandler = new PosterLocationHandler(w, h);
            }

            Matrix4x4 cameraClipToWorld;
            Vector3 cameraPos;
            var camProj = TuneProjectionMatrix(frame);
            GetCameraClipTransformAndWorldPosition(cameraToWorldMatrix, camProj,
                out cameraClipToWorld, out cameraPos);

            qrCodeLocationHandler.UpdateCamera(cameraToWorldMatrix, camProj, cameraPos);

            DoOnSeperateThread(() =>
            {
                // Allocate the buffers to hold the image data
                if ((cameraImageListBuffer == null) || (cameraImageListBuffer.Count != frame.dataLength))
                {
                    cameraImageListBuffer = new List<byte>(frame.dataLength);
                }
                if ((cameraImageBuffer == null) || (cameraImageBuffer.Length != frame.dataLength))
                {
                    cameraImageBuffer = new byte[frame.dataLength];
                    cameraImageGCHandle = GCHandle.Alloc(cameraImageBuffer, GCHandleType.Pinned);
                    cameraImagePtrToData = cameraImageGCHandle.AddrOfPinnedObject();
                }

                frame.CopyRawImageDataIntoBuffer(cameraImageListBuffer);
                // Copy the image data from the list into the byte array
                cameraImageListBuffer.CopyTo(cameraImageBuffer);

                PVCamManager.Instance.SignalFinishedWithFrame();

                // Find the qr code:
                this.FindQRCode(w, h);

                this.isProcessingImage = false;
            });

            return false;
        }

        private void FindQRCode(int pixelWidth, int pixelHeight)
        {
            bool foundQRCode = false;
            Result result = null; 

            try
            {       
                    IBarcodeReader barcodeReader = new BarcodeReader();

                // decode the current frame   
                result = barcodeReader.Decode(imageTextureData, pixelWidth, pixelHeight);
                    if (result != null)
                    {
                        Debug.Log("DECODED TEXT FROM QR:" +result.Text);
                    qrResult = result;
                    foundQRCode = true;
                    }

                }
               
            catch (Exception ex)
            {
                Debug.Log("Exception in QRCode reading");
            }
                         
              if (result!= null)
            {
                ResultPoint[] pixelPositions;
                pixelPositions = result.ResultPoints;
                if (pixelPositions != null)
                {
                    for (int i = 0; i < pixelPositions.Length; i++)
                    {
                        var pixelLocation = new PosterCornerLocation();
                        pixelLocation.Id = (PosterCornerLocation.CornerLocationId)i;
                        pixelLocation.PixelX = pixelPositions[i].X;
                        pixelLocation.PixelY = pixelPositions[i].Y;

                        try
                        {
                            qrCodeLocationHandler.OnCornerLocated(pixelLocation);
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("Exception while locating corners");
                        }
                    }
                }
            }

            if (foundQRCode)
            {
                this.UpdateVersion++;
            }

        }

        /// <summary>
        /// UnProject pixel space vector into directional vector
        /// </summary>
        public static Vector3 UnProjectVector(Matrix4x4 proj, Vector3 to)
        {
            Vector3 from = new Vector3(0, 0, 0);

            var axsX = proj.GetRow(0);
            var axsY = proj.GetRow(1);
            var axsZ = proj.GetRow(2);

            from.z = to.z / axsZ.z;
            from.y = (to.y - (from.z * axsY.z)) / axsY.y;
            from.x = (to.x - (from.z * axsX.z)) / axsX.x;

            return from;
        }

        public static void GetCameraClipTransformAndWorldPosition(Matrix4x4 cameraToWorld, Matrix4x4 cameraProjectionMatrix, out Matrix4x4 cameraClipToWorld, out Vector3 cameraPos)
        {
            cameraPos = cameraToWorld.MultiplyPoint(Vector3.zero);

            Matrix4x4 camProj = cameraProjectionMatrix;

            cameraClipToWorld = cameraToWorld              // PVCamera to 'face' tranform
                                * camProj.inverse;
        }

        public static float GZFlipScale = -1.0f;

        public static Ray RayFromCameraSetup(Vector2 uv, Matrix4x4 camera2World, Matrix4x4 cameraProj, Vector3 cameraPos)
        {
            // Turn the uvPoint on the PVCamera into a position on the near clip plane on our proper camera.
            var uvPointInNegOneToOneSpace = uv * 2 - Vector2.one;

            var examplePoint = new Vector3(uvPointInNegOneToOneSpace.x, -uvPointInNegOneToOneSpace.y, GZFlipScale);

            // Flip X and Y before doing unproject:
            examplePoint = new Vector3(-examplePoint.x, -examplePoint.y, examplePoint.z);

            var posAroundCamera = UnProjectVector(cameraProj, examplePoint);

            // Flip X and Y again after doing unproject:
            posAroundCamera = new Vector3(-posAroundCamera.x, -posAroundCamera.y, posAroundCamera.z);

            var posInWorld = camera2World.MultiplyPoint(posAroundCamera);

            var dirInWorld = (posInWorld - cameraPos).normalized;

            return new Ray(cameraPos, dirInWorld);
        }

        public WorldPosFromRays[] DetectedPositions
        {
            get
            {
                 if (qrCodeLocationHandler == null)
                return null;
                 return qrCodeLocationHandler.DetectedPositions;
            }
        }

        public class PosterCornerLocation
        {
            public enum CornerLocationId
            {
                UpperLeft = 0,
                UpperRight = 1,
                LowerRight = 2,
                LowerLeft = 3
            }
            public CornerLocationId Id;
            public float PixelX;
            public float PixelY;
        }

        private class PosterLocationHandler
        {
            private readonly WorldPosFromRays[] locationCorners;
            private Matrix4x4 cameraToWorld;
            private Matrix4x4 cameraProj;
            private Vector3 cameraPos;
            private readonly int imagePixelWidth;
            private readonly int imagePixelHeight;

            public PosterLocationHandler(
                int imagePixelWidth, int imagePixelHeight)
            {
                this.locationCorners = new WorldPosFromRays[4];
                this.imagePixelWidth = imagePixelWidth;
                this.imagePixelHeight = imagePixelHeight;
            }

            public void UpdateCamera(
                Matrix4x4 cam2World, Matrix4x4 camProj, Vector3 cameraPos)
            {
                this.cameraToWorld = cam2World;
                this.cameraProj = camProj;
                this.cameraPos = cameraPos;
            }

            public WorldPosFromRays[] DetectedPositions
            {
                get
                {
                    return locationCorners;
                }
            }

            public void OnCornerLocated(PosterCornerLocation location)
            {
                Vector2 locationUV = GetLocationUV(location);
                Ray rayThroughUV = GetLocationRay(locationUV);

                if ((rayThroughUV.origin == Vector3.zero)
                    || (rayThroughUV.direction == Vector3.zero))
                {
                    // wierd position, ignore it
                  //  return;
                }

                Debug.Log("Corner Located! Id=" + location.Id + "\nX=" + location.PixelX + " Y=" + location.PixelY);

                if (locationCorners[(int)location.Id] == null)
                {
                    locationCorners[(int)location.Id] = new WorldPosFromRays((int)location.Id);
                }

                locationCorners[(int)location.Id].AddRay(rayThroughUV, this.cameraPos, this.cameraProj);
            }

            private Vector2 GetLocationUV(PosterCornerLocation location)
            {
                return new Vector2(location.PixelX / (this.imagePixelWidth - 1), location.PixelY / (this.imagePixelHeight - 1));
            }

            private Ray GetLocationRay(Vector2 uv)
            {
                return RayFromCameraSetup(uv, this.cameraToWorld, this.cameraProj, this.cameraPos);
            }
        }
    }
}
