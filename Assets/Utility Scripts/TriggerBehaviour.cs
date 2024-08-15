using UnityEngine;
using UnityEngine.Events;

public class TriggerBehaviour : MonoBehaviour
{
    public string compareTag = "Player";
    public UnityEvent<GameObject> onCollected;
    public string message = "OnCollectedItem";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(compareTag))
        {
            other.SendMessage(message, gameObject, SendMessageOptions.DontRequireReceiver);
            onCollected.Invoke(gameObject);
        }
    }
}
