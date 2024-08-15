using UnityEngine;
using UnityEngine.Events;

public class DebugOnlyEvent : MonoBehaviour
{
    public UnityEvent onDebug;
    public UnityEvent onRelease;

    void OnEnable()
    {
#if UNITY_DEBUG
        onDebug.Invoke();
#else
        int debugMode = PlayerPrefs.GetInt("DebugMode", 0);
        if (debugMode > 0 || Debug.isDebugBuild)
        {
            onDebug.Invoke();
        }
        else
        {
            onRelease.Invoke();
        }
#endif
    }

    [ContextMenu("Enable DebugMode")]
    public void EnableDebug()
    {
        PlayerPrefs.SetInt("DebugMode", 1);
    }

    [ContextMenu("Disable DebugMode")]
    public void DisableDebug()
    {
        PlayerPrefs.SetInt("DebugMode", 0);
    }

    [ContextMenu("Toggle DebugMode")]
    public void ToggleDebug()
    {
        int debugMode = PlayerPrefs.GetInt("DebugMode", 0);
        SetDebug((debugMode > 0) ? false : true);
    }

    public void SetDebug(bool b)
    {
        PlayerPrefs.SetInt("DebugMode", (b) ? 1 : 0);
    }

    //[ContextMenu("Set UNITY_DEBUG")]
    //public void SetDebug()
    //{

    //}

    //[ContextMenu("Clear UNITY_DEBUG")]
    //public void ClearDebug()
    //{

    //}

}
