using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    GameObject containerCanvas;

    [SerializeField]
    ItemTable itemTable;

    private void Start()
    {
        itemTable.AssignItemIDs();
    }

    public void OpenContainer(ContainerHandler containerHandler)
    {
        containerCanvas.SetActive(true);
        InventoryBehaviour inv = containerCanvas.GetComponentInChildren<InventoryBehaviour>();
        foreach(Item item in containerHandler.containerItems)
        {
            inv.AddPrefab(item.PrefabName);
        }
    }

    public void CloseContainer(ContainerHandler containerHandler)
    {
        containerCanvas.SetActive(false);
        containerHandler.containerItems.Clear();
        InventoryBehaviour inv = containerCanvas.GetComponentInChildren<InventoryBehaviour>();
        foreach(GameObject item in inv.items)
        {
            Item inventoryItem = item.GetComponent<ItemSlot>().itemInSlot;
            containerHandler.containerItems.Add(inventoryItem);
        }
        inv.RemoveAllItems();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // if (collision.gameObject.tag == "Container")
        Debug.Log(collision.name);
        {
            OpenContainer(collision.GetComponentInParent<ContainerHandler>());
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        //  if (collision.gameObject.tag == "Container")
        {
            CloseContainer(collision.GetComponentInParent<ContainerHandler>());
        }
    }
}
