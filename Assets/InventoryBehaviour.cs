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
    float CellWidth;
    float CellHeight;

    public GridSlot[,] slotArray;
    public bool isContainer = false;
    
    public LinkedList<GameObject> items;
    private float invert = 1;
    Vector3 gridOffset = new Vector3(20, -20, 0);

    void Start()
    {
        GridLayoutGroup lg = GetComponent<GridLayoutGroup>();
        CellWidth = lg.cellSize.x;
        CellHeight = lg.cellSize.y;

        // Initialize Grid positions and indices
        slotArray = new GridSlot[GridDimensions.x, GridDimensions.y];
        for (int y_cell = 0; y_cell < GridDimensions.y; y_cell++)
        {
            for (int x_cell = 0; x_cell < GridDimensions.x; x_cell++)
            {
                GameObject newObject = Instantiate(itemSlotPrefab, this.transform);
                newObject.GetComponent<GridSlot>().x = x_cell * CellWidth + CellWidth / 2;

                if (isContainer)
                {
                    newObject.GetComponent<GridSlot>().x = (GridDimensions.x - 1 - x_cell) * CellWidth + CellWidth / 2;
                }
                newObject.GetComponent<GridSlot>().y = y_cell * CellHeight + CellHeight / 2;
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

        if (!isContainer)
        {
            AddPrefab("Assets/ItemInstance.prefab");
            AddPrefab("Assets/1x2 Variant.prefab");
            AddPrefab("Assets/1x3 Variant.prefab");
            AddPrefab("Assets/2x2 Variant.prefab");
        }
    }

    public void AddPrefab(string name)
    {
        // Instantiate object
        Object prefab = AssetDatabase.LoadAssetAtPath(name, typeof(GameObject));
        GameObject obj = Instantiate(prefab, this.transform) as GameObject;
        InsertItemInGrid(obj);
    }

    public void InsertItemInGrid(GameObject obj)
    {
        obj.transform.SetParent(this.transform);
        int item_width = obj.GetComponent<ItemSlot>().itemInSlot.width;
        int item_height = obj.GetComponent<ItemSlot>().itemInSlot.height;

        for (int i = 0; i < GridDimensions.x; i++)
        {
            for (int j = 0; j < GridDimensions.y; j++)
            {
                if (!IsOccupied(i, j, item_width, item_height))
                {
                    InsertItemInGrid(obj, i, j);
                    return;
                }
            }
        }
    }

    public void RemoveAllItems()
    {
        foreach(GameObject obj in items)
        {
            Destroy(obj);
        }
        items.Clear();
        SetOccupied(0, 0, GridDimensions.x, GridDimensions.y, false);
    }

    private void InsertItemInGrid(GameObject obj, int x, int y)
    {
        // Calculate object offset due to size
        float obj_height = obj.GetComponent<ItemSlot>().itemInSlot.height / 2f - 0.5f;
        float obj_width = obj.GetComponent<ItemSlot>().itemInSlot.width / 2f - 0.5f;
        Vector3 obj_offset = new Vector3(obj_width * CellWidth, -obj_height * CellHeight, 0);

        // Transform object to the correct spot
        obj.transform.localPosition = new Vector3(invert * slotArray[x, y].x, -slotArray[x, y].y, 0) + gridOffset + obj_offset;
        SetOccupied(x, y, obj.GetComponent<ItemSlot>().itemInSlot.width, obj.GetComponent<ItemSlot>().itemInSlot.height, true);

        obj.GetComponent<ItemSlot>().grid_x = x;
        obj.GetComponent<ItemSlot>().grid_y = y;

        if (!items.Contains(obj))
        {
            Debug.Log(obj.name + " added to list!");
            items.AddLast(obj);
        }
        Debug.Log(obj.name + " inserted to grid");
    }

    public void ChildMoving(GameObject obj)
    {
        // When child is moving set it as most visible
        canvas.transform.SetAsLastSibling();
        obj.transform.SetAsLastSibling();
    }

    public bool FitToGrid(ItemSlot itemSlot, bool fromOtherInventory = false)
    {
        // Calculate Grid positions
        float obj_height = itemSlot.itemInSlot.height / 2f - 0.5f;
        float obj_width = itemSlot.itemInSlot.width / 2f - 0.5f;
        Vector3 obj_offset = new Vector3(obj_width * CellWidth, -obj_height * CellHeight, 0);
        Vector3 gridPos = itemSlot.transform.localPosition - obj_offset - gridOffset;

        int grid_x = (int)Mathf.Floor(invert * gridPos.x / CellWidth);
        int grid_y = (int)Mathf.Floor(- gridPos.y / CellHeight);

        if (isContainer)
        {
            grid_x = GridDimensions.x - 1 - grid_x;
        }

        if (grid_x < 0 || grid_x > GridDimensions.x - 1 ||
            grid_y < 0 || grid_y > GridDimensions.y - 1)
        {
            return false;
        }

        Debug.Log("Dragged grid: " + grid_x + " , " + grid_y);

        if (!fromOtherInventory)
        {
            // Unset occupancy for object if items if from the same inventory
            SetOccupied(itemSlot.grid_x, itemSlot.grid_y, itemSlot.itemInSlot.width, itemSlot.itemInSlot.height, false);
        }
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
                if (i < 0 || i > GridDimensions.x - 1 ||
                    j < 0 || j > GridDimensions.y - 1)
                {
                    return true;
                }

                if (slotArray[i, j].occupied)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void SetOccupied(int x, int y, int obj_width, int obj_height, bool value)
    {
        for (int i = x; i < x + obj_width; i++)
        {
            for (int j = y; j < y + obj_height; j++)
            {
                slotArray[i, j].occupied = value;
            }
        }
    }
}
