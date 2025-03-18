using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinMeshRendererTest : MonoBehaviour
{
    public SkinnedMeshRenderer firstCharacterSkinRenderer;  // ù ��° ĳ������ �Ǻ� SkinnedMeshRenderer
    public SkinnedMeshRenderer secondCharacterRenderer;  // �� ��° ĳ������ SkinnedMeshRenderer
    public SkinnedMeshRenderer clothesRenderer;  // �� ��° ĳ������ �� SkinnedMeshRenderer

    void Start()
    {
        // 1. ���� ��Ȱ��ȭ
     //   clothesRenderer.gameObject.SetActive(false);

     //   clothesRenderer.m

        // 2. ù ��° ĳ������ �Ǻ� �޽ø� �� ��° ĳ���Ϳ� ����
        secondCharacterRenderer.sharedMesh = firstCharacterSkinRenderer.sharedMesh;

        // 3. ù ��° ĳ������ �� �迭�� �� ��° ĳ������ �� �迭�� ����
        secondCharacterRenderer.bones = firstCharacterSkinRenderer.bones;

        // 4. ù ��° ĳ������ ��Ƽ������ �� ��° ĳ���Ϳ� ���� (�Ǻ� �ؽ�ó ����)
        secondCharacterRenderer.material = firstCharacterSkinRenderer.material;
    }
}
