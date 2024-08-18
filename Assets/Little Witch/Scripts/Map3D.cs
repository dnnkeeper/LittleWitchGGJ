using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MapObjectInfo
{
    public GameObject mapObject;
    public bool draggable;
}

public class Map3D : MonoBehaviour
{
    public MapObjectsCollection originalObjectsCollection;
    public Transform mapRoot;
    public BoxCollider mapBounds;
    Dictionary<GameObject, GameObject> originalCopies = new Dictionary<GameObject, GameObject>();

    public MapObjectInfo[] mapObjectsSettings;
    public GameObject highlightPrefab;

    public LayerMask tableLayerMask = 1 << 2;

    // Start is called before the first frame update
    void Start()
    {
        if (mapRoot == null)
        {
            mapRoot = transform;
        }
        foreach(var originalObject in originalObjectsCollection.mapObjects)
        {
            var mapObjectCopy = originalObject.CreateMapGameObject(mapRoot, mapBounds);
            originalCopies.Add(originalObject.gameObject, mapObjectCopy);
        }
    }

    void LateUpdate()
    {
        ScaleMapRoot();
        foreach (var kvp in originalCopies)
        {
            var original = kvp.Key;
            var copy = kvp.Value;
            copy.transform.localPosition = original.transform.localPosition;
        }

        foreach (var mapObjectSetting in mapObjectsSettings)
        {
            var copy = originalCopies[mapObjectSetting.mapObject];
            var draggable = copy.GetComponent<DraggableGameObject>();
            if (draggable != null)
            {
                draggable.enabled = mapObjectSetting.draggable;
                draggable.layerMask = tableLayerMask;
                draggable.gameObject.layer = mapRoot.gameObject.layer;
                HighlightObject(copy, draggable.enabled);
            }
        }
    }

    private void HighlightObject(GameObject target, bool activate)
    {
        if (highlightPrefab != null)
        {
            var existingHighlight = target.FindObject(highlightPrefab.name);
            if (existingHighlight != null)
            {
                existingHighlight.SetActive(activate);
            }
            else if (activate)
            {
                var highlight = GameObject.Instantiate(highlightPrefab, target.transform);
                highlight.name = highlightPrefab.name;
            }
        }
    }

    private void OnValidate()
    {
        //ScaleMapRoot();
        if (mapObjectsSettings.Length == 0 && originalObjectsCollection != null)
        {
            mapObjectsSettings = new MapObjectInfo[originalObjectsCollection.mapObjects.Length];
            for (int i = 0; i < originalObjectsCollection.mapObjects.Length; i++)
            {
                mapObjectsSettings[i].mapObject = originalObjectsCollection.mapObjects[i].gameObject;
            }
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    [ContextMenu("ScaleMapRoot")]
    void ScaleMapRoot()
    {
        if (mapRoot != null)
        {
            // Keep uniform scale of map root while preserving its scaling 
            var lossyScale = mapRoot.transform.lossyScale;
            lossyScale.y = lossyScale.x;
            lossyScale.z = lossyScale.x;
            mapRoot.transform.localScale = mapRoot.transform.parent.InverseTransformVector(lossyScale);
        }
    }


}
