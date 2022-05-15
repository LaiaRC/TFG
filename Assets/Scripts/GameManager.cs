using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI debugInventoryInfo;
    public Animator buildModeAnimator;
    public bool isOnBuildingMode = false;
    public bool isOnCanvas;
    public GameObject canvas;
    public bool dragging;
    public bool draggingFromShop = false;
    public bool draggingItemShop = false;
    public bool detected = false;

    public GameObject shop;
    public GameObject allShop;


    private string info = "";

    #region SINGLETON PATTERN
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        ShopItemDrag.canvas = canvas.GetComponent<Canvas>();
    }
    #endregion


    private void Update()
    {
        foreach (KeyValuePair<string, int> inventoryResource in Data.Instance.INVENTORY)
        {
            info += "\n -" + inventoryResource.Key + ": " + inventoryResource.Value;
        }
        //debugInventoryInfo.SetText("Dragging: " + dragging + "\n" + "OnCanvas: " + isOnCanvas + "\n" + "onBuild: " + isOnBuildingMode + "\n" + "draggingShop: " + draggingFromShop + "\n");
        debugInventoryInfo.text = info;
        info = "";
    }


    public void OnBeginDrag()
    {
        dragging = true;
        
    }

    public void OnEndDrag()
    {
        Invoke("endDrag", 0.05f);
    }

    private void endDrag()
    {
        dragging = false;
    }

    public void OnBeginDragFromShop()
    {
        draggingFromShop = true;
    }

    public void OnEndDragFromShop()
    {
        draggingFromShop = false;
    }

    public bool buildBuilding(Building building, TerrenoEdificable terrenoEdificable)
    {
        if (!terrenoEdificable.hasBuilding)
        {
            terrenoEdificable.building = building;
            terrenoEdificable.building_prefab = building.gameObject;
            terrenoEdificable.hasBuilding = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void toggleBuildMode()
    {
        if (isOnBuildingMode){
            buildModeAnimator.Play("buildPanelClosing");
            isOnBuildingMode = false;
            Invoke("hideShop", 0.05f);
        }
        else
        {
            buildModeAnimator.Play("buildPanelOpening");
            isOnBuildingMode = true;
            shop.SetActive(true);
        }
    }

    public void hideAllShop()
    {
        if (isOnBuildingMode)
        {
            buildModeAnimator.Play("buildPanelClosing");
            isOnBuildingMode = false;            
            Invoke("hideShop", 0.05f);
            Invoke("hideFullShop", 0.05f);
        }
    }

    public void showAllShop()
    {
        if (!isOnBuildingMode)
        {
            allShop.SetActive(true);
        }
    }
    public void hideFullShop()
    {
        allShop.SetActive(false);
    }
    public void hideShop()
    {
        shop.SetActive(false);
    }


    public void openShop()
    {
        buildModeAnimator.Play("buildPanelOpening");
    }

    public void closeShop()
    {
        buildModeAnimator.Play("buildPanelClosing");
    }

    public void pointerEnter()
    {
        isOnCanvas = true;
    }

    public void pointerExit()
    {
        isOnCanvas = false;
    }
}
