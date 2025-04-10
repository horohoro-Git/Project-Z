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
        /*  Graphics.ClearRandomWriteTargets();  // GPU 명령 버퍼 강제 초기화
          GL.InvalidateState();               // OpenGL 상태 캐시 무효화
          System.GC.Collect();*/
        // QualitySettings.shadows = ShadowQuality.Disable;
        //QualitySettings.shadowDistance = 30;
        //  QualitySettings.shadowCascades = 2;
        //  QualitySettings.shadowResolution = ShadowResolution.Low;
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;  // 에디터에서만 VSync 강제 해제
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
