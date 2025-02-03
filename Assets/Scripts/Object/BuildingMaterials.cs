using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMaterials : MonoBehaviour
{
    static Queue<Roof> roofs = new Queue<Roof>();
    // Start is called before the first frame update
  
    public static void CreateRoofs()
    {
        for (int i = 0; i < 10000; i++)
        {
            GameObject go = Instantiate(GameInstance.Instance.assetLoader.roof);

            roofs.Enqueue(go.GetComponent<Roof>());
        }
    }

    public static Roof GetRoof()
    {
        if(roofs.Count > 0)
        {
            return roofs.Dequeue();
        }
        else
        {
            GameObject go = Instantiate(GameInstance.Instance.assetLoader.roof);
            return go.GetComponent<Roof>();
        }
    }
}
