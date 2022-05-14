using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class ShopItemHolder : MonoBehaviour
{
    private ShopItem Item;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image iconImage;
    //[SerializeField] private Image currencyImage;
    [SerializeField] private TextMeshProUGUI costText;

    public void initialize(ShopItem item)
    {
        Item = item;

        iconImage.sprite = Item.icon;
        titleText.text = Item.name;
        descriptionText.text = Item.description;
        //currencyImage.sprite = ShopManager2.currencySprites[Item.Currency];
        costText.text = Item.cost;

        unlockItem();
    }

    public void unlockItem()
    {
        iconImage.gameObject.AddComponent<ShopItemDrag>();
        //iconImage.color = new Color(255,255,255,255);
    }
}
