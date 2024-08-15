using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[InitializeOnLoad]
public class MaximizeWhilePlaymode
{

#if UNITY_EDITOR

    private static readonly Type gameViewType = Type.GetType("UnityEditor.GameView,UnityEditor");

    private static readonly PropertyInfo showToolbarProperty =
        gameViewType.GetProperty("showToolbar", BindingFlags.Instance | BindingFlags.NonPublic);

    private static EditorWindow _instance;

    static MaximizeWhilePlaymode()
    {

        //EditorApplication.update += Update;
    }
    public static void ToggleFullScreen()
    {
        if (gameViewType == null)
        {
            Debug.LogError("GameView type not found.");
            return;
        }

        if (showToolbarProperty == null)
        {
            Debug.LogWarning("GameView.showToolbar property not found.");
        }

        if (CloseGameWindow())
            return;

        //Debug.Log("OPEN");

        _instance = (EditorWindow)ScriptableObject.CreateInstance(gameViewType);

        showToolbarProperty?.SetValue(_instance, false);

        var desktopResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        var fullscreenRect = new Rect(Vector2.zero, desktopResolution);
        _instance.ShowPopup();
        _instance.position = fullscreenRect;
        _instance.Focus();
    }

    private static bool CloseGameWindow()
    {
        if (_instance != null)
        {
            //Debug.Log(" CLOSE");

            _instance.Close();
            _instance = null;
            return true;
        }

        return false;
    }


    [InitializeOnLoadMethod]
    static void EditorInit()
    {
        FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler", BindingFlags.Static | BindingFlags.NonPublic);

        EditorApplication.CallbackFunction value = (EditorApplication.CallbackFunction)info.GetValue(null);

        value += EditorGlobalKeyPress;

        info.SetValue(null, value);
    }

    static void EditorGlobalKeyPress()
    {
        if (Event.current.keyCode == KeyCode.F11 && Event.current.type == EventType.KeyDown)
        {
            Debug.Log("Event.current.keyCode " + Event.current.keyCode + " " + Event.current.type);
            ToggleFullScreen();
        }
    }

    private static void Update()
    {

        if (Keyboard.current.f11Key.wasReleasedThisFrame)
        {
            Assembly assembly = typeof(EditorWindow).Assembly;
            Type type = assembly.GetType("UnityEditor.GameView");

            if (EditorWindow.focusedWindow.GetType() == type)
            {
                EditorWindow.focusedWindow.maximized = !EditorWindow.focusedWindow.maximized;
            }
        }
    }
#endif
}