using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

namespace UnityStandardAssets.Water
{
    [ExecuteInEditMode]
    public class WaterURP : MonoBehaviour
    {
        [Header("Settings")]
        public float clipPlaneOffset = 0.07f;
        public bool disablePixelLights = true;
        public LayerMask reflectionLayers = -1;
        public Renderer rendererComponent;
        public int textureSize = 256;
        public WaterMode waterMode = WaterMode.Refractive;
        [SerializeField] private bool _forceClearSkyBox;

        [Header("Debugging")]
        private Material _originalMaterial;
        private static readonly Dictionary<Camera, Camera> _reflectionCameras = new Dictionary<Camera, Camera>();
        private static bool s_insideRendering;
        private int _oldReflectionTextureSize;
        private RenderTexture _reflectionTexture;

        public enum WaterMode
        {
            Simple, Reflective, Refractive
        };

        private void Awake()
        {
            if (!rendererComponent)
                rendererComponent = GetComponent<Renderer>();
        }

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        }

        private void OnDisable()
        {
            TearDownReflectionResources();
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }

        protected void OnDestroy()
        {
            TearDownReflectionResources();
        }

        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (s_insideRendering || !rendererComponent)
                return;

            s_insideRendering = true;
            Camera reflectionCamera = EnsureCamera(camera);

            ConfigureShaderKeywords();
            PrepareForRendering(reflectionCamera, camera);
            ManagePixelLights(() =>
            {
                RenderReflection(context, reflectionCamera, camera, camera.transform);
            });

            s_insideRendering = false;
        }

        private Camera EnsureCamera(Camera currentCamera)
        {
            Camera reflectionCamera;
            if (!_reflectionCameras.TryGetValue(currentCamera, out reflectionCamera))
            {
                reflectionCamera = CreateReflectionCamera(currentCamera);
                _reflectionCameras[currentCamera] = reflectionCamera;
            }

            return reflectionCamera;
        }

        private Camera CreateReflectionCamera(Camera currentCamera)
        {
            string cameraName = $"{currentCamera.name} Reflection";
            GameObject cameraObj = new GameObject(cameraName, typeof(Camera), typeof(Skybox));

            var newCamera = cameraObj.GetComponent<Camera>();
            if (_forceClearSkyBox)
                newCamera.clearFlags = CameraClearFlags.Skybox;
            
            newCamera.enabled = false;
            cameraObj.hideFlags = HideFlags.HideAndDontSave;

            return newCamera;
        }

        private void ConfigureShaderKeywords()
        {
            // Toggles shader keywords based on water mode for the shader
            Shader.DisableKeyword("WATER_SIMPLE");
            Shader.DisableKeyword("WATER_REFLECTIVE");
            Shader.DisableKeyword("WATER_REFRACTIVE");

            string keyword = waterMode.ToString().ToUpper();
            Shader.EnableKeyword($"WATER_{keyword}");
        }

        private void ManagePixelLights(Action renderAction)
        {
            int oldPixelLightCount = QualitySettings.pixelLightCount;
            if (disablePixelLights)
                QualitySettings.pixelLightCount = 0;

            renderAction.Invoke();

            QualitySettings.pixelLightCount = oldPixelLightCount;
        }

        private void PrepareForRendering(Camera reflectionCamera, Camera renderCamera)
        {
            if (_reflectionTexture == null || _oldReflectionTextureSize != textureSize)
            {
                if (_reflectionTexture != null)
                    DestroyImmediate(_reflectionTexture);

                _reflectionTexture = new RenderTexture(textureSize, textureSize, 16);
                _reflectionTexture.name = $"{gameObject.name}_Reflection";
                _reflectionTexture.isPowerOfTwo = true;
                _reflectionTexture.hideFlags = HideFlags.DontSave;

                _oldReflectionTextureSize = textureSize;
            }

            reflectionCamera.CopyFrom(renderCamera);
            reflectionCamera.targetTexture = _reflectionTexture;
            rendererComponent.sharedMaterial.SetTexture("_ReflectionTex", _reflectionTexture);
        }

        private void RenderReflection(ScriptableRenderContext context, Camera reflectionCamera, Camera renderCamera, Transform mirrorTransform)
        {
            // Use reflection matrix to position the camera below the reflective surface to simulate the reflection
            Vector3 position = mirrorTransform.position;
            Vector3 normal = mirrorTransform.up;
            float d = -Vector3.Dot(normal, position) - clipPlaneOffset;
            Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

            Matrix4x4 reflection = Matrix4x4.zero;
            CalculateReflectionMatrix(ref reflection, reflectionPlane);
            reflectionCamera.worldToCameraMatrix = renderCamera.worldToCameraMatrix * reflection;

            Vector4 clipPlane = CameraSpacePlane(reflectionCamera, position, normal, 1.0f);
            Matrix4x4 projection = renderCamera.projectionMatrix;
            CalculateObliqueMatrix(ref projection, clipPlane);
            reflectionCamera.projectionMatrix = projection;

            UniversalRenderPipeline.RenderSingleCamera(context, reflectionCamera);
        }

        // Utility methods for matrix calculations
        private void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
        {
            reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
            reflectionMat.m01 = (-2F * plane[0] * plane[1]);
            reflectionMat.m02 = (-2F * plane[0] * plane[2]);
            reflectionMat.m03 = (-2F * plane[3] * plane[0]);
            reflectionMat.m10 = (-2F * plane[1] * plane[0]);
            reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
            reflectionMat.m12 = (-2F * plane[1] * plane[2]);
            reflectionMat.m13 = (-2F * plane[3] * plane[1]);
            reflectionMat.m20 = (-2F * plane[2] * plane[0]);
            reflectionMat.m21 = (-2F * plane[2] * plane[1]);
            reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
            reflectionMat.m23 = (-2F * plane[3] * plane[2]);
            reflectionMat.m30 = 0F;
            reflectionMat.m31 = 0F;
            reflectionMat.m32 = 0F;
            reflectionMat.m33 = 1F;
        }

        private void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
        {
            Vector4 q = projection.inverse * new Vector4(
                Mathf.Sign(clipPlane.x),
                Mathf.Sign(clipPlane.y),
                1.0f,
                1.0f
            );
            Vector4 c = clipPlane * (2.0F / Vector4.Dot(clipPlane, q));
            
            projection[2] = c.x - projection[3];
            projection[6] = c.y - projection[7];
            projection[10] = c.z - projection[11];
            projection[14] = c.w - projection[15];
        }

        private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
        {
            Vector3 offsetPos = pos + normal * clipPlaneOffset;
            Matrix4x4 m = cam.worldToCameraMatrix;
            Vector3 cpos = m.MultiplyPoint(offsetPos);
            Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
            return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
        }

        private void TearDownReflectionResources()
        {
            foreach (KeyValuePair<Camera, Camera> entry in _reflectionCameras)
            {
                DestroyImmediate(entry.Value.gameObject);
            }
            _reflectionCameras.Clear();

            if (_reflectionTexture != null)
            {
                DestroyImmediate(_reflectionTexture);
                _reflectionTexture = null;
            }
        }
    }
}