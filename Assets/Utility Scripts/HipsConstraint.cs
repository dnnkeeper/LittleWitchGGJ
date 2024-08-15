using UnityEngine;

public class HipsConstraint : MonoBehaviour
{
    //public Transform hipsBone;
    public Transform headTransform, hipsDesiredTransform;
    public float height;
    public float weight = 1f;
    Vector3 initialPosition;

    // private void Awake() {
    //     transform.position = hipsBone.position;
    // }
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.localPosition;
        if (height <= 0f)
            MeasureHeight();
    }

    private void OnEnable()
    {
        ResetPosition();
    }

    private void OnDisable()
    {
        ResetPosition();
    }

    public void ResetPosition()
    {
        transform.localPosition = initialPosition;
    }

    [ContextMenu("Measure Height")]
    public void MeasureHeight()
    {
        height = (transform.position - headTransform.position).magnitude;

    }

    // Update is called once per frame
    void Update()
    {
        var hipsDesiredPosition = hipsDesiredTransform.position + Vector3.Project(headTransform.position - hipsDesiredTransform.position, hipsDesiredTransform.right);

        hipsDesiredPosition = Vector3.Lerp(hipsDesiredTransform.position, hipsDesiredPosition, weight);

        transform.position = headTransform.position + (hipsDesiredPosition - headTransform.position).normalized * height;
        Debug.DrawLine(hipsDesiredTransform.position - hipsDesiredTransform.right, hipsDesiredTransform.position + hipsDesiredTransform.right, Color.magenta);
        Debug.DrawLine(headTransform.position, hipsDesiredPosition, Color.red);

    }
}
