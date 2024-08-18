using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace UnityStandardAssets.Water
{
    [ExecuteInEditMode] // Make water live-update even when not in play mode
    public class Water : MonoBehaviour
    {
        public enum WaterMode
        {
            Simple = 0,
            Reflective = 1,
            Refractive = 2,
        };

        [SerializeField] private bool _forceClearSkyBox;

        public WaterMode waterMode = WaterMode.Refractive;
        public bool disablePixelLights = true;
        public int textureSize = 256;
        public float clipPlaneOffset = 0.07f;
        public LayerMask reflectLayers = -1;
        public LayerMask refractLayers = -1;

        public Renderer rendererComponent;

        //private Dictionary<Camera, Camera> m_ReflectionCameras = new Dictionary<Camera, Camera>(); // Camera -> Camera table
        //private Dictionary<Camera, Camera> m_RefractionCameras = new Dictionary<Camera, Camera>(); // Camera -> Camera table
        //private RenderTexture m_ReflectionTexture;
        //private RenderTexture m_RefractionTexture;
        private WaterMode m_HardwareWaterSupport = WaterMode.Refractive;
        private int m_OldReflectionTextureSize;
        private int m_OldRefractionTextureSize;
        private static bool s_InsideWater;
        Material _originalMaterial;
        public CameraClearFlags reflectionCameraClearFlags = CameraClearFlags.SolidColor;
        
        // This is called when it's known that the object will be rendered by some
        // camera. We render reflections / refractions and do other updates here.
        // Because the script executes in edit mode, reflections for the scene view
        // camera will just work!
        //public void OnWillRenderObject()
        //{
        //    if (!enabled || !rendererComponent || !rendererComponent.sharedMaterial ||
        //        !rendererComponent.enabled)
        //    {
        //        return;
        //    }

        //    Camera cam = Camera.current;
        //    if (!cam)
        //    {
        //        return;
        //    }

        //    // Safeguard from recursive water reflections.
        //    if (s_InsideWater)
        //    {
        //        return;
        //    }
        //    s_InsideWater = true;

        //    // Actual water rendering mode depends on both the current setting AND
        //    // the hardware support. There's no point in rendering refraction textures
        //    // if they won't be visible in the end.
        //    m_HardwareWaterSupport = FindHardwareWaterSupport();
        //    WaterMode mode = GetWaterMode();

        //    Camera reflectionCamera, refractionCamera;
        //    CreateWaterObjects(cam, out reflectionCamera, out refractionCamera);

        //    // find out the reflection plane: position and normal in world space
        //    Vector3 pos = transform.position;
        //    Vector3 normal = transform.up;

        //    // Optionally disable pixel lights for reflection/refraction
        //    int oldPixelLightCount = QualitySettings.pixelLightCount;
        //    if (disablePixelLights)
        //    {
        //        QualitySettings.pixelLightCount = 0;
        //    }

        //    UpdateCameraModes(cam, reflectionCamera);
        //    UpdateCameraModes(cam, refractionCamera);

        //    // Render reflection if needed
        //    if (mode >= WaterMode.Reflective)
        //    {
        //        // Reflect camera around reflection plane
        //        float d = -Vector3.Dot(normal, pos) - clipPlaneOffset;
        //        Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

        //        Matrix4x4 reflection = Matrix4x4.zero;
        //        CalculateReflectionMatrix(ref reflection, reflectionPlane);
        //        Vector3 oldpos = cam.transform.position;
        //        Vector3 newpos = reflection.MultiplyPoint(oldpos);
        //        reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

        //        // UpdateName oblique projection matrix so that near plane is our reflection
        //        // plane. This way we clip everything below/above it for free.
        //        Vector4 clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);
        //        reflectionCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);

        //        // Set custom culling matrix from the current camera
        //        reflectionCamera.cullingMatrix = cam.projectionMatrix * cam.worldToCameraMatrix;

        //        reflectionCamera.cullingMask = ~(1 << 4) & reflectLayers.value; // never render water layer
        //        reflectionCamera.targetTexture = m_ReflectionTexture;
        //        bool oldCulling = GL.invertCulling;
        //        GL.invertCulling = !oldCulling;
        //        reflectionCamera.transform.position = newpos;
        //        Vector3 euler = cam.transform.eulerAngles;
        //        reflectionCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);
        //        reflectionCamera.Render();
        //        reflectionCamera.transform.position = oldpos;
        //        GL.invertCulling = oldCulling;
        //        rendererComponent.sharedMaterial.SetTexture("_ReflectionTex", m_ReflectionTexture);
        //    }

        //    // Render refraction
        //    if (mode >= WaterMode.Refractive)
        //    {
        //        refractionCamera.worldToCameraMatrix = cam.worldToCameraMatrix;

        //        // UpdateName oblique projection matrix so that near plane is our reflection
        //        // plane. This way we clip everything below/above it for free.
        //        Vector4 clipPlane = CameraSpacePlane(refractionCamera, pos, normal, -1.0f);
        //        refractionCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);

        //        // Set custom culling matrix from the current camera
        //        refractionCamera.cullingMatrix = cam.projectionMatrix * cam.worldToCameraMatrix;

        //        refractionCamera.cullingMask = ~(1 << 4) & refractLayers.value; // never render water layer
        //        refractionCamera.targetTexture = m_RefractionTexture;
        //        refractionCamera.transform.position = cam.transform.position;
        //        refractionCamera.transform.rotation = cam.transform.rotation;
        //        refractionCamera.Render();
        //        rendererComponent.sharedMaterial.SetTexture("_RefractionTex", m_RefractionTexture);
        //    }

        //    // Restore pixel light count
        //    if (disablePixelLights)
        //    {
        //        QualitySettings.pixelLightCount = oldPixelLightCount;
        //    }

        //    // UpdateName shader keywords based on water mode
        //    switch (mode)
        //    {
        //        case WaterMode.Simple:
        //            Shader.EnableKeyword("WATER_SIMPLE");
        //            Shader.DisableKeyword("WATER_REFLECTIVE");
        //            Shader.DisableKeyword("WATER_REFRACTIVE");
        //            break;
        //        case WaterMode.Reflective:
        //            Shader.DisableKeyword("WATER_SIMPLE");
        //            Shader.EnableKeyword("WATER_REFLECTIVE");
        //            Shader.DisableKeyword("WATER_REFRACTIVE");
        //            break;
        //        case WaterMode.Refractive:
        //            Shader.DisableKeyword("WATER_SIMPLE");
        //            Shader.DisableKeyword("WATER_REFLECTIVE");
        //            Shader.EnableKeyword("WATER_REFRACTIVE");
        //            break;
        //    }

        //    s_InsideWater = false;
        //}
        void Awake()
        {
            if (rendererComponent == null)
                rendererComponent = GetComponent<Renderer>();
        }

        /// <summary>
        ///     Subscribes to the URP camera rendering callback.
        /// </summary>
        protected void OnEnable()
        {
            if (rendererComponent != null && _originalMaterial)
            {
                rendererComponent.sharedMaterial = _originalMaterial;
            }

            RenderPipelineManager.beginCameraRendering += RenderPipelineManager_BeginCameraRendering;
        }

        // Cleanup all the objects we possibly have created
        void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= RenderPipelineManager_BeginCameraRendering;

            CleanReflections();

            _reflectionCameras.Clear();

            //if (m_ReflectionTexture)
            //{
            //    DestroyImmediate(m_ReflectionTexture);
            //    m_ReflectionTexture = null;
            //}
            //if (m_RefractionTexture)
            //{
            //    DestroyImmediate(m_RefractionTexture);
            //    m_RefractionTexture = null;
            //}
            //foreach (var kvp in m_ReflectionCameras)
            //{
            //    DestroyImmediate((kvp.Value).gameObject);
            //}
            //m_ReflectionCameras.Clear();
            //foreach (var kvp in m_RefractionCameras)
            //{
            //    DestroyImmediate((kvp.Value).gameObject);
            //}
            //m_RefractionCameras.Clear();
        }
        /// <summary>
        ///     Frees resources.
        /// </summary>
        protected void OnDestroy()
        {
            CleanReflections();
        }

        private void CleanReflections()
        {
            foreach (KeyValuePair<Camera, Camera> camPair in _reflectionCameras)
            {
                if (camPair.Value != null)
                {
                    DestroyImmediate(camPair.Value.gameObject);
                }
            }

            if (_reflectionTexture)
            {
                Debug.Log($"<color=red>Destroying</color> reflection texture {_reflectionTexture}", this);
                DestroyImmediate(_reflectionTexture);
                _reflectionTexture = null;
            }

            _reflectionCameras.Clear();
        }

        // This just sets up some matrices in the material; for really
        // old cards to make water texture scroll.
        void Update()
        {
            if (!rendererComponent)
            {
                return;
            }
            Material mat = rendererComponent.sharedMaterial;
            if (!mat)
            {
                return;
            }

            Vector4 waveSpeed = mat.GetVector("WaveSpeed");
            float waveScale = mat.GetFloat("_WaveScale");
            Vector4 waveScale4 = new Vector4(waveScale, waveScale, waveScale * 0.4f, waveScale * 0.45f);

            // Time since level load, and do intermediate calculations with doubles
            double t = Time.timeSinceLevelLoad / 20.0;
            Vector4 offsetClamped = new Vector4(
                (float)Math.IEEERemainder(waveSpeed.x * waveScale4.x * t, 1.0),
                (float)Math.IEEERemainder(waveSpeed.y * waveScale4.y * t, 1.0),
                (float)Math.IEEERemainder(waveSpeed.z * waveScale4.z * t, 1.0),
                (float)Math.IEEERemainder(waveSpeed.w * waveScale4.w * t, 1.0)
                );

            mat.SetVector("_WaveOffset", offsetClamped);
            mat.SetVector("_WaveScale4", waveScale4);
        }

        void UpdateCameraModes(Camera src, Camera dest)
        {
            if (dest == null)
            {
                return;
            }
            // set water camera to clear the same way as current camera
            //dest.clearFlags = src.clearFlags;
            dest.backgroundColor = src.backgroundColor;
            if (src.clearFlags == CameraClearFlags.Skybox)
            {
                Skybox sky = src.GetComponent<Skybox>();
                Skybox mysky = dest.GetComponent<Skybox>();
                if (!sky || !sky.material)
                {
                    mysky.enabled = false;
                }
                else
                {
                    mysky.enabled = true;
                    mysky.material = sky.material;
                }
            }
            // update other values to match current camera.
            // even if we are supplying custom camera&projection matrices,
            // some of values are used elsewhere (e.g. skybox uses far plane)
            dest.farClipPlane = src.farClipPlane;
            dest.nearClipPlane = src.nearClipPlane;
            dest.orthographic = src.orthographic;
            dest.fieldOfView = src.fieldOfView;
            dest.aspect = src.aspect;
            dest.orthographicSize = src.orthographicSize;
        }


        // On-demand create any objects we need for water
        //void CreateWaterObjects(Camera currentCamera, out Camera reflectionCamera, out Camera refractionCamera)
        //{
        //    WaterMode mode = GetWaterMode();

        //    reflectionCamera = null;
        //    refractionCamera = null;

        //    if (mode >= WaterMode.Reflective)
        //    {
        //        // Reflection render texture
        //        if (!m_ReflectionTexture || m_OldReflectionTextureSize != textureSize)
        //        {
        //            if (m_ReflectionTexture)
        //            {
        //                DestroyImmediate(m_ReflectionTexture);
        //            }
        //            m_ReflectionTexture = new RenderTexture(textureSize, textureSize, 16);
        //            m_ReflectionTexture.name = "__WaterReflection" + GetInstanceID();
        //            m_ReflectionTexture.isPowerOfTwo = true;
        //            m_ReflectionTexture.hideFlags = HideFlags.DontSave;
        //            m_OldReflectionTextureSize = textureSize;
        //        }

        //        // Camera for reflection
        //        m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera);
        //        if (!reflectionCamera) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
        //        {
        //            GameObject go = new GameObject("Water Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
        //            reflectionCamera = go.GetComponent<Camera>();
        //            reflectionCamera.enabled = false;
        //            reflectionCamera.transform.position = transform.position;
        //            reflectionCamera.transform.rotation = transform.rotation;
        //            reflectionCamera.gameObject.AddComponent<FlareLayer>();
        //            go.hideFlags = HideFlags.HideAndDontSave;
        //            m_ReflectionCameras[currentCamera] = reflectionCamera;
        //        }
        //    }

        //    if (mode >= WaterMode.Refractive)
        //    {
        //        // Refraction render texture
        //        if (!m_RefractionTexture || m_OldRefractionTextureSize != textureSize)
        //        {
        //            if (m_RefractionTexture)
        //            {
        //                DestroyImmediate(m_RefractionTexture);
        //            }
        //            m_RefractionTexture = new RenderTexture(textureSize, textureSize, 16);
        //            m_RefractionTexture.name = "__WaterRefraction" + GetInstanceID();
        //            m_RefractionTexture.isPowerOfTwo = true;
        //            m_RefractionTexture.hideFlags = HideFlags.DontSave;
        //            m_OldRefractionTextureSize = textureSize;
        //        }

        //        // Camera for refraction
        //        m_RefractionCameras.TryGetValue(currentCamera, out refractionCamera);
        //        if (!refractionCamera) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
        //        {
        //            GameObject go =
        //                new GameObject("Water Refr Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(),
        //                    typeof(Camera), typeof(Skybox));
        //            refractionCamera = go.GetComponent<Camera>();
        //            refractionCamera.enabled = false;
        //            refractionCamera.transform.position = transform.position;
        //            refractionCamera.transform.rotation = transform.rotation;
        //            refractionCamera.gameObject.AddComponent<FlareLayer>();
        //            go.hideFlags = HideFlags.HideAndDontSave;
        //            m_RefractionCameras[currentCamera] = refractionCamera;
        //        }
        //    }
        //}

        WaterMode GetWaterMode()
        {
            if (m_HardwareWaterSupport < waterMode)
            {
                return m_HardwareWaterSupport;
            }
            return waterMode;
        }

        WaterMode FindHardwareWaterSupport()
        {
            if (!rendererComponent)
            {
                return WaterMode.Simple;
            }

            Material mat = rendererComponent.sharedMaterial;
            if (!mat)
            {
                return WaterMode.Simple;
            }

            string mode = mat.GetTag("WATERMODE", false);
            if (mode == "Refractive")
            {
                return WaterMode.Refractive;
            }
            if (mode == "Reflective")
            {
                return WaterMode.Reflective;
            }

            return WaterMode.Simple;
        }

        // Given position/normal of the plane, calculates plane in camera space.
        Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
        {
            Vector3 offsetPos = pos + normal * clipPlaneOffset;
            Matrix4x4 m = cam.worldToCameraMatrix;
            Vector3 cpos = m.MultiplyPoint(offsetPos);
            Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
            return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
        }

        // Calculates reflection matrix around the given plane
        static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
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

        /// <summary>
        ///     Called by Unity when the rendering starts. It is used in this component to render the reflection.
        /// </summary>
        private void RenderPipelineManager_BeginCameraRendering(ScriptableRenderContext context, Camera renderCamera)
        {
            // Avoid other cameras except for the main one.

            var _mirrorTransform = transform;
            
            //var mainCamera = Camera.main;

            if (!rendererComponent || !rendererComponent.sharedMaterial)
            {
                return;
            }

            // Avoid recursive rendering

            if (s_insideRendering)
            {
                return;
            }

            s_insideRendering = true;

            CreateResources(renderCamera, out Camera reflectionCamera);

            //UpdateName shader keywords based on water mode
            switch (waterMode)
            {
                case WaterMode.Simple:
                    Shader.EnableKeyword("WATER_SIMPLE");
                    Shader.DisableKeyword("WATER_REFLECTIVE");
                    Shader.DisableKeyword("WATER_REFRACTIVE");
                    break;
                case WaterMode.Reflective:
                    Shader.DisableKeyword("WATER_SIMPLE");
                    Shader.EnableKeyword("WATER_REFLECTIVE");
                    Shader.DisableKeyword("WATER_REFRACTIVE");
                    break;
                case WaterMode.Refractive:
                    Shader.DisableKeyword("WATER_SIMPLE");
                    Shader.DisableKeyword("WATER_REFLECTIVE");
                    Shader.EnableKeyword("WATER_REFRACTIVE");
                    break;
            }
            // Optionally disable pixel lights for reflection / refraction

            int oldPixelLightCount = QualitySettings.pixelLightCount;
            if (disablePixelLights)
            {
                QualitySettings.pixelLightCount = 0;
            }

            CopyCameraData(renderCamera, reflectionCamera);

            // Update parameters

            reflectionCamera.cullingMask = ~(1 << 4) & reflectLayers.value;

            Renderer theRenderer = GetComponent<Renderer>();

            if (theRenderer != null)
            {
                foreach (Material m in theRenderer.sharedMaterials)
                {
                    if (m.HasProperty("_ReflectionTex"))
                    {
                        m.SetTexture("_ReflectionTex", _reflectionTexture);
                    }

                    m.SetFloat(VarReflectionMaxLodBias, _reflectionTexture.width == 0 ? 0.0f : Mathf.Log(_reflectionTexture.width, 2.0f));
                }
            }

            // Render

            reflectionCamera.enabled = true;
            reflectionCamera.targetTexture = _reflectionTexture;
            RenderReflection(context, renderCamera, reflectionCamera, _mirrorTransform.position, _mirrorTransform.up);
            reflectionCamera.enabled = false;

            // Restore quality
            if (disablePixelLights)
                QualitySettings.pixelLightCount = oldPixelLightCount;

            s_insideRendering = false;
        }

        #region Private Methods
        /// <summary>
        ///     Renders the reflection.
        /// </summary>
        /// <param name="context">Render context of the scriptable render pipeline</param>
        /// <param name="renderCamera">Main camera</param>
        /// <param name="reflectionCamera">Camera that will render reflection</param>
        /// <param name="pos">Reflection plane position</param>
        /// <param name="normal">Reflection plane normal</param>
        private void RenderReflection(ScriptableRenderContext context, Camera renderCamera, Camera reflectionCamera, Vector3 pos, Vector3 normal)
        {
            reflectionCamera.ResetWorldToCameraMatrix();
            reflectionCamera.ResetCullingMatrix();

            // Reflect camera using reflection plane

            float d = -Vector3.Dot(normal, pos) - clipPlaneOffset;
            Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

            Matrix4x4 reflection = GetReflectionMatrix(reflectionPlane);
            Matrix4x4 projection = renderCamera.projectionMatrix;

            reflectionCamera.transform.SetPositionAndRotation(renderCamera.transform.position, renderCamera.transform.rotation);

            // World->ReflectionCamera matrix

            reflectionCamera.worldToCameraMatrix *= reflection;

            // Create projection matrix. Near plane will be our reflection plane so that we will clip everything on the other side.

            Vector4 clipPlane = GetCameraSpacePlane(reflectionCamera, clipPlaneOffset, pos, normal, 1.0f);
            projection = GetObliqueMatrix(projection, clipPlane);
            // Invert the vertical field of view because we can't use GL.invertCulling in URP
            projection.m11 = -projection.m11; 
            projection.m13 = -projection.m13;
            reflectionCamera.projectionMatrix = projection;
            reflectionCamera.cullingMatrix = reflectionCamera.projectionMatrix * reflectionCamera.worldToCameraMatrix;

            // Render

            //GL.invertCulling = true;
            // obsolete
            UniversalRenderPipeline.RenderSingleCamera(context, reflectionCamera);
            // new:
            //var requestData = new UniversalRenderPipeline.SingleCameraRequest()
            //{
            //    destination = reflectionCamera.targetTexture,
            //};
            //if (RenderPipeline.SupportsRenderRequest(reflectionCamera, requestData))
            //{
            //    RenderPipeline.SubmitRenderRequest(reflectionCamera, requestData);
            //}

            //GL.invertCulling = false;

            reflectionCamera.ResetWorldToCameraMatrix();
            reflectionCamera.ResetCullingMatrix();
        }
        /// <summary>
        ///     Computes the reflection matrix around the given plane.
        /// </summary>
        /// <param name="plane">Reflection plane</param>
        /// <returns>Reflected matrix</returns>
        public static Matrix4x4 GetReflectionMatrix(Vector4 plane)
        {
            Matrix4x4 reflectionMat;

            reflectionMat.m00 = 1F - 2F * plane[0] * plane[0];
            reflectionMat.m01 = -2F * plane[0] * plane[1];
            reflectionMat.m02 = -2F * plane[0] * plane[2];
            reflectionMat.m03 = -2F * plane[3] * plane[0];

            reflectionMat.m10 = -2F * plane[1] * plane[0];
            reflectionMat.m11 = 1F - 2F * plane[1] * plane[1];
            reflectionMat.m12 = -2F * plane[1] * plane[2];
            reflectionMat.m13 = -2F * plane[3] * plane[1];

            reflectionMat.m20 = -2F * plane[2] * plane[0];
            reflectionMat.m21 = -2F * plane[2] * plane[1];
            reflectionMat.m22 = 1F - 2F * plane[2] * plane[2];
            reflectionMat.m23 = -2F * plane[3] * plane[2];

            reflectionMat.m30 = 0F;
            reflectionMat.m31 = 0F;
            reflectionMat.m32 = 0F;
            reflectionMat.m33 = 1F;

            return reflectionMat;
        }
        static Matrix4x4 GetObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane)
        {
            Matrix4x4 oblique = projection;
            Vector4 q = projection.inverse * new Vector4(Mathf.Sign(clipPlane.x), Mathf.Sign(clipPlane.y), 1.0f, 1.0f);
            Vector4 c = clipPlane * (2.0F / Vector4.Dot(clipPlane, q));

            //third row = clip plane - fourth row
            oblique[2] = c.x - projection[3];
            oblique[6] = c.y - projection[7];
            oblique[10] = c.z - projection[11];
            oblique[14] = c.w - projection[15];

            return oblique;
        }
        /// <summary>
        ///     Copies data from one camera to another.
        /// </summary>
        /// <param name="src">Source data</param>
        /// <param name="dest">Destination data</param>
        private void CopyCameraData(Camera src, Camera dest)
        {
            if (dest == null)
            {
                return;
            }

            if (_forceClearSkyBox == false)
            {
                dest.clearFlags = reflectionCameraClearFlags;//src.clearFlags;
                dest.backgroundColor = src.backgroundColor;

                if (src.clearFlags == CameraClearFlags.Skybox)
                {
                    Skybox srcSky = src.GetComponent(typeof(Skybox)) as Skybox;
                    Skybox dstSky = dest.GetComponent(typeof(Skybox)) as Skybox;

                    if (dstSky)
                    {
                        if (!srcSky || !srcSky.material)
                        {
                            dstSky.enabled = false;
                        }
                        else
                        {
                            dstSky.enabled = true;
                            dstSky.material = srcSky.material;
                        }
                    }
                }
            }
            dest.stereoSeparation = src.stereoSeparation;
            dest.stereoConvergence = src.stereoConvergence;
            
            dest.farClipPlane = src.farClipPlane;
            dest.nearClipPlane = src.nearClipPlane;
            dest.orthographic = src.orthographic;

            if (XRSettings.enabled == false)
            {
                dest.fieldOfView = src.fieldOfView;
            }

            dest.aspect = src.aspect;
            dest.orthographicSize = src.orthographicSize;
        }

        /// <summary>
        ///     Creates the internal resources if necessary.
        /// </summary>
        /// <param name="currentCamera">Render camera</param>
        /// <param name="reflectionCamera">Reflection camera</param>
        private void CreateResources(Camera currentCamera, out Camera reflectionCamera)
        {
            reflectionCamera = null;

            // Render textures

            if (_oldReflectionTextureSize != textureSize)
            {
                CreateRenderTexture(ref _reflectionTexture);
                _oldReflectionTextureSize = textureSize;
            }

            if (_reflectionTexture == null)
            {
                CreateRenderTexture(ref _reflectionTexture);
            }

            // Reflection camera

            _reflectionCameras.TryGetValue(currentCamera, out reflectionCamera);

            if (!reflectionCamera)
            {
                var reflectionCameraName = $"{currentCamera.name} - Reflection Camera";
                GameObject go = GameObject.Find(reflectionCameraName);
                if (go == null)
                    go = new GameObject(reflectionCameraName, typeof(Camera), typeof(Skybox));

                reflectionCamera = go.GetComponent<Camera>();

                if (!XRSettings.enabled)
                {
                    reflectionCamera.fieldOfView = 60.0f;
                }

                reflectionCamera.transform.SetPositionAndRotation(transform.position, transform.rotation);
                reflectionCamera.enabled = true;
                go.hideFlags = HideFlags.DontSave;

            }
            _reflectionCameras[currentCamera] = reflectionCamera;

            if (_forceClearSkyBox)
            {
                reflectionCamera.clearFlags = CameraClearFlags.Skybox;
            }

            reflectionCamera.enabled = false;
        }

        /// <summary>
        ///     Creates a render texture.
        /// </summary>
        /// <param name="texture">Texture to create</param>
        private void CreateRenderTexture(ref RenderTexture texture)
        {
            if (texture)
            {
                Debug.Log($"<color=red>Destroying</color> reflection texture {texture}", this);
                DestroyImmediate(texture);
            }

            texture = new RenderTexture(textureSize, textureSize, 16);
            texture.name = $"{gameObject.name} Reflection";
            Debug.Log($"<color=green>Creating</color> reflection texture {texture.name}", this);
            texture.isPowerOfTwo = true;
            texture.hideFlags = HideFlags.DontSave;
            texture.filterMode = FilterMode.Trilinear;
            texture.autoGenerateMips = true;
            texture.useMipMap = true; // Mip mapping can be used in shaders for blurring
        }

        /// <summary>
        ///     Given a plane point and normal in world coordinates, computes the plane in camera space.
        /// </summary>
        /// <param name="cam">Camera</param>
        /// <param name="offset">Clip plane offset</param>
        /// <param name="position">Point in plane</param>
        /// <param name="normal">Plane normal</param>
        /// <param name="sideSign">Plane side of the camera</param>
        /// <returns>Plane in camera space</returns>
        private Vector4 GetCameraSpacePlane(Camera cam, float offset, Vector3 position, Vector3 normal, float sideSign)
        {
            Vector3 offsetPos = position + normal * offset;
            Matrix4x4 m = cam.worldToCameraMatrix;
            Vector3 cpos = m.MultiplyPoint(offsetPos);
            Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
            return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
        }

        #endregion

        #region Private Types & Data

        // Constants
        private const string VarReflectionMaxLodBias = "_ReflectionMaxLODBias";

        // Static

        private static bool s_insideRendering;
        private static readonly Dictionary<Camera, Camera> _reflectionCameras = new Dictionary<Camera, Camera>();

        // Internal

        private RenderTexture _reflectionTexture;
        private int _oldReflectionTextureSize;

        #endregion
    }
}
