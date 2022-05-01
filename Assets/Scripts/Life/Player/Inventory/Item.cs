using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public enum ItemType
{
    Used, //소모품
    Ingredient, //재료
}

[CustomEditor(typeof(Item))]
public class ItemInspector : Editor
{
    private Item _item = null;

    // target은 Editor에 있는 변수로 선택한 오브젝트를 받아옴.
    private void OnEnable()
    {
        _item = (Item) target;
    }

    /// <summary>
    /// 인스펙터를 GUI로 그려줌
    /// </summary>
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        _item.Type = (ItemType) EditorGUILayout.EnumPopup("아이템 타입", _item.Type);
        _item.ItemImage = (Sprite) EditorGUILayout.ObjectField("아이템 이미지", _item.ItemImage, typeof(Sprite),true);
        switch (_item.Type)
        {
            case ItemType.Ingredient:
                break;
            case ItemType.Used:
                _item.HillValue = EditorGUILayout.FloatField("회복량", _item.HillValue);
                _item.ItemCoolDown = EditorGUILayout.FloatField("아이템 쿨타임", _item.ItemCoolDown);
                break;
        }
    }
}

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject
{
    public ItemType Type;

    public float HillValue;
    public float ItemCoolDown;

    public Sprite ItemImage;
}
