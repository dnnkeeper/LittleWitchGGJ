using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dexart.Scripts.ObjectsPlacement
{

    public enum PlacementStatus
    {
        Relocating,
        Accommodated
    }

    public interface IPlaceableObject
    {
        bool Relocate(Vector3 pos, Quaternion rot, float distancceToGround = 0);
        bool CanBePlaced();
        void Accommodate();
        void Remove();
    }

    [RequireComponent(typeof(Collider))]
    [DisallowMultipleComponent]
    public class PlaceableObject : MonoBehaviour, IPlaceableObject
    {
        public UnityEvent OnStartMoving, OnAccomodated, OnDestroyed;

        public int ObjectId => Mathf.Abs(GetInstanceID());
        public float angleWithSurface;
        public bool canBePlacedAtPlaceable;
        public bool rotateTowardsZ;

        public Quaternion additionalRotation = Quaternion.identity;

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
        [SerializeField] private Material relocateMaterial;
        [SerializeField] private Material blockedMaterial;

        [SerializeField] protected float angleWithFloorMin = 0f;
        [SerializeField] protected float angleWithFloorMax = 180f;
        [SerializeField] protected Vector3 floorNormal = Vector3.up;


        [SerializeField] public Vector3 colliderBoundsCenterLocal;
        [SerializeField] public Vector3 colliderExtents;
        [SerializeField] public Vector3 colliderExtentsLocal;

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
                        originalPosition = transform.position;
                        originalRotation = transform.rotation;
                        OnStartMoving.Invoke();
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
            colliderExtents = (collider.bounds.extents);

            // if (colliderExtentsLocal.x == 0){
            //     var oldRotation = transform.rotation;
            //     transform.rotation = Quaternion.identity;
            //     colliderExtentsLocal = transform.InverseTransformVector( collider.bounds.extents ); //Quaternion.Inverse(transform.rotation) * colliderExtents;
            //     //transform.rotation = oldRotation;
            // }

        }
        Vector3 originalPosition;
        Quaternion originalRotation = Quaternion.identity;
        [ContextMenu("Calculate Bounds")]
        void CalculateBounds()
        {
            if (transform.rotation != Quaternion.identity)
            {
                originalRotation = transform.rotation;
                transform.rotation = Quaternion.identity;
                Debug.LogWarning("Should rotate to identity before calculating bounds!", this);
            }
            else
            {
                colliderExtentsLocal = transform.InverseTransformVector(collider.bounds.extents);
                colliderBoundsCenterLocal = transform.InverseTransformPoint(collider.bounds.center);
                colliderBoundsCenterLocal = new Vector3(colliderBoundsCenterLocal.x * transform.lossyScale.x, colliderBoundsCenterLocal.y * transform.lossyScale.y, colliderBoundsCenterLocal.z * transform.lossyScale.z);
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
        [ContextMenu("Return Rotation")]
        void ReturnRotation()
        {
            transform.rotation = originalRotation;
        }

        [ContextMenu("Return Position")]
        void ReturnPosition()
        {
            transform.position = originalPosition;
        }

        void Reset()
        {
            colliderExtents = collider.bounds.extents;
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
            Debug.Log($"[{GetType()}] Accommodated {gameObject}", this);
            if (!CanBePlaced()) {
                ReturnPosition();
                ReturnRotation();
            }
            currState = PlacementStatus.Accommodated;
            if (renderer != null)
            {
                SetMaterial(mainMaterials);
            }
            OnAccomodated?.Invoke();
        }

        public void Remove()
        {
            Destroy(this.gameObject);
        }

        public void GetCorrectPositionAndRotation(ref Vector3 position, ref Quaternion rotation)
        {
            var surfaceNormal = rotation * Vector3.up;
            var correctedRotation = rotation * Quaternion.Inverse(additionalRotation);
            rotation = correctedRotation;

            position = position - correctedRotation * (colliderBoundsCenterLocal);
            var halfUp = (surfaceNormal * (Quaternion.Inverse(additionalRotation) * colliderExtentsLocal).y * transform.lossyScale.y);
            position += halfUp;
        }

        public bool Relocate(Vector3 position, Quaternion rotation, float distanceToGround = 0)
        {
            currState = PlacementStatus.Relocating;


            // var surfaceNormal = rotation * Vector3.up;
            // var correctedRotation = rotation * Quaternion.Inverse(additionalRotation);
            // transform.rotation = correctedRotation;

            // transform.position = position - correctedRotation * (colliderBoundsCenterLocal);// + (rotation * Vector3.Project(colliderExtentsLocal, Vector3.up))*0.5f;
            // var halfUp =  ( surfaceNormal * ( Quaternion.Inverse(additionalRotation) * colliderExtentsLocal).y * transform.lossyScale.y );

            // Debug.DrawLine(transform.position, transform.position+halfUp, Color.cyan);
            // transform.position += halfUp;
            Vector3 correctPosition = position;
            Quaternion correctRotation = rotation;
            GetCorrectPositionAndRotation(ref correctPosition, ref correctRotation);
            transform.position = correctPosition;
            transform.rotation = correctRotation;

            var canBePlaced = CanBePlacedAt(position, rotation, distanceToGround);

            if (canBePlaced)
            {
                if (relocateMaterial != null && renderer != null)
                    SetMaterial(relocateMaterial);
            }
            else
            {
                if (blockedMaterial != null && renderer != null)
                    SetMaterial(blockedMaterial);
            }

            return canBePlaced;

        }

        public bool CanBePlaced()
        {
            var rotation = transform.rotation;
            var position = transform.position + rotation * (colliderBoundsCenterLocal);
            // var halfUp = ( rotation * Vector3.up * ( Quaternion.Inverse(additionalRotation) * colliderExtentsLocal).y * transform.lossyScale.y );
            // position += halfUp;
            angleWithSurface = Vector3.Angle(rotation * additionalRotation * Vector3.up, floorNormal);
            if (angleWithSurface < angleWithFloorMin || angleWithSurface > angleWithFloorMax)
            {
                Debug.Log("bad angle " + angleWithSurface);
                return false;
            }

            var colliderExtentsLocalScaled = Vector3.Scale(colliderExtentsLocal, transform.lossyScale);
            return CheckCollisionsAt(position, rotation, colliderExtentsLocalScaled, 10f);
        }

        public bool CanBePlacedAt(Vector3 position, Quaternion rotation, float distanceToGround = 0)
        {
            angleWithSurface = Vector3.Angle(rotation * Vector3.up, floorNormal);
            if (distanceToGround > 0 || angleWithSurface < angleWithFloorMin || angleWithSurface > angleWithFloorMax)
            {
                return false;
            }
            position = position + (rotation * Vector3.up * (Quaternion.Inverse(additionalRotation) * colliderExtentsLocal).y * transform.lossyScale.y);
            rotation = rotation * Quaternion.Inverse(additionalRotation);
            var colliderExtentsLocalScaled = Vector3.Scale(colliderExtentsLocal, transform.lossyScale);

            return CheckCollisionsAt(position, rotation, colliderExtentsLocalScaled);
        }

        bool CheckCollisionsAt(Vector3 position, Quaternion rotation, Vector3 colliderExtentsLocalScaled, float debugRenderTime = 0)
        {
            foreach (var collider in Physics.OverlapBox(position, colliderExtentsLocalScaled * 0.99f, rotation, GetCollisionMask(gameObject), QueryTriggerInteraction.Ignore))
            {
                if (!collider.transform.IsChildOf(transform))
                {
                    Debug.DrawLine(transform.position, position, Color.red, debugRenderTime);
                    DrawBox(position, rotation, colliderExtentsLocalScaled * 2f, Color.red, debugRenderTime);
                    return false;
                }
            }
            Debug.DrawLine(transform.position, position, Color.cyan, debugRenderTime);

            DrawBox(position, rotation, colliderExtentsLocalScaled * 2f, Color.cyan);
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

            var pivotPoint = transform.TransformPoint(colliderBoundsCenterLocal/transform.lossyScale.x);
            var rot = transform.rotation * additionalRotation;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pivotPoint, pivotPoint + rot * Vector3.right * 0.5f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pivotPoint, pivotPoint + rot * Vector3.forward * 0.5f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pivotPoint, pivotPoint + rot * Vector3.up * 0.5f);
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(transform.InverseTransformPoint(pivotPoint), colliderExtentsLocal * 2f);
        }

        private void OnDestroy()
        {
            OnDestroyed.Invoke();
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

        [ContextMenu("Add Box Collider")]
        public void AddBoxCollider()
        {
            BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();

            Collider[] colliders = GetComponentsInChildren<Collider>();
            if (colliders.Length == 0)
            {
                Debug.LogWarning("No colliders found in children.");
                return;
            }

            Bounds bounds =  new Bounds();//colliders[0].bounds;
            foreach (Collider col in colliders)
            {
                if ( (col as BoxCollider != boxCollider) && !col.isTrigger && col.gameObject.activeSelf && col.enabled)
                {
                    Debug.Log("Bounds += " + bounds + " of "+col);
                    bounds.Encapsulate(col.bounds);
                }
            }
            //bounds.center = new Vector3(bounds.center.x/transform.lossyScale.x, bounds.center.y/transform.lossyScale.y, bounds.center.z/transform.lossyScale.z);
            if (boxCollider == null)
                boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center = bounds.center - transform.position;
            boxCollider.size = bounds.size;//new Vector3(bounds.size.x/transform.lossyScale.x, bounds.size.y/transform.lossyScale.y, bounds.size.z/transform.lossyScale.z);
        }
    }
}