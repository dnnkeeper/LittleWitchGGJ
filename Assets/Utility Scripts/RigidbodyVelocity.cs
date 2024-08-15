using UnityEngine;

public class RigidbodyVelocity : MonoBehaviour
{
    public float velocityMagnitude;

    void FixedUpdate()
    {
        velocityMagnitude = GetComponent<Rigidbody>().linearVelocity.magnitude;
    }
}
