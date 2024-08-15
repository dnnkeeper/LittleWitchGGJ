using UnityEngine;

public class VariantSelector : MonoBehaviour
{
    protected int _selectedVariant;
    public int selectedVariant
    {
        get { return _selectedVariant; }
    }

    public virtual void Add(int delta)
    {
        //Debug.Log(selectedVariant+"+"+delta+" == "+(selectedVariant + delta));
        SelectVariant(selectedVariant + delta);
    }

    public virtual int VariantsCount
    {
        get { return transform.childCount; }
    }

    public virtual void SelectVariant(int n)
    {

        if (VariantsCount > 0)
        {
            if (n < 0)
            {
                n = (VariantsCount + n) % VariantsCount;
            }
            var newVariant = Mathf.Max(0, n % VariantsCount);
            if (_selectedVariant != newVariant)
            {
                _selectedVariant = newVariant;
                //Debug.Log($"[{GetType()}] {GetGameObjectPath(gameObject)} SelectVariant {_selectedVariant}", this);
            }
            ApplyVariant(selectedVariant);
        }
        else
        {
            Debug.LogWarning("No variants to select", this);
        }
    }

    public void ApplySelectedVariant()
    {
        ApplyVariant(selectedVariant);
    }

    [ContextMenu("SelecteNext")]
    public virtual void SelectNext()
    {
        SelectVariant(selectedVariant + 1);
    }
    [ContextMenu("SelectPrev")]
    public virtual void SelectPrev()
    {
        SelectVariant(selectedVariant - 1);
    }

    protected virtual void ApplyVariant(int n)
    {
        n = Mathf.Max(0, n % transform.childCount);

        for (int i = 0; i < transform.childCount; i++)
        {
            if (i != n)
                transform.GetChild(i).gameObject.SetActive(false);
            else
            {
                var go = transform.GetChild(i).gameObject;
                go.SetActive(true);

                Debug.Log($"[{GetType()}] {GetGameObjectPath(gameObject)} ApplyVariant {n}: {go}", this);
            }
        }

    }
    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/"+obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        path = "Root" + path;
        return path;
    }
}
