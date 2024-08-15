using System.Collections.Generic;
using UnityEngine;

public class PrefabVariantSelector : VariantSelector
{
    public List<GameObject> prefabsList;

    protected GameObject spawnedInstance;

    public override int VariantsCount
    {
        get { return prefabsList.Count; }
    }
    protected override void ApplyVariant(int n)
    {
        var selectedPrefab = prefabsList[n];
        if (selectedPrefab != null)
        {
            var newInstance = GameObject.Instantiate(selectedPrefab, (selectedPrefab.transform.localPosition), (selectedPrefab.transform.rotation), transform);
            if (spawnedInstance != null)
            {
                GameObject.Destroy(spawnedInstance);
            }
            spawnedInstance = newInstance;
        }
    }

}

