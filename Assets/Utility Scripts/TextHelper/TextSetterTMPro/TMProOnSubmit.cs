using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(TMP_InputField))]
public class TMProOnSubmit : MonoBehaviour
{
    public UnityEvent<string> onSubmit;
    public UnityEvent onAfterSubmit;


    void OnEnable(){
        GetComponent<TMP_InputField>().onSubmit.AddListener(RaiseOnSubmitEvent);
    }

    void OnDisable()
    {
        GetComponent<TMP_InputField>().onSubmit.RemoveListener(RaiseOnSubmitEvent);
    }
    public void RaiseOnSubmitEvent()
    {
        RaiseOnSubmitEvent(GetComponent<TMP_InputField>().text);
    }

    void RaiseOnSubmitEvent(string s){
        onSubmit.Invoke(s);
        onAfterSubmit.Invoke();
    }
}
