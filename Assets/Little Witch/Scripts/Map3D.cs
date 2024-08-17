using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map3D : MonoBehaviour
{
    public MapObjectsCollection mapObjectsCollection;
    public Transform mapRoot;
    public BoxCollider mapBounds;
    Dictionary<GameObject, GameObject> mapObjectCopies = new Dictionary<GameObject, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        if (mapRoot == null)
        {
            mapRoot = transform;
        }
        foreach(var originalObject in mapObjectsCollection.mapObjects)
        {
            var mapObjectCopy = originalObject.CreateMapGameObject(mapRoot, mapBounds);
            mapObjectCopies.Add(originalObject.gameObject, mapObjectCopy);
        }
    }

    void LateUpdate()
    {
        foreach (var kvp in mapObjectCopies)
        {
            kvp.Value.transform.localPosition = kvp.Key.transform.localPosition;
        }
    }
}
