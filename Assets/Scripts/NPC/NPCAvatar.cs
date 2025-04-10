using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAvatar : UMACharacterAvatar
{
    public NPCStruct nPCStruct;
    private void Awake()
    {
        
    }

    public void Setup(NPCStruct nPCStruct)
    {
        this.nPCStruct = nPCStruct;
        SetDefault();
        AttachHair(AssetLoader.recipes[nPCStruct.npc_hair].recipe_name);
        if (nPCStruct.npc_helmet != -1) AddCloth(nPCStruct.npc_helmet);
        if (nPCStruct.npc_armor != -1) AddCloth(nPCStruct.npc_armor);
        if (nPCStruct.npc_arm != -1) AddCloth(nPCStruct.npc_arm);
        if (nPCStruct.npc_leg != -1) AddCloth(nPCStruct.npc_leg);
        if (nPCStruct.npc_boots != -1) AddCloth(nPCStruct.npc_boots);
    }
}
