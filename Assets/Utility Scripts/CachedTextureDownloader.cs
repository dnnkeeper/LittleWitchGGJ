using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
public class CachedTextureDownloader : MonoBehaviour
{
    public Texture2D DefaultImage;
    public string defaultURL = "";
    [SerializeField] private bool _notNeedToBeCompressed;

    public UnityEvent<Texture2D> onDownloaded;
    public UnityEvent<string> onError;

    Texture2D downloadedTexture;

    public static Dictionary<string, UnityWebRequestAsyncOperation> DownloadingHandles = new Dictionary<string, UnityWebRequestAsyncOperation>();

    private string _url;


    public void ResetImageToDefault()
    {
        onDownloaded.Invoke(DefaultImage);
    }

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(defaultURL))
            StartDownloading(defaultURL);
    }
    // private void OnDisable() {
    //     Debug.LogError("[CachedTextureDownloader] onDisable", this);
    // }
    public void DownloadOrApplyFromCache(string url)
    {
        StartDownloading(url);
    }

    public void StartDownloading(string url)
    {
        gameObject.SetActive(true);
        if (!gameObject.activeInHierarchy || !enabled)
        {
            //Download after enabled
            Debug.LogWarning("[CachedTextureDownloader] can't download when inactive! DefaultUrl set " + url, this);
            defaultURL = url;
        }
        else
            StartCoroutine(DownloadImageByURL_Routine(url));
    }

    IEnumerator DownloadImageByURL_Routine(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("[CachedTextureDownloader] url not set", this);
            onError.Invoke("Bad url");
            yield break;
        }

        var filename = url.Substring(url.LastIndexOf("/") + 1);

        if (DownloadingHandles.TryGetValue(url, out UnityWebRequestAsyncOperation cachedAsyncOperation))
        {
            while (!cachedAsyncOperation.isDone)
                yield return null;

            //Additional one frame delay to prevent instant download
            yield return null;
            if (cachedAsyncOperation.isDone)
            {
                Debug.Log($"[CachedTextureDownloader] {name} texture was loaded from cache ({filename})", this);
                InvokeResult(cachedAsyncOperation);
            }
            else
            {
                Debug.LogError($"[CachedTextureDownloader] {name} cachedAsyncOperation hasn't been done yet", this);
            }
            yield break;
        }

        //using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        //{
        Debug.Log($"[CachedTextureDownloader] {name} start downloading {filename}", this);

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

        var asyncOperation = www.SendWebRequest();

        var check = DownloadingHandles.TryAdd(url, asyncOperation);
        if (check)
            _url = url;

        var progress = 0f;
        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= progress * 1.1f)
            {
                //Debug.Log($"[CachedTextureDownloader] downloading {filename} - {asyncOperation.progress*100f}%", this);
                progress = asyncOperation.progress;
            }
            yield return new WaitForSeconds(1f);
        }

        InvokeResult(asyncOperation);
        //}
    }

    void InvokeResult(UnityWebRequestAsyncOperation asyncOperation)
    {
        // var headers = asyncOperation.webRequest.GetResponseHeaders();

        // StringBuilder sb = new StringBuilder();
        // foreach (var h in headers)
        // {
        //     sb.AppendLine($"{h.Key}:{h.Value}");
        // }
        //Debug.Log($"[CachedTextureDownloader] {asyncOperation.webRequest.url} response headers: \n "+sb.ToString());

        if (asyncOperation.webRequest.result == UnityWebRequest.Result.Success)
        {
            var resultTexture = DownloadHandlerTexture.GetContent(asyncOperation.webRequest);
            if (resultTexture != null)
            {
                downloadedTexture = resultTexture;
                Debug.Log($"[CachedTextureDownloader] Texture downloaded {asyncOperation.webRequest.url} size {resultTexture.width}x{resultTexture.height}", this);
                //#if !UNITY_ANDROID && !UNITY_IOS
                if (downloadedTexture.width == downloadedTexture.height && !_notNeedToBeCompressed)
                {
                    int newSize = NearestPowerOfTwo(downloadedTexture.width);
                    if (newSize != downloadedTexture.width)
                    {
                        Debug.Log($"[CachedTextureDownloader] {name} newSize {newSize}x{newSize}");
                        //StartCoroutine(DoScaleTex(resultTexture, newSize));
                        downloadedTexture = GPUTextureScaler.Scaled(downloadedTexture, newSize, newSize);
                        downloadedTexture.Compress(false);
                    }
                    else
                    {
                        downloadedTexture.Compress(false);
                    }
                }
                //#endif
                onDownloaded.Invoke(downloadedTexture);
            }
            else
                Debug.LogError($"[CachedTextureDownloader] Texture corrupted after finished downloading {asyncOperation.webRequest.url}", this);
        }
        else
        {
            Debug.LogError($"[CachedTextureDownloader] {asyncOperation.webRequest.url} error: {asyncOperation.webRequest.error}", this);
            onError.Invoke(asyncOperation.webRequest.error);
            asyncOperation.webRequest.Dispose();
        }
    }
    IEnumerator DoScaleTex(Texture2D tex, int newSize)
    {
        Texture2D scaled = new Texture2D(newSize, newSize, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.MipChain);
        Graphics.ConvertTexture(tex, scaled);
        downloadedTexture = scaled;
        yield return new WaitForEndOfFrame();
        downloadedTexture.Compress(false);
        Debug.Log($"[CachedTextureDownloader] {name} ConvertTexture to {newSize}x{newSize} {UnityEngine.Experimental.Rendering.DefaultFormat.LDR}");
        onDownloaded.Invoke(scaled);

    }

    public static int NearestPowerOfTwo(int x)
    {
        if (x < 0)
        {
            return -1;
        }

        if (x == 0)
        {
            return 1;
        }

        int power = 0;
        while (x > 1)
        {
            x >>= 1;
            power++;
        }

        int lower = 1 << power;
        int upper = lower << 1;

        return (x * 2 - lower < upper - x * 2) ? lower : upper;
    }

    public void CompressTextire(Texture2D tex)
    {
        tex.Compress(false);
    }

    [ContextMenu("ClearCache")]
    public void ClearCache()
    {
        foreach (var handle in DownloadingHandles)
        {
            handle.Value.webRequest.Dispose();
        }
        DownloadingHandles.Clear();
        Debug.Log("[CachedTextureDownloader] cache cleared");
    }

    [ContextMenu("SaveTextureAsPNG")]
    void SaveTextureAsPNG()
    {
        Debug.Log($"Save Texture {downloadedTexture} with resolution {downloadedTexture.width}:{downloadedTexture.height}");
        byte[] itemBGBytes = downloadedTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + $"/{name}.png", itemBGBytes);

    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(_url))
        {
            DownloadingHandles[_url].webRequest.Dispose();
            DownloadingHandles.Remove(_url);
        }
    }
}
