using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

// Instantiates prefabs to fill a grid
[RequireComponent(typeof(GridLayout))]
public class ItemSlotGridDimensioner : MonoBehaviour
{
    [SerializeField]
    GameObject itemSlotPrefab;

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    Vector2Int GridDimensions = new Vector2Int(6, 6);
    float width;
    float height;

    public GridSlot[,] slotArray;
    public bool isContainer = false;
    
    private LinkedList<GameObject> items;

    void Start()
    {
        GridLayoutGroup lg = GetComponent<GridLayoutGroup>();
        width = lg.cellSize.x;
        height = lg.cellSize.y;

        slotArray = new GridSlot[GridDimensions.x, GridDimensions.y];

        for (int x_cell = 0; x_cell < GridDimensions.x; x_cell++)
        {
            for (int y_cell = 0; y_cell < GridDimensions.y; y_cell++)
            {
                GameObject newObject = Instantiate(itemSlotPrefab, this.transform);
                newObject.GetComponent<GridSlot>().x = x_cell * width + width / 2;
                newObject.GetComponent<GridSlot>().y = y_cell * height + width / 2;
                slotArray[x_cell, y_cell] = newObject.GetComponent<GridSlot>();
            }
        }

        if (isContainer)
        {
            return;
        }

        items = new LinkedList<GameObject>();
        AddPrefab("Assets/ItemInstance.prefab", 0, 0);
        AddPrefab("Assets/1x2 Variant.prefab", 0, 1);
        AddPrefab("Assets/1x3 Variant.prefab", 1, 1);
        AddPrefab("Assets/2x2 Variant.prefab", 3, 0);
    }

    private void AddPrefab(string name, int x, int y)
    {
        // Instantiate object
        Vector3 gridOffset = new Vector3(20, -20, 0);
        Object prefab = AssetDatabase.LoadAssetAtPath(name, typeof(GameObject));
        GameObject obj = Instantiate(prefab, this.transform) as GameObject;

        // Calculate object offset due to size
        float obj_height = obj.GetComponent<ItemSlot>().itemInSlot.height / 2f - 0.5f;
        float obj_width = obj.GetComponent<ItemSlot>().itemInSlot.width / 2f - 0.5f;
        Vector3 obj_offset = new Vector3(obj_width * width, - obj_height * height, 0);

        // Transform object to the correct spot
        obj.transform.localPosition = new Vector3(slotArray[x, y].x, -slotArray[x, y].y, 0) + gridOffset + obj_offset;
        
        items.AddLast(obj);
    }

}
