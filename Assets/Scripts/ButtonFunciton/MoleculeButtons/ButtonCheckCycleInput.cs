using HoloToolkit.Unity.InputModule;
using UnityEngine;

/// <summary>
/// Checks if all molecule and reaction are in the right places
/// <summary>
public class ButtonCheckCycleInput : MonoBehaviour, IInputClickHandler {

    public void OnInputClicked(InputClickedEventData eventData) {
        GameObject[] cycleObjects;
        cycleObjects = GameObject.FindGameObjectsWithTag("CyclePosition");
        foreach (GameObject cycleObject in cycleObjects) {
            cycleObject.GetComponent<ComponentView>().Check();
        }
    }
}
