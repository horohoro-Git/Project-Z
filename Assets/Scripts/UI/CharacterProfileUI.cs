using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterProfileUI : MonoBehaviour
{
    [NonSerialized]
    public UMACharacterAvatar avatar;

    public RenderTexture renderTexture;

    public RawImage characterImage;

    private void Awake()
    {
        GameInstance.Instance.characterProfileUI = this;
    }
    public void CreateCharacter(bool load, GameObject characterGO)
    {
        if (!load) Destroy(avatar.gameObject);

        if(load)
        {
            GameObject newCamera = new GameObject();

            Camera camera = newCamera.AddComponent<Camera>();
            camera.targetDisplay = 2;
            camera.targetTexture = renderTexture;   
            newCamera.transform.position = new Vector3(1000, 1.65f, 998.7f);
            newCamera.transform.rotation = Quaternion.Euler(30, 0, 0);

            characterImage.texture = camera.targetTexture;
        }

        avatar = Instantiate(characterGO).GetComponent<UMACharacterAvatar>();

        avatar.transform.position = new Vector3(1000, 0, 1000);
        avatar.transform.rotation = Quaternion.Euler(0, 180, 0);
        
    
    }
    public void AddCloth(int recipeIndex)
    {
        avatar.AddCloth(recipeIndex);
    }
    public void RemoveCloth(string slotName)
    {
        avatar.RemoveCloth(slotName);
    }
}
