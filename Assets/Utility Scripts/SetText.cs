using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class SetText : MonoBehaviour
{

    public string textFormat = "{0}";

    public UnityEvent<string> onTextSet;

    Text textComponent
    {
        get { return GetComponent<Text>(); }
    }

    public void SetTextValue(string text)
    {
        var newText = string.Format(textFormat, text);
        if (newText != textComponent.text)
        {
            textComponent.text = newText;
            onTextSet.Invoke(textComponent.text);
        }
    }

    public void SetTextValue(float f)
    {
        SetTextValue(f.ToString());
    }

    public void SetTextValue(int v)
    {
        SetTextValue(v.ToString());
    }
}
