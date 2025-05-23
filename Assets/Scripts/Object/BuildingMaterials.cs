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
            GameObject go = Instantiate(AssetLoader.loadedAssets[LoadURL.Roof]);

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
            GameObject go = Instantiate(AssetLoader.loadedAssets[LoadURL.Roof]);
            return go.GetComponent<Roof>();
        }
    }

    private void OnApplicationQuit()
    {
        while (roofs.Count > 0)
        {
            Roof roof = roofs.Dequeue();
            Destroy(roof.gameObject);
        }
    }
}
