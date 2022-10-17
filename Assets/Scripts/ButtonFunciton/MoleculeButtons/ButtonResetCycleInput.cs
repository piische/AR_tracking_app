using HoloToolkit.Unity.InputModule;
using UnityEngine;

/// <summary>
/// Resett the hole cycle to the origin state
/// <summary>
public class ButtonResetCycleInput : MonoBehaviour, IInputClickHandler {

    public void OnInputClicked(InputClickedEventData eventData) {
        GameObject[] cycleObjects;
        cycleObjects = GameObject.FindGameObjectsWithTag("CyclePosition");
        foreach (GameObject cycleObject in cycleObjects) {
            cycleObject.GetComponent<ComponentView>().RemoveCompnend();
        }
    }
}
