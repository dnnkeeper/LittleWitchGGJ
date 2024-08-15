using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class OffscreenMarker : MonoBehaviour
{
    Camera mainCam;

    public Transform target;

    public float planeDistance = 1f;

    public float minDistanceToTarget = 20f;

    public float maxDistanceToTarget = 100f;

    public float shrinkMod = 2f;

    public bool allowOffScreen = false;

    public UnityEvent AppearedOnScreen;
    public UnityEvent DisappearedFromScreen;

    public UnityEvent OnLeftSide;
    public UnityEvent OnRightSide;

    public UnityEvent OnReached;

    public Vector3 initialLocalScale = Vector3.one;

    private void Awake()
    {
        initialLocalScale = transform.localScale;
        localPosOnScreenPlaneSmooth = Vector3.forward * planeDistance;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;

        SetProxyTarget();
    }

    public void SetProxyTarget()
    {
        if (proxyTarget != null)
        {
            GameObject.Destroy(proxyTarget.gameObject);
        }

        proxyTarget = new GameObject(name + "_proxy").transform;

        if (target != null)
        {
            proxyTarget.parent = target;
            proxyTarget.localPosition = Vector3.zero;
            proxyTarget.localRotation = Quaternion.identity;
        }
        else
        {
            proxyTarget.parent = transform.parent;
            proxyTarget.position = transform.position;
            proxyTarget.rotation = transform.rotation;
        }
        //Debug.LogWarning("Offscreen marker target was Null, created new one", target);

        transform.parent = mainCam.transform;
    }

    private void onPreRender()
    {
        if (isActiveAndEnabled)
            LateUpdate();
    }

    private void Reset()
    {
        target = transform.parent;
    }

    [SerializeField]
    private bool _onScreen;

    public bool onScreen
    {
        get { return _onScreen; }
        set
        {
            if (value != _onScreen)
            {
                _onScreen = value;
                if (_onScreen)
                {
                    AppearedOnScreen.Invoke();
                }
                else
                {
                    DisappearedFromScreen.Invoke();
                }
            }
        }
    }

    bool isOnLeftSide;
    public bool IsOnLeftSide
    {
        get => isOnLeftSide; set
        {
            if (isOnLeftSide != value)
            {
                isOnLeftSide = value;
                if (isOnLeftSide)
                {
                    OnLeftSide.Invoke();
                }
                else
                {
                    OnRightSide.Invoke();
                }
            }
        }
    }

    bool wasEnabled;
    void OnEnable()
    {
        wasEnabled = true;

        isOnLeftSide = false;

        StopAllCoroutines();

        // mainCam = Camera.main;
        // if (mainCam != null)
        // {
        //     var camEvents = mainCam.GetComponent<CameraOnRenderEvents>();
        //     if (camEvents != null)
        //     {
        //         Debug.Log("camEvents.onPreRender += onPreRender",this);
        //         camEvents.onPreRender += onPreRender;
        //     }
        // }

        if (target == null)
        {
            target = new GameObject(name + "_target").transform;

            target.parent = transform.parent;
            target.position = transform.position;
            target.rotation = transform.rotation;
        }
        //canvas = GetComponent<Canvas>();
        //canvas.enabled = true;
    }

    void OnDisable()
    {
        // if (mainCam != null)
        // { 
        //     var camEvents = mainCam.GetComponent<CameraOnRenderEvents>();
        //     if (camEvents != null)
        //         camEvents.onPreRender -= onPreRender;
        // }

        if (proxyTarget != null)
        {

            Destroy(proxyTarget.gameObject);
        }
    }

    IEnumerator DisableRoutine()
    {
        float timer = 0f;
        while (timer < 1.0f)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, target.position, timer);
            transform.localScale = Vector3.Lerp(transform.localScale, initialLocalScale, timer);
            yield return null;
        }
    }

    public float screenSizeMultiplier = 0.9f;

    public float disappearDistance = 1f;

    Vector3 localPosOnScreenPlaneSmooth;

    float smoothAmount = 120f;

    public bool rotateTowardTarget;

    public float distanceProgress;

    public bool isReached;

    Transform proxyTarget;

    // Update is called once per frame
    void LateUpdate()
    {
        if (mainCam == null)
            return;

        if (proxyTarget == null)
        {
            Debug.LogWarning("[OffscreenMarker] proxyTarget was destroyed");
            if (target != null)
            {
                SetProxyTarget();
            }
            return;
        }
        else if (!proxyTarget.gameObject.activeInHierarchy || !proxyTarget.gameObject.activeSelf)
        {
            Debug.LogWarning("[OffscreenMarker] proxyTarget disabled");
            transform.parent = proxyTarget.parent;
            return;
        }

        var cameraToTarget = target.position - mainCam.transform.position;

        bool backward = false;

        if (Vector3.Angle(mainCam.transform.forward, cameraToTarget) > 150.0f)
        {
            backward = true;
        }
        else
        {
            backward = false;
        }

        float distanceToTarget = cameraToTarget.magnitude;

        distanceProgress = Mathf.InverseLerp(minDistanceToTarget, maxDistanceToTarget, distanceToTarget);

        Vector3 planeCenter = mainCam.transform.position + mainCam.transform.forward * (Mathf.Lerp(planeDistance, planeDistance * shrinkMod, distanceProgress));

        Plane screenPlane = new Plane(mainCam.transform.forward, planeCenter);

        Ray fromCameraToTargetRay = new Ray(mainCam.transform.position, cameraToTarget);

        Vector3 projectedPosition = mainCam.transform.position + cameraToTarget.normalized;

        if (Vector3.Dot(fromCameraToTargetRay.direction, screenPlane.normal) > 0f && screenPlane.Raycast(fromCameraToTargetRay, out float rayHit))
        {
            projectedPosition = fromCameraToTargetRay.origin + fromCameraToTargetRay.direction * rayHit;

            //Debug.DrawLine(mainCam.transform.position, planeCenter, Color.red);
            //Debug.DrawLine(planeCenter, projectedPosition, Color.red);
            //Debug.DrawLine(target.position, projectedPosition, Color.red);
        }
        else if (screenPlane.Raycast(new Ray(target.position, mainCam.transform.forward), out float rayHitBackward))
        {
            projectedPosition = target.position + mainCam.transform.forward * rayHitBackward;

            //Debug.DrawLine(mainCam.transform.position, planeCenter, Color.red);
            //Debug.DrawLine(planeCenter, projectedPosition, Color.green);
            //Debug.DrawLine(target.position, projectedPosition, Color.blue);
        }

        Vector3 worldPosOnScreenPlane = projectedPosition;

        var verticalFOVHalf = mainCam.fieldOfView * Mathf.Deg2Rad / 2;
        var TanVerticalFOVHalf = Mathf.Tan(verticalFOVHalf);
        var horizontalFOVHalf = Mathf.Atan(TanVerticalFOVHalf * mainCam.aspect);
        var TanHorizontalFOVHalf = Mathf.Tan(horizontalFOVHalf);

        Vector3 localPosOnScreenPlane = mainCam.transform.InverseTransformPoint(worldPosOnScreenPlane);
        //localPosOnScreenPlane.z = Mathf.Max(Mathf.Abs(localPosOnScreenPlane.z), TanHorizontalFOVHalf * planeDistance);

        var maxVerticalValue = TanVerticalFOVHalf * localPosOnScreenPlane.z;
        var maxHorizontalValue = TanHorizontalFOVHalf * localPosOnScreenPlane.z;

        if (Mathf.Abs(localPosOnScreenPlane.x) > maxHorizontalValue || Mathf.Abs(localPosOnScreenPlane.y) > maxVerticalValue)
        {
            onScreen = false;
        }
        else
        {
            onScreen = true;
        }

        if (!allowOffScreen)
        {
            localPosOnScreenPlane.x = Mathf.Sign(localPosOnScreenPlane.x) * Mathf.Min(Mathf.Abs(localPosOnScreenPlane.x), screenSizeMultiplier * maxHorizontalValue);
            localPosOnScreenPlane.y = backward ? -screenSizeMultiplier * maxVerticalValue : Mathf.Sign(localPosOnScreenPlane.y) * Mathf.Min(Mathf.Abs(localPosOnScreenPlane.y), screenSizeMultiplier * maxVerticalValue);
            if (backward)
                smoothAmount = 3f;
            else
                smoothAmount = Mathf.Lerp(smoothAmount, 60f, Time.deltaTime);
        }

        if (localPosOnScreenPlane.x < 0f)
        {
            IsOnLeftSide = true;
        }
        else
        {
            IsOnLeftSide = false;
        }

        if (wasEnabled)
        {
            wasEnabled = false;

            if (isOnLeftSide)
                OnLeftSide.Invoke();
            else
                OnRightSide.Invoke();
        }


        localPosOnScreenPlaneSmooth = Vector3.Lerp(localPosOnScreenPlaneSmooth, localPosOnScreenPlane, Time.deltaTime * smoothAmount);

        worldPosOnScreenPlane = mainCam.transform.TransformPoint(localPosOnScreenPlaneSmooth);

        if (!isReached && cameraToTarget.magnitude < disappearDistance)
        {
            //canvas.enabled = false;

            //enabled = false;

            if (onScreen)
            {
                isReached = true;
                OnReached.Invoke();
                return;
            }
        }
        else
        {
            isReached = false;
        }

        transform.position = worldPosOnScreenPlane;

        if (rotateTowardTarget)
            transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        else
            transform.rotation = Quaternion.LookRotation(mainCam.transform.forward, mainCam.transform.up);

        transform.localScale = Vector3.Lerp(transform.localScale, initialLocalScale * planeDistance, Time.deltaTime * 2.0f);

        //transform.position = mainCam.transform.position + mainCam.transform.forward;
    }
}
