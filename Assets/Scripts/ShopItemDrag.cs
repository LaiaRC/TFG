using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    
    public static Canvas canvas;

    private RectTransform rt;
    private CanvasGroup cg;
    private Image img;

    private Vector3 originPos;
    private bool drag;
    private ShopItem Item;
    private bool detected = false;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();

        img = GetComponent<Image>();
        originPos = rt.anchoredPosition;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("UI"), LayerMask.NameToLayer("UI"));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        drag = true;
        cg.blocksRaycasts = false;
        img.maskable = false;
        GameManager.Instance.draggingItemShop = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        drag = false;
        cg.blocksRaycasts = true;
        img.maskable = false;
        rt.anchoredPosition = originPos;
        GameManager.Instance.draggingItemShop = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void initialize(ShopItem item)
    {
        Item = item;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GameManager.Instance.detected)
        {
            GameManager.Instance.detected = true;
            GameManager.Instance.hideAllShop();            

            Color c = img.color;
            c.a = 0f;
            img.color = c;


            GridBuildingSystem.current.InitializeWithBuilding(Item.prefab);
        }        
    }

    private void OnEnable()
    {
        drag = false;
        cg.blocksRaycasts = true;
        img.maskable = true;
        rt.anchoredPosition = originPos;

        Color c = img.color;
        c.a = 1f;
        img.color = c;
    }
}
