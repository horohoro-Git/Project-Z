using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinMeshRendererTest : MonoBehaviour
{
    public SkinnedMeshRenderer firstCharacterSkinRenderer;  // 첫 번째 캐릭터의 피부 SkinnedMeshRenderer
    public SkinnedMeshRenderer secondCharacterRenderer;  // 두 번째 캐릭터의 SkinnedMeshRenderer
    public SkinnedMeshRenderer clothesRenderer;  // 두 번째 캐릭터의 옷 SkinnedMeshRenderer

    void Start()
    {
        // 1. 옷을 비활성화
     //   clothesRenderer.gameObject.SetActive(false);

     //   clothesRenderer.m

        // 2. 첫 번째 캐릭터의 피부 메시를 두 번째 캐릭터에 적용
        secondCharacterRenderer.sharedMesh = firstCharacterSkinRenderer.sharedMesh;

        // 3. 첫 번째 캐릭터의 본 배열을 두 번째 캐릭터의 본 배열로 설정
        secondCharacterRenderer.bones = firstCharacterSkinRenderer.bones;

        // 4. 첫 번째 캐릭터의 머티리얼을 두 번째 캐릭터에 적용 (피부 텍스처 포함)
        secondCharacterRenderer.material = firstCharacterSkinRenderer.material;
    }
}
