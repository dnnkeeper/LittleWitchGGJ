using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class OnSceneLoadedBehaviour : MonoBehaviour
{

    public UnityEvent<string> onSceneLoaded;
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name + " mode: " + mode);
        onSceneLoaded.Invoke(scene.name);
    }

    // called when the game is terminated
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}