using UnityEngine;

namespace SharedAssets.Utility_Scripts
{
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformScaler : MonoBehaviour
    {
        private RectTransform _rectTransform;

        private void OnEnable()
        {
            _rectTransform = gameObject.GetComponent<RectTransform>();
        }

        [ContextMenu("SetHeight")]
        public void SetHeight(float height)
        {
            _rectTransform.sizeDelta = new Vector2(_rectTransform.rect.width, height);
        }

        [ContextMenu("SetWidth")]
        public void SetWidth(float width)
        {
            _rectTransform.sizeDelta = new Vector2(width, _rectTransform.rect.height);
        }
    }
}