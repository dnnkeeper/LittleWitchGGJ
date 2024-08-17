using Dexart.Scripts.ObjectsPlacement;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlaceableObject))]
public class DraggableGameObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private PlaceableObject placeableObject;
    public Bounds dragLimitBounds;
    public void OnBeginDrag(PointerEventData eventData)
    {
        placeableObject = GetComponent<PlaceableObject>();
        placeableObject.GetComponent<Collider>().enabled = false;
        placeableObject.Relocate(eventData.pointerCurrentRaycast.worldPosition, placeableObject.transform.rotation);
    }

    public void OnDrag(PointerEventData eventData)
    {
        placeableObject = GetComponent<PlaceableObject>();
        if (eventData.pointerCurrentRaycast.gameObject != gameObject)
        {
            if (dragLimitBounds == null || dragLimitBounds.size.sqrMagnitude < 0.01f || dragLimitBounds.Contains(eventData.pointerCurrentRaycast.worldPosition)) {
                DrawBox(dragLimitBounds.center, Quaternion.identity, dragLimitBounds.size, Color.green, Time.deltaTime);

                placeableObject.Relocate(eventData.pointerCurrentRaycast.worldPosition, placeableObject.transform.rotation);
            }
            else
            {
                DrawBox(dragLimitBounds.center, Quaternion.identity, dragLimitBounds.size, Color.red, Time.deltaTime);
            }
        }
    }

    public void DrawBox(Vector3 pos, Quaternion rot, Vector3 scale, Color c, float t = 0)
    {
        // create matrix
        Matrix4x4 m = new Matrix4x4();
        m.SetTRS(pos, rot, scale);

        var point1 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
        var point2 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
        var point3 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
        var point4 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));

        var point5 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
        var point6 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
        var point7 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
        var point8 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));

        Debug.DrawLine(point1, point2, c, t);
        Debug.DrawLine(point2, point3, c, t);
        Debug.DrawLine(point3, point4, c, t);
        Debug.DrawLine(point4, point1, c, t);

        Debug.DrawLine(point5, point6, c, t);
        Debug.DrawLine(point6, point7, c, t);
        Debug.DrawLine(point7, point8, c, t);
        Debug.DrawLine(point8, point5, c, t);

        Debug.DrawLine(point1, point5, c, t);
        Debug.DrawLine(point2, point6, c, t);
        Debug.DrawLine(point3, point7, c, t);
        Debug.DrawLine(point4, point8, c, t);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        placeableObject = GetComponent<PlaceableObject>();
        placeableObject.GetComponent<Collider>().enabled = true;
        placeableObject.Accommodate();
    }
}
