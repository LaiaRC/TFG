using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Requirement
{
    public string resourceNameKey;
    public int quantity;
}

[System.Serializable]
public class RequirementList
{
    public List<Requirement> list;
}

public class Building : MonoBehaviour
{
    
    public string building_name; //nom del building
    public string id;
    public List<Requirement> production_cost;
    public List<RequirementList> upgrade_cost;
    public List<string> resources; //Llista dels recursos que produeix el building
    public List<GameObject> resourcesButtons; //Llista amb els sprites de cada resource 
    public List<Image> resourceButtonsIcons;
    public int level; //nivell de millora del building -> new resource unlocked
    public bool isProducing = false; 
    public int maxLevel;
    public string initialActiveResource;
    public TextMeshProUGUI upgradeText1;
    public TextMeshProUGUI upgradeText2;
    public Image upgradeIcon1;
    public Image upgradeIcon2;
    public TextMeshProUGUI resourceTimeText;
    public Image activeResourceIcon;
    public Button playButton;
    public Button pauseButton;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI maxText;
    public Button upgradeButton;
    public Slider timeBar;
    public Image level1Background;
    public Image level2Background;
    public Image level3Background;
    public Tilemap level2Tilemap;
    public Tilemap level3Tilemap;

    public bool placed { get; private set; }
    public BoundsInt area;
    public GameObject confirmUI;
    public GameObject canvasInterior;

    private string activeResource; //Quina resource s'esta produïnt
    private float activeResourceTime = 0;
    private float timeToNextItem = 0;
    private string resourceTimeAux = "";
    private float timeLeft;
    private Vector3 origin;
    private bool showUI = false;
    

    private void Start()
    {
        level = 1;
        activeResource = initialActiveResource;
        if(Data.Instance.RESOURCES.TryGetValue(activeResource, out Resource resource))
        {
            activeResourceTime = resource.time;
        }
        canvasInterior.SetActive(false);
        playButton.gameObject.SetActive(false);
        setCanvasInterior();
        timeBar.minValue = 0;
        timeBar.maxValue = activeResourceTime;

        level1Background.gameObject.SetActive(true);
        level2Background.gameObject.SetActive(false);
        level3Background.gameObject.SetActive(false);

        //set resources buttons disabled
        /*resourcesButtons[0].gameObject.SetActive(true);
        resourcesButtons[1].gameObject.SetActive(true);
        resourcesButtons[2].gameObject.SetActive(true);
        resourceButtonsIcons[0].gameObject.SetActive(true);
        resourceButtonsIcons[1].gameObject.SetActive(true);
        resourceButtonsIcons[2].gameObject.SetActive(true);*/

        resourcesButtons[0].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        resourcesButtons[1].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
        resourcesButtons[2].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);

