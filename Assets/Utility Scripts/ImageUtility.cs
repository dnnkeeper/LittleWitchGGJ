using UnityEngine;
using UnityEngine.UI;

namespace Utility.CommonUtils
{
    [RequireComponent(typeof(Image))]
    public class ImageUtility : MonoBehaviour
    {

        private Image _image;

        public void FullAlpha() => SetAlpha(1f);

        public void HalfAlpha() => SetAlpha(0.5f);

        public void QuarterAlpha() => SetAlpha(0.25f);


        private void SetAlpha(float alpha)
        {
            if (!_image)
                SetImage();

            var color = _image.color;
            color.a = alpha;
            _image.color = color;
        }

        private void SetImage()
        {
            _image = GetComponent<Image>();
        }
    }
}