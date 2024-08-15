using UnityEngine;
using UnityEngine.Events;

public class OnBooleanEventBehaviour : MonoBehaviour
{
    public UnityEvent OnBooleanTrue, OnBooleanFalse;

    public void Invoke(bool b)
    {
        if (b)
            OnBooleanTrue.Invoke();
        else
            OnBooleanFalse.Invoke();
    }
}
