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
    public bool isMirrored = false;
    
    private LinkedList<GameObject> items;
    private float invert = 1;
    Vector3 gridOffset = new Vector3(20, -20, 0);

    void Start()
    {
        GridLayoutGroup lg = GetComponent<GridLayoutGroup>();
        width = lg.cellSize.x;
        height = lg.cellSize.y;

        // Initialize Grid positions and indices
        slotArray = new GridSlot[GridDimensions.x, GridDimensions.y];
        for (int y_cell = 0; y_cell < GridDimensions.y; y_cell++)
        {
            for (int x_cell = 0; x_cell < GridDimensions.x; x_cell++)
            {
                GameObject newObject = Instantiate(itemSlotPrefab, this.transform);
                newObject.GetComponent<GridSlot>().x = x_cell * width + width / 2;

                if (isMirrored)
                {
                    newObject.GetComponent<GridSlot>().x = (GridDimensions.x - 1 - x_cell) * width + width / 2;
                }
                newObject.GetComponent<GridSlot>().y = y_cell * height + height / 2;
                newObject.GetComponent<GridSlot>().x_idx = x_cell;
                newObject.GetComponent<GridSlot>().y_idx = y_cell;
                slotArray[x_cell, y_cell] = newObject.GetComponent<GridSlot>();
            }
        }

        if (isMirrored)
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
        InsertItemInGrid(obj, x, y);
    }

    private void InsertItemInGrid(GameObject obj, int x, int y)
    {
        // Calculate object offset due to size
        float obj_height = obj.GetComponent<ItemSlot>().itemInSlot.height / 2f - 0.5f;
        float obj_width = obj.GetComponent<ItemSlot>().itemInSlot.width / 2f - 0.5f;
        Vector3 obj_offset = new Vector3(obj_width * width, -obj_height * height, 0);

        // Transform object to the correct spot
        obj.transform.localPosition = new Vector3(invert * slotArray[x, y].x, -slotArray[x, y].y, 0) + gridOffset + obj_offset;
        SetOccupied(x, y, obj.GetComponent<ItemSlot>().itemInSlot.width, obj.GetComponent<ItemSlot>().itemInSlot.height, true);

        obj.GetComponent<ItemSlot>().grid_x = x;
        obj.GetComponent<ItemSlot>().grid_y = y;

        if (!items.Contains(obj))
        {
            Debug.Log(name + " added to list!");
            items.AddLast(obj);
        }
        Debug.Log(name + " inserted to grid");
    }

    public bool FitToGrid(ItemSlot itemSlot)
    {
        // Calculate Grid positions
        float obj_height = itemSlot.itemInSlot.height / 2f - 0.5f;
        float obj_width = itemSlot.itemInSlot.width / 2f - 0.5f;
        Vector3 obj_offset = new Vector3(obj_width * width, -obj_height * height, 0);
        Vector3 gridPos = itemSlot.transform.localPosition - obj_offset - gridOffset;

        int grid_x = (int)Mathf.Floor(invert * gridPos.x / width);
        int grid_y = (int)Mathf.Floor(- gridPos.y / height);

        if (isMirrored)
        {
            grid_x = GridDimensions.x - 1 - grid_x;
        }

        Debug.Log("Dragged grid: " + grid_x + " , " + grid_y);

        // Unset occupancy for object
        SetOccupied(itemSlot.grid_x, itemSlot.grid_y, itemSlot.itemInSlot.width, itemSlot.itemInSlot.height, false);
        bool check = IsOccupied(grid_x, grid_y, itemSlot.itemInSlot.width, itemSlot.itemInSlot.height);
        if (check)
        {
            // Reset Occupancy
            SetOccupied(itemSlot.grid_x, itemSlot.grid_y, itemSlot.itemInSlot.width, itemSlot.itemInSlot.height, true);
            Debug.Log("Grid Occupied!");
            return false;
        }

        InsertItemInGrid(itemSlot.gameObject, grid_x, grid_y);
        return true;
    }
 

    private bool IsOccupied(int x, int y, int obj_width, int obj_height)
    {
        for (int i = x; i < x + obj_width; i++)
        {
            for (int j = y; j < y + obj_height; j++)
            {
                if (slotArray[i, j].occupied)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void SetOccupied(int x, int y, int obj_width, int obj_height, bool value)
    {
        for (int i = x; i < x + obj_width; i++)
        {
            for (int j = y; j < y + obj_height; j++)
            {
                Debug.Log("Set " + i + " , " + j + " occupied");
                slotArray[i, j].occupied = value;
            }
        }
    }
}