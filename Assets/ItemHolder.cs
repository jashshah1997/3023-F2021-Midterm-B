using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemHolder : MonoBehaviour
{
    public ItemSlotGridDimensioner grid;
    LinkedList<ItemSlot> items;

    // Start is called before the first frame update
    void Start()
    {
        items = new LinkedList<ItemSlot>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
