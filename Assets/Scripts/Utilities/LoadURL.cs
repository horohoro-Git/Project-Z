using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadURL : MonoBehaviour
{
    public const string Floor = "Floor";
    public const string Wall = "wall";
    public const string Door = "doorSwitch";
    public const string Roof = "roof";
    public const string PreviewFloor = "PreloadFloor";
    public const string PreviewWall = "PreloadWall";
    public const string PreviewDoor = "PreloadDoor";
    public const string Flower_Orange = "flower_orange";
    public const string Flower_Yellow = "flower_yellow";
    public const string Flower_Pink = "flower_pink";
    public const string Grasses = "grasses";
    public const string Tree = "Tree";
    public const string Human_Male = "human_male";
    public const string Log = "log";

    

    public string bundleUrl;
    public string bundleUrl2;

    void Start()
    {
        AssetLoader loader = GetComponent<AssetLoader>();
        // StartCoroutine(loader.DownloadAssetBundle(bundleUrl, true));
        //  StartCoroutine(loader.DownloadAssetBundle(bundleUrl2, false));
        StartCoroutine(loader.DownloadAssetBundle("AAA",false));
    }

}
