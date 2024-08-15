using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasCameraSetter : MonoBehaviour
{
    Canvas _canvas;
    public Canvas canvas
    {
        get
        {
            if (_canvas == null)
            {
                _canvas = GetComponent<Canvas>();
            }
            return _canvas;
        }
    }

    private void OnEnable()
    {

    }

    public void SetCanvasRenderMode(int n)
    {
        var mode = (RenderMode)n;
        Debug.Log($"{canvas.name} render mode set to {mode}");
        canvas.renderMode = mode;
    }

    void Update()
    {
        if (canvas.worldCamera == null || !canvas.worldCamera.isActiveAndEnabled)
        {
            canvas.worldCamera = Camera.main;
        }
    }
}