        resourceButtonsIcons[0].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        resourceButtonsIcons[1].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
        resourceButtonsIcons[2].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
    }

    void Update()
    {
        if (placed)
        {

            if (Time.time >= timeToNextItem && isProducing)
            {
                produce();
            }

            //Update requirements to update building (inventory)
            if (level != maxLevel)
            {
                if(Data.Instance.INVENTORY.TryGetValue(upgrade_cost[level - 1].list[0].resourceNameKey, out int quantity))
                {
                    if (quantity > 1000)
                    {
                        string[] aux = (quantity / 1000f).ToString().Split(',');
                        upgradeText1.text = aux[0] + "." + aux[1].ToCharArray()[0] + "k/" + upgrade_cost[level - 1].list[0].quantity;
                    }
                    else
                    {
                        upgradeText1.text = quantity + "/" + upgrade_cost[level - 1].list[0].quantity;
                    }
                }
                else
                {
                    //Set to 0
                    upgradeText1.text = "0/" + upgrade_cost[level - 1].list[0].quantity;
                }

                if (upgrade_cost[level - 1].list.Count > 1)
                {
                    //Set upgrade requirement 2
                    if (Data.Instance.INVENTORY.TryGetValue(upgrade_cost[level - 1].list[1].resourceNameKey, out int quantity2))
                    {
                        upgradeText2.text = quantity2 + "/" + upgrade_cost[level - 1].list[1].quantity;
                    }
                    else
                    {
                        //Set to 0
                        upgradeText2.text = "0/" + upgrade_cost[level - 1].list[1].quantity;
                    }
                }
                else
                {
                    if (upgradeText2.isActiveAndEnabled)
                    {
                        upgradeText2.gameObject.SetActive(false);
                    }
                }
            }         

            //Show resource producing and time to next
            if (Data.Instance.RESOURCES.TryGetValue(activeResource, out Resource aResource))
            {

                if ((timeToNextItem - Time.time) >= 0 && isProducing)
                {
                    resourceTimeAux = (timeToNextItem - Time.time).ToString("F0");
                }
                else
                {
                    resourceTimeAux = "-";
                }
            }
            resourceTimeText.text = resourceTimeAux;
            resourceTimeAux = "";

            //Slider config
            if (isProducing)
            {
                timeBar.value = timeToNextItem - Time.time;
            }
        }
    }

    public void setCanvasInterior()
    {
        #region UPGRADE

        //Update requirement 1
        upgradeText1.text = "0/" + upgrade_cost[0].list[0].quantity;
        if (Data.Instance.RESOURCES.TryGetValue(upgrade_cost[0].list[0].resourceNameKey, out Resource resource))
        {
            upgradeIcon1.sprite = resource.icon;
        }

        //If has 2 update requirements set value, if not delete
        if (upgrade_cost[0].list.Count > 1)
        {
            upgradeText2.text = "0/" + upgrade_cost[0].list[1].quantity;
            if (Data.Instance.RESOURCES.TryGetValue(upgrade_cost[0].list[1].resourceNameKey, out Resource resource2))
            {
                upgradeIcon2.sprite = resource2.icon;
            }
        }
        else
        {
            //Set active false
            upgradeText2.gameObject.SetActive(false);
            upgradeIcon2.gameObject.SetActive(false);
        }
        #endregion

        #region LEVEL
        levelText.text = level.ToString();
        #endregion

        #region TIME GROUP
        if(Data.Instance.RESOURCES.TryGetValue(activeResource, out Resource resource3))
        {
            activeResourceIcon.sprite = resource3.icon;
        }
        #endregion
    }

    public void produce()
    {
        if (Data.Instance.RESOURCES.TryGetValue(activeResource, out Resource resource))
        {

            #region CHECK REQUIREMENTS
            foreach (Requirement requirement in resource.requirements)
            {
                if(Data.Instance.INVENTORY.TryGetValue(requirement.resourceNameKey, out int requirementQuantity))
                {
                    if(requirementQuantity < requirement.quantity)
                    {
                        return;
                    }
                }
                else
                {
                    //Player don't have the requirement resource to produce 
                    return;
                }
            }
            #endregion

            #region PAY REQUIREMENTS

            foreach (Requirement requirement in resource.requirements)
            {
                if (Data.Instance.INVENTORY.TryGetValue(requirement.resourceNameKey, out int requirementQuantity))
                {
                    if (requirementQuantity >= requirement.quantity)
                    {
                        requirementQuantity -= requirement.quantity;
                        Data.Instance.updateInventory(requirement.resourceNameKey, requirementQuantity);
                    }
                }
            }

            #endregion
            timeToNextItem = Time.time + activeResourceTime;
            if (Data.Instance.INVENTORY.TryGetValue(activeResource, out int quantity))
            {
                quantity += 1;
                Data.Instance.updateInventory(activeResource, quantity);
            }
            else
            {
                Data.Instance.INVENTORY.Add(activeResource, 1);
            }
        }
    }
    public void setActiveResource(string activeResource)
    {
        if (activeResource != this.activeResource)
        {
            for (int i = 0; i < level; i++)
            {
                if (resources[i] == activeResource)
                {
                    this.activeResource = activeResource;
                    if (Data.Instance.RESOURCES.TryGetValue(activeResource, out Resource resource))
                    {
                        activeResourceTime = resource.time;
                    }
                    //Reset time to produce
                    timeToNextItem = Time.time + activeResourceTime;
                    timeLeft = activeResourceTime;

                    //Change sprite
                    switch (level)
                    {
                        case 1:
                            resourcesButtons[0].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                            resourcesButtons[1].GetComponent<Image>().color = new Color(0.25f,0.25f, 0.25f, 0.5f);
                            resourcesButtons[2].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);

                            resourceButtonsIcons[0].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                            resourceButtonsIcons[1].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
                            resourceButtonsIcons[2].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
                            break;
                        case 2:
                            resourcesButtons[0].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                            resourcesButtons[1].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                            resourcesButtons[2].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);

                            resourceButtonsIcons[0].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                            resourceButtonsIcons[1].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                            resourceButtonsIcons[2].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
                            break;

                        case 3:
                            resourcesButtons[0].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                            resourcesButtons[1].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                            resourcesButtons[2].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);

                            resourceButtonsIcons[0].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                            resourceButtonsIcons[1].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                            resourceButtonsIcons[2].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                            break;
                    }
                    resourcesButtons[i].GetComponent<Image>().color = new Color(1,1,1,1);
                    resourceButtonsIcons[i].GetComponent<Image>().color = new Color(1,1,1,1);
                    if (Data.Instance.RESOURCES.TryGetValue(activeResource, out Resource resourceAux))
                    {
                        activeResourceIcon.sprite = resourceAux.icon;
                    }

                    //Slider config
                    timeBar.maxValue = activeResourceTime;
                }
            }            
        }
    }

    public void showBuildingInterior()
    {
        if (!GameManager.Instance.isOnCanvas)
        {
            canvasInterior.SetActive(true);
            GameManager.Instance.isOnCanvas = true;
        }
    }

    public void hideBuildingInterior()
    {
        canvasInterior.SetActive(false);
        GameManager.Instance.isOnCanvas = false;
    }

    public void pause()
    {
        isProducing = false;
        timeLeft = timeToNextItem - Time.time;
        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
    }

    public void play()
    {
        isProducing = true;
        timeToNextItem = Time.time + timeLeft;
        pauseButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(false);
    }

    public void upgrade()
    {
        if (level < maxLevel)
        {
            //Check if has all the requirements
            if(Data.Instance.BUILDINGS.TryGetValue(id, out GameObject building)){
                bool enoughResource = false;
                foreach (Requirement requirement in building.GetComponent<Building>().upgrade_cost[level - 1].list)
                {
                    enoughResource = false;
                    //Check inventory                    

                    if (Data.Instance.INVENTORY.TryGetValue(requirement.resourceNameKey, out int quantity))
                    {
                        if(quantity >= requirement.quantity)
                        {
                            enoughResource = true;                            
                        }
                    }
                    //Et falta algun resource!
                    if (!enoughResource) return;
                }
                if (enoughResource)
                {
                    foreach (Requirement requirement in building.GetComponent<Building>().upgrade_cost[level - 1].list)
                    {
                        if (Data.Instance.INVENTORY.TryGetValue(requirement.resourceNameKey, out int quantity))
                        {
                            if (quantity >= requirement.quantity)
                            {
                                quantity -= requirement.quantity;
                                Data.Instance.updateInventory(requirement.resourceNameKey, quantity);                                
                            }
                        }
                    }
                    this.level++;
                    resourcesButtons[level-1].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                    resourceButtonsIcons[level-1].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);

                    //Change upgrade icons 
                    if (level < maxLevel)
                    {
                        if (Data.Instance.RESOURCES.TryGetValue(upgrade_cost[level - 1].list[0].resourceNameKey, out Resource resource))
                        {
                            upgradeIcon1.sprite = resource.icon;
                        }
                        if (upgrade_cost[level - 1].list.Count > 1)
                        {
                            if (Data.Instance.RESOURCES.TryGetValue(upgrade_cost[level - 1].list[1].resourceNameKey, out Resource resource2))
                            {
                                upgradeIcon2.sprite = resource2.icon;
                            }
                        }
                    }

                    //Upgrade level text
                    levelText.SetText(level.ToString());
                }
            }
        }

        if(level == 2)
        {
            level1Background.gameObject.SetActive(false);
            level2Background.gameObject.SetActive(true);
            level3Background.gameObject.SetActive(false);

            level2Tilemap.gameObject.SetActive(true);
        }
        if (level == maxLevel)
        {
            upgradeText1.gameObject.SetActive(false);
            upgradeText2.gameObject.SetActive(false);
            upgradeIcon1.gameObject.SetActive(false);
            upgradeIcon2.gameObject.SetActive(false);
            upgradeButton.gameObject.SetActive(false);
            maxText.gameObject.SetActive(true);

            level1Background.gameObject.SetActive(false);
            level2Background.gameObject.SetActive(false);
            level3Background.gameObject.SetActive(true);

            level3Tilemap.gameObject.SetActive(true);
        }
    }

    #region Building Methods

    public bool canBePlaced()
    {
        Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = positionInt;

        if (GridBuildingSystem.current.canTakeArea(areaTemp))
        {
            return true;
        }
        return false;
    }

    public void place()
    {
        confirmUI.SetActive(true);
    }

    public void confirmBuilding()
    {
        if (canBePlaced())
        {
            //Check requirements
            if (GameManager.Instance.checkRequirements(id))
            {
                //Apply cost and update inventory
                GameManager.Instance.buy(id);

                //Build building gameObject
                Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(transform.position);
                BoundsInt areaTemp = area;
                areaTemp.position = positionInt;
                placed = true;
                GridBuildingSystem.current.takeArea(areaTemp);
                confirmUI.SetActive(false);
                GameManager.Instance.detected = false;
                GameManager.Instance.showAllShop();
                GameManager.Instance.draggingItemShop = false;
                GameManager.Instance.pointerEnter();

                //Update amount of buildings in inventory of buildings
                if(Data.Instance.BUILDING_INVENTORY.TryGetValue(id, out int quantity)){
                    quantity += 1;
                    Data.Instance.BUILDING_INVENTORY[id] = quantity;
                }
                else
                {
                    //It's the first building (of this type)
                    Data.Instance.BUILDING_INVENTORY.Add(id, 1);
                }
            }
        }
    }

    public void cancelBuilding()
    {
        GameManager.Instance.detected = false;
        GameManager.Instance.draggingItemShop = false;
        GameManager.Instance.showAllShop();
        Destroy(gameObject);
    }

    public void checkPlacement()
    {
        if (canBePlaced())
        {
            place();
            origin = transform.position;
        }
        else
        {
            Destroy(transform.gameObject);
        }
        GameManager.Instance.openShop();
    }
       
    #endregion
}
