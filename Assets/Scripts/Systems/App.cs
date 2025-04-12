using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class App : MonoBehaviour
{
    public Debugging debugging;
    Scene scene;
    private void Awake()
    {
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;  
        Application.targetFrameRate = -1;
#endif
     //   QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 60;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        DontDestroyOnLoad(this.gameObject);
        SceneManager.LoadSceneAsync("developScene", LoadSceneMode.Additive);
    }
    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        GameInstance.Instance.app = this;
        GameInstance.Instance.quit = false;
    }
    private void OnApplicationQuit()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
       // GameInstance.Instance.assetLoader.ClearAsset();
     
        if (GameInstance.Instance.assetLoader != null) GameInstance.Instance.assetLoader.Clear();
        GameInstance.Instance.Reset();
        GameInstance.Instance.quit = true;
 
       // GameInstance.Instance.Reset();
    }
    private void OnSceneUnloaded(Scene scene)
    {
    }
}
