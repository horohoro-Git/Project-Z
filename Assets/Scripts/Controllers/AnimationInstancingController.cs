using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AnimInstancing = AnimationInstancing.AnimationInstancing;
using AnimInfo = AnimationInstancing.AnimationInfo;
using AnimationInstancing;
using static AnimationInstancing.AnimationInstancingMgr;
using UnityEngine.Rendering;

public class AnimationInstancingController : MonoBehaviour
{
    AnimInstancing instancing;

    public AnimInstancing GetAnimation { get { if (instancing == null) instancing = GetComponentInChildren<AnimInstancing>(); return instancing;  } }

    public AnimInstancing a;
    float currentTime;
    Dictionary<string, AnimInfo> animations = new Dictionary<string, AnimInfo>();
    private void Start()
    {
        Invoke("NewStart", 1f);
        // GetAnimation.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        
    }
    int boneIndex;
    void NewStart()
    {
      
     //   GetAnimation.AttackItem("hand_l", GetAnimation, new GameObject(), out int boneIndex);
        /* for (int i = 0; i < GetAnimation.GetAnimationCount(); i++)
         {
             AnimInfo info = GetAnimation.aniInfo[i];
             Debug.Log(info.animationName);
             animations[info.animationName] = info;
         }*/
        //    GetAnimation.AttackItem("hand_l", GetAnimation, a, out boneIndex);
        GetAnimation.PlayAnimation("Zombie@Idle01");

        //Debug.Log(GetAnimation.GetAnimationInfoList().Count);
    }

    public void StopAnimation()
    {
       // GetAnimation.
    }
    public void PlayerAnimation(string name)
    {
        GetAnimation.CrossFade(name, 0.25f);
        //GetAnimation.
      //  GetAnimation.PlayAnimation(name);
    }
    public void DamagedAnimation(string name)
    {
       /* float 
        if(GetAnimation.GetCurrentAnimationInfo().totalFrame)*/
        GetAnimation.PlayAnimation(name);
    }
}
