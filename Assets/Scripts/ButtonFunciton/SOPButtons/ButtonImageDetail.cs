using HoloToolkit.Unity.InputModule;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for loading and showing an image
/// <summary>
public class ButtonImageDetail : MonoBehaviour, IInputClickHandler {

    public GameObject ImageDetailPrefab;
    private float _SpaceBetween = 1f;
    private List<ImageModel> _Image;
    public List<ImageModel> ImageModel {
        get { return _Image; }
        set {
            if (_Image != value) {
                _Image = value;
            }
        }
    }

    /// <summary>
    /// Button Function
    /// Create a new Gameobject and asing the image and metadata to it
    /// <summary>
    public void OnInputClicked(InputClickedEventData eventData) {
        GameObject parent = gameObject.transform.parent.gameObject;
        ConnectionHandler conectionHandler = GameObject.Find("Controller").GetComponent<ConnectionHandler>();
        int shownImageDetails = 0;

        foreach (ImageModel imageModel in _Image) {
            float xShift = 0 + (shownImageDetails * _SpaceBetween);
            float yShift = 0;
            float zShift = 0f;

            GameObject panel = Instantiate(ImageDetailPrefab);
            panel.transform.parent = parent.transform;
            panel.transform.rotation = parent.transform.rotation;
            panel.transform.localPosition = new Vector3(xShift, yShift, zShift);

            Transform innerPanel = panel.transform.Find("ImageDetails");
            innerPanel.transform.Find("Title").GetComponent<Text>().text = imageModel.imageName;
            innerPanel.transform.Find("Description").GetComponent<Text>().text = imageModel.description;
            Image ImageField = innerPanel.transform.Find("Image").GetComponent<Image>(); 
            Action <Texture2D> changePicture = new Action<Texture2D>((texture) => {
                int targetWidth = 512;
                int targetHeight = 512;
                RenderTexture renderTexture = RenderTexture.GetTemporary(targetWidth, targetHeight, 0, RenderTextureFormat.ARGB32);
                Graphics.Blit(texture, renderTexture);
                texture.Resize(targetWidth, targetHeight);
                RenderTexture active = RenderTexture.active;
                RenderTexture.active = renderTexture;
                texture.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
                texture.Apply(false, true);
                RenderTexture.active = active;
                RenderTexture.ReleaseTemporary(renderTexture);
                
                Sprite sprite = Sprite.Create(texture, ImageField.sprite.rect, ImageField.sprite.pivot);
                ImageField.sprite = sprite;
            });

            conectionHandler.SetNewImage(imageModel.imageName, changePicture);
            shownImageDetails++;
        }
    }
}
