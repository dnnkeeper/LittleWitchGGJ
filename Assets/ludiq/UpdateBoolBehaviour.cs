using Ludiq.Reflection;
using UnityEngine;
using UnityEngine.Events;
public class UpdateBoolBehaviour : MonoBehaviour
{

    [Filter(typeof(bool), Methods = true, Fields = true, Properties = true, Gettable = true, Inherited = true)]
    public UnityMember member;
    public UnityEvent<bool> onUpdate;

    public UpdateMode updateMode = UpdateMode.Update;

    void PerformUpdate()
    {
        if (member.isAssigned)
            onUpdate.Invoke(member.GetOrInvoke<bool>());
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
