using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class Debugging : MonoBehaviour
{
    [SerializeField]
    TMP_Text fps_Text;
    [SerializeField]
    TMP_Text usedMemory_Text;
    [SerializeField]
    TMP_Text heapMemory_Text;
    float updateTimer = 0f;
    List<float> totalMemoryData = new List<float>(60);
    [SerializeField]
    Image stackBar;
    [SerializeField]
    Image border;
    [SerializeField]
    TMP_Text max_Text;
    [SerializeField]
    TMP_Text min_Text;
    Queue<Image> stacks = new Queue<Image>();
    Queue<Image> usingStacks = new Queue<Image>();

    float maxValue = 0;
    float minValue = 9999999;

    private void Awake()
    {
        for(int i = 0; i< 60; i++)
        {
            Image newImage = Instantiate(stackBar);
            newImage.rectTransform.SetParent(border.rectTransform);
            stacks.Enqueue(newImage);
        }

        DontDestroyOnLoad(this.gameObject);
    }
   
    // Update is called once per frame
    void Update()
    {
        if(Time.time - updateTimer >= 1f)
        {
            updateTimer = Time.time;
            FPS();
            UsedMemory();
            UpdateGraph();
        }
    }

    void FPS()
    {
        int fps = (int)(1 / Time.deltaTime);
        fps_Text.text = string.Format("FPS : {0} ({1}ms)", fps, (Time.deltaTime * 1000f).ToString("F2"));
    }
   
    void UsedMemory()
    {
        long usedTotalMemory = Profiler.GetTotalAllocatedMemoryLong();

        long totalMemory = System.GC.GetTotalMemory(false);
        float totalMB = totalMemory / (1024 * 1024);
        float usedTotalMB = usedTotalMemory / (1024 * 1024);

        maxValue = maxValue < usedTotalMB ? usedTotalMB : maxValue;
        minValue = minValue > usedTotalMB ? usedTotalMB : minValue;
        totalMemoryData.Add(usedTotalMB);
        if (totalMemoryData.Count == 60)
        {
            bool max = maxValue == totalMemoryData[0];
            bool min = minValue == totalMemoryData[0];

            totalMemoryData.RemoveAt(0);
            if(max) maxValue = totalMemoryData.Max();
            if(min) minValue = totalMemoryData.Min();
       
        }

        max_Text.text = maxValue.ToString();
        min_Text.text = minValue.ToString();

        usedMemory_Text.text = string.Format("Used Memory : {0}MB", usedTotalMB.ToString());
        heapMemory_Text.text = string.Format("Heap Memory : {0}MB", totalMB.ToString());
    }


    void UpdateGraph()
    {
        while(usingStacks.Count > 0)
        {
            Image usingImage = usingStacks.Dequeue();
            usingImage.gameObject.SetActive(false);
            stacks.Enqueue(usingImage);
        }


        for(int i = 0; i < totalMemoryData.Count; i++)
        {
            Image usingImage = stacks.Dequeue();
            usingImage.gameObject.SetActive(true);
            usingImage.rectTransform.localPosition = new Vector3(-10 *(totalMemoryData.Count - i - 1) + 300f, -100f, 0);
            usingImage.rectTransform.localScale = new Vector3(1, totalMemoryData[i]/ maxValue, 1);
            usingStacks.Enqueue(usingImage);
        }
    }
    private void OnApplicationQuit()
    {
        while (usingStacks.Count > 0)
        {
            Image image = usingStacks.Dequeue();
            Destroy(image.gameObject);
        }
        while (stacks.Count > 0)
        {
            Image image = stacks.Dequeue();
            Destroy(image.gameObject);
        }
    }
}
