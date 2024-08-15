using UnityEngine;

public class SetRotation : MonoBehaviour
{
    public Quaternion rotation = Quaternion.identity;
    // Update is called once per frame
    void Update()
    {
        transform.rotation = rotation;
    }

    void LateUpdate(){
        transform.rotation = rotation;
    }
}
