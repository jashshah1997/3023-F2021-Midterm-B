using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Instantiates prefabs to fill a grid
[RequireComponent(typeof(GridLayout))]
public class ItemSlotGridDimensioner : MonoBehaviour
{
    [SerializeField]
    GameObject itemSlotPrefab;

    [SerializeField]
    Vector2Int GridDimensions = new Vector2Int(6, 6);

    public GameObject[,] slotArray;

    void Start()
    {
        slotArray = new GameObject[GridDimensions.x, GridDimensions.y];

        for (int x_cell = 0; x_cell < GridDimensions.x; x_cell++)
        {
            for (int y_cell = 0; y_cell < GridDimensions.y; y_cell++)
            {
                GameObject newObject = Instantiate(itemSlotPrefab, this.transform);
                newObject.GetComponent<GridSlot>().x = x_cell;
                newObject.GetComponent<GridSlot>().y = y_cell;
                slotArray[x_cell, y_cell] = newObject;
            }
        }
    }
}
