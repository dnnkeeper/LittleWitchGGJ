using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum PlacementStatus
{
    Relocating,
    Accommodated
}

public interface IPlaceableObject
{
    bool Relocate(Vector3 pos, Quaternion rot, float distancceToGround = 0, bool positionIsOnGround = true);
    bool CanBePlaced();
    void Accommodate();
    void Remove();
}

[DisallowMultipleComponent]
public class PlaceableObject : MonoBehaviour, IPlaceableObject
{
    public UnityEvent OnStartMoving, OnAccomodated;

    public int ObjectId => Mathf.Abs(GetInstanceID());
    public float angleWithSurface;
    public bool canBePlacedAtPlaceable;
    public bool rotateTowardsZ;

    //public Quaternion additionalRotation = Quaternion.identity;

    //[SerializeField]Transform transformPivot;

    private Collider _collider;
    protected new Collider collider
    {
        get
        {
            if (_collider == null)
                _collider = GetComponent<Collider>();
            return _collider;
        }
    }
    private Renderer _renderer;
    public new Renderer renderer
    {
        get
        {
            if (_renderer == null)
                _renderer = GetComponent<Renderer>();
            return _renderer;
        }
    }

    //public Vector3 StickDirection => stickDirection;

    [SerializeField] private float stickOffsetDistance = -1f;

    [SerializeField] private List<Material> mainMaterials = new();
    public Material relocateMaterial;
    public Material blockedMaterial;

    [SerializeField] protected float angleWithFloorMin = 0f;
    [SerializeField] protected float angleWithFloorMax = 180f;
    [SerializeField] protected Vector3 floorNormal = Vector3.up;
    //[SerializeField] public Vector3 collider.bounds.center;
    //[SerializeField] public Vector3 colliderExtents;
    //[SerializeField] public Vector3 collider.bounds.extents;

    private Rigidbody rb;

    PlacementStatus _currState;
    protected PlacementStatus currState
    {
        get { return _currState; }
        set
        {
            if (_currState != value)
            {
                _currState = value;
                if (value == PlacementStatus.Accommodated)
                {
                    foreach (var colliderInfo in _colliders)
                    {
                        colliderInfo.Key.enabled = colliderInfo.Value;
                    }
                }
                else
                {
                    lastValidPosition = transform.position;
                    lastValidRotation = transform.rotation;
                    OnStartMoving?.Invoke();
                    foreach (var colliderInfo in _colliders)
                    {
                        colliderInfo.Key.enabled = false;
                    }
                }
            }
        }
    }

    private Dictionary<Collider, bool> _colliders;

    void OnValidate()
    {
        // if (transformPivot == null)
        //     transformPivot = transform;            
        //colliderExtents = (collider.bounds.extents);

        // if (collider.bounds.extents.x == 0){
        //     var oldRotation = transform.rotation;
        //     transform.rotation = Quaternion.identity;
        //     collider.bounds.extents = transform.InverseTransformVector( collider.bounds.extents ); //Quaternion.Inverse(transform.rotation) * colliderExtents;
        //     //transform.rotation = oldRotation;
        // }

    }
    Vector3 lastValidPosition;
    Quaternion lastValidRotation = Quaternion.identity;
    [ContextMenu("Calculate Bounds")]
    void CalculateBounds()
    {
        lastValidRotation = transform.rotation;
        if (transform.rotation != Quaternion.identity)
        {
            transform.rotation = Quaternion.identity;
            Debug.LogWarning("Should rotate to identity before calculating bounds!", this);
        }
        var bounds = GetEncapsulatedRenderersBounds();
        colliderCenterLocal = transform.InverseTransformPoint(collider.bounds.center);
        fromColliderCenterToPivot = transform.position - collider.bounds.center;
        colliderExtentsLocal = transform.InverseTransformVector(collider.bounds.extents);
        transform.rotation = lastValidRotation;
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
    [ContextMenu("Return Rotation")]
    void ReturnRotation()
    {
        transform.rotation = lastValidRotation;
    }

    [ContextMenu("Return Position")]
    void ReturnPosition()
    {
        transform.position = lastValidPosition;
    }

    public void ReturnToValidPosition()
    {
        if (transform.position != lastValidPosition)
            Debug.LogError($"{transform.position} != {lastValidPosition}");
        ReturnPosition();
        ReturnRotation();
    }

    void Reset()
    {
        //colliderExtents = collider.bounds.extents;
        CalculateBounds();
    }

    private void Awake()
    {
        if (mainMaterials.Count == 0 && renderer != null)
        {
            foreach (var mat in renderer.materials)
                mainMaterials.Add(mat);
        }
        _colliders = new Dictionary<Collider, bool>();
        // var myCollider = GetComponent<Collider>();
        // if (myCollider != null)
        //     _colliders.Add(myCollider, myCollider.enabled);
        foreach (var collider in GetComponentsInChildren<Collider>(true))
        {
            _colliders.Add(collider, collider.enabled);
        }
        // if (transformPivot == null){
        //     transformPivot = transform;
        // }
        currState = PlacementStatus.Accommodated;

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            OnStartMoving.AddListener(delegate { rb.isKinematic = true; });
            OnAccomodated.AddListener(delegate { rb.isKinematic = false; });
        }
    }

