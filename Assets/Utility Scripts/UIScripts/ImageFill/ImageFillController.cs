using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageFillController : MonoBehaviour
{
    Image image;

    Coroutine holdRoutine;

    public float timeToActivate = 2f;

    public UnityEvent onStart;

    public UnityEvent onBreak;

    public UnityEvent onComplete;

    float fillAmount;

    public void StartFillRoutine()
    {
        if (holdRoutine != null)
        {
            //StopCoroutine(holdRoutine);
            return;
        }
        onStart.Invoke();
        holdRoutine = StartCoroutine(OnProgress(timeToActivate, image.fillAmount * timeToActivate, progress =>
        {
            fillAmount = progress;
            if (progress >= 1f)
                onComplete.Invoke();
        }));
    }


    public void StopFillRoutine()
    {
        if (holdRoutine != null)
        {
            StopCoroutine(holdRoutine);
            holdRoutine = null;
            onBreak.Invoke();
            fillAmount = 0f;
        }
    }

    IEnumerator OnProgress(float fillTime = 1f, float offset = 0f, System.Action<float> onProgress = null)
    {
        float elapsedTime = offset;
        while (elapsedTime <= fillTime)
        {
            yield return null;

            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / fillTime;

            if (onProgress != null)
                onProgress.Invoke(progress);
        }
    }

    // Use this for initialization
    void Start()
    {
        image = GetComponent<Image>();
    }

    public bool resetOnDisable;

    private void OnDisable()
    {
        if (resetOnDisable)
        {
            fillAmount = 0;
            if (image != null)
                image.fillAmount = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = (fillAmount == 0f) ? Mathf.Lerp(image.fillAmount, 0f, Time.deltaTime) : fillAmount;
    }
}
