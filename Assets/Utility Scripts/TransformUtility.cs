using System.Collections;
using UnityEngine;

public class TransformUtility : MonoBehaviour
{
    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    public void SetInactive(bool inactive)
    {
        gameObject.SetActive(!inactive);
    }
    public void Adopt(GameObject target)
    {
        target.transform.SetParent(transform);
    }

    public void DestroyTarget(GameObject target)
    {
        GameObject.Destroy(target);
    }

    public void Destroy(GameObject target)
    {
        GameObject.Destroy(target);
    }

    public void DestroySelf()
    {
        GameObject.Destroy(gameObject);
    }
    public void DestroySelf(float delay)
    {
        GameObject.Destroy(gameObject, delay);
    }
    public void SetParentNull()
    {
        transform.SetParent(null);
    }

    public void SetParentToTargetParentPreservingPos(Transform target)
    {
        SetParentPreservingPos(target.parent);
    }

    public void SetParentPreservingPos(Transform target)
    {
        var pos = transform.position;

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            pos = rb.position;
        }

        transform.SetParent(target, true);

        transform.position = pos;

        if (rb != null)
        {
            rb.position = pos;
        }
    }

    public void CopyPosition(Transform source)
    {
        transform.position = source.position;
    }

    public void CopyRotation(Transform source)
    {
        transform.rotation = source.rotation;
    }

    public void SetLocalScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }

    public void ScaleAround(Vector3 pivot, Vector3 newScale)
    {
        Vector3 A = transform.localPosition;
        Vector3 B = pivot;

        Vector3 C = A - B; // diff from object pivot to desired pivot/origin

        float RS = newScale.x / transform.localScale.x; // relative scale factor

        // calc final position post-scale
        Vector3 FP = B + C * RS;

        // finally, actually perform the scale/translation
        transform.localScale = newScale;
        transform.localPosition = FP;
    }

    public void ActivateChildWithName(string sName)
    {
        var child = transform.Find(sName);
        if (child != null)
        {
            child.gameObject.SetActive(true);
            lastEnabledChild = child;
        }
    }

    public void DeactivateChildWithName(string sName)
    {
        var child = transform.Find(sName);
        if (child != null)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void EnableOnlyThisChild()
    {
        foreach (Transform child in transform.parent)
        {
            if (child != transform)
                child.gameObject.SetActive(false);
        }
        transform.gameObject.SetActive(true);
    }

    public void DisableChildren()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void EnableOnlyChildN(int num)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            if (i == num)
            {
                var child = transform.GetChild(i);
                child.gameObject.SetActive(false);
                child.gameObject.SetActive(true);
                lastEnabledChild = child;

            }
            else
                transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void EnableChildN(int num)
    {
        var child = transform.GetChild(num);
        if (child == null) return;
        lastEnabledChild = child;
        child.gameObject.SetActive(true);
        //StartCoroutine(DeactivateAfterDelay(childVar, 3f));
    }
    Transform lastEnabledChild;
    public void DisableLastActiveChildAfterDelay(float delay)
    {
        if (lastEnabledChild != null)
        {
            var childVar = lastEnabledChild.gameObject;
            StartCoroutine(DeactivateAfterDelay(childVar, 3f));
        }
    }
    private static IEnumerator DeactivateAfterDelay(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    public void ResetLocalTransform()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

}
