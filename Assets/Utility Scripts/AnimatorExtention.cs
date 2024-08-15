using System.Collections.Generic;
using UnityEngine;

public class AnimatorExtention : MonoBehaviour
{
    Animator animator;

    public bool rebindOnChildrenChange = true;
    protected void Start()
    {
        animator = GetComponent<Animator>();
        OnTransformChildrenChanged();
    }


    HashSet<Transform> children = new HashSet<Transform>();

    struct PosRot
    {
        public Vector3 pos;
        public Quaternion rot;
    }
    private void OnTransformChildrenChanged()
    {
        List<Transform> addedChildren = new List<Transform>();
        foreach (Transform t in transform)
        {
            if (!children.Contains(t))
            {
                children.Add(t);
                addedChildren.Add(t);
            }
        }

        List<Transform> removedChildren = new List<Transform>();
        foreach (Transform t in children)
        {
            if (t.parent != transform)
            {
                removedChildren.Add(t);
            }
        }

        foreach (var c in removedChildren)
        {
            children.Remove(c);
        }


        // foreach (Transform item in addedChildren)
        // {

        // }

        if (rebindOnChildrenChange)
        {
            Dictionary<Transform, PosRot> childrenPosRots = new Dictionary<Transform, PosRot>();
            foreach (Transform removedItem in removedChildren)
            {
                PosRot posRot = new PosRot();
                posRot.pos = removedItem.position;
                posRot.rot = removedItem.rotation;
                childrenPosRots.Add(removedItem, posRot);
            }

            //Debug.Log("Rebind animator", animator);
            animator.Rebind();

            foreach (KeyValuePair<Transform, PosRot> kvp in childrenPosRots)
            {
                //Debug.Log("Restore position and rotation of "+kvp.Key);
                kvp.Key.SetPositionAndRotation(kvp.Value.pos, kvp.Value.rot);
            }
        }
    }

    public void SetBoolTrue(string paramName)
    {
        if (animator != null)
            animator.SetBool(paramName, true);
    }

    public void SetBoolFalse(string paramName)
    {
        if (animator != null)
            animator.SetBool(paramName, false);
    }

    public void ToggleBool(string paramName)
    {
        if (animator != null)
            animator.SetBool(paramName, !animator.GetBool(paramName));
    }


}

public static class AnimatorExtentions
{
    public static void SetBoolTrue(this Animator animator, string paramName)
    {
        animator.SetBool(paramName, true);
    }

    public static void SetBoolFalse(this Animator animator, string paramName)
    {
        animator.SetBool(paramName, false);
    }
}
