using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AnimInstancing = AnimationInstancing.AnimationInstancing;
using AnimInfo = AnimationInstancing.AnimationInfo;

public class AnimationInstancingController : MonoBehaviour
{
    AnimInstancing instancing;

    public AnimInstancing GetAnimation { get { if (instancing == null) instancing = GetComponentInChildren<AnimInstancing>(); return instancing;  } }


    Dictionary<string, AnimInfo> animations = new Dictionary<string, AnimInfo>();
    private void Start()
    {
        Invoke("NewStart", 1f);
    }

    void NewStart()
    {
       /* for (int i = 0; i < GetAnimation.GetAnimationCount(); i++)
        {
            AnimInfo info = GetAnimation.aniInfo[i];
            Debug.Log(info.animationName);
            animations[info.animationName] = info;
        }*/
        GetAnimation.PlayAnimation("Zombie@Idle01");
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
}
