using UnityEngine;
using UnityEngine.Events;

namespace Utility.CommonUtils
{
    public class CollisionEventTrigger : MonoBehaviour
    {
        public string triggerTag = "Player";

        public float collisionThresold = 0f;

        public UnityEvent<GameObject> onTriggerEnter;

        public UnityEvent<GameObject> onTriggerExit;

        public UnityEvent<GameObject> onCollisionEnter;

        public UnityEvent<GameObject> onCollisionExit;

        public UnityEvent<GameObject> onParticlesHit;

        public string messageToCollider;

        public bool debugLog;

        public void OnTriggerEnter(Collider other)
        {
            if (string.IsNullOrEmpty(triggerTag) || other.CompareTag(triggerTag))
            {
                onTriggerEnter.Invoke(other.gameObject);
                if (debugLog)
                    Debug.Log(other.gameObject.name + " entered to " + gameObject.name, this);

                if (!string.IsNullOrEmpty(messageToCollider))
                {
                    other.SendMessage(messageToCollider + "Enter", gameObject, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (string.IsNullOrEmpty(triggerTag) || other.CompareTag(triggerTag))
            {
                onTriggerExit.Invoke(other.gameObject);

                if (debugLog)
                    Debug.Log(other.gameObject.name + " exited from " + gameObject.name, this);

                if (!string.IsNullOrEmpty(messageToCollider))
                {
                    other.SendMessage(messageToCollider + "Exit", gameObject, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.impulse.magnitude < collisionThresold)
                return;

            if (string.IsNullOrEmpty(triggerTag) || (collision.rigidbody != null && collision.rigidbody.CompareTag(triggerTag)))
            {
                onCollisionEnter.Invoke(collision.rigidbody != null ? collision.rigidbody.gameObject : collision.collider.gameObject);

                if (debugLog)
                    Debug.Log(collision.rigidbody.name + " entered to " + gameObject.name, this);

                if (!string.IsNullOrEmpty(messageToCollider))
                {
                    var messageTarget = collision.rigidbody != null ? collision.rigidbody.gameObject : collision.gameObject;
                    messageTarget.SendMessage(messageToCollider + "Enter", gameObject, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        public void OnCollisionExit(Collision collision)
        {
            if (string.IsNullOrEmpty(triggerTag) || (collision.rigidbody != null && collision.rigidbody.CompareTag(triggerTag)))
            {
                onCollisionExit.Invoke(collision.rigidbody != null ? collision.rigidbody.gameObject : collision.collider.gameObject);

                if (debugLog)
                    Debug.Log(collision.rigidbody.name + " exited from " + gameObject.name, this);

                if (!string.IsNullOrEmpty(messageToCollider))
                {
                    var messageTarget = collision.rigidbody != null ? collision.rigidbody.gameObject : collision.gameObject;
                    messageTarget.SendMessage(messageToCollider + "Exit", gameObject, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        void OnParticleHit(ParticleSystem particlesSystem)
        {
            if (debugLog)
                Debug.Log(gameObject.name + " Hit by particles from " + particlesSystem.gameObject.name, this);
            onParticlesHit.Invoke(particlesSystem.gameObject);
        }
    }
}