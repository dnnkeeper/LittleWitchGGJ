using UnityEngine;

public class FPSLimit : MonoBehaviour
{
    public int limit = 60;

    private void OnEnable()
    {
        Application.targetFrameRate = limit;
    }
}
