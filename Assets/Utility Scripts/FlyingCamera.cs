using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

#if CROSS_PLATFORM_INPUT
using UnityStandardAssets.CrossPlatformInput;
#endif	

namespace Utility.CameraUtil
{
    [System.Serializable]
    struct TransformInfo
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    public class FlyingCamera : MonoBehaviour
    {

        public string horizontalAxisName = "Horizontal";
        public string verticalAxisName = "Vertical";
        public string forwardAxisName = "Jump";

        public float xSpeed = 120.0f;
        public float ySpeed = 120.0f;

        public float rotationLerpSpeed = 10f;
        public float fovLerpSpeed = 1f;

        public bool hideCursor = true;

        //bool rightMouseDown;

        float speedMod = 1f;

        public float maxSpeed = 100f;
        public float accelerationRate = 10f;
        public float decelerationRate = 10f;

        public float targetSpeed = 5f;

        Vector3 moveDirection, targetVelocity;

        Vector3 targetPosition;

        float targetFOV;
        Camera cam;
        void Start()
        {
            cam = GetComponent<Camera>();
            targetFOV = cam.fieldOfView;

            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
        }

        float x, y;

        bool isMoving;

        public void GoForward()
        {
            enabled = true;
            StopAllCoroutines();
            StartCoroutine(GoForwardRoutine());
        }

        public void Stop()
        {
            StopAllCoroutines();
        }

        IEnumerator GoForwardRoutine()
        {
            Debug.Log("GoForwardRoutine");
            while (true)
            {
                moveDirection = Vector3.forward;

                targetSpeed = Mathf.Clamp(targetSpeed * (1f + Input.mouseScrollDelta.y * 0.05f), 0f, maxSpeed);

                targetVelocity = Vector3.Lerp(targetVelocity, moveDirection * targetSpeed, accelerationRate * Time.deltaTime);
                
                targetPosition = transform.position + transform.TransformVector(targetVelocity * Time.deltaTime * speedMod);

                transform.position = targetPosition;

                yield return null;
            }
        }
        bool isFocused;

        void Update()
        {
            float deltaTime = Time.deltaTime;

            if (!enabled)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                string key = SceneManager.GetActiveScene().name +"_Cam1";
                UpdateCameraInfo(key);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                string key = SceneManager.GetActiveScene().name + "_Cam2";
                UpdateCameraInfo(key);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                string key = SceneManager.GetActiveScene().name + "_Cam3";
                UpdateCameraInfo(key);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                string key = SceneManager.GetActiveScene().name + "_Cam4";
                UpdateCameraInfo(key);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                string key = SceneManager.GetActiveScene().name + "_Cam5";
                UpdateCameraInfo(key);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                string key = SceneManager.GetActiveScene().name + "_Cam6";
                UpdateCameraInfo(key);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                string key = SceneManager.GetActiveScene().name + "_Cam7";
                UpdateCameraInfo(key);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                string key = SceneManager.GetActiveScene().name + "_Cam8";
                UpdateCameraInfo(key);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                string key = SceneManager.GetActiveScene().name + "_Cam9";
                UpdateCameraInfo(key);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                string key = SceneManager.GetActiveScene().name + "_Cam0";
                UpdateCameraInfo(key);
            }

            if (Input.GetMouseButtonDown(0))
            {
                isFocused = true;
                //Debug.Log("RMB");

                moveDirection = Vector3.zero;

                targetVelocity = Vector3.zero;
                if (hideCursor)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }

            if (isFocused || isMoving)
            {
                targetPosition = transform.position;

                moveDirection = Vector3.zero;

                bool move = isMoving;

                isMoving = false;

                if (Input.GetKey(KeyCode.W))
                {
                    move = true;
                    moveDirection += Vector3.forward;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    move = true;
                    moveDirection -= Vector3.forward;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    move = true;
                    moveDirection -= Vector3.right;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    move = true;
                    moveDirection += Vector3.right;
                }
                if (Input.GetKey(KeyCode.Q))
                {
                    move = true;
                    moveDirection -= Vector3.up;
                }
                if (Input.GetKey(KeyCode.E))
                {
                    move = true;
                    moveDirection += Vector3.up;
                }

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    speedMod = 3f;
                }
                else
                {
                    if (Input.GetKey(KeyCode.C))
                    {
                        speedMod = 0.5f;
                    }
                    else
                        speedMod = 1f;
                }
                Vector3 mousePos = Input.mousePosition;
                mousePos.x = (mousePos.x / Screen.width) - 0.5f;
                mousePos.y = (mousePos.y / Screen.height) - 0.5f;

                //if (Application.isEditor || Screen.fullScreen || (Input.GetMouseButton(0) && Cursor.visible))
                //{
                    x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                    y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                //}

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    x -= xSpeed * Time.deltaTime * 0.2f;
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    x += xSpeed * Time.deltaTime * 0.2f;
                }

