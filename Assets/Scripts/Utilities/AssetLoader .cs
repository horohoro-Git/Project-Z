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
        if (!GameInstance.Instance.housingSystem.CheckFloor(x, y))
        {
            GameObject f = Instantiate(floor, root.transform);
            f.transform.position = new Vector3(x * 2 + 1, 0.1f, y * 2 + 1);
            floor_List.Add(f);
            GameInstance.Instance.housingSystem.BuildFloor(x, y,f);
        }
    }

    public void LoadWall(Vector3 point, int x, int y)
    {
       // if (GameInstance.Instance.housingSystem.CheckFloor(x, y)) // 바닥 확인
        {
            float pointX = point.x;
            float pointY = point.z;

            //크기 확인
            float disA = pointX - x * 2;
            float disB = (x * 2 + 2) - pointX;
            float disC = pointY - y * 2;
            float disD = (y * 2 + 2) - pointY;
            float min = 2;
            if(disA < min) min = disA;
            if(disB < min) min = disB;
            if(disC < min) min = disC;
            if(disD < min) min = disD;

            if(min == disA) Debug.Log("좌측 벽 생성 " + min);    
      
            if (min == disB) Debug.Log("우측 벽 생성 " + min);
      
            if(min == disC) Debug.Log("아래 벽 생성 " + min);
      
            if(min == disD) Debug.Log("윗벽 생성 " + min);
      
        }
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
