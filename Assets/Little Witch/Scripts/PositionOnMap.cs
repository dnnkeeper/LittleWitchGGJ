using UnityEngine;

public class PositionOnMap : MonoBehaviour
{
    public Transform map;
    public Transform mapReferenceTransform;
    public RectTransform mapGameObjectPrefab;
    public RectTransform mapGameObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (mapGameObject == null)
        {
            mapGameObject = GameObject.Instantiate(mapGameObjectPrefab, map.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var flat_position = mapReferenceTransform.InverseTransformPoint(transform.position);
        flat_position.z = 0;
        mapGameObject.anchoredPosition = flat_position;
    }
}
