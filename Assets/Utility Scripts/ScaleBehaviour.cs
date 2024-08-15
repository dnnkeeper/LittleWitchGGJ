using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class ScaleBehaviour : MonoBehaviour
{
    public UnityEvent onScaleStart;
    public UnityEvent onScaleFinish;

    public AnimationCurve easingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public Vector3 initialScale = Vector3.zero;
    Vector3 originalLocalScale, currentScale;
    public float tweenTime = 1f;

    public bool playOnEnable = true;
    void Awake()
    {
        originalLocalScale = transform.localScale;
        currentScale = originalLocalScale;
    }

    private void OnEnable()
    {
        if (playOnEnable)
            Play();
    }
    void Update()
    {
        transform.localScale = currentScale;
    }

    [ContextMenu("Play")]
    public void Play()
    {
        if (!isActiveAndEnabled)
            return;
        //Debug.Log("Play ScaleBehaviour");
        transform.localScale = initialScale;
        StartCoroutine(Scale_Routine(transform.gameObject, originalLocalScale, tweenTime));
    }

    IEnumerator Scale_Routine(GameObject target, Vector3 targetScale, float targetTime)
    {
        float t = 0;
        Vector3 startScale = target.transform.localScale;

        onScaleStart.Invoke();

        while (t < targetTime)
        {
            currentScale = Vector3.LerpUnclamped(startScale, targetScale, easingCurve.Evaluate(t / targetTime));
            t += Time.deltaTime;
            //Debug.Log((t/targetTime).ToString("0.0000"));
            yield return null;
        }

        currentScale = targetScale;
        onScaleFinish.Invoke();
    }

    void OnDisable()
    {
        StopAllCoroutines();
        transform.localScale = originalLocalScale;
    }
}
