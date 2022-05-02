using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UsedSlot : MonoBehaviour
{
    public Item Item;
    public Image SlotImage;
    public int ItemCount;
    private Text _countText;


    private void Awake()
    {
        _countText = SlotImage.GetComponentInChildren<Text>();
    }
    
    private void Update()
    {
        _countText.text = $"{ItemCount}";

        
        if (Input.GetKeyDown(KeyCode.Alpha1) && ItemCount !=0)
        {
            --ItemCount;
            FindObjectOfType<Player>().HP += Item.HillValue;
            if (ItemCount == 0)
                FindObjectOfType<Inventory>().ClearUsed();   
        }
    }
}
