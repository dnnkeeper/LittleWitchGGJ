using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.Controls;
#endif
public class GetKeyDownEventTrigger : MonoBehaviour
{
    public UnityEvent onKeyDown;


    public KeyCode keyCode;

    // Update is called once per frame
    void Update()
    {
        if (GetKeyDown(keyCode))
        {
            onKeyDown.Invoke();
        }
    }

#if !ENABLE_LEGACY_INPUT_MANAGER
    static Dictionary<KeyCode, Key> lookup;
    bool isMouseKey;
    public string keyCodeString;
    ButtonControl ButtonControl;
    private void Awake()
    {
        keyCodeString = keyCode.ToString();
        if (keyCodeString.StartsWith("Mouse"))
        {
            isMouseKey = true;
            
            switch (keyCodeString)
            {
                case "Mouse0":
                    ButtonControl = Mouse.current.leftButton;
                    return;
                case "Mouse1":
                    ButtonControl = Mouse.current.rightButton;
                    return;
                case "Mouse2":
                    ButtonControl = Mouse.current.middleButton;
                    return;
                case "Mouse3":
                    ButtonControl = Mouse.current.backButton;
                    return;
                case "Mouse4":
                    ButtonControl = Mouse.current.forwardButton;
                    return;
            }
        }
    }
#endif

    bool GetKeyDown(KeyCode key)
    {
#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKeyDown(key);
#else
        if (lookup == null)
        {
            lookup = new Dictionary<KeyCode, Key>();
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                var textVersion = keyCode.ToString();
                if (Enum.TryParse<Key>(textVersion, true, out var value))
                    lookup[keyCode] = value;
            }
            lookup[KeyCode.Return] = Key.Enter;
            lookup[KeyCode.KeypadEnter] = Key.NumpadEnter;
        }
        if (isMouseKey)
        {
            return ButtonControl.wasPressedThisFrame;
        }

        return Keyboard.current[lookup[key]].wasPressedThisFrame;
#endif
    }
}
