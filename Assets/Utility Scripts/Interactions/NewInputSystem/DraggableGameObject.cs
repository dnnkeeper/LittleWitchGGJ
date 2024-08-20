using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlaceableObject))]
public class DraggableGameObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private PlaceableObject placeableObject;
    public Bounds dragLimitBounds;
    Vector3 dragStartWorldPos;
    bool isDragging;
    public float snapDistance = 0.1f;
    public LayerMask layerMask = 1 << 2;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!enabled)
            return;
        placeableObject.GetComponent<Collider>().enabled = false;
        //placeableObject.Relocate(eventData.pointerCurrentRaycast.worldPosition, placeableObject.transform.rotation);
    }
    float targetRotationY = 0;
    public void OnDrag(PointerEventData eventData)
    {
        if (!enabled)
            return;
        
        
        if (Mouse.current.rightButton.IsPressed())
        {
            float snapAngle = 10f;
            targetRotationY += eventData.delta.x * 0.5f;
            if (Mathf.Abs(targetRotationY) >= snapAngle)
            {
                //.Log("targetRotationY = " + targetRotationY);
                var Y = Mathf.Round(targetRotationY / snapAngle) * snapAngle;
                //Debug.Log("Y = " + Y);
                Quaternion newRotation = Quaternion.Euler(0, Y, 0) * placeableObject.transform.rotation;
                placeableObject.Relocate(transform.position, newRotation, 0f, false);
                targetRotationY = 0f;
            }
            //Quaternion newRotation = Quaternion.Euler(0, eventData.delta.x * 0.5f, 0) * placeableObject.transform.rotation;
            //placeableObject.Relocate(transform.position, newRotation, 0f, false);
        }
        else if (Mouse.current.leftButton.IsPressed())
        {
            //Debug.Log("Dragging");
            var screenPoint = eventData.pointerCurrentRaycast.screenPosition;
            var ray = eventData.enterEventCamera.ScreenPointToRay(screenPoint);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 200, Color.red, Time.deltaTime);
            // Raycast only on ground layer:
            if ( Physics.Raycast(ray, out RaycastHit hit, 200, layerMask, QueryTriggerInteraction.Ignore))
            {
                var hitPosition = hit.point;
                if (transform.parent != null)
                {
                    var localPosition = transform.parent.InverseTransformPoint(hitPosition);
                    localPosition = SnapPositionToGrid(localPosition, snapDistance);
                    Debug.Log($"HitPositionLocal: {localPosition}");
                    hitPosition = transform.parent.TransformPoint(localPosition);
                    //hitPosition = SnapPositionToGrid(hitPosition, transform.parent.lossyScale.x);
                    Debug.Log($"HitPosition: {hitPosition}");
                }
                else
                {
                    hitPosition = SnapPositionToGrid(hitPosition, snapDistance);
                }
                hitPosition.y = placeableObject.transform.position.y;

                if (!isDragging)
                {
                    isDragging = true;
                    dragStartWorldPos = hitPosition;
                }
                if (dragLimitBounds == null || dragLimitBounds.size.sqrMagnitude < 0.01f || dragLimitBounds.Contains(hitPosition))
                {
                    DrawBox(dragLimitBounds.center, Quaternion.identity, dragLimitBounds.size, Color.green, Time.deltaTime);
                    placeableObject.transform.position = hitPosition;
                    //placeableObject.Relocate(hitPosition, placeableObject.transform.rotation);
                }
                else
                {
                    DrawBox(dragLimitBounds.center, Quaternion.identity, dragLimitBounds.size, Color.red, Time.deltaTime);
                }
            }
        }
        
    }

    Vector3 SnapPositionToGrid(Vector3 originalPosition, float snapStep)
    {
        if (snapStep <= Mathf.Epsilon)
            return originalPosition;
        return new Vector3(
            Mathf.Round(originalPosition.x / snapStep) * snapStep,
            originalPosition.y, //Mathf.Round(originalPosition.y / snapDistance) * snapDistance,
            Mathf.Round(originalPosition.z / snapStep) * snapStep
        );
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
        isDragging = false;
        placeableObject.GetComponent<Collider>().enabled = true;
        placeableObject.Accommodate();
    }

    void OnEnable()
    {
        if (placeableObject == null)
            placeableObject = GetComponent<PlaceableObject>();
    }
}
