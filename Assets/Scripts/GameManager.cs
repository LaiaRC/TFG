using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class InventoryObject
{
    public Sprite icon;
    public string quantity;
}

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI debugInventoryInfo;
    public Animator buildModeAnimator;
    public bool isOnBuildingMode = false;
    public bool isShopOpeningOrClosing = false;
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
    public float offlineBoostTimeModifier = 0;
    public bool isHellfireUnlocked = false;
    public int isTutoDone = 0;
    public int isRestart = 0;
    public GameObject loadingScreen;
    public AudioClip pumpkinLaugh;
    public AudioSource audioSource;

    public GameObject shop;
    public GameObject allShop;
    public GameObject descriptionDialog;
    public GameObject offlineDialog;
    public GameObject inventoryDialog;
    public GameObject portalDialog;
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
    public string hidenMonster = "";
    public int hidenMonsterIndex = 0;
    public GameObject resourcePanel;
    public GameObject inventoryContainer;

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
    public static int CONSTRUCTION_TYPE = 9;
    public static int IS_PRODUCER = 10;
    public static int IS_CONVERTER = 11;

    //Player dictionary
    public static int HOUR = 0;

    public static int MIN = 1;
    public static int SEC = 2;
    public static int DAY = 3;
    public static int MONTH = 4;
    public static int YEAR = 5;
    public static int TUTO_DONE = 6;
    public static int IS_RESTART = 7;

    //Monster stats dictionary
    public static int IS_UNLOCKED = 0;

    public static int UPGRADE_LEVEL = 1;
    public static int HIDDEN_MONSTER_INDEX = 2;
    //public static int QUANTITY = 2; //The quantity of each monster produced

    public static float LOAD_TIME = 10;
    public static float REDUCTION_FACTOR = 1;
    public static float TIME_ESCALE_MULTIPLIER = 100;

    //BOOST VARIABLES
    public static int PRODUCER_BOOST = 0;

    public static int CONVERTER_BOOST = 0;

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

        #endregion FILL LIST KEYS
    }

    #endregion SINGLETON PATTERN

    public void Start()
    {
        //carregar buildings a la bbdd
        Data.Instance.setBuildings(buildings);

        //Load game
        SaveManager.Instance.Load();

        hideAllDialogs();

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
            //for (int i = 0; i < monstersKeys.Count; i++)
            //{
            //  if (inventoryResource.Key.Equals(monstersKeys[i]))
            //{
            info += "\n -" + inventoryResource.Key + ": " + inventoryResource.Value;
            // }
            //}
        }
        //debugInventoryInfo.SetText("isDialogOpen: " + isDialogOpen + "\n" + "draggingItemShop: " + draggingItemShop + "\n" + "draggingFromShop: " + draggingFromShop + "\n");
        debugInventoryInfo.text = info;
        info = "";

        //Update after coming back from pause mode
        if (isPaused)
        {
            //Load loadin scene
            SceneManager.LoadScene("loadingScene");

            //SaveManager.Instance.Load();
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
        if (isOnBuildingMode && !isShopOpeningOrClosing)
        {
            buildModeAnimator.Play("buildPanelClosing");
            isOnBuildingMode = false;
            Invoke("hideShop", 0.4f);
            isShopOpeningOrClosing = true;
        }
        else if (!isShopOpeningOrClosing)
        {
            isOnCanvas = true;
            buildModeAnimator.Play("buildPanelOpening");
            isOnBuildingMode = true;
            shop.SetActive(true);
        }
    }

    public void hideAllShop()
    {
        if (isOnBuildingMode && !isShopOpeningOrClosing)
        {
            buildModeAnimator.Play("buildPanelClosing");
            isOnBuildingMode = false;
            Invoke("hideShop", 0.4f);
            Invoke("hideFullShop", 0.4f);
            isShopOpeningOrClosing = true;
        }
    }

    public void showAllShop()
    {
        if (!isOnBuildingMode && !isShopOpeningOrClosing)
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
        isShopOpeningOrClosing = false;
        shop.SetActive(false);
    }

    public void openShop()
    {
        buildModeAnimator.Play("buildPanelOpening");
    }

    public void closeShop()
    {
        //Hide description panel just in case
        if (!isOnBuildingMode)
        {
            hideDescriptionDialog();
            buildModeAnimator.Play("buildPanelClosing");
        }
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

    public void showInventoryDialog()
    {
        //Load inventory info (only updated when inventory opens)
        fillInventoryDialog();

        isDialogOpen = true;
        canvas.GetComponent<Transform>().Find("BlackPanel").gameObject.SetActive(true);
        inventoryDialog.SetActive(true);
        canvas.GetComponent<Transform>().Find("UIBlock").gameObject.SetActive(false);
    }

    public void hideInventoryDialog()
    {
        inventoryDialog.SetActive(false);
        canvas.GetComponent<Transform>().Find("BlackPanel").gameObject.SetActive(false);
        canvas.GetComponent<Transform>().Find("UIBlock").gameObject.SetActive(true);
        Invoke("setDialogOpen", 0.05f);
    }

    public void showPortalDialog()
    {
        if (!isOnCanvas)
        {
            //check if has at least one monster
            int numMonsters = 0;

            for (int i = 0; i < monstersKeys.Count; i++)
            {
                if (Data.Instance.INVENTORY.ContainsKey(monstersKeys[i]))
                {
                    numMonsters += Data.Instance.INVENTORY[monstersKeys[i]];
                }
            }

            if (numMonsters > 0)
            {
                portalDialog.transform.Find("MonsterText").gameObject.SetActive(false);

                //Load monster cards (TODO)
            }
            else
            {
                portalDialog.transform.Find("MonsterText").gameObject.SetActive(true);
            }

            isDialogOpen = true;
            canvas.GetComponent<Transform>().Find("BlackPanel").gameObject.SetActive(true);
            portalDialog.SetActive(true);
            canvas.GetComponent<Transform>().Find("UIBlock").gameObject.SetActive(false);
        }
    }

    public void hidePortalDialog()
    {
        portalDialog.SetActive(false);
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

    public bool checkRequirements(string id)
    {
        bool enoughResource = false;

        if (Data.Instance.BUILDINGS.TryGetValue(id, out GameObject building))
        {
            if (building.GetComponent<Construction>().production_cost.Count > 0)
            {
                if (building.GetComponent<Construction>().production_cost[0].list.Count > 0)
                {
                    foreach (RequirementBuilding requeriment in building.GetComponent<Construction>().production_cost[0].list)
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
            int num = 0;
            //Get num of constructions
            if (Data.Instance.BUILDING_INVENTORY.TryGetValue(buildingToBuild, out int numConstruction))
            {
                num = numConstruction;
            }

            //check if it's free
            if (building.GetComponent<Construction>().production_cost.Count > 0)
            {
                if (building.GetComponent<Construction>().production_cost[num].list.Count > 0)
                {
                    foreach (RequirementBuilding requeriment in building.GetComponent<Construction>().production_cost[num].list)
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
    }

    public void fillDescriptionDialog(string title, string description, Sprite image, Sprite iconResource1, Sprite iconResource2, Sprite iconResource3, string constructionType)
    {
        descriptionDialog.GetComponent<Transform>().Find("Title").GetComponent<TextMeshProUGUI>().text = title;

        //change word color if it's decoration boost
        switch (title)
        {
            case "Gargoyle":
                descriptionDialog.GetComponent<Transform>().Find("Description").GetComponent<TextMeshProUGUI>().SetText("<color=#00FFEF>Producer</color>");
                descriptionDialog.GetComponent<Transform>().Find("Description").GetComponent<TextMeshProUGUI>().SetText(descriptionDialog.GetComponent<Transform>().Find("Description").GetComponent<TextMeshProUGUI>().text + description);
                break;

            case "Obelisk":
                descriptionDialog.GetComponent<Transform>().Find("Description").GetComponent<TextMeshProUGUI>().SetText("<color=#FF6E00>Converter</color>");
                descriptionDialog.GetComponent<Transform>().Find("Description").GetComponent<TextMeshProUGUI>().SetText(descriptionDialog.GetComponent<Transform>().Find("Description").GetComponent<TextMeshProUGUI>().text + description);
                break;

            case "Blood Moon Tower":
                descriptionDialog.GetComponent<Transform>().Find("Description").GetComponent<TextMeshProUGUI>().SetText(description + "<color=#24EE00>Summoning Circle</color>");
                break;

            case "Mage Guardian":
                descriptionDialog.GetComponent<Transform>().Find("Description").GetComponent<TextMeshProUGUI>().SetText("Gain 50% more      <sprite=4>in the restart minigame");
                break;

            default:
                descriptionDialog.GetComponent<Transform>().Find("Description").GetComponent<TextMeshProUGUI>().SetText(description);
                break;
        }

        descriptionDialog.GetComponent<Transform>().Find("Image").GetComponent<Image>().sprite = image;

        if (constructionType.Equals("decorationBoost") || constructionType.Equals("summoningCircle"))
        {
            descriptionDialog.GetComponent<Transform>().Find("Produces").gameObject.SetActive(false);
            descriptionDialog.GetComponent<Transform>().Find("IconResource1").gameObject.SetActive(false);
            descriptionDialog.GetComponent<Transform>().Find("IconResource2").gameObject.SetActive(false);
            descriptionDialog.GetComponent<Transform>().Find("IconResource3").gameObject.SetActive(false);
        }
        else
        {
            descriptionDialog.GetComponent<Transform>().Find("Produces").gameObject.SetActive(true);
            descriptionDialog.GetComponent<Transform>().Find("IconResource1").gameObject.SetActive(true);
            descriptionDialog.GetComponent<Transform>().Find("IconResource2").gameObject.SetActive(true);
            descriptionDialog.GetComponent<Transform>().Find("IconResource3").gameObject.SetActive(true);

            descriptionDialog.GetComponent<Transform>().Find("IconResource1").GetComponent<Image>().sprite = iconResource1;
            descriptionDialog.GetComponent<Transform>().Find("IconResource2").GetComponent<Image>().sprite = iconResource2;
            descriptionDialog.GetComponent<Transform>().Find("IconResource3").GetComponent<Image>().sprite = iconResource3;
        }

        //Type
        descriptionDialog.GetComponent<Transform>().Find("Producer").gameObject.SetActive(false); 
        descriptionDialog.GetComponent<Transform>().Find("Converter").gameObject.SetActive(false);
        descriptionDialog.GetComponent<Transform>().Find("SummoningCircle").gameObject.SetActive(false);
        descriptionDialog.GetComponent<Transform>().Find("DecorationBoost").gameObject.SetActive(false);

        switch (constructionType){
            case "producer":
                descriptionDialog.GetComponent<Transform>().Find("Producer").gameObject.SetActive(true);
                break;
            case "converter":
                descriptionDialog.GetComponent<Transform>().Find("Converter").gameObject.SetActive(true);
                break;
            case "summoningCircle":
                descriptionDialog.GetComponent<Transform>().Find("SummoningCircle").gameObject.SetActive(true);
                break;
            case "decorationBoost":
                descriptionDialog.GetComponent<Transform>().Find("DecorationBoost").gameObject.SetActive(true);
                break;
        }        
    }

    public void fillOfflineDialog(string timeAwayText)
    {
        //Calculate max time out
        int timeBoost = 4; //default 4h
        if (Data.Instance.BOOSTS.TryGetValue(Data.OFFLINE_MAXTIME_BOOST, out int quantity))
        {
            timeBoost += quantity;
        }

        offlineDialog.transform.Find("TimePanel").transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(timeAwayText + "/" + timeBoost.ToString() + "h");

        //Calculate info text
        string prod = "25%";
        if (Data.Instance.BOOSTS.TryGetValue(Data.OFFLINE_PRODUCTIVITY_BOOST, out int prodQuantity))
        {
            switch (prodQuantity)
            {
                case 1:
                    prod = "50%";
                    break;

                case 2:
                    prod = "75%";
                    break;

                case 3:
                    prod = "100%";
                    break;

                default:
                    prod = "25%";
                    break;
            }
        }

        offlineDialog.GetComponent<Transform>().Find("InfoPanel").transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("While you are away the productivity is " + prod + " and only works for " + timeBoost.ToString() + "h at most.\n You can upgrade this with the Spectre and the Necromancer at the shop.");

        //Fill grid with offline resources
        foreach (KeyValuePair<string, int> resourceOld in Data.Instance.INVENTORY)
        {
            if (resourceOld.Key.Contains("Old"))
            {
                foreach (KeyValuePair<string, int> resource in Data.Instance.INVENTORY)
                {
                    if (!resource.Key.Contains("Old") && resourceOld.Key.Contains(resource.Key) && resourceOld.Key != resource.Key)
                    {
                        //Check broomstickOld and stick
                        if (!(resourceOld.Key.Equals("broomstickOld") && resource.Key.Equals("stick")) && !(resourceOld.Key.Equals("batWingOld") && resource.Key.Equals("bat")) && !(resourceOld.Key.Equals("witchHatOld") && resource.Key.Equals("witch")))
                        {
                            if (resourceOld.Value != resource.Value)
                            {
                                GameObject panel = Instantiate(resourcePanel, offlineDialog.transform.Find("Scrollback").transform.GetChild(0).transform);

                                if (Data.Instance.RESOURCES.TryGetValue(resource.Key, out Resource res))
                                {
                                    panel.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = res.icon;
                                }
                                else
                                {
                                    //It's a monster
                                    if (Data.Instance.MONSTERS.TryGetValue(resource.Key, out MonsterInfo monster))
                                    {
                                        panel.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = monster.icon;
                                    }
                                }
                                panel.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText((resource.Value - resourceOld.Value).ToString());

                                //change color if negative or positive
                                if ((resource.Value - resourceOld.Value) < 0)
                                {
                                    panel.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color32(255, 0, 0, 255);
                                }
                                else if ((resource.Value - resourceOld.Value) > 0)
                                {
                                    panel.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color32(0, 255, 0, 255);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void addInventory()
    {
        foreach (KeyValuePair<string, Resource> resource in Data.Instance.RESOURCES)
        {
            if (Data.Instance.INVENTORY.ContainsKey(resource.Key))
            {
                Data.Instance.INVENTORY.Remove(resource.Key);
            }
            Data.Instance.INVENTORY.Add(resource.Key, 10000);
        }

        Data.Instance.INVENTORY.Add(Data.SKELETON, 10);
        Data.Instance.INVENTORY.Add(Data.JACK_LANTERN, 10);
        Data.Instance.INVENTORY.Add(Data.GOBLIN, 10);
        Data.Instance.INVENTORY.Add(Data.BAT, 10);
        Data.Instance.INVENTORY.Add(Data.ZOMBIE, 10);
        Data.Instance.INVENTORY.Add(Data.GHOST, 10);
        Data.Instance.INVENTORY.Add(Data.CLOWN, 10);
        Data.Instance.INVENTORY.Add(Data.VAMPIRE, 10);
        Data.Instance.INVENTORY.Add(Data.WITCH, 10);
        //Data.Instance.INVENTORY.Add(Data.REAPER, 1);
    }

    public void buildConstructions()
    {
        foreach (KeyValuePair<string, float[]> construction in Data.Instance.CONSTRUCTIONS)
        {
            for (int i = 0; i < buildings.Count; i++)
            {
                if (construction.Key.Contains(buildings[i].GetComponent<Construction>().id))
                {
                    //Instantiate building prefab
                    Vector3 constructionPosition = new Vector3(construction.Value[POS_X], construction.Value[POS_Y], 0);

                    Vector3Int positionIntAux = GridBuildingSystem.current.gridLayout.LocalToCell(constructionPosition);
                    buildings[i].GetComponent<Construction>().area.position = new Vector3Int((int)(constructionPosition.x - buildings[i].GetComponent<Construction>().area.size.x / 1.8), (int)(constructionPosition.y - buildings[i].GetComponent<Construction>().area.size.y / 5), (int)constructionPosition.z);
                    BoundsInt areaTempAux = buildings[i].GetComponent<Construction>().area;

                    if (GridBuildingSystem.current.canTakeArea(areaTempAux))
                    {
                        GameObject obj = Instantiate(buildings[i], constructionPosition, Quaternion.identity);
                        if (construction.Value[CONSTRUCTION_TYPE] == 1)
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

                            temp.numType = (int)construction.Value[NUM_TYPE];
                            temp.activeResourceTime = construction.Value[ACTIVE_RESOURCE_TIME];
                            temp.activeMonsterTime = construction.Value[ACTIVE_RESOURCE_TIME];
                            temp.setSelectedTab(temp.activeMonster);
                            temp.setUI(temp.activeMonster);
                            Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(constructionPosition);
                            BoundsInt areaTemp = temp.area;
                            areaTemp.position = positionInt;
                            GridBuildingSystem.current.takeArea(areaTemp);

                            #endregion SUMMONING CIRCLE
                        }
                        else if (construction.Value[CONSTRUCTION_TYPE] == 2)
                        {
                            //It's a decoration boost

                            #region DECORATION BOOST

                            DecorationBoost temp = obj.GetComponent<DecorationBoost>();

                            //Set decoration boost values
                            temp.numType = (int)construction.Value[NUM_TYPE];

                            Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(constructionPosition);
                            BoundsInt areaTemp = temp.area;
                            areaTemp.position = positionInt;
                            GridBuildingSystem.current.takeArea(areaTemp);

                            #endregion DECORATION BOOST
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

                            temp.numType = (int)construction.Value[NUM_TYPE];
                            temp.activeResourceTime = construction.Value[ACTIVE_RESOURCE_TIME];
                            temp.updateUI();

                            Vector3Int positionInt = GridBuildingSystem.current.gridLayout.WorldToCell(temp.gameObject.transform.position);
                            temp.area.position = new Vector3Int((int)(temp.transform.position.x - temp.area.size.x / 1.8), (int)(temp.transform.position.y - temp.area.size.y / 5), (int)temp.transform.position.z);
                            BoundsInt buildingArea = temp.area;
                            GridBuildingSystem.current.takeArea(buildingArea);

                            /*Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(constructionPosition);
                            BoundsInt areaTemp = temp.area;
                            areaTemp.position = new Vector3Int((int)(temp.transform.position.x - temp.area.size.x / 1.8), (int)(temp.transform.position.y - temp.area.size.y / 5), (int)temp.transform.position.z);
                            GridBuildingSystem.current.takeArea(areaTemp);*/

                            #endregion GENERAL BUILDING
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
                        if (Data.Instance.BUILDING_INVENTORY.TryGetValue(buildings[i].GetComponent<Construction>().id, out int quantity))
                        {
                            Data.Instance.BUILDING_INVENTORY[buildings[i].GetComponent<Construction>().id] = quantity + 1;
                        }
                        else
                        {
                            //First building of that type
                            Data.Instance.BUILDING_INVENTORY.Add(buildings[i].GetComponent<Construction>().id, 1);
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

        //Calculate max time out depending on boost
        float maxTimeOut = 4 * 3600; //Default value -> 4h

        if (Data.Instance.BOOSTS.TryGetValue(Data.OFFLINE_MAXTIME_BOOST, out int quantity))
        {
            for (int i = 0; i < quantity; i++)
            {
                maxTimeOut += 3600; // +1h for each boost
            }
        }

        if (offlineTime > maxTimeOut)
        {
            offlineTime = maxTimeOut;
        }

        //Show offline dialog
        TimeSpan offlineTimeAux = localDate - previousDateTime; //in hours, min, etc
        calculateOfflineDialogText(offlineTimeAux);
        showOfflineDialog();
    }

    public void calculateOfflineDialogText(TimeSpan offlineTimeAux)
    {
        if (offlineTimeAux.Hours > 0)
        {
            if (offlineTimeAux.Minutes > 0)
            {
                fillOfflineDialog(offlineTimeAux.Hours + "h " + offlineTimeAux.Minutes + "min");
            }
            else
            {
                if (offlineTimeAux.Seconds > 0)
                {
                    fillOfflineDialog(offlineTimeAux.Hours + "h " + offlineTimeAux.Seconds + "s");
                }
                else
                {
                    fillOfflineDialog(offlineTimeAux.Hours + "h");
                }
            }
        }
        else
        {
            if (offlineTimeAux.Minutes > 0)
            {
                if (offlineTimeAux.Seconds > 0)
                {
                    fillOfflineDialog(offlineTimeAux.Minutes + "min " + offlineTimeAux.Seconds + "s");
                }
                else
                {
                    fillOfflineDialog(offlineTimeAux.Minutes + "min");
                }
            }
            else
            {
                fillOfflineDialog(offlineTimeAux.Seconds + "s");
            }
        }
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
        int numMonsters = 0;
        for (int i = 0; i < monstersKeys.Count; i++)
        {
            if (Data.Instance.INVENTORY.ContainsKey(monstersKeys[i])) //Load minigame only if it has minim 1 monster
            {
                numMonsters += Data.Instance.INVENTORY[monstersKeys[i]];
            }
        }
        if (numMonsters > 0)
        {
            SaveManager.Instance.Save();
            audioSource.clip = pumpkinLaugh;
            audioSource.Play();
            loadingScreen.SetActive(true);
            Invoke("load", 3f);
        }
    }

    public void load()
    {
        SceneManager.LoadScene("miniGame");
    }

    public void applyBoost(string id)
    {
        switch (id)
        {
            #region BUILDINGS BOOSTS

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
                    if (building.GetComponent<Building>() != null && building.GetComponent<Building>().id.Equals("hellIsland"))
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
                        item.hasReachedLimit = false;
                        item.updateTextAmount(item.currentQuantity);
                        item.setRequirementTextConfig();
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
                        item.hasReachedLimit = false;
                        item.updateTextAmount(item.currentQuantity);
                        item.setRequirementTextConfig();
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
                        item.hasReachedLimit = false;
                        item.updateTextAmount(item.currentQuantity);
                        item.setRequirementTextConfig();
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
                        item.hasReachedLimit = false;
                        item.updateTextAmount(item.currentQuantity);
                        item.setRequirementTextConfig();
                    }
                }

                break;

            case "well":
                //Unlock another welll
                foreach (ShopItemHolder item in buildingShopItems)
                {
                    if (item.titleText.text.Contains("Well"))
                    {
                        item.maxQuantity += 1;
                        item.hasReachedLimit = false;
                        item.updateTextAmount(item.currentQuantity);
                        item.setRequirementTextConfig();
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
                        item.hasReachedLimit = false;
                        item.updateTextAmount(item.currentQuantity);
                        item.setRequirementTextConfig();
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
                        item.hasReachedLimit = false;
                        item.updateTextAmount(item.currentQuantity);
                        item.setRequirementTextConfig();
                    }
                }

                break;

            #endregion BUILDINGS BOOSTS

            #region MONSTER BOOSTS

            case "jackOLantern":
                if (Data.Instance.MONSTERS.TryGetValue(Data.JACK_LANTERN, out MonsterInfo monsterAux1))
                {
                    //Update monster stats dictionary
                    if (!monsterAux1.isUnlocked || isRestart == 1)
                    {
                        Data.Instance.MONSTERS_STATS[Data.JACK_LANTERN] = new int[] { 1, 1 }; //check to modify to see if in load it is set to 1,1 again
                        Data.Instance.MONSTERS[Data.JACK_LANTERN].upgradeLevel = 1;
                    }
                    monsterAux1.isUnlocked = true;
                    unlockedMonsters.Add(Data.JACK_LANTERN); //it is added twice?
                    if (hidenMonsterIndex < 2)
                    {
                        hidenMonster = Data.BAT;
                        hidenMonsterIndex = 2;
                    }

                    //Get summoning circle
                    for (int i = 0; i < constructionsBuilt.Count; i++)
                    {
                        if (constructionsBuilt[i].GetComponent<SummoningCircle>() != null)
                        {
                            constructionsBuilt[i].GetComponent<SummoningCircle>().setUI(constructionsBuilt[i].GetComponent<SummoningCircle>().selectedTab);
                        }
                    }

                    boostShop.GetComponent<BoostShop>().setMonsterTabs();

                    //Set new monster to inventory
                    if (!Data.Instance.INVENTORY.ContainsKey(Data.JACK_LANTERN))
                    {
                        Data.Instance.INVENTORY.Add(Data.JACK_LANTERN, 0);
                    }
                }

                break;

            case "bat":

                if (Data.Instance.MONSTERS.TryGetValue(Data.BAT, out MonsterInfo monsterAux))
                {
                    if (!monsterAux.isUnlocked || isRestart == 1)
                    {
                        //Update monster stats dictionary
                        Data.Instance.MONSTERS_STATS[Data.BAT] = new int[] { 1, 1 };
                        Data.Instance.MONSTERS[Data.BAT].upgradeLevel = 1;
                    }
                    monsterAux.isUnlocked = true;
                    unlockedMonsters.Add(Data.BAT);
                    if (hidenMonsterIndex < 3)
                    {
                        hidenMonster = Data.GOBLIN;
                        hidenMonsterIndex = 3;
                    }
                    //Get summoning circle
                    for (int i = 0; i < constructionsBuilt.Count; i++)
                    {
                        if (constructionsBuilt[i].GetComponent<SummoningCircle>() != null)
                        {
                            constructionsBuilt[i].GetComponent<SummoningCircle>().setUI(constructionsBuilt[i].GetComponent<SummoningCircle>().selectedTab);
                        }
                    }

                    boostShop.GetComponent<BoostShop>().setMonsterTabs();

                    //Set new monster to inventory
                    if (!Data.Instance.INVENTORY.ContainsKey(Data.BAT))
                    {
                        Data.Instance.INVENTORY.Add(Data.BAT, 0);
                    }
                }

                break;

            case "goblin":

                if (Data.Instance.MONSTERS.TryGetValue(Data.GOBLIN, out MonsterInfo monsterAux2))
                {
                    if (!monsterAux2.isUnlocked || isRestart == 1)
                    {
                        //Update monster stats dictionary
                        Data.Instance.MONSTERS_STATS[Data.GOBLIN] = new int[] { 1, 1 };
                        Data.Instance.MONSTERS[Data.GOBLIN].upgradeLevel = 1;
                    }
                    monsterAux2.isUnlocked = true;
                    unlockedMonsters.Add(Data.GOBLIN);
                    if (hidenMonsterIndex < 4)
                    {
                        hidenMonster = Data.GHOST;
                        hidenMonsterIndex = 4;
                    }

                    //Get summoning circle
                    for (int i = 0; i < constructionsBuilt.Count; i++)
                    {
                        if (constructionsBuilt[i].GetComponent<SummoningCircle>() != null)
                        {
                            constructionsBuilt[i].GetComponent<SummoningCircle>().setUI(constructionsBuilt[i].GetComponent<SummoningCircle>().selectedTab);
                        }
                    }

                    boostShop.GetComponent<BoostShop>().setMonsterTabs();

                    //Set new monster to inventory
                    if (!Data.Instance.INVENTORY.ContainsKey(Data.GOBLIN))
                    {
                        Data.Instance.INVENTORY.Add(Data.GOBLIN, 0);
                    }
                }

                break;

            case "ghost":

                if (Data.Instance.MONSTERS.TryGetValue(Data.GHOST, out MonsterInfo monsterAux3))
                {
                    if (!monsterAux3.isUnlocked || isRestart == 1)
                    {
                        //Update monster stats dictionary
                        Data.Instance.MONSTERS_STATS[Data.GHOST] = new int[] { 1, 1 };
                        Data.Instance.MONSTERS[Data.GHOST].upgradeLevel = 1;
                    }
                    monsterAux3.isUnlocked = true;
                    unlockedMonsters.Add(Data.GHOST);
                    if (hidenMonsterIndex < 5)
                    {
                        hidenMonster = Data.CLOWN;
                        hidenMonsterIndex = 5;
                    }

                    //Get summoning circle
                    for (int i = 0; i < constructionsBuilt.Count; i++)
                    {
                        if (constructionsBuilt[i].GetComponent<SummoningCircle>() != null)
                        {
                            constructionsBuilt[i].GetComponent<SummoningCircle>().setUI(constructionsBuilt[i].GetComponent<SummoningCircle>().selectedTab);
                        }
                    }

                    boostShop.GetComponent<BoostShop>().setMonsterTabs();

                    //Set new monster to inventory
                    if (!Data.Instance.INVENTORY.ContainsKey(Data.GHOST))
                    {
                        Data.Instance.INVENTORY.Add(Data.GHOST, 0);
                    }
                }

                break;

            case "clown":

                if (Data.Instance.MONSTERS.TryGetValue(Data.CLOWN, out MonsterInfo monsterAux4))
                {
                    if (!monsterAux4.isUnlocked || isRestart == 1)
                    {
                        //Update monster stats dictionary
                        Data.Instance.MONSTERS_STATS[Data.CLOWN] = new int[] { 1, 1 };
                        Data.Instance.MONSTERS[Data.CLOWN].upgradeLevel = 1;
                    }
                    monsterAux4.isUnlocked = true;
                    unlockedMonsters.Add(Data.CLOWN);
                    if (hidenMonsterIndex < 6)
                    {
                        hidenMonster = Data.ZOMBIE;
                        hidenMonsterIndex = 6;
                    }

                    //Get summoning circle
                    for (int i = 0; i < constructionsBuilt.Count; i++)
                    {
                        if (constructionsBuilt[i].GetComponent<SummoningCircle>() != null)
                        {
                            constructionsBuilt[i].GetComponent<SummoningCircle>().setUI(constructionsBuilt[i].GetComponent<SummoningCircle>().selectedTab);
                        }
                    }

                    boostShop.GetComponent<BoostShop>().setMonsterTabs();

                    //Set new monster to inventory
                    if (!Data.Instance.INVENTORY.ContainsKey(Data.CLOWN))
                    {
                        Data.Instance.INVENTORY.Add(Data.CLOWN, 0);
                    }
                }

                break;

            case "zombie":

                if (Data.Instance.MONSTERS.TryGetValue(Data.ZOMBIE, out MonsterInfo monsterAux5))
                {
                    if (!monsterAux5.isUnlocked || isRestart == 1)
                    {
                        //Update monster stats dictionary
                        Data.Instance.MONSTERS_STATS[Data.ZOMBIE] = new int[] { 1, 1 };
                        Data.Instance.MONSTERS[Data.ZOMBIE].upgradeLevel = 1;
                    }
                    monsterAux5.isUnlocked = true;
                    unlockedMonsters.Add(Data.ZOMBIE);
                    if (hidenMonsterIndex < 7)
                    {
                        hidenMonster = Data.VAMPIRE;
                        hidenMonsterIndex = 7;
                    }

                    //Get summoning circle
                    for (int i = 0; i < constructionsBuilt.Count; i++)
                    {
                        if (constructionsBuilt[i].GetComponent<SummoningCircle>() != null)
                        {
                            constructionsBuilt[i].GetComponent<SummoningCircle>().setUI(constructionsBuilt[i].GetComponent<SummoningCircle>().selectedTab);
                        }
                    }

                    boostShop.GetComponent<BoostShop>().setMonsterTabs();

                    //Set new monster to inventory
                    if (!Data.Instance.INVENTORY.ContainsKey(Data.ZOMBIE))
                    {
                        Data.Instance.INVENTORY.Add(Data.ZOMBIE, 0);
                    }
                }

                break;

            case "vampire":

                if (Data.Instance.MONSTERS.TryGetValue(Data.VAMPIRE, out MonsterInfo monsterAux6))
                {
                    if (!monsterAux6.isUnlocked || isRestart == 1)
                    {
                        //Update monster stats dictionary
                        Data.Instance.MONSTERS_STATS[Data.VAMPIRE] = new int[] { 1, 1 };
                        Data.Instance.MONSTERS[Data.VAMPIRE].upgradeLevel = 1;
                    }
                    monsterAux6.isUnlocked = true;
                    unlockedMonsters.Add(Data.VAMPIRE);
                    if (hidenMonsterIndex < 8)
                    {
                        hidenMonster = Data.WITCH;
                        hidenMonsterIndex = 8;
                    }

                    //Get summoning circle
                    for (int i = 0; i < constructionsBuilt.Count; i++)
                    {
                        if (constructionsBuilt[i].GetComponent<SummoningCircle>() != null)
                        {
                            constructionsBuilt[i].GetComponent<SummoningCircle>().setUI(constructionsBuilt[i].GetComponent<SummoningCircle>().selectedTab);
                        }
                    }

                    boostShop.GetComponent<BoostShop>().setMonsterTabs();

                    //Set new monster to inventory
                    if (!Data.Instance.INVENTORY.ContainsKey(Data.VAMPIRE))
                    {
                        Data.Instance.INVENTORY.Add(Data.VAMPIRE, 0);
                    }
                }

                break;

            case "witch":

                if (Data.Instance.MONSTERS.TryGetValue(Data.WITCH, out MonsterInfo monsterAux7))
                {
                    if (!monsterAux7.isUnlocked || isRestart == 1)
                    {
                        //Update monster stats dictionary
                        Data.Instance.MONSTERS_STATS[Data.WITCH] = new int[] { 1, 1 };
                        Data.Instance.MONSTERS[Data.WITCH].upgradeLevel = 1;
                    }
                    monsterAux7.isUnlocked = true;
                    unlockedMonsters.Add(Data.WITCH);
                    if (hidenMonsterIndex < 9)
                    {
                        hidenMonster = Data.REAPER;
                        hidenMonsterIndex = 9;
                    }

                    //Get summoning circle
                    for (int i = 0; i < constructionsBuilt.Count; i++)
                    {
                        if (constructionsBuilt[i].GetComponent<SummoningCircle>() != null)
                        {
                            constructionsBuilt[i].GetComponent<SummoningCircle>().setUI(constructionsBuilt[i].GetComponent<SummoningCircle>().selectedTab);
                        }
                    }

                    boostShop.GetComponent<BoostShop>().setMonsterTabs();

                    //Set new monster to inventory
                    if (!Data.Instance.INVENTORY.ContainsKey(Data.WITCH))
                    {
                        Data.Instance.INVENTORY.Add(Data.WITCH, 0);
                    }
                }

                break;

            case "reaper":

                if (Data.Instance.MONSTERS.TryGetValue(Data.REAPER, out MonsterInfo monsterAux8))
                {
                    if (!monsterAux8.isUnlocked || isRestart == 1)
                    {
                        //Update monster stats dictionary
                        Data.Instance.MONSTERS_STATS[Data.REAPER] = new int[] { 1, 1 };
                        Data.Instance.MONSTERS[Data.REAPER].upgradeLevel = 1;
                    }
                    monsterAux8.isUnlocked = true;
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

                    //Set new monster to inventory
                    if (!Data.Instance.INVENTORY.ContainsKey(Data.REAPER))
                    {
                        Data.Instance.INVENTORY.Add(Data.REAPER, 0);
                    }
                }

                break;

            #endregion MONSTER BOOSTS

            #region SPECIALS

            case "spectre":
                //Unlock the spectre
                foreach (ShopItemHolder item in buildingShopItems)
                {
                    if (item.titleText.text.Contains("Spectre"))
                    {
                        item.isLocked = false;
                        item.setToUnlocked();
                    }
                }
                break;

            case "necromancer":
                //Unlock the spectre
                foreach (ShopItemHolder item in buildingShopItems)
                {
                    if (item.titleText.text.Contains("Necromancer"))
                    {
                        item.isLocked = false;
                        item.setToUnlocked();
                    }
                }
                break;

            case "mageGuardian":
                //Unlock the spectre
                foreach (ShopItemHolder item in buildingShopItems)
                {
                    if (item.titleText.text.Contains("Mage Guardian"))
                    {
                        item.isLocked = false;
                        item.setToUnlocked();
                    }
                }
                break;

            case "demonLord":
                //Unlock the spectre
                foreach (ShopItemHolder item in buildingShopItems)
                {
                    if (item.titleText.text.Contains("Demon Lord"))
                    {
                        item.isLocked = false;
                        item.setToUnlocked();
                    }
                }
                break;

                #endregion SPECIALS
        }
    }

    public void applyAllBoosts()
    {
        //When loading the game
        foreach (KeyValuePair<string, int> boost in Data.Instance.BOOSTS)
        {
            applyBoost(boost.Key);
        }

        if (Data.Instance.MONSTERS.TryGetValue(Data.SKELETON, out MonsterInfo monsterAux1))
        {
            //Update monster stats dictionary
            if (!monsterAux1.isUnlocked || isRestart == 1)
            {
                Data.Instance.MONSTERS_STATS[Data.SKELETON] = new int[] { 1, 1 }; //check to modify to see if in load it is set to 1,1 again
                Data.Instance.MONSTERS[Data.SKELETON].upgradeLevel = 1;
            }
        }
    }

    public void updateShopPrices(string id)
    {
        foreach (ShopItemHolder item in buildingShopItems)
        {
            if (item.Item.id.Equals(id))
            {
                if (Data.Instance.BUILDINGS.TryGetValue(id, out GameObject construction))
                {
                    if (Data.Instance.BUILDING_INVENTORY.TryGetValue(id, out int quantity))
                    {
                        if (construction.GetComponent<Construction>().production_cost.Count > 0 && quantity < item.Item.maxQuantity)
                        {
                            item.requirementText1 = construction.GetComponent<Construction>().production_cost[quantity].list[0].quantity.ToString();

                            if (Data.Instance.RESOURCES.TryGetValue(construction.GetComponent<Construction>().production_cost[quantity].list[0].resourceNameKey, out Resource resource))
                            {
                                item.resource1Icon.sprite = resource.icon;
                            }

                            item.resourceText1.gameObject.SetActive(true);
                            item.resource1Icon.gameObject.SetActive(true);
                            item.resource1Icon.color = new Color(1, 1, 1, 1);

                            if (construction.GetComponent<Construction>().production_cost[quantity].list.Count > 1)
                            {
                                item.requirementText2 = construction.GetComponent<Construction>().production_cost[quantity].list[1].quantity.ToString();

                                if (Data.Instance.RESOURCES.TryGetValue(construction.GetComponent<Construction>().production_cost[quantity].list[1].resourceNameKey, out Resource resource2))
                                {
                                    item.resource2Icon.sprite = resource2.icon;
                                }
                                item.resourceText2.gameObject.SetActive(true);
                                item.resource2Icon.gameObject.SetActive(true);
                                item.resource2Icon.color = new Color(1, 1, 1, 1);
                            }
                            else
                            {
                                item.resourceText2.gameObject.SetActive(false);
                                //item.resource2Icon.gameObject.SetActive(false);
                                item.resource2Icon.color = new Color(1, 1, 1, 0);
                            }
                        }
                    }
                }
            }
        }
    }

    /*public void loadBoosts()
    {
        foreach (GameObject construction in constructionsBuilt)
        {
            if(construction.GetComponent<DecorationBoost>() != null)
            {
                //Add boost to dictionary
                if (Data.Instance.BOOSTS.TryGetValue(construction.GetComponent<Construction>().id, out int quantity))
                {
                    Data.Instance.BOOSTS[construction.GetComponent<Construction>().id] += 1;
                }
                else
                {
                    Data.Instance.BOOSTS.Add(construction.GetComponent<Construction>().id, 1);
                }
            }
        }
    }*/

    public void hideAllDialogs()
    {
        canvas.GetComponent<Transform>().Find("DescriptionDialog").gameObject.SetActive(false);

        inventoryDialog.SetActive(false);
        portalDialog.SetActive(false);

        canvas.GetComponent<Transform>().Find("UIBlock").gameObject.SetActive(true);
        Invoke("setDialogOpen", 0.05f);
    }

    public void hideAllDialogsBlackPanel()
    {
        canvas.GetComponent<Transform>().Find("DescriptionDialog").gameObject.SetActive(false);

        inventoryDialog.SetActive(false);
        portalDialog.SetActive(false);
        hideOfflineDialog();

        canvas.GetComponent<Transform>().Find("BlackPanel").gameObject.SetActive(false);
        canvas.GetComponent<Transform>().Find("UIBlock").gameObject.SetActive(true);
        Invoke("setDialogOpen", 0.05f);
    }

    public string numToString(int num)
    {
        string output = "";

        if (num >= 1000)
        {
            string[] aux = (num / 1000f).ToString().Split(',');
            if (aux.Length > 1)
            {
                output = aux[0] + "." + aux[1].ToCharArray()[0] + "k";
            }
            else
            {
                output = aux[0] + "k";
            }
        }
        else
        {
            output = num.ToString();
        }

        return output;
    }

    public void fillInventoryDialog()
    {
        //Clear all panels
        for (int i = 0; i < inventoryContainer.transform.childCount; i++)
        {
            Destroy(inventoryContainer.transform.GetChild(i).gameObject);
        }

        List<InventoryObject> dropsPanels = new List<InventoryObject>();
        List<InventoryObject> monsterPanels = new List<InventoryObject>();
        List<InventoryObject> resourcesPanels = new List<InventoryObject>();

        //Fill grid with inventory resources
        foreach (KeyValuePair<string, int> resource in Data.Instance.INVENTORY)
        {
            if (!resource.Key.Contains("Old"))
            {
                //check quantity
                if (resource.Value > 0)
                {

                    InventoryObject aux = new InventoryObject();

                    if (Data.Instance.RESOURCES.TryGetValue(resource.Key, out Resource res))
                    {
                        aux.icon = res.icon;
                    }
                    else
                    {
                        //It's a monster
                        if (Data.Instance.MONSTERS.TryGetValue(resource.Key, out MonsterInfo monster))
                        {
                            aux.icon = monster.icon;
                        }
                    }

                    aux.quantity = resource.Value.ToString();

                    bool isResource = true;

                    for (int i = 0; i < monstersKeys.Count; i++)
                    {
                        if (resource.Key.Equals(monstersKeys[i]))
                        {
                            monsterPanels.Add(aux);
                            isResource = false;
                        }
                    }

                    for (int i = 0; i < dropsKeys.Count; i++)
                    {
                        if (resource.Key.Equals(dropsKeys[i]))
                        {
                            dropsPanels.Add(aux);
                            isResource = false;
                        }
                    }

                    if (isResource)
                    {
                        resourcesPanels.Add(aux);
                    }
                }
            }
        }

        //Add panels to grid
        foreach (InventoryObject drop in dropsPanels)
        {
            GameObject panel = Instantiate(resourcePanel, inventoryDialog.transform.Find("Scrollback").transform.GetChild(0).transform);
            panel.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = drop.icon;
            panel.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(drop.quantity);
        }
        foreach (InventoryObject monster in monsterPanels)
        {
            GameObject panel = Instantiate(resourcePanel, inventoryDialog.transform.Find("Scrollback").transform.GetChild(0).transform);
            panel.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = monster.icon;
            panel.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(monster.quantity);
        }
        foreach (InventoryObject resourceAux in resourcesPanels)
        {
            GameObject panel = Instantiate(resourcePanel, inventoryDialog.transform.Find("Scrollback").transform.GetChild(0).transform);
            panel.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = resourceAux.icon;
            panel.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(resourceAux.quantity);
        }
    }
}