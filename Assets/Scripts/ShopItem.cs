using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "GameObjects/Shop Item", order = 0)]
public class ShopItem : MonoBehaviour
{
    public string name = "Default";
    public string id = "Default";
    public string description = "Description";
    //public string cost = "Cost";
    public string resourceText1 = "resource1";
    public string resourceText2 = "resource2";
    public Sprite resource1Icon;
    public Sprite resource2Icon;
    public int maxQuantity;
    //public int price;
    //public CurrencyType currency;
    public ObjectType type;
    public Sprite icon;
    public GameObject prefab;
    public Sprite produces1;
    public Sprite produces2;
    public Sprite produces3;
}

public enum ObjectType
{
    Buildings,
    BoostDecorations,
    Decorations,
    DarkMarketItem
}