    public void Accommodate()
    {

        Debug.Log($"[{GetType()}] Accommodated {gameObject.name}", gameObject);
        //SnapToGround();
       
        currState = PlacementStatus.Accommodated;
        if (renderer != null)
        {
            SetMaterial(mainMaterials);
        }

        if (!CanBePlaced())
        {
            Debug.LogWarning($"Can't be placed here, return to {lastValidPosition}");
            //DrawBox(transform.position, transform.rotation, colliderExtentsLocalScaled * 2f, Color.red, 10f);

            ReturnPosition();
            ReturnRotation();
            // Somehow it's not working, so we need to wait a bit
            //Invoke(nameof(ReturnToValidPosition), 0.1f);
            //DrawBox(transform.position, transform.rotation, colliderExtentsLocalScaled * 2f, Color.blue, 10f);
        }

        OnAccomodated?.Invoke();

    }

    void SnapToGround()
    {
        float offset = colliderExtents.y;
        var castStart = colliderCenter + Vector3.up * offset;
        //Debug.DrawLine(transform.position, castStart, Color.blue, 2f);
        var boxCastExtents = colliderExtents * 0.99f;
        
        //DrawBox(colliderCenter, transform.rotation, colliderExtents * 2f, Color.cyan, 2f);
        //DrawBox(colliderCenter, transform.rotation, boxCastExtents * 2f, Color.blue, 2f);

        bool wascolliderEnabled = collider.enabled;
        collider.enabled = false;
        if (Physics.BoxCast(castStart, boxCastExtents, Vector3.down, out RaycastHit hit, transform.rotation, 10f, GetCollisionMask(gameObject), QueryTriggerInteraction.Ignore))
        {
            //Debug.Log($"SnapToGround hit.distance: {hit.distance}-{colliderExtents.y}");
            //Debug.DrawLine(castStart, castStart + Vector3.down * hit.distance, Color.red, 2f);
            var hitOffset = Vector3.down * (hit.distance - offset);
            var hitCenter = castStart + hitOffset;
            //DrawBox(hitCenter, transform.rotation, boxCastExtents * 2f, Color.red, 2f);
            if (CheckCollisionsAt(transform.position + hitOffset, transform.rotation, colliderExtentsLocalScaled))
            {
                Relocate(hitOffset, transform.rotation);
            }
        }
        else
        {
            //DrawBox(castStart, transform.rotation, colliderExtents * 2f, Color.red, 2f);

            //Debug.DrawLine(castStart, castStart + Vector3.down * 10f, Color.red, 2f);
        }
        collider.enabled = wascolliderEnabled;
    }
    Vector3 colliderCenter => transform.TransformPoint(colliderCenterLocal);
    Vector3 colliderExtents => transform.TransformVector(colliderExtentsLocal);
    public Vector3 colliderCenterLocal, colliderExtentsLocal;
    Vector3 fromColliderCenterToPivot;
    Vector3 colliderExtentsLocalScaled => Vector3.Scale(colliderExtentsLocal, transform.lossyScale);

