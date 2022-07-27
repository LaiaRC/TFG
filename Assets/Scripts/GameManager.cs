using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

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
    public float offlineTime = 0;
    public int offlineBoostMultiplier = 100; //600
    public float offlineBoostTime = 0;
    public bool isHellfireUnlocked = false;

    public GameObject shop;
    public GameObject allShop;
    public GameObject descriptionDialog;
    public GameObject offlineDialog;
    public GameObject boostShop;
    public List<Sprite> resourcesIcons;
    public List<Sprite> monstersIcons;
    public List<Sprite> dropsIcons;
    public List<Sprite> villagersIcons;
    public List<GameObject> buildings;
    public List<GameObject> constructionsBuilt;
    public TextMeshProUGUI saveText;
    public TextMeshProUGUI loadText;
    public Button boostTimeButton;
    public List<String> monstersKeys;
    public List<String> dropsKeys;
    public List<ShopItemHolder> buildingShopItems = new List<ShopItemHolder>();
    public List<string> unlockedMonsters = new List<string>();
    public string hidenMonster = "jackOLantern";
    public int hidenMonsterIndex = 1;

    public Camera mainCamera;

    //Constructions dictionary
    public static int POS_X = 0;
    public static int POS_Y = 1;
    public static int LEVEL = 2; //HIDDEN_MONSTER_INDEX (summoningCircle)
    public static int ACTIVE_RESOURCE = 3;
    public static int TIME_LEFT = 4;
    public static int PRODUCING = 5;
    public static int PAUSED = 6;
    public static int NUM_TYPE = 7;
    public static int ACTIVE_RESOURCE_TIME = 8;

    //Player dictionary
    public static int HOUR = 0;
    public static int MIN = 1;
    public static int SEC = 2;
    public static int DAY = 3;
    public static int MONTH = 4;
    public static int YEAR = 5;

    //Monster stats dictionary
    public static int IS_UNLOCKED = 0;
    public static int UPGRADE_LEVEL = 1;
    public static int HIDDEN_MONSTER_INDEX = 2;
    //public static int QUANTITY = 2; //The quantity of each monster produced

    private string info = "";
    private bool offlineBoostApplied = false;

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

        #region FILL LIST KEYS
        monstersKeys.Add(Data.SKELETON);
        monstersKeys.Add(Data.JACK_LANTERN);
        monstersKeys.Add(Data.BAT);
        monstersKeys.Add(Data.GOBLIN);
        monstersKeys.Add(Data.GHOST);
        monstersKeys.Add(Data.CLOWN);
        monstersKeys.Add(Data.ZOMBIE);
        monstersKeys.Add(Data.VAMPIRE);
        monstersKeys.Add(Data.WITCH);
        monstersKeys.Add(Data.REAPER);

        dropsKeys.Add(Data.LOLLIPOP);
        dropsKeys.Add(Data.RING);
        dropsKeys.Add(Data.BEER);
        dropsKeys.Add(Data.SWORD);
        dropsKeys.Add(Data.SHIELD);
        dropsKeys.Add(Data.STICK);
        dropsKeys.Add(Data.GEM);
        dropsKeys.Add(Data.SCARE);
        #endregion
    }
    #endregion

    public void Start()
    {    
        //carregar buildings a la bbdd
        Data.Instance.setBuildings(buildings);

        //Load game
        SaveManager.Instance.Load();

        if (hidenMonster == "")
        {
            hidenMonster = "jackOLantern"; //Just in case
        }

        //Just to debug (hardcoded drops) (si ja estan afegits peta)
        /*Data.Instance.INVENTORY.Add(Data.LOLLIPOP, 50);
        Data.Instance.INVENTORY.Add(Data.RING, 50);
        Data.Instance.INVENTORY.Add(Data.BEER, 50);
        Data.Instance.INVENTORY.Add(Data.SWORD, 50);
        Data.Instance.INVENTORY.Add(Data.SHIELD, 50);
        Data.Instance.INVENTORY.Add(Data.STICK, 50);
        Data.Instance.INVENTORY.Add(Data.GEM, 50);*/
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
            //produceOfflineResources();
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

    public void showOfflineDialog()
    {
        isDialogOpen = true;
        canvas.GetComponent<Transform>().Find("BlackPanel").gameObject.SetActive(true);
        canvas.GetComponent<Transform>().Find("OfflineDialog").gameObject.SetActive(true);
        canvas.GetComponent<Transform>().Find("UIBlock").gameObject.SetActive(false);
    }

    public void hideOfflineDialog()
    {
        canvas.GetComponent<Transform>().Find("OfflineDialog").gameObject.SetActive(false);
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
                foreach (RequirementBuilding requeriment in building.GetComponent<Building>().production_cost)
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
                foreach (RequirementBuilding requeriment in building.GetComponent<Building>().production_cost)
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

    public void fillOfflineDialog(string description, string boostTimeText)
    {
        offlineDialog.GetComponent<Transform>().Find("Description").GetComponent<TextMeshProUGUI>().text = description;
        offlineDialog.GetComponent<Transform>().Find("BoostTimeText").GetComponent<TextMeshProUGUI>().text = boostTimeText;
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

                    Vector3Int positionIntAux = GridBuildingSystem.current.gridLayout.LocalToCell(constructionPosition);
                    BoundsInt areaTempAux = buildings[i].GetComponent<Building>().area;
                    areaTempAux.position = positionIntAux;

                    if (GridBuildingSystem.current.canTakeArea(areaTempAux))
                    {
                        GameObject obj = Instantiate(buildings[i], constructionPosition, Quaternion.identity);
                        if (construction.Key.Contains("summoningCircle"))
                        {
                            #region SUMMONING CIRCLE
                            //It's summoning circle
                            SummoningCircle temp = obj.GetComponent<SummoningCircle>();
                            //set building values
                            hidenMonsterIndex = (int)construction.Value[LEVEL];
                            hidenMonster = temp.getHiddenMonster(hidenMonsterIndex);
                            temp.placed = true;

                            temp.activeMonster = temp.getStringMonster((int)construction.Value[ACTIVE_RESOURCE]);
                            if (construction.Value[PRODUCING] == 0)
                            {
                                temp.isProducing = false;
                            }
                            else
                            {
                                temp.isProducing = true;
                            }

                            if (construction.Value[PAUSED] == 0)
                            {
                                temp.isPaused = false;
                                temp.play();
                            }
                            else
                            {
                                temp.isPaused = true;
                                temp.pause();
                            }

                            temp.time = construction.Value[ACTIVE_RESOURCE_TIME] - construction.Value[TIME_LEFT];
                            temp.timeLeft = construction.Value[TIME_LEFT];

                            temp.numTypeBuildings = (int)construction.Value[NUM_TYPE];
                            temp.activeResourceTime = construction.Value[ACTIVE_RESOURCE_TIME];
                            temp.activeMonsterTime = construction.Value[ACTIVE_RESOURCE_TIME];
                            temp.setSelectedTab(temp.activeMonster);
                            temp.setUI(temp.activeMonster);
                            temp.setActiveMonsterUI(temp.activeMonster);
                            Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(constructionPosition);
                            BoundsInt areaTemp = temp.area;
                            areaTemp.position = positionInt;
                            GridBuildingSystem.current.takeArea(areaTemp);
                            #endregion
                        }
                        else
                        {
                            #region GENERAL BUILDING
                            Building temp = obj.GetComponent<Building>();
                            //set building values
                            temp.level = (int)construction.Value[LEVEL];
                            temp.placed = true;

                            temp.activeResource = temp.resources[(int)construction.Value[ACTIVE_RESOURCE]];
                            if (construction.Value[PRODUCING] == 0)
                            {
                                temp.isProducing = false;
                            }
                            else
                            {
                                temp.isProducing = true;
                            }

                            if (construction.Value[PAUSED] == 0)
                            {
                                temp.isPaused = false;
                                temp.play();
                            }
                            else
                            {
                                temp.isPaused = true;
                                temp.pause();
                            }

                            temp.time = construction.Value[ACTIVE_RESOURCE_TIME] - construction.Value[TIME_LEFT];
                            temp.timeLeft = construction.Value[TIME_LEFT];

                            temp.numTypeBuildings = (int)construction.Value[NUM_TYPE];
                            temp.activeResourceTime = construction.Value[ACTIVE_RESOURCE_TIME];
                            temp.updateUI();
                            Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(constructionPosition);
                            BoundsInt areaTemp = temp.area;
                            areaTemp.position = positionInt;
                            GridBuildingSystem.current.takeArea(areaTemp);
                            #endregion
                        }

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
    }

    public void calculateOfflineTime()
    {
        DateTime previousDateTime = new DateTime(Data.Instance.PLAYER["Year"], Data.Instance.PLAYER["Month"], Data.Instance.PLAYER["Day"], Data.Instance.PLAYER["Hour"], Data.Instance.PLAYER["Minute"], Data.Instance.PLAYER["Second"]);

        updateLocalDate();

        offlineTime = (float)(localDate - previousDateTime).TotalSeconds;

        float maxTimeOut = 18000;

        if(offlineTime > maxTimeOut)
        {
            offlineTime = maxTimeOut;
        }

        offlineBoostTime = offlineTime;

        if (offlineBoostTime >= 1) {

            //Show offline dialog
            TimeSpan offlineTimeAux = localDate - previousDateTime; //in hours, min, etc
            calculateOfflineDialogText(offlineTimeAux);
            showOfflineDialog();

            boostTimeButton.gameObject.SetActive(true);
            boostTimeButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Apply boost " + (offlineBoostTime/offlineBoostMultiplier).ToString("F2") + "s");
        }
    }

    public void calculateOfflineDialogText(TimeSpan offlineTimeAux)
    {
        if (offlineTimeAux.Hours > 0)
        {
            if (offlineTimeAux.Minutes > 0)
            {
                if (offlineTimeAux.Seconds > 0)
                {
                    fillOfflineDialog("You were away for " + offlineTimeAux.Hours + "h " + offlineTimeAux.Minutes + "min " + offlineTimeAux.Seconds + "s", "Boost time -> " + (offlineBoostTime / offlineBoostMultiplier).ToString("F2") + "s");
                }
                else
                {
                    fillOfflineDialog("You were away for " + offlineTimeAux.Hours + "h " + offlineTimeAux.Minutes + "min", "Boost time -> " + (offlineBoostTime / offlineBoostMultiplier).ToString("F2") + "s");
                }
            }
            else
            {
                if (offlineTimeAux.Seconds > 0)
                {
                    fillOfflineDialog("You were away for " + offlineTimeAux.Hours + "h " + offlineTimeAux.Seconds + "s", "Boost time -> " + (offlineBoostTime / offlineBoostMultiplier).ToString("F2") + "s");

                }
                else
                {
                    fillOfflineDialog("You were away for " + offlineTimeAux.Hours + "h", "Boost time -> " + (offlineBoostTime / offlineBoostMultiplier).ToString("F2") + "s");

                }
            }
        }
        else
        {
            if (offlineTimeAux.Minutes > 0)
            {
                if (offlineTimeAux.Seconds > 0)
                {
                    fillOfflineDialog("You were away for " + offlineTimeAux.Minutes + "min " + offlineTimeAux.Seconds + "s", "Boost time -> " + (offlineBoostTime / offlineBoostMultiplier).ToString("F2") + "s");

                }
                else
                {
                    fillOfflineDialog("You were away for " + offlineTimeAux.Minutes + "min", "Boost time -> " + (offlineBoostTime / offlineBoostMultiplier).ToString("F2") + "s");

                }
            }
            else
            {
                fillOfflineDialog("You were away for " + offlineTimeAux.Seconds + "s", "Boost time -> " + (offlineBoostTime / offlineBoostMultiplier).ToString("F2") + "s");

            }
        }
    }

    public void applyBoostTime()
    {
        if (!offlineBoostApplied)
        {
            Time.timeScale = offlineBoostMultiplier;
            Invoke("stopOfflineProduction", offlineBoostTime);
        }
    }

    public void stopOfflineProduction()
    {
        boostTimeButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Boosted");
        boostTimeButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        Time.timeScale = 1;
        offlineBoostApplied = true;
    }

    public void setMonstersDictionary()
    {
        foreach (KeyValuePair<string, int[]> monsterStats in Data.Instance.MONSTERS_STATS)
        {
            if (Data.Instance.MONSTERS.TryGetValue(monsterStats.Key, out MonsterInfo monster))
            {
                if (monsterStats.Value[IS_UNLOCKED] == 0)
                {
                    monster.isUnlocked = false;
                }
                else
                {
                    monster.isUnlocked = true;
                }

                monster.upgradeLevel = monsterStats.Value[UPGRADE_LEVEL];
            }
        }
    }

    public void loadMiniGame()
    {
        SceneManager.LoadScene("miniGame");
    }

    public void applyBoost(string id)
    {
        switch (id)
        {
            case "crypt":
                //Unlock the crypt
                foreach (ShopItemHolder item in buildingShopItems)
                {
                    if (item.titleText.text.Contains("Crypt"))
                    {
                        item.isLocked = false;
                        item.setToUnlocked();
                    }
                }
                
            break;

            case "magicWorkshop":
                //Unlock the magic workshop
                foreach (ShopItemHolder item in buildingShopItems)
                {
                    if (item.titleText.text.Contains("Magic Workshop"))
                    {
                        item.isLocked = false;
                        item.setToUnlocked();
                    }
                }

                break;

            case "deepForest":
                //Unlock the deep forest
                foreach (ShopItemHolder item in buildingShopItems)
                {
                    if (item.titleText.text.Contains("Deep Forest"))
                    {
                        item.isLocked = false;
                        item.setToUnlocked();
                    }
                }

                break;

            case "hellfire":
                //Unlock the hellfire Zone
                isHellfireUnlocked = true;
                foreach (GameObject building in constructionsBuilt)
                {
                    if(building.GetComponent<Building>() != null && building.GetComponent<Building>().id.Equals("hellIsland"))
                    {
                        building.GetComponent<Building>().updateUI();
                    }
                }

                break;

            case "graveyard":
                //Unlock another graveyard
                foreach (ShopItemHolder item in buildingShopItems)
                {
                    if (item.titleText.text.Contains("Graveyard"))
                    {
                        item.maxQuantity += 1;
                        item.updateTextAmount(item.currentQuantity);
                    }
                }

                break;

            case "forest":
                //Unlock another graveyard
                foreach (ShopItemHolder item in buildingShopItems)
                {
                    if (item.titleText.text.Contains("Forest"))
                    {
                        item.maxQuantity += 1;
                        item.updateTextAmount(item.currentQuantity);
                    }
                }

                break;

            case "vegetablePatch":
                //Unlock another graveyard
                foreach (ShopItemHolder item in buildingShopItems)
                {
                    if (item.titleText.text.Contains("Vegetable"))
                    {
                        item.maxQuantity += 1;
                        item.updateTextAmount(item.currentQuantity);
                    }
                }

                break;

            case "swamp":
                //Unlock another graveyard
                foreach (ShopItemHolder item in buildingShopItems)
                {
                    if (item.titleText.text.Contains("Swamp"))
                    {
                        item.maxQuantity += 1;
                        item.updateTextAmount(item.currentQuantity);
                    }
                }

                break;

            case "abandonedHospital":
                //Unlock another graveyard
                foreach (ShopItemHolder item in buildingShopItems)
                {
                    if (item.titleText.text.Contains("Abandoned"))
                    {
                        item.maxQuantity += 1;
                        item.updateTextAmount(item.currentQuantity);
                    }
                }

                break;

            case "crypt2":
                //Unlock another graveyard
                foreach (ShopItemHolder item in buildingShopItems)
                {
                    if (item.titleText.text.Contains("Crypt"))
                    {
                        item.maxQuantity += 1;
                        item.updateTextAmount(item.currentQuantity);
                    }
                }

                break;

            case "jackOLantern":

                if (Data.Instance.MONSTERS.TryGetValue(Data.JACK_LANTERN, out MonsterInfo monsterAux1))
                {
                    if (!monsterAux1.isUnlocked)
                    {
                        monsterAux1.isUnlocked = true;

                        //Update monster stats dictionary
                        Data.Instance.MONSTERS_STATS[Data.JACK_LANTERN] = new int[] { 1, 1 };

                        unlockedMonsters.Add(Data.JACK_LANTERN);
                        hidenMonster = Data.BAT;
                        hidenMonsterIndex = 2;

                        //Get summoning circle
                        for (int i = 0; i < constructionsBuilt.Count; i++)
                        {
                            if (constructionsBuilt[i].GetComponent<SummoningCircle>() != null)
                            {
                                constructionsBuilt[i].GetComponent<SummoningCircle>().setUI(constructionsBuilt[i].GetComponent<SummoningCircle>().selectedTab);
                            }
                        }

                        boostShop.GetComponent<BoostShop>().setMonsterTabs();
                    }
                }

                break;

            case "bat":

                if (Data.Instance.MONSTERS.TryGetValue(Data.BAT, out MonsterInfo monsterAux))
                {
                    if (!monsterAux.isUnlocked)
                    {
                        monsterAux.isUnlocked = true;

                        //Update monster stats dictionary
                        Data.Instance.MONSTERS_STATS[Data.BAT] = new int[] { 1, 1 };

                        unlockedMonsters.Add(Data.BAT);
                        hidenMonster = Data.GOBLIN;
                        hidenMonsterIndex = 3;
                        //Get summoning circle
                        for (int i = 0; i < constructionsBuilt.Count; i++)
                        {
                            if (constructionsBuilt[i].GetComponent<SummoningCircle>() != null)
                            {
                                constructionsBuilt[i].GetComponent<SummoningCircle>().setUI(constructionsBuilt[i].GetComponent<SummoningCircle>().selectedTab);
                            }
                        }

                        boostShop.GetComponent<BoostShop>().setMonsterTabs();
                    }
                }

                break;

            case "goblin":

                if (Data.Instance.MONSTERS.TryGetValue(Data.GOBLIN, out MonsterInfo monsterAux2))
                {
                    if (!monsterAux2.isUnlocked)
                    {
                        monsterAux2.isUnlocked = true;

                        //Update monster stats dictionary
                        Data.Instance.MONSTERS_STATS[Data.GOBLIN] = new int[] { 1, 1 };

                        unlockedMonsters.Add(Data.GOBLIN);
                        hidenMonster = Data.GHOST;
                        hidenMonsterIndex = 4;

                        //Get summoning circle
                        for (int i = 0; i < constructionsBuilt.Count; i++)
                        {
                            if (constructionsBuilt[i].GetComponent<SummoningCircle>() != null)
                            {
                                constructionsBuilt[i].GetComponent<SummoningCircle>().setUI(constructionsBuilt[i].GetComponent<SummoningCircle>().selectedTab);
                            }
                        }

                        boostShop.GetComponent<BoostShop>().setMonsterTabs();
                    }
                }

                break;

            case "ghost":

                if (Data.Instance.MONSTERS.TryGetValue(Data.GHOST, out MonsterInfo monsterAux3))
                {
                    if (!monsterAux3.isUnlocked)
                    {
                        monsterAux3.isUnlocked = true;

                        //Update monster stats dictionary
                        Data.Instance.MONSTERS_STATS[Data.GHOST] = new int[] { 1, 1 };

                        unlockedMonsters.Add(Data.GHOST);
                        hidenMonster = Data.CLOWN;
                        hidenMonsterIndex = 5;

                        //Get summoning circle
                        for (int i = 0; i < constructionsBuilt.Count; i++)
                        {
                            if (constructionsBuilt[i].GetComponent<SummoningCircle>() != null)
                            {
                                constructionsBuilt[i].GetComponent<SummoningCircle>().setUI(constructionsBuilt[i].GetComponent<SummoningCircle>().selectedTab);
                            }
                        }

                        boostShop.GetComponent<BoostShop>().setMonsterTabs();
                    }
                }

                break;

            case "clown":

                if (Data.Instance.MONSTERS.TryGetValue(Data.CLOWN, out MonsterInfo monsterAux4))
                {
                    if (!monsterAux4.isUnlocked)
                    {
                        monsterAux4.isUnlocked = true;

                        //Update monster stats dictionary
                        Data.Instance.MONSTERS_STATS[Data.CLOWN] = new int[] { 1, 1 };

                        unlockedMonsters.Add(Data.CLOWN);
                        hidenMonster = Data.ZOMBIE;
                        hidenMonsterIndex = 6;

                        //Get summoning circle
                        for (int i = 0; i < constructionsBuilt.Count; i++)
                        {
                            if (constructionsBuilt[i].GetComponent<SummoningCircle>() != null)
                            {
                                constructionsBuilt[i].GetComponent<SummoningCircle>().setUI(constructionsBuilt[i].GetComponent<SummoningCircle>().selectedTab);
                            }
                        }

                        boostShop.GetComponent<BoostShop>().setMonsterTabs();
                    }
                }

                break;

            case "zombie":

                if (Data.Instance.MONSTERS.TryGetValue(Data.ZOMBIE, out MonsterInfo monsterAux5))
                {
                    if (!monsterAux5.isUnlocked)
                    {
                        monsterAux5.isUnlocked = true;

                        //Update monster stats dictionary
                        Data.Instance.MONSTERS_STATS[Data.ZOMBIE] = new int[] { 1, 1 };

                        unlockedMonsters.Add(Data.ZOMBIE);
                        hidenMonster = Data.VAMPIRE;
                        hidenMonsterIndex = 7;

                        //Get summoning circle
                        for (int i = 0; i < constructionsBuilt.Count; i++)
                        {
                            if (constructionsBuilt[i].GetComponent<SummoningCircle>() != null)
                            {
                                constructionsBuilt[i].GetComponent<SummoningCircle>().setUI(constructionsBuilt[i].GetComponent<SummoningCircle>().selectedTab);
                            }
                        }

                        boostShop.GetComponent<BoostShop>().setMonsterTabs();
                    }
                }

                break;

            case "vampire":

                if (Data.Instance.MONSTERS.TryGetValue(Data.VAMPIRE, out MonsterInfo monsterAux6))
                {
                    if (!monsterAux6.isUnlocked)
                    {
                        monsterAux6.isUnlocked = true;

                        //Update monster stats dictionary
                        Data.Instance.MONSTERS_STATS[Data.VAMPIRE] = new int[] { 1, 1 };

                        unlockedMonsters.Add(Data.VAMPIRE);
                        hidenMonster = Data.WITCH;
                        hidenMonsterIndex = 8;
                        //Get summoning circle
                        for (int i = 0; i < constructionsBuilt.Count; i++)
                        {
                            if (constructionsBuilt[i].GetComponent<SummoningCircle>() != null)
                            {
                                constructionsBuilt[i].GetComponent<SummoningCircle>().setUI(constructionsBuilt[i].GetComponent<SummoningCircle>().selectedTab);
                            }
                        }

                        boostShop.GetComponent<BoostShop>().setMonsterTabs();
                    }
                }

                break;

            case "witch":

                if (Data.Instance.MONSTERS.TryGetValue(Data.WITCH, out MonsterInfo monsterAux7))
                {
                    if (!monsterAux7.isUnlocked)
                    {
                        monsterAux7.isUnlocked = true;

                        //Update monster stats dictionary
                        Data.Instance.MONSTERS_STATS[Data.WITCH] = new int[] { 1, 1 };

                        unlockedMonsters.Add(Data.WITCH);
                        hidenMonster = Data.REAPER;
                        hidenMonsterIndex = 9;
                        //Get summoning circle
                        for (int i = 0; i < constructionsBuilt.Count; i++)
                        {
                            if (constructionsBuilt[i].GetComponent<SummoningCircle>() != null)
                            {
                                constructionsBuilt[i].GetComponent<SummoningCircle>().setUI(constructionsBuilt[i].GetComponent<SummoningCircle>().selectedTab);
                            }
                        }

                        boostShop.GetComponent<BoostShop>().setMonsterTabs();
                    }
                }

                break;

            case "reaper":

                if (Data.Instance.MONSTERS.TryGetValue(Data.REAPER, out MonsterInfo monsterAux8))
                {
                    if (!monsterAux8.isUnlocked)
                    {
                        monsterAux8.isUnlocked = true;

                        //Update monster stats dictionary
                        Data.Instance.MONSTERS_STATS[Data.REAPER] = new int[] { 1, 1 };

                        unlockedMonsters.Add(Data.REAPER);
                        hidenMonster = "none";
                        hidenMonsterIndex = 100;
                        //Get summoning circle
                        for (int i = 0; i < constructionsBuilt.Count; i++)
                        {
                            if (constructionsBuilt[i].GetComponent<SummoningCircle>() != null)
                            {
                                constructionsBuilt[i].GetComponent<SummoningCircle>().setUI(constructionsBuilt[i].GetComponent<SummoningCircle>().selectedTab);
                            }
                        }

                        boostShop.GetComponent<BoostShop>().setMonsterTabs();
                    }
                }

                break;
        }
    }

    public void applyAllBoosts()
    {
        //When loading the game
        foreach (KeyValuePair<string, int> boost in Data.Instance.BOOSTS)
        {
            if(boost.Value == 1) //Maybe can be applied twice or more (>= 1 ?)
            {
                applyBoost(boost.Key);
            }
        }
    }
}
