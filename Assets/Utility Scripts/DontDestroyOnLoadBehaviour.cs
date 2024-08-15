using UnityEngine;

public class DontDestroyOnLoadBehaviour : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