    public void Remove()
    {
        Destroy(this.gameObject);
    }

    public bool Relocate(Vector3 targetPosition, Quaternion rotation, float distanceToGround = 0, bool positionIsOnGround = true)
    {
        currState = PlacementStatus.Relocating;

        //GetCorrectPositionAndRotation(ref targetPosition, ref rotation);
        if (positionIsOnGround)
            targetPosition = targetPosition + fromColliderCenterToPivot + transform.up * colliderExtents.y;


        var canBePlaced = CanBePlacedAt(targetPosition, rotation, distanceToGround);
        
        if (canBePlaced)
        {
            transform.position = targetPosition;
            transform.rotation = rotation;
            if (relocateMaterial != null && renderer != null)
                SetMaterial(relocateMaterial);
            lastValidPosition = transform.position;
            lastValidRotation = transform.rotation;
            DrawBox(transform.position, transform.rotation, colliderExtentsLocalScaled * 2f, Color.cyan, Time.deltaTime);
        }
        else
        {
            if (blockedMaterial != null && renderer != null)
                SetMaterial(blockedMaterial);
            
            DrawBox(transform.position, transform.rotation, colliderExtentsLocalScaled * 2f, Color.red, Time.deltaTime);
        }

        return canBePlaced;

    }

    void Update()
    {
        DrawBox(lastValidPosition, lastValidRotation, colliderExtentsLocalScaled * 2f, Color.yellow, Time.deltaTime);
    }

    public void GetCorrectPositionAndRotation(ref Vector3 position, ref Quaternion rotation)
    {
        var surfaceNormal = rotation * Vector3.up;
        var correctedRotation = rotation;
        rotation = correctedRotation;

        position = position - correctedRotation * (colliderCenterLocal);
        var halfUp = (surfaceNormal * colliderExtentsLocal.y * transform.lossyScale.y);
        position += halfUp;
    }

    public bool CanBePlaced()
    {
        var rotation = transform.rotation;
        var position = transform.position;// + rotation * (colliderCenter);
        // var halfUp = ( rotation * Vector3.up * ( Quaternion.Inverse(additionalRotation) * collider.bounds.extents).y * transform.lossyScale.y );
        // position += halfUp;
        angleWithSurface = Vector3.Angle(rotation * Vector3.up, floorNormal);
        if (angleWithSurface < angleWithFloorMin || angleWithSurface > angleWithFloorMax)
        {
            Debug.Log("bad angle " + angleWithSurface);
            return false;
        }

        return CheckCollisionsAt(position, rotation, colliderExtentsLocalScaled, Time.deltaTime);
    }

    public bool CanBePlacedAt(Vector3 position, Quaternion rotation, float distanceToGround = 0)
    {
        angleWithSurface = Vector3.Angle(rotation * Vector3.up, floorNormal);
        if (distanceToGround > 0 || angleWithSurface < angleWithFloorMin || angleWithSurface > angleWithFloorMax)
        {
            return false;
        }
        position = position + (rotation * Vector3.up * colliderExtents.y * transform.lossyScale.y);
        return CheckCollisionsAt(position, rotation, colliderExtentsLocalScaled, Time.deltaTime);
    }

