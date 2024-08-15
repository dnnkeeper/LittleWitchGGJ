using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasRenderer))]
public class FadeInOnEnableCanvas : MonoBehaviour
{

    public float duration = 1f;

    public float durationStay = 1f;

    public float durationOut = 1f;

    float defaultAlpha;

    public bool fadeOut;

    CanvasRenderer rend;

    void OnEnable()
    {
        rend = GetComponent<CanvasRenderer>();
        defaultAlpha = rend.GetAlpha();
        if (enabled)
        {
            if (duration > 0f)
            {
                rend.SetAlpha(fadeOut ? 1f : 0f);
                StopAllCoroutines();
                StartCoroutine(CrossFadeAlpha(fadeOut ? 0 : 1f, duration));
            }
            else
            {
                rend.SetAlpha(1f);
            }
        }
    }

    void OnDisable()
    {
        GetComponent<CanvasRenderer>().SetAlpha(1f);
    }

    public void FadeInAndOut()
    {
        gameObject.SetActive(true);
        Invoke("FadeOutAndDisable", duration + durationStay);
    }

    public void FadeInAndFadeOut()
    {
        gameObject.SetActive(true);
        Invoke("FadeOut", duration + durationStay);
    }
    /*
    public void EnableFadeOutAndDisable()
    {
        gameObject.SetActive(true);
        enabled = false;
        GetComponent<Image>().canvasRenderer.SetAlpha(1f);
        Invoke("FadeOutAndDisable", duration + durationStay);
    }*/

    public void FadeOutAndDisable()
    {
        rend.SetAlpha(1f);
        StopAllCoroutines();
        StartCoroutine(CrossFadeAlpha(0f, duration));
        Invoke("Disable", durationOut);
    }

    public void FadeOut()
    {
        rend.SetAlpha(1f);
        StopAllCoroutines();
        StartCoroutine(CrossFadeAlpha(0f, duration));
    }

    public float progress;

    public float alpha;

    public UnityEvent onFinished;

    IEnumerator CrossFadeAlpha(float value, float dur)
    {
        float t = 0f;

        CanvasRenderer rend = GetComponent<CanvasRenderer>();
        CanvasGroup groupRend = GetComponent<CanvasGroup>();

        float startA = rend.GetAlpha();

        while (t <= dur)
        {
            t += Time.deltaTime;
            progress = t / dur;
            alpha = Mathf.Lerp(startA, value, progress);
            rend.SetAlpha(alpha);
            if (groupRend != null)
            {
                groupRend.alpha = alpha;
            }
            yield return null;
        }

        rend.SetAlpha(value);

        onFinished.Invoke();

        //Debug.Log("CrossFadeAlpha complete "+value);

        //rend.SetAlpha(value);
    }

    void Disable()
    {
        gameObject.SetActive(false);
        //enabled = true;
    }
}
