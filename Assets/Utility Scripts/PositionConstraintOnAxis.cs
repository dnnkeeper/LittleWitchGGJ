using UnityEngine;

public class PositionConstraintOnAxis : MonoBehaviour
{
    public Transform sourceTransform;
    public Vector3 axis = Vector3.up;

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + Vector3.Project(sourceTransform.position - transform.position, axis);
    }
}
