using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockInputBehaviour : MonoBehaviour
{
    private void OnEnable()
    {
        LockInputManager.Register(gameObject);
    }

    private void OnDisable()
    {
        LockInputManager.Unregister(gameObject);
    }
}
