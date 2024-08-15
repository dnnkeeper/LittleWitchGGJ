using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
public class LockInputManager : MonoBehaviour
{
    static HashSet<GameObject> registeredLockRequests = new HashSet<GameObject>();

    public UnityEvent<bool> onInputLockValueChanged;

    public UnityEvent onInputLocked, onInputUnlocked;

    /// <summary>
    /// For debugging or inspecting who locked input
    /// </summary>
    public List<GameObject> registeredLockRequestsList = new List<GameObject>();

    bool isInputLocked;
    // Update is called once per frame
    void Update()
    {
        UpdateLockState();
        if (registeredLockRequestsList.Count != registeredLockRequests.Count)
        {
            registeredLockRequestsList = registeredLockRequests.ToList();
        }
    }

    public static void ToggleRegistration(GameObject instigator)
    {
        if (registeredLockRequests.Contains(instigator))
            Unregister(instigator);
        else
            Register(instigator);
    }

    public static void Register(GameObject instigator)
    {
        registeredLockRequests.Add(instigator);
    }

    public static void Unregister(GameObject instigator)
    {
        if (instigator != null)
            registeredLockRequests.Remove(instigator);
    }

    public void ClearList()
    {
        if (registeredLockRequests != null)
            foreach (GameObject instigator in registeredLockRequests) { Unregister(instigator); }
        else
            Debug.LogWarning($"{GetType()} registeredLockRequests == null");
    }

    void UpdateLockState()
    {
        bool needCleanup = false;

        int activeLockRequests = 0;
        foreach (var go in registeredLockRequests)
        {
            if (go == null)
            {
                needCleanup = true;
                continue;
            }
            if (go.activeInHierarchy)
            {
                activeLockRequests++;
            }
        }
        if (needCleanup)
        {
            needCleanup = false;
            registeredLockRequests = registeredLockRequests.Where(go => go != null).ToHashSet();
        }

        if (activeLockRequests > 0)
        {
            if (!isInputLocked)
            {
                isInputLocked = true;
                onInputLockValueChanged.Invoke(true);
                Debug.Log($"[LockInputManager] locked input at {name}");
                onInputLocked.Invoke();
            }
        }
        else
        {
            if (isInputLocked)
            {
                isInputLocked = false;
                onInputLockValueChanged.Invoke(false);
                Debug.Log($"[LockInputManager] un-locked input at {name}");
                onInputUnlocked.Invoke();
            }
        }
    }
}
