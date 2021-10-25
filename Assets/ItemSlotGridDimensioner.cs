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

    public GridSlot[,] slotArray;
    public bool isContainer = false;

    private LinkedList<GameObject> items;

    void Start()
    {
        GridLayoutGroup lg = GetComponent<GridLayoutGroup>();
        float width = lg.cellSize.x;
        float height = lg.cellSize.y;

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

        Vector3 gridOffset = new Vector3(20, -20, 0);
        Object prefab = AssetDatabase.LoadAssetAtPath("Assets/ItemInstance.prefab", typeof(GameObject));
        GameObject obj = Instantiate(prefab, this.transform) as GameObject;
        obj.transform.localPosition = new Vector3(slotArray[0, 0].x, -slotArray[0, 0].y, 0) + gridOffset;

        Object prefab1 = AssetDatabase.LoadAssetAtPath("Assets/1x2 Variant.prefab", typeof(GameObject));
        GameObject obj1 = Instantiate(prefab1, this.transform) as GameObject;
        obj1.transform.localPosition = new Vector3(slotArray[0, 1].x, - slotArray[0, 1].y, 0) + gridOffset;

        Object prefab2 = AssetDatabase.LoadAssetAtPath("Assets/1x3 Variant.prefab", typeof(GameObject));
        GameObject obj2 = Instantiate(prefab2, this.transform) as GameObject;
        obj2.transform.localPosition = new Vector3(slotArray[1, 1].x, - slotArray[1, 1].y, 0) + gridOffset;

        Object prefab3 = AssetDatabase.LoadAssetAtPath("Assets/2x2 Variant.prefab", typeof(GameObject));
        GameObject obj3 = Instantiate(prefab3, this.transform) as GameObject;
        obj3.transform.localPosition = new Vector3(slotArray[3, 0].x, - slotArray[3, 0].y, 0) + gridOffset;

        // Add all items to the item list
        items = new LinkedList<GameObject>();
        items.AddLast(obj);
    }

}
