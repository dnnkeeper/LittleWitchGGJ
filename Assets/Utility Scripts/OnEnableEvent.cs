using UnityEngine;
using UnityEngine.Events;

public class OnEnableEvent : MonoBehaviour
{

    [Header("Main")]
    public UnityEvent onEnable;
    public UnityEvent onDisable;

    [Header("Additional")]
    public UnityEvent onAwake;
    public UnityEvent onStart;

    public void EnableGameObject()
    {
        gameObject.SetActive(true);
    }

    public void DisableGameObject()
    {
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        onAwake.Invoke();
    }

    private void Start()
    {
        onStart.Invoke();
    }

    void OnEnable()
    {
        if (enabled)
        {
            if (!gameObject.activeInHierarchy)
                Debug.LogWarning($"[OnEnableEvent] {GetGameObjectPath(gameObject)} OnEnable but is not active in hierarchy", this);
            //else
            //    Debug.Log($"[OnEnableEvent] {GetGameObjectPath(gameObject)}", this);
            onEnable.Invoke();
        }
    }

    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        path = "Root" + path;
        return path;
    }

    void OnDisable()
    {
        if (enabled)
        {
            onDisable.Invoke();
        }
    }
}
