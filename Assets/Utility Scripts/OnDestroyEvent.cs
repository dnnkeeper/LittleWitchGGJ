using UnityEngine;
using UnityEngine.Events;

public class OnDestroyEvent : MonoBehaviour
{
    public UnityEvent onDestroyed;
    private void OnDestroy()
    {
        onDestroyed.Invoke();
    }
}
