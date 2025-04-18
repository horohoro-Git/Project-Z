using UMA;
using UnityEngine;
using UnityEngine.Rendering;

public class SkinnedMeshRendererLODTest : MonoBehaviour
{
    public SlotData highPolySlot;
    public SlotData[] lowPolySlot = new SlotData[7];
    private UMAData umaData;

    void Start()
    {
        umaData = GetComponent<UMAData>();
        for(int i = 0; i< 7; i++)
        {

            Debug.Log(umaData.umaRecipe.slotDataList[i].slotName + " " + lowPolySlot[i].slotName);
            umaData.umaRecipe.slotDataList[i] = lowPolySlot[i];

            umaData.Dirty(true, true, true);
        }
    }
  /*  void Update()
    {
        //  float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        float ss = Camera.main.orthographicSize;
        int lodLevel = (ss < 5f) ? 1 : 0;
      //  Debug.Log(lodLevel);
      //  umaData.umaRecipe.slotDataList[0] = (lodLevel == 0) ? highPolySlot : lowPolySlot;
        umaData.Dirty(true, true, true);
    }*/

}