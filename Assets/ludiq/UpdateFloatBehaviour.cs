using Ludiq.Reflection;
using UnityEngine;
using UnityEngine.Events;
public class UpdateFloatBehaviour : MonoBehaviour
{

    [Filter(typeof(float), Methods = true, Fields = true, Properties = true, Gettable = true, Inherited = true)]
    public UnityMember member;
    public UnityEvent<float> onUpdate;

    public UpdateMode updateMode = UpdateMode.Update;

    void PerformUpdate()
    {
        if (member.isAssigned)
            onUpdate.Invoke(member.GetOrInvoke<float>());
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
