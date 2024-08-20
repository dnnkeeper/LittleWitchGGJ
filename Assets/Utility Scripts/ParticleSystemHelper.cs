using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParticleSystemHelper : MonoBehaviour
{ 
    public void SetShapeMeshRenderer(MeshRenderer renderer)
    {
        var ps = GetComponent<ParticleSystem>();

        var emissionShape = ps.shape;
        emissionShape.meshRenderer = renderer;
        Debug.Log($"SetShapeMeshRenderer {renderer}", this);
    }
    public void SetShapeMeshRenderer()
    {
        SetShapeMeshRenderer(GetComponent<MeshRenderer>());
    }

    public void SetShapeMeshRendererParent()
    {
        SetShapeMeshRenderer(GetComponentInParent<MeshRenderer>());
    }
}
