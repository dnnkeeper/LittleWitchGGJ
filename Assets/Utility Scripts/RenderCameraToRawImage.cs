using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class RenderCameraToRawImage : MonoBehaviour
{

    public Color origColor;
    RenderTexture targetRenderTexture;

    public Camera sourceCamera;

    Vector2 targetPixelRectSize;

    //Resolution res;

    RawImage _targetRawImage;
    RawImage targetRawImage
    {
        get
        {
            if (_targetRawImage == null)
                _targetRawImage = GetComponent<RawImage>();
            return _targetRawImage;
        }
    }

    // Use this for initialization
    void Start()
    {
        //res = Screen.currentResolution;

    }

    Texture originalTexture;

    // Update is called once per frame
    void Update()
    {
        UpdateTargetSize();
    }

    void UpdateTargetSize()
    {
        var pixelRectSize = targetRawImage.rectTransform.rect.size * transform.lossyScale;
        if (sourceCamera != null && (Mathf.RoundToInt(pixelRectSize.x) != Mathf.RoundToInt(targetPixelRectSize.x) || Mathf.RoundToInt(pixelRectSize.y) != Mathf.RoundToInt(targetPixelRectSize.y)))
        {
            //Debug.Log("pixelRect changed. Create new renderTexture! " + pixelRectSize.x + " != "+ targetPixelRectSize.x + " || "+ pixelRectSize.y + " != " + targetPixelRectSize.y);

            targetPixelRectSize = pixelRectSize;

            CreateRenderTexture();

            sourceCamera.targetTexture = targetRenderTexture;

            targetRawImage.texture = targetRenderTexture;

            //res = Screen.currentResolution;
        }
    }

    public FilterMode filterMode = FilterMode.Bilinear;

    void CreateRenderTexture()
    {
        if (targetRenderTexture != null)
        {
            targetRenderTexture.Release();
        }

        var sizeDelta = targetPixelRectSize;//targetRawImage.rectTransform.rect.size;
        if (sizeDelta.x > 0 && sizeDelta.y > 0)
        {
            targetRenderTexture = new RenderTexture((int)sizeDelta.x, (int)sizeDelta.y, 24, RenderTextureFormat.Default);
            targetRenderTexture.filterMode = filterMode;
            targetRenderTexture.useMipMap = false;
            //targetRenderTexture.anisoLevel = 0;
            targetRenderTexture.name = name + "_RenderTargetTexture";
            targetRenderTexture.Create();
            Debug.Log("[RenderToRawImage] Created new Render Texture: " + sizeDelta + " for " + name, transform);
        }
        else
        {
            Debug.LogError("[RenderToRawImage] Couldn't create render texture with size " + sizeDelta);
        }
    }

    void OnEnable()
    {
        if (sourceCamera == null)
        {
            Debug.LogError("No sourceCamera", this);
            return;
        }

        UpdateTargetSize();

        targetPixelRectSize = targetRawImage.rectTransform.rect.size * transform.lossyScale;

        if (targetRenderTexture == null)
        {
            CreateRenderTexture();
        }

        if (targetRenderTexture == null)
        {
            Debug.LogError("No render texture created");
            return;
        }
        if (targetRenderTexture != null)
        {

            sourceCamera.targetTexture = targetRenderTexture;

            originalTexture = targetRawImage.texture;
            origColor = targetRawImage.color;

            targetRawImage.texture = targetRenderTexture;
            targetRawImage.color = Color.white;

            sourceCamera.enabled = true;
        }
    }

    void OnDisable()
    {
        if (targetRenderTexture != null)
        {
            if (sourceCamera != null)
                sourceCamera.targetTexture = null;
            if (targetRawImage != null)
            {
                targetRawImage.texture = originalTexture;
                targetRawImage.color = origColor;
            }
            targetRenderTexture.Release();
            Destroy(targetRenderTexture);
            targetRenderTexture = null;
        }

        if (sourceCamera != null)
            sourceCamera.enabled = false;

        //GC.Collect ();
    }

}
