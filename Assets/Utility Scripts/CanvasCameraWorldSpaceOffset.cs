using UnityEngine;

public class CanvasCameraWorldSpaceOffset : MonoBehaviour
{
    Canvas canvas;
    public Vector3 canvasLocalPosition;
    public Vector3 canvasLocalRotation;
    public Vector3 localScale = new Vector3(0.001f, 0.001f, 0.001f);
    private void OnEnable()
    {
        canvas = GetComponent<Canvas>();
    }


    void Update()
    {
        if (canvas.worldCamera != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.transform.position = canvas.worldCamera.transform.TransformPoint(canvasLocalPosition);
            canvas.transform.rotation = canvas.worldCamera.transform.rotation * Quaternion.Euler(canvasLocalRotation);
            canvas.transform.localScale = localScale;
        }
    }
}
