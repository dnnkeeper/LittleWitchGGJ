using UnityEngine;

public class TargetLookRotationY : MonoBehaviour
{
    public bool findMainCamera;

    public Transform target;

    public bool lookAway;

    private void Start()
    {
        if (findMainCamera && target == null)
        {
            target = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        Quaternion temp = Quaternion.LookRotation((lookAway ? -1f : 1f) * (target.position - transform.position).normalized, target.up);
        transform.SetPositionAndRotation(transform.position, Quaternion.Euler(new Vector3(transform.eulerAngles.x, temp.eulerAngles.y, transform.eulerAngles.z)));
    }
}
