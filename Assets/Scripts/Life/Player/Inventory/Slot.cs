using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Slot : MonoBehaviour
{
    public Item Item;

    public Image SlotImage;

    private void Awake()
    {
        SlotImage = this.transform.GetChild(0).GetComponent<Image>();
    }

    

    // Update is called once per frame
    void Update()
    {
        if (SlotImage.gameObject.activeSelf)
        {
            SlotImage.sprite = Item.ItemImage;
        }
    }
}