    bool CheckCollisionsAt(Vector3 position, Quaternion rotation, Vector3 colliderExtentsLocalScaled, float debugRenderTime = 0)
    {
        foreach (var collider in Physics.OverlapBox(position, colliderExtentsLocalScaled * 0.99f, rotation, GetCollisionMask(gameObject), QueryTriggerInteraction.Ignore))
        {
            if (!collider.transform.IsChildOf(transform))
            {
                Debug.DrawLine(transform.position, position, Color.red, debugRenderTime);
                //DrawBox(position, rotation, colliderExtentsLocalScaled * 2f, Color.red, debugRenderTime);
                return false;
            }
        }
        //Debug.DrawLine(transform.position, position, Color.cyan, debugRenderTime);

        //DrawBox(position, rotation, colliderExtentsLocalScaled * 2f, Color.cyan, debugRenderTime);
        return true;
    }
    public void DrawBox(Vector3 pos, Quaternion rot, Vector3 scale, Color c, float t = 0)
    {
        // create matrix
        Matrix4x4 m = new Matrix4x4();
        m.SetTRS(pos, rot, scale);

        var point1 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
        var point2 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
        var point3 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
        var point4 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));

        var point5 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
        var point6 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
        var point7 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
        var point8 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));

        Debug.DrawLine(point1, point2, c, t);
        Debug.DrawLine(point2, point3, c, t);
        Debug.DrawLine(point3, point4, c, t);
        Debug.DrawLine(point4, point1, c, t);

        Debug.DrawLine(point5, point6, c, t);
        Debug.DrawLine(point6, point7, c, t);
        Debug.DrawLine(point7, point8, c, t);
        Debug.DrawLine(point8, point5, c, t);

        Debug.DrawLine(point1, point5, c, t);
        Debug.DrawLine(point2, point6, c, t);
        Debug.DrawLine(point3, point7, c, t);
        Debug.DrawLine(point4, point8, c, t);
    }

    private void SetMaterial(Material material)
    {
        if (material == renderer.material)
            return;

        var materials = renderer.materials;

        for (int i = 0; i < materials.Length; i++)
            materials[i] = material;

        renderer.materials = materials;
    }

    private void SetMaterial(List<Material> materials)
    {
        renderer.materials = materials.ToArray();
    }

    void OnDrawGizmosSelected()
    {
        var pivotPoint = colliderCenter;
        var rot = transform.rotation;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pivotPoint, pivotPoint + rot * Vector3.right * 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pivotPoint, pivotPoint + rot * Vector3.forward * 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pivotPoint, pivotPoint + rot * Vector3.up * 0.5f);
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(colliderCenterLocal, colliderExtentsLocal * 2f);

        //MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        //var bounds = renderers[0].bounds;
        //foreach (MeshRenderer rend in renderers)
        //{
        //    if (rend.gameObject.activeSelf && rend.enabled)
        //    {
        //        Gizmos.matrix = Matrix4x4.identity;
        //        Gizmos.color = Color.blue;
        //        Gizmos.DrawWireCube(rend.bounds.center, rend.bounds.extents * 2);
        //        bounds.Encapsulate(rend.bounds);
        //    }
        //}
        //Gizmos.matrix = Matrix4x4.identity;
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);

    }

    public LayerMask GetCollisionMask(GameObject gameObject)
    {
        int mask = 0;

        for (int i = 0; i < 32; i++)
        {
            if (!Physics.GetIgnoreLayerCollision(gameObject.layer, i))
            {
                mask |= (1 << i);
            }
        }

        return mask;
    }

    public Bounds GetEncapsulatedRenderersBounds()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        var bounds = renderers[0].bounds;
        foreach (MeshRenderer rend in renderers)
        {
            if (rend.gameObject.activeSelf && rend.enabled)
            {
                bounds.Encapsulate(rend.bounds);
            }
        }
        return bounds;
    }

    [ContextMenu("Add Box Collider")]
    public void AddBoxCollider()
    {
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();

        var bounds = GetEncapsulatedRenderersBounds();

        //bounds.center = new Vector3(bounds.center.x/transform.lossyScale.x, bounds.center.y/transform.lossyScale.y, bounds.center.z/transform.lossyScale.z);
        if (boxCollider == null)
            boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.center = transform.InverseTransformPoint(bounds.center);
        boxCollider.size = transform.InverseTransformVector(bounds.size);//new Vector3(bounds.size.x/transform.lossyScale.x, bounds.size.y/transform.lossyScale.y, bounds.size.z/transform.lossyScale.z);
    }

    void OnEnable()
    {
        if (collider == null)
        {
            AddBoxCollider();
        }
        if (colliderExtentsLocal.sqrMagnitude == 0)
        {
            CalculateBounds();
        }
    }
}