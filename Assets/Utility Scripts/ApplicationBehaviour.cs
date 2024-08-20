using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationBehaviour : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }

    public void LoadlLevelAsync(string level)
    {
        Application.LoadLevelAsync(level);
    }

    public void LoadLevelAdditiveAsync(string level)
    {
        Application.LoadLevelAdditiveAsync(level);
    }
}
