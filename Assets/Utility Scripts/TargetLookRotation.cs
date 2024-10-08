﻿using UnityEngine;

namespace Utility.CommonUtils
{
    public class TargetLookRotation : MonoBehaviour
    {
        public bool findMainCamera;

        public Transform target;

        public bool lookAway;

        public bool useLerp;

        public float speed = 1f;

        public bool onPreRenderUpdate;

        public void ToggleLookAway()
        {
            lookAway = !lookAway;
        }

        private void OnEnable()
        {
            var mainCam = Camera.main;
            if (onPreRenderUpdate && mainCam != null)
            {
                // var camEvents = mainCam.GetComponent<CameraOnRenderEvents>();
                // if (camEvents != null)
                //     camEvents.onPreRender += UpdateRotation;
            }
        }

        void OnDisable()
        {
            var mainCam = Camera.main;
            if (onPreRenderUpdate && mainCam != null)
            {
                // var camEvents = mainCam.GetComponent<CameraOnRenderEvents>();
                // if (camEvents != null)
                //     camEvents.onPreRender -= UpdateRotation;
            }
        }
        private void Start()
        {

        }

        public bool lateUpdate;

        private void Update()
        {
            if (!lateUpdate)
            {
                UpdateRotation();
            }
        }

        void LateUpdate()
        {
            if (lateUpdate)
            {
                UpdateRotation();
            }
        }

        void UpdateRotation()
        {
            if (target == null)
            {
                if (findMainCamera)
                    target = Camera.main.transform;
                else
                {
                    Debug.LogWarning(this + " lost target. Disabled");
                    enabled = false;
                    return;
                }
            }

            var targetRotation = Quaternion.LookRotation((lookAway ? -1f : 1f) * (target.position - transform.position).normalized, target.up);

            if (useLerp)
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            else
                transform.rotation = targetRotation;
        }
    }
}