using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class ShopManager2 : MonoBehaviour
{
    public static ShopManager2 current;
    //public List<ShopItem> items;


    //[SerializeField] private List<Sprite> sprites;

    private RectTransform rt;
    private RectTransform prt;
    private bool opened = false;
    private bool dragging = false;

    [SerializeField] private GameObject itemPrefab;
    private Dictionary<ObjectType, List<ShopItem>> shopItems = new Dictionary<ObjectType, List<ShopItem>>(5);

    [SerializeField] public TabGroup shopTabs;

    private void Awake()
    {
        current = this;

        rt = GetComponent<RectTransform>();
        prt = transform.parent.GetComponent<RectTransform>();        
    }

    private void Start()
    {
        gameObject.SetActive(false);
        
        load();
        initialize();

        for (int i = 0; i < shopItems.Keys.Count; i++)
        {
            ObjectType key = shopItems.Keys.ToArray()[i];
            for (int j = 0; j < shopItems[key].Count; j++)
            {
                ShopItem item = shopItems[key][j];

                //shopTabs.transform.GetChild(i).GetChild(j).GetComponent<ShopItemHolder>().unlockItem();
            }
        }
    }
    private void Update()
    {        
        if (!GameManager.Instance.isOnCanvas && !GameManager.Instance.dragging && Input.GetMouseButtonUp(0) && !GameManager.Instance.isDialogOpen)
        {
            GameManager.Instance.toggleBuildMode();
        }  
    }

    private void load()
    {
        ShopItem[] items = Resources.LoadAll<ShopItem>("Shop");
        System.Array.Sort(items, delegate (ShopItem x, ShopItem y) { return x.order.CompareTo(y.order); });

        shopItems.Add(ObjectType.Buildings, new List<ShopItem>());
        shopItems.Add(ObjectType.BoostDecorations, new List<ShopItem>());
        shopItems.Add(ObjectType.Decorations, new List<ShopItem>());
        shopItems.Add(ObjectType.DarkMarketItem, new List<ShopItem>());

        foreach (var item in items)
        {            
            shopItems[item.type].Add(item);
        }
    }

    private void initialize()
    {
        for (int i = 0; i < shopItems.Keys.Count; i++)
        {
            foreach (var item in shopItems[(ObjectType)i])
            {
                GameObject itemObject = Instantiate(itemPrefab, shopTabs.objectsToSwap[i].transform);
                itemObject.GetComponent<ShopItemHolder>().initialize(item);
            }
        }
    }    
}
