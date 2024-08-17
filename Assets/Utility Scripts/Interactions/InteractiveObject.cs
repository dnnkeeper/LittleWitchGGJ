using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class InteractiveObject : MonoBehaviour
{
    public UnityEvent<bool> onInteraction;
    public UnityEvent<InteractionManager> onInteractionInstigator;
    public UnityEvent<bool> onInteractionAvailable;


    [SerializeField] bool Switcheable = true;
    bool switchOn;

    [SerializeField] bool _canBeUsed = true;
    public bool canBeUsed => _canBeUsed && (Switcheable || !switchOn);

    private void OnValidate()
    {
        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        var interactionManager = other.gameObject.GetComponent<InteractionManager>();
        if (interactionManager != null)
        {
            interactionManager.RegisterInteractionPossibility(this);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        var interactionManager = other.gameObject.GetComponent<InteractionManager>();
        if (interactionManager != null)
        {
            interactionManager.RemoveInteractionPossibility(this);
        }
    }

    public void SetCanBeUsed(bool val)
    {
        _canBeUsed = val;
    }


    [ContextMenu("Interact")]
    void Interact()
    {
        Interact(null);
    }

    public void Interact(InteractionManager instigator)
    {
        if (instigator != null)
        {
            onInteractionInstigator.Invoke(instigator);
            Debug.Log(instigator + " interacted with " + this, this);
        }

        if (Switcheable)
            Switch(!switchOn);
        else
            Switch(true);
    }

    public void OnInteractionAvailable(bool b)
    {
        onInteractionAvailable.Invoke(canBeUsed && b);
    }

    public void Switch(bool isOn)
    {
        if (switchOn != isOn)
        {
            switchOn = isOn;
            onInteraction.Invoke(switchOn);
        }
    }
}
