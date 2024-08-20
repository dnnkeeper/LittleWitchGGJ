using UnityEngine;
using UnityEngine.Rendering;

public enum UpdateMode
{
    UPDATE,
    LATE_UPDATE,
    BOTH,
    NONE
}


public class TargetLookRotationY : MonoBehaviour
{
    public bool findMainCamera;

    public Transform target;

    public bool lookAway;

    public UpdateMode updateMode = UpdateMode.LATE_UPDATE;

    public bool rotateBeforeRendering;

    private void Start()
    {
        if (findMainCamera && target == null)
        {
            target = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += RenderPipelineManager_BeginCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= RenderPipelineManager_BeginCameraRendering;
    }

    private void RenderPipelineManager_BeginCameraRendering(ScriptableRenderContext context, Camera renderCamera)
    {
        if (rotateBeforeRendering)
        {
            Rotate();
        }
    }

    private void Update()
    {
        if (updateMode == UpdateMode.UPDATE || updateMode == UpdateMode.BOTH)
        {
            Rotate();
        }
    }

    void LateUpdate()
    {
        if (updateMode == UpdateMode.LATE_UPDATE || updateMode == UpdateMode.BOTH)
        {
            Rotate();
        }
    }

    public void Rotate()
    {
        Quaternion rot = Quaternion.LookRotation((lookAway ? -1f : 1f) * (target.position - transform.position).normalized, target.up);
        transform.SetPositionAndRotation(transform.position, Quaternion.Euler(new Vector3(transform.eulerAngles.x, rot.eulerAngles.y, transform.eulerAngles.z)));
    }
}
