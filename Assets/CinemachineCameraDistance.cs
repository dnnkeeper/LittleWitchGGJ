using UnityEngine;
using Unity.Cinemachine;

public class CinemachineCameraDistance : MonoBehaviour
{
    CinemachineThirdPersonFollow cinemachineThirdPersonFollow;
    public float lerpAmount = 15f;
    public float sensivityModifier = 1f;
    public float min = 1f;
    public float max = 10f;
    float targetDistance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cinemachineThirdPersonFollow = GetComponent<CinemachineThirdPersonFollow>();
        targetDistance = cinemachineThirdPersonFollow.CameraDistance;
    }

    public void AddCameraDistance(float add)
    {
        SetCameraDistance(targetDistance + add * sensivityModifier);
    }

    public void SetCameraDistance(float distance)
    {
        targetDistance = Mathf.Clamp(distance, min, max);
    }

    private void Update()
    {
        if (lerpAmount > 0f)
        {
            cinemachineThirdPersonFollow.CameraDistance = Mathf.Lerp(cinemachineThirdPersonFollow.CameraDistance, targetDistance, lerpAmount * Time.deltaTime);
        }
        else
        {
            cinemachineThirdPersonFollow.CameraDistance = targetDistance;

        }
    }
}
