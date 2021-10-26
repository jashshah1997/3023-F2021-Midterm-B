using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

// Instantiates prefabs to fill a grid
[RequireComponent(typeof(GridLayout))]
public class InventoryBehaviour : MonoBehaviour
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
    private float invert = 1;
    Vector3 gridOffset = new Vector3(20, -20, 0);

    void Start()
    {
        GridLayoutGroup lg = GetComponent<GridLayoutGroup>();
        width = lg.cellSize.x;
        height = lg.cellSize.y;

        slotArray = new GridSlot[GridDimensions.x, GridDimensions.y];

        for (int y_cell = 0; y_cell < GridDimensions.y; y_cell++)
        {
            for (int x_cell = 0; x_cell < GridDimensions.x; x_cell++)
            {
                GameObject newObject = Instantiate(itemSlotPrefab, this.transform);
                newObject.GetComponent<GridSlot>().x = x_cell * width + width / 2;

                if (isContainer)
                {
                    newObject.GetComponent<GridSlot>().x = (GridDimensions.x - 1 - x_cell) * width + width / 2;
                }
                newObject.GetComponent<GridSlot>().y = y_cell * height + height / 2;
                newObject.GetComponent<GridSlot>().x_idx = x_cell;
                newObject.GetComponent<GridSlot>().y_idx = y_cell;
                slotArray[x_cell, y_cell] = newObject.GetComponent<GridSlot>();
            }
        }

        if (isContainer)
        {
            invert = -1f;
            gridOffset.x = gridOffset.x * (-1f);
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
        Object prefab = AssetDatabase.LoadAssetAtPath(name, typeof(GameObject));
        GameObject obj = Instantiate(prefab, this.transform) as GameObject;

        // Calculate object offset due to size
        float obj_height = obj.GetComponent<ItemSlot>().itemInSlot.height / 2f - 0.5f;
        float obj_width = obj.GetComponent<ItemSlot>().itemInSlot.width / 2f - 0.5f;
        Vector3 obj_offset = new Vector3(obj_width * width, - obj_height * height, 0);

        // Transform object to the correct spot
        obj.transform.localPosition = new Vector3(invert * slotArray[x, y].x, -slotArray[x, y].y, 0) + gridOffset + obj_offset;
        SetOccupied(x, y, obj.GetComponent<ItemSlot>().itemInSlot.width, obj.GetComponent<ItemSlot>().itemInSlot.height);

        items.AddLast(obj);
        Debug.Log(name + " added");
    }

    private void SetOccupied(int x, int y, int obj_width, int obj_height)
    {
        for (int i = x; i < x + obj_width; i++)
        {
            for (int j = y; j < y + obj_height; j++)
            {
                Debug.Log("Set " + i + " , " + j + " occupied");
                slotArray[i, j].occupied = true;
            }
        }
    }
}
