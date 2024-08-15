using Ludiq.Reflection;
using UnityEngine;

public class ExposedMemberSetter : MonoBehaviour
{
    [Filter(NonPublic = true, Setters = true, Fields = true, Properties = true, Settable = true, Inherited = true)]
    public UnityMember member;

    public void Set(object t)
    {
        member.InvokeOrSet(t);
    }
}
