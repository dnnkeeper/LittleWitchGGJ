using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class HasDepthTexture : MonoBehaviour
{
    public UnityEvent<bool> OnHasDepthTexture;
    [NonSerialized] public bool hasDepthTexture;
    // Update is called once per frame
    void Update()
    {
        hasDepthTexture = UniversalRenderPipeline.asset.supportsCameraDepthTexture;
        OnHasDepthTexture.Invoke(hasDepthTexture);
        if (hasDepthTexture)
            Shader.EnableKeyword("_HAS_DEPTH_TEXTURE");
        else
            Shader.DisableKeyword("_HAS_DEPTH_TEXTURE");

    }
}
