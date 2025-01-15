using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class App : MonoBehaviour
{
    GameInstance gameInstance;
    private void Awake()
    {
        gameInstance = GameInstance.Instance;

        DontDestroyOnLoad(this.gameObject);

        SceneManager.LoadSceneAsync("developScene", LoadSceneMode.Additive);
    }


}
