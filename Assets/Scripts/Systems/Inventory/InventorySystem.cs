using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{

    GameObject[,] inventoryArray = new GameObject[7, 10];
    [NonSerialized]
    public int inventorySize = 0;
    public GameObject info;
    public RectTransform border;
    public GameObject draggingItem;
    public Sprite defaultSlot;
    [NonSerialized]
    public GameObject draggedItem;

    public GraphicRaycaster graphicRaycaster;
    private void Awake()
    {
        GameInstance.Instance.inventorySystem = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
