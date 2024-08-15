using Ludiq.Reflection;
using UnityEngine;
using UnityEngine.Events;
public class UpdateVectorBehaviour : MonoBehaviour
{

    [Filter(typeof(Vector3), Methods = true, Fields = true, Properties = true, Gettable = true, Inherited = true)]
    public UnityMember member;
    public UnityEvent<Vector3> onUpdate;
    public UpdateMode updateMode = UpdateMode.Update;

    void PerformUpdate()
    {
        if (member.isAssigned)
            onUpdate.Invoke(member.GetOrInvoke<Vector3>());
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
