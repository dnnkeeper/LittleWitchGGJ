using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LockMousePanel : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onLocked, onUnlocked;
    public UnityEvent<bool> onCursorVisible;

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return) )
    //    {
    //        Unlock();
    //    }
    //}
    bool hasFocus;
    public static bool isCursorLocked;
    public bool lockOnClick = true;
    public CursorLockMode lockState = CursorLockMode.None;
    void Update()
    {

        if (Cursor.lockState != lockState) //
        {
            Cursor.lockState = lockState;
            Debug.Log("[LockCursor] Cursor.lockState change detected. Cursor.lockState = " + lockState);
            if (lockState == CursorLockMode.Locked)
            {
                Lock();
            }
            else
            {
                Unlock();
            }
        }
    }
    public void Unlock()
    {
        if (!isActiveAndEnabled)
            return;
        isCursorLocked = false;
        lockState = CursorLockMode.None;
        //Debug.Log("LockMousePanel onUnlocked");
        onUnlocked.Invoke();
    }
    public void Lock()
    {
        if (!isActiveAndEnabled)
            return;
        isCursorLocked = true;
        lockState = CursorLockMode.Locked;
        //Debug.Log("LockMousePanel onLocked");
        onLocked.Invoke();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (lockOnClick && lockState != CursorLockMode.Locked)
        {
            Debug.Log("[LockCursor] LockMousePanel.OnPointerClick lock cursor");
            lockState = CursorLockMode.Locked;
        }
    }

    public void HideCursor(bool value)
    {
        Debug.Log("[LockCursor] LockMousePanel.HideCursor " + value);
        SetCursorVisible(!value);
    }

    public void SetCursorVisible(bool value)
    {
        Debug.Log("[LockCursor] LockMousePanel.SetCursorVisible " + value);
        Cursor.visible = value;
        onCursorVisible.Invoke(value);
    }

    //public void SetCursorVisibileAndUnlocked(bool value)
    //{
    //    Debug.Log("[LockCursor] LockMousePanel.SetCursorLockAndVisibility " + value);
    //    if (value)
    //    {
    //        Cursor.visible = true;
    //        Unlock();
    //    }
    //    else
    //    {
    //        Cursor.visible = true;
    //        Lock();
    //    }
    //}

    private void OnDisable()
    {
        Debug.Log("[LockCursor] LockMousePanel.OnDisable unlock cursor");

        SetCursorVisible(true);
        Cursor.lockState = CursorLockMode.None;
    }

    void OnDestroy()
    {
        Debug.Log("[LockCursor] LockMousePanel.OnDestroy unlock cursor");

        SetCursorVisible(true);
        Cursor.lockState = CursorLockMode.None;
    }

    // void OnApplicationFocus(bool hasFocus)
    // {
    //     this.hasFocus = hasFocus;
    //     if (hasFocus)
    //     {
    //         Debug.Log("Application is focussed");
    //         Lock();
    //     }
    //     else
    //     {
    //         Debug.Log("Application lost focus");
    //         Unlock();
    //     }
    // }

}
