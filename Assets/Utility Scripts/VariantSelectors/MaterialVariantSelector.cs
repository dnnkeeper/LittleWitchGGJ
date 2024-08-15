using System.Collections.Generic;
using UnityEngine;

public class MaterialVariantSelector : VariantSelector
{
    public new Renderer renderer;
    public int materialIndex;
    public List<Material> interchangeableMaterials;

    public override int VariantsCount
    {
        get { return interchangeableMaterials.Count; }
    }

    private void Reset()
    {
        renderer = GetComponent<Renderer>();
    }

    void OnEnable()
    {
        if (renderer == null)
        {
            renderer = GetComponent<Renderer>();
        }
        ApplySelectedVariant();
    }

    protected override void ApplyVariant(int n)
    {
        var sharedMaterials = renderer.sharedMaterials;
        if (interchangeableMaterials.Count > n)
        {
            sharedMaterials[materialIndex] = interchangeableMaterials[n];
            renderer.sharedMaterials = sharedMaterials;
        }
        else
        {
            Debug.LogWarning($"{n} is greater than count of variants for {name}: ({interchangeableMaterials.Count}) ", this);
        }
        //Debug.Log("Apply material "+interchangeableMaterials[n].name+" to "+renderer.name, this);
    }

}
