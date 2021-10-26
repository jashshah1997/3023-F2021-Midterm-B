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
        Debug.Log("Begin Drag");
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
        Debug.Log("End Drag");
        gameObject.transform.position = objectLastPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        itemSlot = GetComponent<ItemSlot>();
        int w = itemSlot.itemInSlot.width;
        int h = itemSlot.itemInSlot.height;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
