using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var systems = GameObject.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        foreach (var system in systems)
        {
            if (!system.transform.IsChildOf(transform))
            {
                Debug.LogWarning($"[{GetType()}] EventSystem {system} is not a child of {transform} will be destroyed!", transform);
                Destroy(system.gameObject);
            }
        }
    }
}
