using Ludiq.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class SetTextFromMember : MonoBehaviour
{
    [Filter(typeof(string), Methods = true, Fields = true, Gettable = true, Inherited = true, NonPublic = true)]
    public UnityMember getMember;

    public IText textTarget;

    public bool onStart, onUpdate, onEnable;


    private void Reset()
    {
        textTarget = GetComponent<IText>();
        if (textTarget == null)
        {
            if (GetComponent<Text>() != null)
                textTarget = gameObject.AddComponent<TextSetter>();
            else
                Debug.LogWarning("[SetTextFromMember] Please add proper component with IText interface", this);
        }
    }
    private void Awake()
    {
        if (textTarget == null)
            textTarget = GetComponent<IText>();
    }
    void Start()
    {
        if (onStart)
        {
            UpdateText();
        }

        if (!onEnable && !onUpdate)
        {
            enabled = false;
        }
    }


    private void OnEnable()
    {
        if (onEnable)
        {
            UpdateText();
        }
    }

    private void Update()
    {
        if (onUpdate)
        {
            UpdateText();
        }
    }

    public void UpdateText()
    {
        var newValue = getMember.GetOrInvoke<string>();
        textTarget.text = newValue;
    }
}