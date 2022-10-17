using HoloToolkit.Unity.InputModule;
using UnityEngine;


public class AirTab : MonoBehaviour, IInputClickHandler
{
    public string url;

    #region IInputClickHandler
    public void OnInputClicked(InputClickedEventData eventData)
    {
        Application.OpenURL(url);
    }
    #endregion IInputClickHandler
}