
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.UI;

public class AirTabHandler : MonoBehaviour, IInputClickHandler
{
    public GameObject myPrefab;

    public string textTitle;
    public string textInfo;

    private string textTitleReset = "ready to analyze";
    private string textInfoReset = "\r\nWelcome to the MoleculeViewer\r\n \r\n \r\n Scan a medication pack to see whats inside";

    public string wikiURL;

    public Sprite spriteImage;
    public Sprite spriteStructure;

    // Methods to set the information to the UI board
    public void SetTitle()
    {
        Text txtTitle = GameObject.Find("GameObject/Cube/Canvas/Title").GetComponent<Text>();
        txtTitle.text = textTitle;
    }

    public void SetInfo()

   
    {
        Text txtInfo = GameObject.Find("GameObject/Cube/Canvas/Info").GetComponent<Text>();
        txtInfo.text = textInfo;
    }


    public void SetImage()

        {
           GameObject image = GameObject.Find("GameObject/Cube/Canvas/Image");
           image.GetComponent<Image>().sprite = spriteImage;
        }
    public void SetStructure()
    {
        GameObject structure;

        if (spriteStructure != null)
        {
            structure = GameObject.Find("GameObject/Cube/Canvas/Structure");
            structure.GetComponent<Image>().sprite = spriteStructure;
        }
        else
        {
            structure = GameObject.Find("GameObject/Cube/Canvas/Structure");
            structure.SetActive(false);
        }
}


    private void SetWikiURL(string wikiURL)
    {
        Text url = GameObject.Find("GameObject/Cube/Canvas/WikiButton/URL").GetComponent<Text>();
        url.text = wikiURL;
    }

    private void SetAllInfo()
    {
        SetTitle();
        SetInfo();
        SetImage();
        SetStructure();
        SetWikiURL(wikiURL);
    }

    // Methods to reset the information to the UI board default appearance
    private void ResetTitle()
    {
        Text txtTitle = GameObject.Find("GameObject/Cube/Canvas/Title").GetComponent<Text>();
        txtTitle.text = textTitleReset;
    }

    private void ResetInfo()
    {
        Text txtInfo = GameObject.Find("GameObject/Cube/Canvas/Info").GetComponent<Text>();
        txtInfo.text = textInfoReset;
    }

    public void ResetImage()
    {
        spriteImage = Resources.Load<Sprite>("Images/chemistry");
        GameObject image = GameObject.Find("Image");
        image.GetComponent<Image>().sprite = spriteImage;
    }
    public void ResetStructure()
    {
        GameObject structure = GameObject.Find("GameObject/Cube/Canvas/Structure");
        structure.SetActive(false);
    }
        public void ResetObjects()
    {
        GameObject bexin = GameObject.Find("Bexin(Clone)");
        GameObject earth = GameObject.Find("Erde(Clone)");
        GameObject motilium = GameObject.Find("Motilium(Clone)");
        GameObject zaldiar = GameObject.Find("Zaldiar(Clone)");

        if (bexin != null)
            bexin.SetActive(false);
        if (earth != null)
            earth.SetActive(false);
        if (motilium != null)
            motilium.SetActive(false);
        if (zaldiar != null)
            zaldiar.SetActive(false);
    }

    public void ResetDefaultUIBoard()
    {
        ResetTitle();
        ResetInfo();
        ResetImage();
        ResetStructure();
        ResetObjects();

        GameObject resetButton = GameObject.Find("GameObject/Cube/Canvas/ResetButton");
        resetButton.SetActive(false);
        GameObject wikiButton = GameObject.Find("GameObject/Cube/Canvas/WikiButton");
        wikiButton.SetActive(false);
        GameObject centerButton = GameObject.Find("GameObject/Cube/Canvas/CenterButton");
        centerButton.SetActive(false);
    }
  
    
    #region IInputClickHandler
    public void OnInputClicked(InputClickedEventData eventData)
    {
        SetAllInfo();      
    }
    #endregion IInputClickHandler
}





