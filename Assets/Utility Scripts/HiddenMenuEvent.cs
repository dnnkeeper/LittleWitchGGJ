using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HiddenMenuEvent : MonoBehaviour
{
    public int countToActivate = 5;
    int tapCount;

    public UnityEvent<bool> onToggle;
    public UnityEvent onActivated;

    public bool isShown;

    private void Awake()
    {
        onToggle.Invoke(isShown);
    }

    public void OnTap()
    {
        tapCount++;
        if (tapCount >= countToActivate)
        {
            isShown = !isShown;
            Debug.Log($"Hidden Menu Toggled {isShown}");
            tapCount = 0;
            onToggle.Invoke(isShown);
            if (isShown)
                onActivated.Invoke();
        }
    }
}
