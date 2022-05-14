using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "GameObjects/Shop Item", order = 0)]
public class ShopItem : MonoBehaviour
{
    public string name = "Default";
    public string description = "Description";
    public string cost = "Cost";
    public int level;
    //public int price;
    //public CurrencyType currency;
    public ObjectType type;
    public Sprite icon;
    public GameObject prefab;
}

public enum ObjectType
{
    Buildings,
    BoostDecorations,
    Decorations,
    DarkMarketItem
}
