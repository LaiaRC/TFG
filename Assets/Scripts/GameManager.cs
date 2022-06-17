using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

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
    public bool saved = false;
    public bool isPaused = false;
    public DateTime localDate;
    public int offlineTime = 0;

    public GameObject shop;
    public GameObject allShop;
    public GameObject descriptionDialog;
    public List<Sprite> resourcesIcons;
    public List<GameObject> buildings;
    public List<GameObject> constructionsBuilt;
    public TextMeshProUGUI saveText;
    public TextMeshProUGUI loadText;

    public Camera mainCamera;

    //Constructions dictionary
    public static int POS_X = 0;
    public static int POS_Y = 1;
    public static int LEVEL = 2;
    public static int ACTIVE_RESOURCE = 3;
    public static int TIME_LEFT = 4;
    public static int PRODUCING = 5;
    public static int NUM_TYPE = 6;
    public static int ACTIVE_RESOURCE_TIME = 7;

    //Player dictionary
    public static int HOUR = 0;
    public static int MIN = 1;
    public static int SEC = 2;


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

    public void Start()
    {    
        //carregar buildings a la bbdd
        Data.Instance.setBuildings(buildings);

        //Load game
        SaveManager.Instance.Load();
    }

    private void Update()
    {
        foreach (KeyValuePair<string, int> inventoryResource in Data.Instance.INVENTORY)
        {
            info += "\n -" + inventoryResource.Key + ": " + inventoryResource.Value;
        }
        //debugInventoryInfo.SetText("isDialogOpen: " + isDialogOpen + "\n" + "draggingItemShop: " + draggingItemShop + "\n" + "draggingFromShop: " + draggingFromShop + "\n");
        debugInventoryInfo.text = info;
        info = "";

        //Update after coming back from pause mode
        if (isPaused)
        {
            SaveManager.Instance.Load();
            isPaused = false;
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            isPaused = true;
            //Save when exiting game
            SaveManager.Instance.Save();
        }
    }

    public void updateLocalDate()
    {
        localDate = DateTime.Now;
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
            isOnCanvas = true;
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

    public void buildConstructions()
    {
        foreach (KeyValuePair<string, float[]> construction in Data.Instance.CONSTRUCTIONS)
        {
            for (int i = 0; i < buildings.Count; i++)
            {
                if (construction.Key.Contains(buildings[i].GetComponent<Building>().id))
                {
                    //Instantiate building prefab
                    Vector3 constructionPosition = new Vector3(construction.Value[POS_X], construction.Value[POS_Y], 0);
                    GameObject obj = Instantiate(buildings[i], constructionPosition, Quaternion.identity);
                    Building temp = obj.GetComponent<Building>();

                    //set building values
                    temp.level = (int)construction.Value[LEVEL];
                    temp.placed = true;
                    temp.activeResource = temp.resources[(int)construction.Value[ACTIVE_RESOURCE]];                   
                    if(construction.Value[PRODUCING] == 0){
                        temp.isProducing = false;
                       temp.pause();
                    }
                    else
                    {
                        temp.isProducing = true;
                        temp.play();
                    }

                    temp.time = construction.Value[ACTIVE_RESOURCE_TIME]  - construction.Value[TIME_LEFT];
                    temp.timeLeft = construction.Value[TIME_LEFT];
                    
                    temp.numTypeBuildings = (int)construction.Value[NUM_TYPE];
                    temp.activeResourceTime = construction.Value[ACTIVE_RESOURCE_TIME];
                    temp.updateUI();                    

                    Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(constructionPosition);
                    BoundsInt areaTemp = temp.area;
                    areaTemp.position = positionInt;

                    /*Debug.Log(
                        "-----------Construction--------" + "\nx: " +
                        construction.Value[GameManager.POS_X] + "\ny: " +
                        construction.Value[GameManager.POS_Y] + "\nlvl: " +
                        construction.Value[GameManager.LEVEL] + "\nar: " +
                        construction.Value[GameManager.ACTIVE_RESOURCE] + "\ntl: " +
                        construction.Value[GameManager.TIME_LEFT] + "\nip: " +
                        construction.Value[GameManager.PRODUCING] + "\nart: " +
                        construction.Value[GameManager.ACTIVE_RESOURCE_TIME] + "\n" +
                        "-----------temp------------\nx: " +
                        temp.transform.position.x + "\ny: "+
                        temp.transform.position.y + "\nlvl: " +
                        temp.level + "\naR: " +
                       temp.activeResource + "\ntl: " +
                       temp.timeLeft + "\nt: " +
                       temp.time + "\nip: " +
                       temp.isProducing + "\nart: " +
                       temp.activeResourceTime + "\n");*/

                    GridBuildingSystem.current.takeArea(areaTemp);


                    //Add building to built constructions list
                    constructionsBuilt.Add(obj);

                    //Add building to building_inventory dictionary
                    if (Data.Instance.BUILDING_INVENTORY.TryGetValue(buildings[i].GetComponent<Building>().id, out int quantity))
                    {
                        Data.Instance.BUILDING_INVENTORY[buildings[i].GetComponent<Building>().id] = quantity + 1;
                    }
                    else
                    {
                        //First building of that type
                        Data.Instance.BUILDING_INVENTORY.Add(buildings[i].GetComponent<Building>().id, 1);
                    }
                }
            }          
        }
    }

    public void calculateOfflineTime()
    {
        int previousTimeSeconds = Data.Instance.PLAYER["Hour"] * 3600 + Data.Instance.PLAYER["Minute"] * 60 + Data.Instance.PLAYER["Second"];

        updateLocalDate();

        int currentTimeSeconds = localDate.Hour * 3600 + localDate.Minute * 60 + localDate.Second;

        offlineTime = currentTimeSeconds - previousTimeSeconds;
    }
}
