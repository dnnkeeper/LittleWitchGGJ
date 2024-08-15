using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class FindMainCameraBehaviour : MonoBehaviour
{
    public UnityEvent<Camera> onMainCameraFound;
    public UnityEvent<GameObject> onMainCameraObjectFound;
    public UnityEvent<Transform> onMainCameraTransformFound;
    void OnEnable()
    {
        FindActiveMainCamera();
    }

    public void FindActiveMainCamera()
    {
        var mainCamera = Camera.main;
        onMainCameraFound.Invoke(mainCamera);
        if (mainCamera != null)
        {
            onMainCameraObjectFound.Invoke(mainCamera.gameObject);
            onMainCameraTransformFound.Invoke(mainCamera.transform);
        }

    }
}
