using Dexart.Scripts.ObjectsPlacement;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlaceableObject))]
public class DraggableGameObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private PlaceableObject placeableObject;
    public void OnBeginDrag(PointerEventData eventData)
    {
        placeableObject = GetComponent<PlaceableObject>();
        placeableObject.Relocate(eventData.pointerCurrentRaycast.worldPosition, placeableObject.transform.rotation);
    }

    public void OnDrag(PointerEventData eventData)
    {
        placeableObject = GetComponent<PlaceableObject>();
        placeableObject.Relocate(eventData.pointerCurrentRaycast.worldPosition, placeableObject.transform.rotation);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        placeableObject = GetComponent<PlaceableObject>();
        placeableObject.Accommodate();
    }
}
