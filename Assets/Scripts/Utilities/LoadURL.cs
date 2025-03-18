using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadURL : MonoBehaviour
{
    public const string Floor = "floor";
    public const string Wall = "wall";
    public const string Door = "door";
    public const string Roof = "roof";
    public const string PreviewFloor = "preview_floor";
    public const string PreviewWall = "preview_wall";
    public const string PreviewDoor = "preview_door";
    public const string Flower_Orange = "flower_orange";
    public const string Flower_Yellow = "flower_yellow";
    public const string Flower_Pink = "flower_pink";
    public const string Grasses = "grasses";
    public const string Tree = "tree";
    public const string Human_Male = "human_male";
    public const string Log = "log";
    public const string Heal = "heal";
    public const string LevelUp = "levelup";
    public const string Male = "male";
    public const string UMA_GLIB = "UMA_GLIB";



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
