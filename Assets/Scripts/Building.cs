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
    public int level = 1; //nivell de millora del building -> new resource unlocked
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
    public Vector3 position;
    public BoundsInt tempArea;
    public string activeResource; //Quina resource s'esta produïnt
    public int numTypeBuildings = 0;
    public float timeLeft;
    public float time = 0; //comptador de temps fins el activeResourceTime
    public bool isPaused = false;

    public bool placed;
    public BoundsInt area;
    public GameObject confirmUI;
    public GameObject canvasInterior;

    public float activeResourceTime = 0;   //temps que triga a fer-se el active resource
    private Vector3 origin;
    private bool showUI = false;
    private bool enoughResources = false;
    

    private void Start()
    {
        if (level == 1)
        {
            activeResource = initialActiveResource;

            resourcesButtons[0].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            resourcesButtons[1].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
            resourcesButtons[2].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);

            resourceButtonsIcons[0].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            resourceButtonsIcons[1].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
            resourceButtonsIcons[2].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
        }
        if(Data.Instance.RESOURCES.TryGetValue(activeResource, out Resource resource))
        {
            activeResourceTime = resource.time;
        }
        canvasInterior.SetActive(false);
        playButton.gameObject.SetActive(false);
        setCanvasInterior();
        timeBar.minValue = 0;
        timeBar.maxValue = activeResourceTime;

       /*level1Background.gameObject.SetActive(true);
        level2Background.gameObject.SetActive(false);
        level3Background.gameObject.SetActive(false);*/

        //Save building position
        position = transform.position;
    }

    void Update()
    {
        if (placed)
        {
            if (isProducing)
            {
                if (playButton.gameObject.activeInHierarchy)
                {
                    playButton.gameObject.SetActive(false);
                    pauseButton.gameObject.SetActive(true);
                }
                
                time += Time.deltaTime;
                timeLeft = activeResourceTime - time;
               
                if (timeLeft <= 0)
                {
                    produce();
                    if (enoughResources)
                    {
                        time = 0;
                    }
                    else
                    {
                        isProducing = false;
                    }
                }
            }
            else
            {

                timeBar.value = timeLeft;
                resourceTimeText.SetText("-");
                if (isPaused)
                {
                    playButton.gameObject.SetActive(true);
                    pauseButton.gameObject.SetActive(false);
                }
                else
                {
                    //Was producing but ran out of requirements to produce
                    if (checkRequirementsToProduce())
                    {
                        isProducing = true;
                    }
                }
            }

            //Update requirements to update building (inventory)
            if (level != maxLevel)
            {
                if(Data.Instance.INVENTORY.TryGetValue(upgrade_cost[level - 1].list[0].resourceNameKey, out int quantity))
                {
                    if (quantity > 1000)
                    {
                        string[] aux = (quantity / 1000f).ToString().Split(',');
                        if (aux.Length > 1)
                        {
                            upgradeText1.text = aux[0] + "." + aux[1].ToCharArray()[0] + "k/" + upgrade_cost[level - 1].list[0].quantity;
                        }
                        else
                        {
                            upgradeText1.text = aux[0] + "k/" + upgrade_cost[level - 1].list[0].quantity;
                        }
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

                if (isProducing)
                {
                    resourceTimeText.SetText(timeLeft.ToString("F0"));
                }
                else
                {
                    resourceTimeText.SetText("-");
                }
            }

            //Slider config
            if (isProducing)
            {
                timeBar.value = timeLeft;
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

    public bool checkRequirementsToProduce()
    {
        bool hasRequirements = false;
        if (Data.Instance.RESOURCES.TryGetValue(activeResource, out Resource resource))
        {

            #region CHECK REQUIREMENTS

            //Check if it has requirements or it's free
            if (resource.requirements.Length > 0)
            {
                foreach (Requirement requirement in resource.requirements)
                {
                    if (Data.Instance.INVENTORY.TryGetValue(requirement.resourceNameKey, out int requirementQuantity))
                    {
                        if (requirementQuantity < requirement.quantity)
                        {
                            enoughResources = false;
                            return false;
                        }
                        else
                        {
                            enoughResources = true;
                            hasRequirements = true;
                        }
                    }
                    else
                    {
                        //Player don't have the requirement resource to produce 
                        enoughResources = false;
                        return false;
                    }
                }
            }
            else
            {
                //The resource is free
                enoughResources = true;
                hasRequirements = true;
            }
            #endregion

            
        }
        return hasRequirements;
    }

    public void produce()
    {
        if (Data.Instance.RESOURCES.TryGetValue(activeResource, out Resource resource))
        {

            if (checkRequirementsToProduce())
            {
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

                #region ADD TO INVENTORY
                if (Data.Instance.INVENTORY.TryGetValue(activeResource, out int quantity))
                {
                    quantity += 1;
                    Data.Instance.updateInventory(activeResource, quantity);
                }
                else
                {
                    Data.Instance.INVENTORY.Add(activeResource, 1);
                }
                #endregion
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
                    time = 0;

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

                    //Play when changing resource
                    if (!isProducing)
                    {
                        play();
                    }
                }
            }            
        }
    }

    public void showBuildingInterior()
    {

        if (!GameManager.Instance.isOnCanvas && !GameManager.Instance.isDialogOpen)
        {

            canvasInterior.SetActive(true);
            GameManager.Instance.isOnCanvas = true;
        }
    }

    public void hideBuildingInterior()
    {
        canvasInterior.SetActive(false);
        Invoke("isOnCanvasOff", 0.1f);
    }

    void isOnCanvasOff()
    {
        GameManager.Instance.isOnCanvas = false;
    }

    public void pause()
    {
        isProducing = false;
        isPaused = true;
        timeBar.value = timeLeft;
    }

    public void play()
    {
        isProducing = true;
        isPaused = false;
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
                    resourcesButtons[level - 1].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                    resourceButtonsIcons[level - 1].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                }
            }
        }
        updateUI();
    }

    public int getNumActiveResource()
    {
        int num = 0;
        for (int i = 0; i < resources.Count; i++)
        {
            if (activeResource.Equals(resources[i]))
            {
                num = i;
            }
        }
        return num;
    }

    public void updateUI()
    {
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

        if (isProducing)
        {
            play();
        }
        else
        {
            pause();
        }

        timeBar.value = timeLeft;

        //Upgrade level text
        levelText.SetText(level.ToString());
               

        if (level == 2)
        {
            level1Background.gameObject.SetActive(false);
            level2Background.gameObject.SetActive(true);
            level3Background.gameObject.SetActive(false);

            level2Tilemap.gameObject.SetActive(true);
            level3Tilemap.gameObject.SetActive(false);
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

            level2Tilemap.gameObject.SetActive(true);
            level3Tilemap.gameObject.SetActive(true);
        }

        for (int i = 0; i < resources.Count; i++)
        {
            if (i < level)
            {
                if (activeResource.Equals(resources[i]))
                {
                    resourcesButtons[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    resourceButtonsIcons[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
                else
                {
                    resourcesButtons[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                    resourceButtonsIcons[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                }
            }
            else
            {
                resourcesButtons[i].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
                resourceButtonsIcons[i].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
            }
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
        GameManager.Instance.isOnCanvas = true;
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
                tempArea = areaTemp;
                confirmUI.SetActive(false);
                GameManager.Instance.detected = false;
                GameManager.Instance.showAllShop();
                GameManager.Instance.draggingItemShop = false;
                Invoke("isOnCanvasOff", 0.1f);

                //Update amount of buildings in inventory of buildings
                if (Data.Instance.BUILDING_INVENTORY.TryGetValue(id, out int quantity)) {
                    quantity += 1;
                    Data.Instance.BUILDING_INVENTORY[id] = quantity;
                    numTypeBuildings = quantity;
                }
                else
                {
                    //It's the first building (of this type)
                    Data.Instance.BUILDING_INVENTORY.Add(id, 1);
                    numTypeBuildings = 1;
                }

                //Save building in dictionary
                if (!Data.Instance.CONSTRUCTIONS.ContainsKey(id + numTypeBuildings))
                {
                    Data.Instance.CONSTRUCTIONS.Add(id + numTypeBuildings, new float[] { transform.position.x, transform.position.y, level, getNumActiveResource(), timeLeft, isProducing ? 1 : 0, isPaused ? 1 : 0, numTypeBuildings, activeResourceTime });
                }
                GameManager.Instance.constructionsBuilt.Add(this.gameObject);
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
