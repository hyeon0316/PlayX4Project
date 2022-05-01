using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Slot[] _slots;

    private void Awake()
    {
        _slots = GetComponentsInChildren<Slot>();
    }

    public void AddUsed(string itemName)
    {
        _slots[0].SlotImage.gameObject.SetActive(true); 
        _slots[0].Item = Resources.Load<Item>($"Items/{itemName}");
    }

    public void DeleteUsed()
    {
        //todo: 소모 아이템 이므로 따로 아이템 개수 변수를 한번씩 줄여주는 방식 
    }
    
    public void AddIngredient(string itemName)
    {
        _slots[1].SlotImage.gameObject.SetActive(true); 
        _slots[1].Item = Resources.Load<Item>($"Items/{itemName}");
    }

    public void DeleteIngredient()
    {
        _slots[1].SlotImage.gameObject.SetActive(false); 
    }


}
