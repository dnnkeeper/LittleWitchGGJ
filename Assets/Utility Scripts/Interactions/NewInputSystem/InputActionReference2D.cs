using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputActionReference2D : MonoBehaviour
{
    public InputActionReference inputActionReference;
    public UnityEvent<Vector2> onInputPerformed;
    public UnityEvent<float> onInputPerformedX, onInputPerformedY;

    void OnEnable()
    {
        inputActionReference.action.performed += OnPerformed;
    }

    void OnDisable()
    {
        inputActionReference.action.performed -= OnPerformed;
    }

    private void OnPerformed(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        onInputPerformed.Invoke(value);
        onInputPerformedX.Invoke(value.x);
        onInputPerformedY.Invoke(value.y);
    }

}
