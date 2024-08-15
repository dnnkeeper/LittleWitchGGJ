using Ludiq.Reflection;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventIntListenerBehaviour : MonoBehaviour
{
    [Filter(typeof(UnityEvent<int>), Fields = true, Inherited = true)]
    public UnityMember sourceEvent;

    public UnityEvent<int> actionEvent;

    void OnEnable()
    {
        AddListenerDelegate();
    }

    void OnDisable()
    {
        RemoveListenerDelegate();
    }

    UnityAction<int> unityAction;

    void AddListenerDelegate()
    {

        unityAction = EventHandler;

        var addListenerMethodInfo = typeof(UnityEvent<int>).GetMethod("AddListener", BindingFlags.Public | BindingFlags.Instance);

        var addListener = (Action<UnityAction<int>>)Delegate.CreateDelegate(typeof(Action<UnityAction<int>>), sourceEvent.Get(), addListenerMethodInfo);

        addListener(unityAction);
    }

    void RemoveListenerDelegate()
    {

        if (unityAction == null)
            return;

        var removeListenerMethodInfo = typeof(UnityEvent<int>).GetMethod("RemoveListener", BindingFlags.Public | BindingFlags.Instance);

        var removeListener = (Action<UnityAction<int>>)Delegate.CreateDelegate(typeof(Action<UnityAction<int>>), sourceEvent.Get(), removeListenerMethodInfo);

        removeListener(unityAction);
    }

    public void EventHandler(int i)
    {

        //Debug.Log("Event Handler invoked "+i, this);

        actionEvent.Invoke(i);
    }
}
