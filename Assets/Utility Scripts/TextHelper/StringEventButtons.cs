using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class StringEventButtons : MonoBehaviour
{
    public Button buttonPrefab;
    public UnityEvent<string> OnButtonClickEvent;
    List<GameObject> buttonList = new List<GameObject>();
    public void AddButton(string buttonName)
    {
        var newBtn = GameObject.Instantiate(buttonPrefab, transform);
        newBtn.gameObject.SetActive(true);
        buttonList.Add(newBtn.gameObject);
        var ITextSetter = newBtn.GetComponentInChildren<IText>();
        if (ITextSetter != null)
            ITextSetter.text = buttonName;
        newBtn.onClick.AddListener(() => { OnButtonClickEvent.Invoke(buttonName); });
    }

    public void ClearButtons()
    {
        foreach (var btn in buttonList)
        {
            Destroy(btn);
        }
        buttonList.Clear();
    }
}
