using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DateTimeStruct
{
    public int year, month, day, hour, minute, second, millisecond;
    public DateTime dateTime
    {
        get { return new DateTime(year, month, day, hour, minute, second, millisecond); }
    }
    public DateTimeStruct(DateTime dateTime)
    {
        this.millisecond = dateTime.Millisecond;
        this.second = dateTime.Second;
        this.minute = dateTime.Minute;
        this.hour = dateTime.Hour;
        this.day = dateTime.Day;
        this.month = dateTime.Month;
        this.year = dateTime.Year;
    }
}

public class DateTimerBehaviour : MonoBehaviour
{
    DateTime targetTime;
    [SerializeField] DateTimeStruct targetTimeStruct;
    public UnityEvent<double> onRemainingTimeSeconds;
    public UnityEvent onTimeHasCome;

    private void Awake()
    {
        targetTime = new DateTime(targetTimeStruct.year, targetTimeStruct.month, targetTimeStruct.day, targetTimeStruct.hour, targetTimeStruct.minute, targetTimeStruct.second, targetTimeStruct.millisecond);
    }

    public void SetTargetTime(DateTimeStruct dateTimeStruct)
    {
        targetTime = dateTimeStruct.dateTime;
        targetTimeStruct = dateTimeStruct;
        Debug.Log($"[DateTimeBehaviour] Timer set to {targetTime}", this);
    }

    public void SetTargetTime(DateTime target)
    {
        SetTargetTime(target.Year, target.Month, target.Day, target.Hour, target.Minute, target.Second + 5, target.Millisecond);
    }

    public void SetTargetTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
    {
        targetTimeStruct.year = year;
        targetTimeStruct.month = month;
        targetTimeStruct.day = day;
        targetTimeStruct.hour = hour;
        targetTimeStruct.minute = minute;
        targetTimeStruct.second = second;
        targetTimeStruct.millisecond = millisecond;
        SetTargetTime(targetTimeStruct);
    }

    public void SetTargetTime(string DateTimeJson)
    {
        DateTimeStruct targetTime = JsonUtility.FromJson<DateTimeStruct>(DateTimeJson);
        SetTargetTime(targetTime);
    }

    [ContextMenu("Set Test Time 5s")]
    public void SetTestTime()
    {
        DateTime now = DateTime.UtcNow;
        SetTargetTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second + 5, now.Millisecond);
    }

    [ContextMenu("Log Target Time as Json")]
    void LogTargetJsonTime()
    {
        Debug.Log(JsonUtility.ToJson(new DateTimeStruct(targetTimeStruct.dateTime)));
    }

    // Update is called once per frame
    void Update()
    {
        if (DateTime.UtcNow < targetTime)
        {
            onRemainingTimeSeconds.Invoke((targetTime - DateTime.UtcNow).TotalSeconds);
        }
        else
        {
            onTimeHasCome.Invoke();
            enabled = false;
        }
    }
}
