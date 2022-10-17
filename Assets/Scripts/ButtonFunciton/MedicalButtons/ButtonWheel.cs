using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for handling the buttons on the wheel (detail information about the implant)
/// <summary>
public class ButtonWheel : MonoBehaviour, IInputClickHandler {
    private Color _OriginalColor;
    private Color _HoverColor;
    private bool _IsPressed;
    private Color _PressedColor;
    private Color _PressedHoverColor;
    public GameObject InfoPanelPrefab;
    private GameObject _InfoPanel;
    public char MenuInfo;

    private void Start() {
        float r = 24f / 255f;
        float g = 59f / 255f;
        float b = 240f / 255f;
        _OriginalColor = new Color(r, g, b, 1f);
        r = 121f / 255f;
        g = 142f / 255f;
        b = 246f / 255f;
        _HoverColor = new Color(r, g, b, 1f);
        r = 221f / 255f;
        g = 227f / 255f;
        b = 253f / 255f;
        _PressedColor = new Color(r, g, b, 1f);
        r = 183f / 255f;
        g = 195f / 255f;
        b = 251f / 255f;
        _PressedHoverColor = new Color(r, g, b, 1f);
        _IsPressed = false;

        gameObject.GetComponent<Renderer>().material.color = _OriginalColor;
    }

    void OnMouseOver() {
        if (!_IsPressed) {
            gameObject.GetComponent<Renderer>().material.color = _HoverColor;
        } else {
            gameObject.GetComponent<Renderer>().material.color = _PressedHoverColor;
        }
    }

    void OnMouseExit() {
        if (!_IsPressed) {
            gameObject.GetComponent<Renderer>().material.color = _OriginalColor;
        } else {
            gameObject.GetComponent<Renderer>().material.color = _PressedColor;
        }
    }

    /// <summary>
    /// Button Function
    /// Checks which button this script is asing and execute one specific part
    /// <summary>
    public void OnInputClicked(InputClickedEventData eventData) {
        if (_IsPressed) {

            Destroy(_InfoPanel);
            gameObject.GetComponent<Renderer>().material.color = _HoverColor;
            _IsPressed = false;
        } else {
            _InfoPanel = Instantiate(InfoPanelPrefab);
            _InfoPanel.transform.parent = gameObject.transform.parent;
            _InfoPanel.transform.rotation = gameObject.transform.parent.rotation;
            float x, y, z;
            ImplantPartModel implantPart = gameObject.transform.parent.parent.GetComponent<ImplantButton>().GetImplantPart();

            switch (MenuInfo) {
                case 'P':
                    x = 0f;
                    y = 0.407f;
                    z = 0;
                    _InfoPanel.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = implantPart.price;
                    _InfoPanel.transform.Find("Canvas").transform.Find("Title").GetComponent<Text>().text = "Preis";

                    break;
                case 'M':
                    x = 0.411f;
                    y = 0.139f;
                    z = 0;
                    _InfoPanel.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = implantPart.material;
                    _InfoPanel.transform.Find("Canvas").transform.Find("Title").GetComponent<Text>().text = "Material";
                    break;
                case 'O':
                    x = 0.379f;
                    y = -0.18f;
                    z = 0;
                    _InfoPanel.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = implantPart.operation;
                    _InfoPanel.transform.Find("Canvas").transform.Find("Title").GetComponent<Text>().text = "Operation";
                    break;
                case 'H':
                    x = -0.379f;
                    y = -0.18f;
                    z = 0;
                    _InfoPanel.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = implantPart.manufacturer;
                    _InfoPanel.transform.Find("Canvas").transform.Find("Title").GetComponent<Text>().text = "Hersteller";
                    break;
                case 'S':
                    x = -.411f;
                    y = 0.139f;
                    z = 0;
                    string message = implantPart.similarImplant.Replace(";", ";\n- ");
                    message = message.Insert(0, "- ");
                    _InfoPanel.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = message;
                    _InfoPanel.transform.Find("Canvas").transform.Find("Title").GetComponent<Text>().text = "Vergleichbare Implantate";
                    break;
                default:
                    x = 0.0f;
                    y = 0.0f;
                    z = 0;
                    _InfoPanel.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = " UNKNOWN ERORR ! ! !";
                    _InfoPanel.transform.Find("Canvas").transform.Find("Title").GetComponent<Text>().text = "ERROR";
                    break;
            }
            _InfoPanel.transform.localPosition = new Vector3(x, y, z);
            gameObject.GetComponent<Renderer>().material.color = _PressedColor;
            _IsPressed = true;
        }
    }
}