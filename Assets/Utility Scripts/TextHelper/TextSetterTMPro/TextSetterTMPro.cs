using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextSetterTMPro : MonoBehaviour, IText {

    public string textFormat = "{0}";

    public UnityEvent<string> onTextChanged;

    TextMeshProUGUI textComponent => GetComponent<TextMeshProUGUI>();
    public string text
    {
        get { return textComponent.text; }
        set
        {
            if (textComponent.text != value)
            {
                textComponent.text = value;
                onTextChanged.Invoke(textComponent.text);
            }
        }
    }

    public void SetTextValue(string value)
    {
        text = string.Format(@textFormat, value);
    }

    public void SetTextValue(float f)
    {
        text = (string.Format(@textFormat, f));
    }

    public void SetTextValue(int v)
    {
        text = (string.Format(@textFormat, v));
    }

    public void SetTextValue(UInt16 v)
    {
        text = (string.Format(@textFormat, v));
    }

    public void SetTextValue(UInt32 v)
    {
        text = (string.Format(@textFormat, v));
    }

    public void SetTextValue(UInt64 v)
    {
        text = (string.Format(@textFormat, v));
    }

    public void SetTextValue(Vector2 v)
    {
        text = (string.Format(@textFormat, v));
    }

    public void SetTextValue(Vector2Int v)
    {
        text = (string.Format(@textFormat, v));
    }

    public void SetTextValue(Vector3 v)
    {
        text = (string.Format(@textFormat, v));
    }

    public void SetTextValue(Vector3Int v)
    {
        text = (string.Format(@textFormat, v));
    }

    public void SetTextValue(Vector4 v)
    {
        text = (string.Format(@textFormat, v));
    }

    public void SetTextValue(bool v)
    {
        text = (string.Format(@textFormat, v));
    }

    public void SetTextValue(long v)
    {
        text = (string.Format(@textFormat, v));
    }

    public void SetTextValue(double v)
    {
        text = (string.Format(@textFormat, v));
    }

    public void SetTextValue(UnityEngine.Object v)
    {
        text = (string.Format(@textFormat, v));
    }

    public void SetTextValueFromTimespan(double timeSpanInSeconds)
    {
        //text = TimeSpan.FromSeconds(timeSpanInSeconds).ToString(@textFormat);
        text = (string.Format(@textFormat, TimeSpan.FromSeconds(timeSpanInSeconds)));

    }
    public void SetTextValueFromTimespan(float timeSpanInSeconds)
    {
        SetTextValueFromTimespan((double)timeSpanInSeconds);
    }

    public void CopyText(TextMeshProUGUI reference)
    {
        text = reference.text;
    }
}
