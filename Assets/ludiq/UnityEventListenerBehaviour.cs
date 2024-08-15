using Ludiq.Reflection;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventListenerBehaviour : MonoBehaviour
{
    [Filter(typeof(UnityEvent), Fields = true, Inherited = true)]
    public UnityMember sourceEvent;

    public UnityEvent actionEvent;

    void OnEnable()
    {
        AddListenerDelegate();
    }

    void OnDisable()
    {
        RemoveListenerDelegate();
    }

    UnityAction unityAction;

    void AddListenerDelegate()
    {
        unityAction = EventHandler;

        var addListenerMethodInfo = typeof(UnityEvent).GetMethod("AddListener", BindingFlags.Public | BindingFlags.Instance);

        var addListener = (Action<UnityAction>)Delegate.CreateDelegate(typeof(Action<UnityAction>), sourceEvent.Get(), addListenerMethodInfo);

        addListener(unityAction);
    }

    void RemoveListenerDelegate()
    {
        if (unityAction == null)
            return;

        var removeListenerMethodInfo = typeof(UnityEvent).GetMethod("RemoveListener", BindingFlags.Public | BindingFlags.Instance);

        var removeListener = (Action<UnityAction>)Delegate.CreateDelegate(typeof(Action<UnityAction>), sourceEvent.Get(), removeListenerMethodInfo);

        removeListener(unityAction);
    }

    public void EventHandler()
    {

        //Debug.Log("Event Handler invoked "+i, this);

        actionEvent.Invoke();
    }
}
