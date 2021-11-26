using System;
using General;
using Input;
using UnityEngine;

public class ItemScript : SingletonMonoBehavior<ItemScript>
{
    
    //指定物件編號
    [SerializeField] private int defineItemIndex;

    //按鍵提示
    [SerializeField] private GameObject keyboardHint;


    public void ApproachItem()
    {
        keyboardHint.SetActive(true);
    }

    public void UnApproachItem()
    {
        keyboardHint.SetActive(false);
    }
}

