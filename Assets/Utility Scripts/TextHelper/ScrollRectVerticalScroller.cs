using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(ScrollRect))]
public class ScrollRectVerticalScroller : MonoBehaviour
{
    ScrollRect scrollRect;
    public float lerpSpeed = 10f;
    public float timeLag = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scrollRect = GetComponent<ScrollRect> ();
    }
    public void ScrollToBottom()
    {
        StopAllCoroutines();
        StartCoroutine(nameof(ScrollToBottom_Routine));
    }

    IEnumerator ScrollToBottom_Routine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < timeLag)
        {
            if ( Mathf.Abs(scrollRect.verticalNormalizedPosition) > 0.01f)
            {
                elapsedTime = 0;
            }
            else
            {
                elapsedTime += Time.deltaTime;
            }
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, 0, lerpSpeed*Time.deltaTime);
            yield return null;
        }
        //Debug.Log("Scrolled to the bottom", this);
    }
}