                if (Input.GetKey(KeyCode.UpArrow))
                {
                    y -= xSpeed * Time.deltaTime * 0.2f;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    y += xSpeed * Time.deltaTime * 0.2f;
                }

                if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus) )
                {
                    targetSpeed = Mathf.Clamp(targetSpeed * 1.2f, 0f, maxSpeed);
                }
                else if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus) )
                {
                    targetSpeed = Mathf.Clamp(targetSpeed / 1.2f, 0f, maxSpeed);
                }

                if (Input.GetKeyDown(KeyCode.PageUp) || Input.GetKeyDown(KeyCode.J))
                {
                    targetFOV = Mathf.Clamp(cam.fieldOfView + 5f, 1f, 120f);
                }
                else if (Input.GetKeyDown(KeyCode.PageDown) || Input.GetKeyDown(KeyCode.K))
                {
                    targetFOV = Mathf.Clamp(cam.fieldOfView - 5f, 1f, 120f);
                }
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovLerpSpeed * Time.deltaTime);

                Quaternion rotation = Quaternion.Euler(y, x, 0);

                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationLerpSpeed * Time.deltaTime);

                if (move)
                {
                    targetSpeed = Mathf.Clamp(targetSpeed * (1f + Input.mouseScrollDelta.y * 0.05f), 0f, maxSpeed);

                    targetVelocity = Vector3.Lerp(targetVelocity, moveDirection * targetSpeed, accelerationRate * deltaTime);
                }
                else
                {
                    targetVelocity = Vector3.Lerp(targetVelocity, moveDirection * targetSpeed, decelerationRate * deltaTime);
                }

                targetPosition = transform.position + transform.TransformVector(targetVelocity * deltaTime * speedMod);

                transform.position = targetPosition;
            }
            //else
            //{
            //    moveDirection = Vector3.zero;

            //    //if (hideCursor)
            //    //{
            //    //    Cursor.visible = true;
            //    //    Cursor.lockState = CursorLockMode.None;
            //    //}
            //}

            if (Input.GetKeyDown(KeyCode.T))
            {
                if ( Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1000f))
                {
                    rayHitPosition = hit.point;
                }
            }
            else if (Input.GetKeyUp(KeyCode.T))
            {
                if (rayHitPosition.sqrMagnitude > 0f)
                {
                    StopAllCoroutines();
                    StartCoroutine(FlyAroundPivot());
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopAllCoroutines();
                isFocused = false;
                if (hideCursor)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
        Vector3 rayHitPosition;

        IEnumerator FlyAroundPivot()
        {
            float d = 0;
            Vector3 deltaVector = transform.position - rayHitPosition;
            while (d < 360f)
            {
                transform.position = rayHitPosition + Quaternion.Euler(0, -d, 0) * deltaVector;
                transform.LookAt(rayHitPosition);
                x = transform.eulerAngles.y;
                y = transform.eulerAngles.x;
                d += Time.deltaTime * 60f;
                yield return null;
            }
        }
        Vector3 vel = Vector3.zero;
        float vx, vy;
        IEnumerator FlyTo(TransformInfo info, float smoothTime = 3f)
        {
            float t = 0;
            vel = Vector3.zero;
            vx = vy = 0;
            while ( (transform.position-info.position).sqrMagnitude > 0.1f )
            {
                transform.position = Vector3.SmoothDamp(transform.position, info.position, ref vel, smoothTime);
                x = Mathf.SmoothDampAngle(x, info.rotation.eulerAngles.y, ref vx, smoothTime);
                y = Mathf.SmoothDampAngle(y, info.rotation.eulerAngles.x, ref vy, smoothTime);
                t += Time.deltaTime;// / smoothTime;
                yield return null;
            }
        }

        private void UpdateCameraInfo(string key)
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand))
            {
                PlayerPrefs.SetString(key, JsonUtility.ToJson(new TransformInfo() { position = transform.position, rotation = transform.rotation }));
                Debug.Log($"Camera Info Saved: {key}");
            }
            else
            {
                if (PlayerPrefs.HasKey(key))
                {
                    TransformInfo info = JsonUtility.FromJson<TransformInfo>(PlayerPrefs.GetString(key));
                    Debug.Log($"Camera Info Loaded: {key}");

                    if (Input.GetKey(KeyCode.LeftAlt))
                    {
                        StopAllCoroutines();
                        StartCoroutine(FlyTo(info));
                    }
                    else
                    {
                        transform.position = info.position;
                        transform.rotation = info.rotation;
                        x = transform.eulerAngles.y;
                        y = transform.eulerAngles.x;
                    }
                }
            }
        }
    }
}