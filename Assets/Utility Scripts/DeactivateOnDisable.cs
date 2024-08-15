using UnityEngine;

public class DeactivateOnDisable : MonoBehaviour
{

    void OnDisable()
    {
        //Debug.Log("OnDisable");
        gameObject.SetActive(false);
    }
}
