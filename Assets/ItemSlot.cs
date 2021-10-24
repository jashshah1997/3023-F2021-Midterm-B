using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Display item in the slot, update image, make clickable when there is an item, invisible when there is not
public class ItemSlot : MonoBehaviour
{
    public Item itemInSlot = null;

    [SerializeField]
    private int itemCount = 0;
    public int ItemCount
    {
        get
        {
            return itemCount;
        }
        set
        {
            itemCount = value;
        }
    }

    [SerializeField]
    private Image icon;
    [SerializeField]
    private TMPro.TextMeshProUGUI itemCountText;

    void Start()
    {
        RefreshInfo();
    }

    public void UseItemInSlot()
    {
        if(itemInSlot != null)
        {
            itemInSlot.Use();
            if (itemInSlot.isConsumable)
            {
                itemCount--;
                RefreshInfo();
            }
        }
    }

    public void RefreshInfo()
    {
        if(ItemCount < 1)
        {
            itemInSlot = null;
        }

        if(itemInSlot != null) // If an item is present
        {
            //update image and text
            itemCountText.text = ItemCount.ToString();
            icon.sprite = itemInSlot.icon;
            icon.gameObject.SetActive(true);
        } else
        {
            // No item
            itemCountText.text = "";
            icon.gameObject.SetActive(false);
        }
    }
}
