using System.Collections;
using System.Collections.Generic;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;

public class UMACharacterAvatar : MonoBehaviour
{
    public DynamicCharacterAvatar avatar;
    public UMAData collectionList;
    public UMAWardrobeRecipe recipe;
    GameObject itemObject;
    //public WardrobeCollection wardrobeCollection;

    public void SetDefault()
    {
        avatar.SetSlot("MaleDefaultUnderwear");
        avatar.BuildCharacter();
    }

    private void Start()
    {
        // avatar.Save

        /*  avatar.SetSlot("Cape", "SamplePack01_M_Recipe");
          avatar.SetSlot("Legs", "KnickerbockerBlackM_Recipe");
          avatar.SetSlot("Feet", "SchoesLaceUpBlackM_Recipe");
          avatar.SetSlot("Chest", "GambesonM_Recipe");
          avatar.SetSlot("Helmet", "HatLeatherM_Recipe");
          avatar.SetSlot("Hands", "GlovesLinenM_Recipe");*/

        // avatar.LoadFromRecipe(recipe, DynamicCharacterAvatar.LoadOptions.useDefaults);
        //avatar.Add = recipe;

        //     avatar.LoadFromRecipe(recipe);
        // avatar.SetSlot("Hair", "HairStyle1");
        /*    avatar.umaData = collectionList;
            ApplyClothes();*/
     //   avatar.SetSlot(recipe);
       // avatar.BuildCharacter();
       // Invoke("AddClothes", 2f);
    }
    void AddClothes()
    {
        //avatar.ClearSlot("Cape");
        //  avatar.BuildCharacter();
        if (GameInstance.Instance.assetLoader.assetLoadSuccessful)
        {
          /*  avatar.SetSlot(GameInstance.Instance.assetLoader.loadedRecipes[AssetLoader.backpackRecipeKeys[0]]);
            avatar.SetSlot(GameInstance.Instance.assetLoader.loadedRecipes[AssetLoader.chestRecipeKeys[0]]);
            avatar.SetSlot(GameInstance.Instance.assetLoader.loadedRecipes[AssetLoader.helmetRecipeKeys[0]]);
            avatar.SetSlot(GameInstance.Instance.assetLoader.loadedRecipes[AssetLoader.legsRecipeKeys[0]]);
            avatar.SetSlot(GameInstance.Instance.assetLoader.loadedRecipes[AssetLoader.handsRecipeKeys[0]]);
            avatar.SetSlot(GameInstance.Instance.assetLoader.loadedRecipes[AssetLoader.feetRecipeKeys[0]]);*/
            avatar.BuildCharacter();
            Invoke("RemoveClothes", 2f);
        }
        else
        {
            Invoke("AddClothes", 2f);
        }
    }

    void RemoveClothes()
    {
       // avatar.ClearSlot("Cape");
        avatar.BuildCharacter();

    }

    public void AddCloth(int recipeIndex)
    {

        avatar.SetSlot(GameInstance.Instance.assetLoader.loadedRecipes[AssetLoader.recipeKeys[recipeIndex]]);

        avatar.BuildCharacter();
    }

    public void RemoveCloth(string slotName)
    {
        avatar.ClearSlot(slotName);
        avatar.BuildCharacter();
    }

    public void AddItem(GameObject itemGO)
    {
        itemObject = Instantiate(itemGO);
        AttachItem attachItem = GetComponentInChildren<AttachItem>();
        itemObject.transform.SetParent(attachItem.transform);
        itemObject.transform.localPosition = Vector3.zero;
        itemObject.transform.localRotation = Quaternion.Euler(-90, -90, 0);
    }

    public void RemoveItem()
    {
        if (itemObject != null)
        {
            Destroy(itemObject);
            itemObject = null;
        }
    }
}
