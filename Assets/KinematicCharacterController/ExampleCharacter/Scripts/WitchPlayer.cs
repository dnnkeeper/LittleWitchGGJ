using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine.InputSystem;

namespace KinematicCharacterController.Examples
{
    public class WitchPlayer : MonoBehaviour
    {
        public ExampleCharacterController Character;
        //public ExampleCharacterCamera CharacterCamera;
        public Transform lookRotationReference;

        public InputActionReference MoveAction;
        public InputActionReference LookAction;
        public InputActionReference UpAction;
        public InputActionReference InteractionAction;
        public float lookRotationSmoothRate = 0f;

        public float sensivityModifier = 1f;

        [Range(0, 90)]
        public float clampPitchMax = 70f;

        [Range(-90, 0)]
        public float clampPitchMin = -25f;

        private void Start()
        {

            // Tell camera to follow transform
            //CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            //CharacterCamera.IgnoredColliders.Clear();
            //CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    Cursor.lockState = CursorLockMode.Locked;
            //}

            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            // Handle rotating the camera along with physics movers
            //if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
            //{
            //    CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
            //    CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
            //}

            HandleCameraInput();
        }
        protected Quaternion lookRotation;
        protected Vector3 lookRotationEuler;
        
        private void HandleCameraInput()
        {
            var lookInputDelta = GetLookInputDelta() * sensivityModifier;
          
            lookRotationEuler.x += lookInputDelta.x;

            lookRotationEuler.y = Mathf.Clamp(lookRotationEuler.y + lookInputDelta.y, clampPitchMin, clampPitchMax);

            var _lookRotation =  Quaternion.Euler(new Vector3(0f, lookRotationEuler.x, 0f)); //global axis rotation

            _lookRotation = _lookRotation * Quaternion.Euler(new Vector3(lookRotationEuler.y, 0f, 0f)); //local axis rotation

            lookRotation = lookRotationSmoothRate > 0 ? Quaternion.Lerp(lookRotation, _lookRotation, lookRotationSmoothRate * Time.deltaTime) : _lookRotation;

            lookRotationReference.transform.rotation = lookRotation;
        }

        protected virtual Vector2 GetLookInputDelta()
        {
            Vector2 lookInputDelta = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
            lookInputDelta = LookAction.action.ReadValue<Vector2>();
#else
            lookInputDelta.x = Input.GetAxis(lookXAxisName);
            lookInputDelta.y = Input.GetAxis(lookYAxisName);
#endif
#if UNITY_WEBGL
            //WORKAROUND for WEBGL mouse screen wraparound bug
            lookInputDelta.x = Mathf.Clamp(lookInputDelta.x, -Time.deltaTime * 500f, Time.deltaTime * 500f);
            lookInputDelta.y = Mathf.Clamp(lookInputDelta.y, -Time.deltaTime * 500f, Time.deltaTime * 500f);
#endif
            return lookInputDelta;
        }


        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            Vector2 moveInput = MoveAction.action.ReadValue<Vector2>();
            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = moveInput.y;
            characterInputs.MoveAxisRight = moveInput.x;
            characterInputs.CameraRotation = lookRotationReference.rotation;
            characterInputs.JumpDown = UpAction.action.triggered;
            //characterInputs.CrouchDown = DownAction.action.triggered;
            //characterInputs.CrouchUp = !DownAction.action.triggered;

            if (InteractionAction.action.triggered)
            {
                Character.GetComponent<InteractionManager>().InteractWithClosestObject();
            }

            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;

            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();
            characterInputs.MoveAxisForward = 0f;
            characterInputs.MoveAxisRight = 0f;
            characterInputs.CameraRotation = lookRotationReference.rotation;
            Character.SetInputs(ref characterInputs);
        }
    }
}