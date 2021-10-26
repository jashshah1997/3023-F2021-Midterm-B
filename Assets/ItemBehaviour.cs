using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemBehaviour : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 lastMousePosition;
    private Vector2 objectLastPosition;
    private ItemSlot itemSlot;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag " + gameObject.name);
        lastMousePosition = eventData.position;
        objectLastPosition = gameObject.transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentMousePosition = eventData.position;
        Vector2 diff = currentMousePosition - lastMousePosition;

        Vector3 newPosition = new Vector3(diff.x, diff.y, transform.position.z);
        gameObject.transform.position += newPosition;
        lastMousePosition = currentMousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag " + gameObject.name);

        // Check if it fits to parent container
        InventoryBehaviour parentInventory = gameObject.GetComponentInParent<InventoryBehaviour>();
        bool added_to_parent = parentInventory.FitToGrid(itemSlot);
        if (added_to_parent)
        {
            // All Done!
            return;
        }

        // Try to fit the object to some other inventory
        InventoryBehaviour[] allInventories = FindObjectsOfType<InventoryBehaviour>();
        foreach (InventoryBehaviour inventory in allInventories)
        {
            if (inventory.Equals(parentInventory))
            {
                // Dont add to parent inventory
                continue;
            }

            Debug.Log("Add to other inventory...");

            // Add old grid elements to reset occpuancy on parent inventory
            // if added to some other inventory
            int old_grid_x = itemSlot.grid_x;
            int old_grid_y = itemSlot.grid_y;

            // 1. Set Object parent to be the other inventory
            itemSlot.transform.SetParent(inventory.transform);
            bool added_to_other = inventory.FitToGrid(itemSlot, true);

            if (added_to_other)
            {
                // If added to other inventory remove from list of parent
                parentInventory.items.Remove(itemSlot.gameObject);

                // Clear parent occupancy
                parentInventory.SetOccupied(old_grid_x, old_grid_y, itemSlot.itemInSlot.width, itemSlot.itemInSlot.height, false);
                return;
            }

            // Reset parent if not added
            itemSlot.transform.SetParent(parentInventory.transform);
        }

        // Reset position if not added
        gameObject.transform.position = objectLastPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        itemSlot = GetComponent<ItemSlot>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
