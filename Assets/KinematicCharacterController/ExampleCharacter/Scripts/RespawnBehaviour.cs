using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RespawnBehaviour : MonoBehaviour
{
    Vector3 spawnPosition;
    Quaternion spawnRotation;
    public UnityEvent onRespawned;
    // Start is called before the first frame update
    void Start()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
    }

    void Respawn()
    {
        Debug.Log("Respawning " + gameObject.name+ "at "+spawnPosition, this);
        gameObject.SetActive(false);
        var controller = GetComponent<KinematicCharacterMotor>();
        controller.SetPosition(spawnPosition);
        controller.SetRotation(spawnRotation);
        gameObject.SetActive(true);
    }

    void OnRespawnTriggered(GameObject other)
    {
        Respawn();
        onRespawned.Invoke();
    }
}
