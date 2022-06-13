using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    public bool isDialogOpen = false;


    public GameObject shop;
    public GameObject allShop;
    public GameObject descriptionDialog;


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
        //debugInventoryInfo.SetText("isDialogOpen: " + isDialogOpen + "\n" + "draggingItemShop: " + draggingItemShop + "\n" + "draggingFromShop: " + draggingFromShop + "\n");
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

    public void toggleBuildMode()
    {
        if (isOnBuildingMode){
            buildModeAnimator.Play("buildPanelClosing");
            isOnBuildingMode = false;
            Invoke("hideShop", 0.4f);
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
            Invoke("hideShop", 0.4f);
            Invoke("hideFullShop", 0.4f);
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
        //Hide description panel just in case
        hideDescriptionDialog();
        buildModeAnimator.Play("buildPanelClosing");
    }

    public void showDescriptionDialog()
    {
        isDialogOpen = true;
        canvas.GetComponent<Transform>().Find("BlackPanel").gameObject.SetActive(true);
        canvas.GetComponent<Transform>().Find("DescriptionDialog").gameObject.SetActive(true);
        canvas.GetComponent<Transform>().Find("UIBlock").gameObject.SetActive(false);
    }

    public void hideDescriptionDialog()
    {
        canvas.GetComponent<Transform>().Find("DescriptionDialog").gameObject.SetActive(false);
        canvas.GetComponent<Transform>().Find("BlackPanel").gameObject.SetActive(false);
        canvas.GetComponent<Transform>().Find("UIBlock").gameObject.SetActive(true);
        Invoke("setDialogOpen", 0.05f);
    }

    public void setDialogOpen()
    {
        //Must me done with a few seconds of delay or shop closes too when pressing dialog close button
        isDialogOpen = false;
    }

    public void pointerEnter()
    {
        isOnCanvas = true;
    }

    public void pointerExit()
    {
        isOnCanvas = false;
    }

    public bool checkRequirements(string buildingToBuild)
    {
        //At the moment only works for buildings (generalise to objectToBuild in the future)

        bool enoughResource = false;
        if (Data.Instance.BUILDINGS.TryGetValue(buildingToBuild, out GameObject building))
        {
            if (building.GetComponent<Building>().production_cost.Count > 0)
            {
                foreach (Requirement requeriment in building.GetComponent<Building>().production_cost)
                {
                    enoughResource = false;
                    if (Data.Instance.INVENTORY.TryGetValue(requeriment.resourceNameKey, out int quantity))
                    {
                        if (quantity >= requeriment.quantity)
                        {
                            //Tenim suficient de 1 dels resources necessaris falta comprovar si tenim suficient de tots
                            enoughResource = true;
                        }
                    }
                    //Et falta algun resource!
                    if (!enoughResource) return enoughResource;
                }
                if (enoughResource)
                {
                    return enoughResource;
                }
            }
            else
            {
                //The building is free
                enoughResource = true;
            }
        }
        return enoughResource;
    }

    public void buy(string buildingToBuild)
    {
        //Apply cost of shop item and update Inventory
        if (Data.Instance.BUILDINGS.TryGetValue(buildingToBuild, out GameObject building))
        {
            if (building.GetComponent<Building>().production_cost.Count > 0)
            {                
                foreach (Requirement requeriment in building.GetComponent<Building>().production_cost)
                {
                    if (Data.Instance.INVENTORY.TryGetValue(requeriment.resourceNameKey, out int quantity))
                    {
                        if (quantity >= requeriment.quantity)
                        {
                            quantity -= requeriment.quantity;
                            Data.Instance.updateInventory(requeriment.resourceNameKey, quantity);
                        }
                    }
                }
            }
        }
    }

    public void fillDescriptionDialog(string title, string description, Sprite image, Sprite iconResource1, Sprite iconResource2, Sprite iconResource3)
    {
        descriptionDialog.GetComponent<Transform>().Find("Title").GetComponent<TextMeshProUGUI>().text = title;
        descriptionDialog.GetComponent<Transform>().Find("Description").GetComponent<TextMeshProUGUI>().text = description;
        descriptionDialog.GetComponent<Transform>().Find("Image").GetComponent<Image>().sprite = image;
        descriptionDialog.GetComponent<Transform>().Find("IconResource1").GetComponent<Image>().sprite = iconResource1;
        descriptionDialog.GetComponent<Transform>().Find("IconResource2").GetComponent<Image>().sprite = iconResource2;
        descriptionDialog.GetComponent<Transform>().Find("IconResource3").GetComponent<Image>().sprite = iconResource3;
    }

    public void addInventory()
    {
        foreach (KeyValuePair<string, Resource> resource in Data.Instance.RESOURCES)
        {
            if(Data.Instance.INVENTORY.ContainsKey(resource.Key))
            {
                Data.Instance.INVENTORY.Remove(resource.Key);
            }
            Data.Instance.INVENTORY.Add(resource.Key, 99999);
        }
    }
}
