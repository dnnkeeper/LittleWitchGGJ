using UnityEngine;
using UnityEngine.Events;

public class GameObjectReferenceFinder : MonoBehaviour
{
    public Transform root;
    public string path = "child/target";
    public UnityEvent<Transform> onTransformFound;
    public UnityEvent<GameObject> onGameObjectFound;
    [SerializeField] bool findOnStart = true;
    public void Start()
    {
        if (findOnStart)
        {
            FindByPath();
        }
    }

    public void FindByPath()
    {
        if (root == null)
            root = transform;
        var target = root.transform.Find(path);
        if (target != null)
        {
            onTransformFound.Invoke(target);
            onGameObjectFound.Invoke(target.gameObject);
        }
    }
}
