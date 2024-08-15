using UnityEngine;

public class RotationBehaviour : MonoBehaviour
{
    public Vector3 rotationSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
