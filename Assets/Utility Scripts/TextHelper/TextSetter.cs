using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//using TMPro;

public interface IText{
    public string text {get; set;}
}

[RequireComponent(typeof(Text))]
public class TextSetter : MonoBehaviour, IText {

    public string textFormat = "{0}";

    public UnityEvent<string> onTextChanged;

    Text textComponent => GetComponent<Text>();

    public string text{
        get{ return textComponent.text; }
        set {
            if (textComponent.text != value){
                textComponent.text = value;
                onTextChanged.Invoke(value);
            }
        }
    }
    
    private void OnValidate() {
        textComponent.text = textFormat;
    }
    
    public void SetTextValue(string value)
    {
        text = value;
    }

    public void SetTextValue(float f)
    {
        SetTextValue(string.Format(textFormat, f));
    }

    public void SetTextValue(int v)
    {
        SetTextValue(string.Format(textFormat, v));
    }

    [ContextMenu("TestFloatValue")]
    void TestFloatValue(){

        SetTextValue(0.35f);
    }

}
