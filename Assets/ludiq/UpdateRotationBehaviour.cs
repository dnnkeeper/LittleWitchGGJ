using Ludiq.Reflection;
using UnityEngine;
using UnityEngine.Events;


public enum UpdateMode
{
    Update,
    FixedUpdate,
    LateUpdate,
    None
}

public class UpdateRotationBehaviour : MonoBehaviour
{

    [Filter(typeof(Quaternion), Methods = true, Fields = true, Properties = true, Gettable = true, Inherited = true)]
    public UnityMember member;
    public UnityEvent<Quaternion> onUpdate;

    public UpdateMode updateMode = UpdateMode.Update;

    [ContextMenu("Log Info")]
    void LogInfo()
    {
        Debug.Log(member.type);
        if (member.fieldInfo != null)
        {
            Debug.Log(member.fieldInfo + " MemberType:" + member.fieldInfo.MemberType);
        }
        if (member.methodInfo != null)
        {
            Debug.Log(member.methodInfo + " MemberType:" + member.methodInfo.MemberType);
        }
        if (member.propertyInfo != null)
        {
            Debug.Log(member.propertyInfo + " MemberType:" + member.propertyInfo.MemberType);
        }
    }

    public void PerformUpdate()
    {
        if (member.isAssigned)
        {
            onUpdate.Invoke(member.GetOrInvoke<Quaternion>());
        }
    }

    void Update()
    {
        if (updateMode != UpdateMode.Update)
            return;

        PerformUpdate();
    }

    void FixedUpdate()
    {
        if (updateMode != UpdateMode.FixedUpdate)
            return;

        PerformUpdate();
    }

    void LateUpdate()
    {
        if (updateMode != UpdateMode.LateUpdate)
            return;

        PerformUpdate();
    }
}
