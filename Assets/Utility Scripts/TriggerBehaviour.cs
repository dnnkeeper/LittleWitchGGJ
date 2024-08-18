using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TriggerBehaviour : MonoBehaviour
{
    public string compareTag = "Player";
    [FormerlySerializedAs("onCollected")]
    public UnityEvent<GameObject> onTriggered;
    public string message = "OnCollectedItem";
    public bool debugLog;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(compareTag))
        {
            other.SendMessage(message, gameObject, SendMessageOptions.DontRequireReceiver);
            onTriggered.Invoke(gameObject);
            if (debugLog)
            {
                Debug.Log("Triggered by " + other.name);
            }
        }
    }
}
