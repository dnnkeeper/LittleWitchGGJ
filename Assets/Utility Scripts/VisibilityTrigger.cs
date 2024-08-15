using UnityEngine;
using UnityEngine.Events;
public class VisibilityTrigger : MonoBehaviour
{
    public UnityEvent onBecameVisible, onBecameInvisible;
    void OnBecameVisible()
    {
        onBecameVisible.Invoke();
    }

    void OnBecameInvisible()
    {
        onBecameInvisible.Invoke();
    }
}
