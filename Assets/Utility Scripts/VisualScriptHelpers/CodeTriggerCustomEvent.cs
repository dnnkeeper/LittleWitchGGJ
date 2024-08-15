using Unity.VisualScripting;
using UnityEngine;

public class CodeTriggerCustomEvent : MonoBehaviour
{
    public string eventName;
    public bool invokeOnEnable;

    void OnValidate()
    {
        if (invokeOnEnable)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogWarning($"{this.name} eventName is not set!", this);
                invokeOnEnable = false;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }
    }

    void OnEnable()
    {
        if (invokeOnEnable)
        {
            InvokeDefaultCustomEvent();
        }
    }

    public void InvokeCustomVoidEvent(GameObject target, string eventName)
    {
        CustomEvent.Trigger(target, eventName);
    }
    public void InvokeCustomIntEvent(GameObject target, string eventName, int v)
    {
        CustomEvent.Trigger(target, eventName, v);
    }

    public void InvokeCustomBoolEvent(GameObject target, string eventName, bool v)
    {
        CustomEvent.Trigger(target, eventName, v);
    }

    public void InvokeCustomStringEvent(GameObject target, string eventName, string v)
    {
        CustomEvent.Trigger(target, eventName, v);
    }

    public void InvokeCustomFloatEvent(GameObject target, string eventName, float v)
    {
        CustomEvent.Trigger(target, eventName, v);
    }

    public void InvokeCustomGameObjectEvent(GameObject target, string eventName, GameObject go)
    {
        CustomEvent.Trigger(target, eventName, go);
    }

    public void InvokeDefaultCustomEvent()
    {

        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogError($"{this.name} eventName is not set!", this);
        }
        else
        {
            InvokeCustomVoidEvent(gameObject, eventName);
        }
    }
}
