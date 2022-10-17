using UnityEngine;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour
{
    Sprite sprite;
    // Use this for initialization
    void Start()
    {
        sprite = Resources.Load<Sprite>("Images/bexin_Image");

        GameObject image = GameObject.Find("Image");
        image.GetComponent<Image>().sprite = sprite;
    }
}