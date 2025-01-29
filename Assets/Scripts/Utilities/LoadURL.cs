using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadURL : MonoBehaviour
{
    public string bundleUrl;
    public string bundleUrl2;

    void Start()
    {
        AssetLoader loader = GetComponent<AssetLoader>();
        StartCoroutine(loader.DownloadAssetBundle(bundleUrl, true));
        StartCoroutine(loader.DownloadAssetBundle(bundleUrl2, false));
    }

}
