using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTransformBehaviour : MonoBehaviour
{
    public Transform targetTransform;

    Vector3 localPosition;
    Quaternion localRotation;

    void Update()
    {
        if (transform.localPosition != localPosition)
        {
            localPosition = transform.localPosition;
            targetTransform.localPosition = localPosition;
        }

        if (transform.localRotation != localRotation)
        {
            localRotation = transform.localRotation;
            targetTransform.localRotation = localRotation;
        }
    }
}
