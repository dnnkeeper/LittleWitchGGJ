using UnityEngine;

public class PositionOnMap3D : MonoBehaviour
{
    public Transform map;
    public GameObject mapGameObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateMapGameObject();
    }

    [ContextMenu("Create MapGameObject")]
    void CreateMapGameObject()
    {
        if (mapGameObject == null)
        {
            mapGameObject = CreateVisualCopy(gameObject, map.transform);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

    // Update is called once per frame
    void Update()
    {
        //mapGameObject.transform.localPosition = transform.localPosition;
        if (mapGameObject != null)
        {
            transform.localPosition = mapGameObject.transform.localPosition;
        }
    }

    public GameObject CreateVisualCopy(GameObject original, Transform parent = null)
    {
        GameObject copy = new GameObject(original.gameObject.name + "_Copy");
        copy.transform.SetParent(parent);
        CopyComponents(original, copy);
        return copy;
    }

    private void CopyComponents(GameObject original, GameObject copy)
    {
        // Copy Transform
        copy.transform.localPosition = original.transform.localPosition;
        copy.transform.localRotation = original.transform.localRotation;
        copy.transform.localScale = original.transform.localScale;

        // Copy MeshRenderer
        MeshRenderer originalMeshRenderer = original.GetComponent<MeshRenderer>();
        if (originalMeshRenderer != null && originalMeshRenderer.enabled)
        {
            // Copy MeshFilter
            MeshFilter originalMeshFilter = original.GetComponent<MeshFilter>();
            if (originalMeshFilter != null)
            {
                MeshFilter copyMeshFilter = copy.AddComponent<MeshFilter>();
                copyMeshFilter.sharedMesh = originalMeshFilter.sharedMesh;
            }

            MeshRenderer copyMeshRenderer = copy.AddComponent<MeshRenderer>();
            copyMeshRenderer.sharedMaterials = originalMeshRenderer.sharedMaterials;
        }
        copy.gameObject.SetActive(original.gameObject.activeSelf);
        // Recursively copy children
        foreach (Transform child in original.transform)
        {
            GameObject childCopy = new GameObject(child.name);
            childCopy.transform.SetParent(copy.transform);
            CopyComponents(child.gameObject, childCopy);
        }
    }
}
