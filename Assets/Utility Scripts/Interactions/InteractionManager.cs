using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class InteractionManager : MonoBehaviour
{
    public UnityEvent<GameObject> onObjectInteractionAvailable;
    public UnityEvent onNoObjectInteractionAvailable;
    public UnityEvent<GameObject> onObjectInteractionHappened;


    public List<InteractiveObject> interactiveObjects = new List<InteractiveObject>();

    public Transform viewTransform;
    public float interactionViewAngle = 45f;

    public float maximumInteractionDistance = 10f;

    InteractiveObject _closestObject;
    InteractiveObject closestObject
    {
        get { return _closestObject; }
        set
        {
            if (value != _closestObject)
            {
                _closestObject = value;
                if (_closestObject != null)
                {
                    //Debug.Log("onObjectInteractionAvailable with " + _closestObject);
                    onObjectInteractionAvailable.Invoke(_closestObject.gameObject);
                }
                else
                {
                    //Debug.Log("onNoObjectInteractionAvailable out of " + interactiveObjects.Count);
                    onNoObjectInteractionAvailable.Invoke();
                }
            }
        }
    }

    public void RegisterInteractionPossibility(InteractiveObject targetObject)
    {
        interactiveObjects.RemoveAll(item => item == null);

        if (targetObject != null && !interactiveObjects.Contains(targetObject))
            interactiveObjects.Add(targetObject);
    }

    public void RemoveInteractionPossibility(InteractiveObject targetObject)
    {
        if (targetObject != null && interactiveObjects.Contains(targetObject))
            interactiveObjects.Remove(targetObject);
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    public void SetDisabled(bool disabled)
    {
        enabled = !disabled;
    }
    private void OnDisable()
    {
        closestObject = null;
    }
    private void OnEnable()
    {

    }

    public void InteractWithClosestObject()
    {
        if (closestObject != null)
        {
            closestObject.Interact(this);
            onObjectInteractionHappened.Invoke(closestObject.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        InteractiveObject newClosestObject = null;
        float closestInteractiveDistance = Mathf.Infinity;
        if (viewTransform == null)
        {
            viewTransform = transform;
        }

        foreach (var targetObject in interactiveObjects)
        {
            if (targetObject == null)
            {
                continue;
            }
            if (!targetObject.isActiveAndEnabled)
            {
                continue;
            }
            var fromMeToTarget = (targetObject.transform.position - viewTransform.position);
            var d = fromMeToTarget.sqrMagnitude;

            if (d > maximumInteractionDistance * maximumInteractionDistance)
            {
                RemoveInteractionPossibility(targetObject);
                return;
            }

            //Debug.Log($"angle {Vector3.Angle(viewTransform.forward, fromMeToTarget)}");
            if (d < closestInteractiveDistance && targetObject.canBeUsed && Vector3.Angle(viewTransform.forward, fromMeToTarget) <= interactionViewAngle)
            {
                newClosestObject = targetObject;
                closestInteractiveDistance = d;
                Debug.DrawLine(viewTransform.position, targetObject.transform.position, Color.red);
            }
            else
            {
                Debug.DrawLine(viewTransform.position, targetObject.transform.position, Color.blue);
            }
        }

        if (newClosestObject != null)
        {
            if (closestObject != null)
            {
                closestObject.OnInteractionAvailable(false);
            }
            closestObject = newClosestObject;
            //onObjectInteractionAvailable.Invoke(closestObject.gameObject);
        }
        else
        {
            if (closestObject != null)
            {
                closestObject.OnInteractionAvailable(false);
            }
            closestObject = null;
            //onNoObjectInteractionAvailable.Invoke();
        }

        if (closestObject != null && closestObject.canBeUsed)
        {
            closestObject.OnInteractionAvailable(true);
        }
    }
}
