using Ludiq.Reflection;
using UnityEngine;
using UnityEngine.Events;
public class UpdateIntBehaviour : MonoBehaviour
{

    [Filter(typeof(int), Methods = true, Fields = true, Properties = true, Gettable = true, Inherited = true)]
    public UnityMember member;
    public UnityEvent<int> onUpdate;

    public UpdateMode updateMode = UpdateMode.Update;

    public void PerformUpdate()
    {
        if (member.isAssigned)
            onUpdate.Invoke(member.GetOrInvoke<int>());
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
