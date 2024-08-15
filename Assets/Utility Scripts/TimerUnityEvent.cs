using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class TimerUnityEvent : MonoBehaviour
{
    public float defaultTimerDuration = 5f;
    public UnityEvent onTimerStart;
    public UnityEvent<float> onTimerProgress;
    public UnityEvent onTimerEnd;
    public bool startOnEnable = true;
    private IEnumerator coroutine;

    private void OnEnable()
    {
        if (startOnEnable)
            StartTimer();
    }

    public void StartTimer()
    {
        onTimerStart.Invoke();
        coroutine = TimerRoutine(defaultTimerDuration);
        StartCoroutine(coroutine);
    }

    public void StopTimer() 
    { 
        if (coroutine != null)
            StopCoroutine(coroutine);
    }

    IEnumerator TimerRoutine(float duration)
    {
        float targetTime = duration;
        float t = 0;
        while (t < targetTime)
        {
            onTimerProgress.Invoke(t / targetTime);
            t += Time.deltaTime;
            yield return null;
        }
        onTimerProgress.Invoke(1f);
        onTimerEnd.Invoke();
    }
}
