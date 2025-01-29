using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
public class AssetLoader : MonoBehaviour
{
    GameObject root;
    [NonSerialized]
    public AssetBundle bundle;

    public string floor_url = "Assets/Edited/Floor/Floor.prefab";
    [NonSerialized]
    public GameObject floor; 
    List<GameObject> floor_List = new List<GameObject>();
    public void Awake()
    {
        GameInstance.Instance.assetLoader = this;
    }
    public void Start()
    {
        root = new GameObject();
        root.name = "root";
        root.transform.position = Vector3.zero;
    }
    public IEnumerator DownloadAssetBundle(string url, bool justShader)
    {

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
       
        if (www.result == UnityWebRequest.Result.Success)
        {
            if (justShader)
            {
                AssetBundle.LoadFromMemory(www.downloadHandler.data);
                yield break;
            }
            else bundle = AssetBundle.LoadFromMemory(www.downloadHandler.data);
            LoadAsset();
        }
        else
        {
            Debug.LogError("다운로드 실패: " + www.error);
        }

    }

    public void LoadFloor(int x, int y)
    {
        GameObject f = Instantiate(floor, root.transform);
        f.transform.position = new Vector3(x * 2 + 1, 0.1f, y * 2 +1);
        floor_List.Add(f);
    }

    public void LoadAsset()
    {
        if (bundle != null)
        {
     /*       foreach (string assetName in bundle.GetAllAssetNames())
            {
                Debug.Log("번들에 포함된 에셋 이름: " + assetName);
            }*/
            // 번들에서 에셋을 로드 (예: GameObject)
            floor = bundle.LoadAsset<GameObject>(floor_url);

        }
    }

    public void Clear()
    {
        AssetBundle.UnloadAllAssetBundles(bundle);
    }
}
