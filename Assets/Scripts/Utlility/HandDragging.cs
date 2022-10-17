using HoloToolkit.Unity.InputModule;
using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.XR.WSA;

/// <summary>
/// Script for moving a gameobject aroung (drag)
/// <summary>
public class HandDragging : MonoBehaviour, IManipulationHandler {
    [SerializeField]
    float DragSpeed = 4f;

    [SerializeField]
    float DragScale = 4f;

    [SerializeField]
    float MaxDragDistance = 1000f;

    Vector3 lastPosition;

    [SerializeField]
    bool draggingEnabled = true;

    public void SetDragging(bool enabled) {
        draggingEnabled = enabled;
    }

    public void OnManipulationStarted(ManipulationEventData eventData) {
        //remove anchor for moving object
        if (gameObject != null) {
            WorldAnchor anchor = gameObject.GetComponent<WorldAnchor>();
            if (anchor) {
                // remove any world anchor component from the game object so that it can be moved
                DestroyImmediate(anchor);
            }
        }
        InputManager.Instance.PushModalInputHandler(gameObject);
        lastPosition = transform.position;

    }

    public void OnManipulationUpdated(ManipulationEventData eventData) {
        if (draggingEnabled) {
            Drag(eventData.CumulativeDelta);
        }
    }

    public void OnManipulationCompleted(ManipulationEventData eventData) {
        InputManager.Instance.PopModalInputHandler();
        //after finishing moving set anchor
        if (gameObject != null) {
            WorldAnchor anchor = gameObject.GetComponent<WorldAnchor>();
            if (!anchor && GetComponent<WorldAnchorManager>() != null) {
                WorldAnchorManager.Instance.AttachAnchor(gameObject);
            }
        }
    }

    public void OnManipulationCanceled(ManipulationEventData eventData) {
        InputManager.Instance.PopModalInputHandler();
        //when abborting the movment the object has also bee added to the world anker manager
        if (gameObject != null && GetComponent<WorldAnchorManager>() != null) {
            WorldAnchor anchor = gameObject.GetComponent<WorldAnchor>();
            if (!anchor) {
                WorldAnchorManager.Instance.AttachAnchor(gameObject);
            }
        }
    }

    void Drag(Vector3 positon) {
        var targetPosition = lastPosition + positon * DragScale;
        if (Vector3.Distance(lastPosition, targetPosition) <= MaxDragDistance) {
            transform.position = Vector3.Lerp(transform.position, targetPosition, DragSpeed);
        }
    }
}
