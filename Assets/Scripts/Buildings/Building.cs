using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

[System.Serializable]
public class RequirementBuilding
{
    public string resourceNameKey;
    public int quantity;
}

[System.Serializable]
public class RequirementList
{
    public List<RequirementBuilding> list;
}

public class Building : Construction
{    
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
    public Image timeBar;
    public Image level1Background;
    public Image level2Background;
    public Image level3Background;
    public GameObject level2Grid;
    public GameObject level3Grid;
    public Image requirementIcon;
    public Image requirementIcon1;
    public Image requirementIcon2;
    public TextMeshProUGUI requirementText;
    public TextMeshProUGUI requirement1Text;
    public TextMeshProUGUI requirement2Text;
    public int extraNumResources = 0;
    public int isProducer;
    public int isConverter;
    public TextMeshProUGUI numResources;

    public string activeResource; //Quina resource s'esta produïnt
    public float timeLeft;
    public float time = 0; //comptador de temps fins el activeResourceTime
    public bool isPaused = false;    
    public GameObject canvasInterior;

    public float activeResourceTime = 0;   //temps que triga a fer-se el active resource
    protected bool showUI = false;
    protected bool enoughResources = false;
    protected bool addedToInventory = false;

    //Audio variables
    protected static int PLAY = 2;
    protected static int PAUSE = 3;
    protected static int UPGRADE = 4;
    protected static int DEFAULT = 5;
    protected static int CLOSE = 6;

    public void setCanvasInterior()
    {
        #region UPGRADE

        //Update requirement 1
        upgradeText1.text = "0/" + GameManager.Instance.numToString(upgrade_cost[0].list[0].quantity);
        setTextColor(upgradeText1, 0, upgrade_cost[0].list[0].quantity);

        if (Data.Instance.RESOURCES.TryGetValue(upgrade_cost[0].list[0].resourceNameKey, out Resource resource))
        {
            upgradeIcon1.sprite = resource.icon;
        }

        //If has 2 update requirements set value, if not delete
        if (upgrade_cost[0].list.Count > 1)
        {
            upgradeText2.text = "0/" + GameManager.Instance.numToString(upgrade_cost[0].list[1].quantity);
            setTextColor(upgradeText2, 0, upgrade_cost[0].list[1].quantity);

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

        //Change upgrade icons 
        if (level < maxLevel)
        {
            if (Data.Instance.RESOURCES.TryGetValue(upgrade_cost[level - 1].list[0].resourceNameKey, out Resource resource4))
            {
                upgradeIcon1.sprite = resource4.icon;
            }
            if (upgrade_cost[level - 1].list.Count > 1)
            {
                if (Data.Instance.RESOURCES.TryGetValue(upgrade_cost[level - 1].list[1].resourceNameKey, out Resource resource5))
                {
                    upgradeIcon2.sprite = resource5.icon;
                }
            }
        }
        #endregion

        #region LEVEL
        levelText.text = level.ToString();
        #endregion

        #region TIME GROUP
        if(Data.Instance.RESOURCES.TryGetValue(activeResource, out Resource resource3))
        {
            activeResourceIcon.sprite = resource3.icon;

            if(resource3.requirements.Length > 0)
            {               
                //1 requirement
                    
                if(Data.Instance.RESOURCES.TryGetValue(resource3.requirements[0].resourceNameKey, out Resource requirement1Resource))
                {
                    requirementIcon1.sprite = requirement1Resource.icon;
                }
                requirementIcon1.gameObject.SetActive(true);
                requirementIcon.gameObject.SetActive(false);
                requirementIcon2.gameObject.SetActive(false);

                if(Data.Instance.INVENTORY.TryGetValue(resource3.requirements[0].resourceNameKey, out int quantity))
                {
                    requirement1Text.SetText(GameManager.Instance.numToString(quantity) + "/" + GameManager.Instance.numToString(resource3.requirements[0].quantity));
                    setTextColor(requirement1Text, quantity, resource3.requirements[0].quantity);

                }
                else
                {
                    requirementText.SetText("0/" + GameManager.Instance.numToString(resource3.requirements[0].quantity));
                    setTextColor(requirementText, 0, resource3.requirements[0].quantity);
                }
                requirementText.gameObject.SetActive(false);
                requirement1Text.gameObject.SetActive(true);
                requirement2Text.gameObject.SetActive(false);

                if (resource3.requirements.Length > 1)
                {
                    //Has 2 requirements
                    requirementIcon2.gameObject.SetActive(true);

                    if (Data.Instance.RESOURCES.TryGetValue(resource3.requirements[1].resourceNameKey, out Resource requirement2Resource))
                    {
                        requirementIcon2.sprite = requirement2Resource.icon;
                    }

                    if (Data.Instance.INVENTORY.TryGetValue(resource3.requirements[1].resourceNameKey, out int quantity2))
                    {
                        requirementText.SetText(GameManager.Instance.numToString(quantity2) + "/" + resource3.requirements[1].quantity);
                        setTextColor(requirementText, quantity2, resource3.requirements[1].quantity);

                    }
                    else
                    {
                        requirementText.SetText("0/" + GameManager.Instance.numToString(resource3.requirements[1].quantity));
                        setTextColor(requirementText, 0, resource3.requirements[1].quantity);
                    }
                    requirement2Text.gameObject.SetActive(true);
                }

            }
            else
            {
                //It's free
                requirementIcon.gameObject.SetActive(false);
                requirementIcon1.gameObject.SetActive(false);
                requirementIcon2.gameObject.SetActive(false);

                requirementText.SetText("free");
                requirementText.gameObject.SetActive(true);
                requirement1Text.gameObject.SetActive(false);
                requirement2Text.gameObject.SetActive(false);
            }

            //Set num resources
            updateNumResource(1);
        }
        #endregion        
    }

    public virtual bool checkRequirementsToProduce()
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

    public virtual void produce()
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
                    if (isProducer == 1)
                    {
                        if (Data.Instance.BOOSTS.TryGetValue(Data.PRODUCER_BOOST, out int numProd))
                        {
                            extraNumResources = numProd;
                        }
                        else
                        {
                            extraNumResources = 0;
                        }
                    }else if (isConverter == 1)
                    {
                        if (Data.Instance.BOOSTS.TryGetValue(Data.CONVERTER_BOOST, out int numProd2))
                        {
                            extraNumResources = numProd2;
                        }
                        else
                        {
                            extraNumResources = 0;
                        }
                    }

                    quantity += 1 + extraNumResources;
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
            bool isUnlocked = false;
            for (int i = 0; i < level; i++)
            {
                if (resources[i] == activeResource)
                {
                    isUnlocked = true;
                    //Audio
                    audioSource.clip = sounds[PLAY];
                    audioSource.Play();
                    
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
                    timeBar.fillAmount = activeResourceTime;

                    //Play when changing resource
                    if (!isProducing)
                    {
                        play();
                    }

                    //Update UI
                    updateUI();
                }
            }

            if (!isUnlocked)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
        }
        else
        {
            audioSource.clip = sounds[DEFAULT];
            audioSource.Play();
        }
    }

    public void showBuildingInterior()
    {
        audioSource.clip = sounds[DEFAULT];
        audioSource.Play();

        if (!GameManager.Instance.isOnCanvas && !GameManager.Instance.isDialogOpen)
        {

            canvasInterior.SetActive(true);
            GameManager.Instance.isOnCanvas = true;
        }
    }

    public void hideBuildingInterior()
    {
        audioSource.clip = sounds[CLOSE];
        audioSource.Play();

        canvasInterior.SetActive(false);
        Invoke("isOnCanvasOff", 0.1f);
    }        

    public virtual void pause()
    {
        audioSource.clip = sounds[PAUSE];
        audioSource.Play();

        isProducing = false;
        isPaused = true;
        timeBar.fillAmount = timeLeft;
    }

    public virtual void play()
    {
        audioSource.clip = sounds[PLAY];
        audioSource.Play();

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
                foreach (RequirementBuilding requirement in building.GetComponent<Building>().upgrade_cost[level - 1].list)
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
                    else
                    {
                        audioSource.clip = sounds[ERROR];
                        audioSource.Play();
                    }
                    //Et falta algun resource!
                    if (!enoughResource) return;
                }
                if (enoughResource)
                {
                    audioSource.clip = sounds[UPGRADE];
                    audioSource.Play();

                    foreach (RequirementBuilding requirement in building.GetComponent<Building>().upgrade_cost[level - 1].list)
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

                    //Set new resource to inventory
                    if (!Data.Instance.INVENTORY.ContainsKey(resources[level - 1]))
                    {
                        Data.Instance.INVENTORY.Add(resources[level - 1], 0);
                    }
                }
                else
                {
                    audioSource.clip = sounds[ERROR];
                    audioSource.Play();
                }
            }
        }
        updateUI();
    }

    public virtual int getNumActiveResource()
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
            //play();
            isProducing = true;
            isPaused = false;
        }
        else
        {
            //pause();
            isProducing = false;
            isPaused = true;
            timeBar.fillAmount = timeLeft;
        }

        timeBar.fillAmount = timeLeft;

        //Upgrade level text
        levelText.SetText(level.ToString());
               

        if (level == 2)
        {
            level1Background.gameObject.SetActive(false);
            level2Background.gameObject.SetActive(true);
            level3Background.gameObject.SetActive(false);

            level2Grid.SetActive(true);
            level3Grid.SetActive(false);

            //Just in case
            if (id.Equals("hellIsland") && GameManager.Instance.isHellfireUnlocked)
            {
                upgradeText1.gameObject.SetActive(true);
                upgradeIcon1.gameObject.SetActive(true);

                if (Data.Instance.BUILDINGS.TryGetValue(id, out GameObject building))
                {
                    if(building.GetComponent<Building>().upgrade_cost[level - 1].list.Count > 1)
                    {
                        upgradeText2.gameObject.SetActive(true);
                        upgradeIcon2.gameObject.SetActive(true);
                    }
                    else
                    {
                        upgradeText2.gameObject.SetActive(false);
                        upgradeIcon2.gameObject.SetActive(false);
                    }
                }
                
                upgradeButton.gameObject.SetActive(true);
                maxText.gameObject.SetActive(false);
            }
        }
        if (level == maxLevel || (id.Equals("hellIsland") && level == 2 && !GameManager.Instance.isHellfireUnlocked))
        {
            upgradeText1.gameObject.SetActive(false);
            upgradeText2.gameObject.SetActive(false);
            upgradeIcon1.gameObject.SetActive(false);
            upgradeIcon2.gameObject.SetActive(false);
            upgradeButton.gameObject.SetActive(false);
            maxText.gameObject.SetActive(true);

            if (id.Equals("hellIsland") && level == 2 && !GameManager.Instance.isHellfireUnlocked)
            {
                maxText.SetText("Unlock in The Merchant");
                maxText.color = new Color(1, 1, 1, 1);
                maxText.fontSize = 14;
            }
            else
            {
                maxText.SetText("MAX");
                maxText.color = new Color(1, 1, 1, 1);
                maxText.fontSize = 36;
            }

            level1Background.gameObject.SetActive(false);
            level2Background.gameObject.SetActive(false);
            level3Background.gameObject.SetActive(true);

            level2Grid.SetActive(true);
            level3Grid.SetActive(true);
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

        //Update time group
        updateUITimeGroup();
    }

    public void updateUITimeGroup()
    {
        //Update time group
        #region TIME GROUP
        if (Data.Instance.RESOURCES.TryGetValue(activeResource, out Resource resource3))
        {
            activeResourceIcon.sprite = resource3.icon;

            if (resource3.requirements.Length > 0)
            {
                if (resource3.requirements.Length > 1)
                {
                    //Has 2 requirements

                    //requirement 1

                    if (Data.Instance.RESOURCES.TryGetValue(resource3.requirements[0].resourceNameKey, out Resource requirement1Resourceaux))
                    {
                        requirementIcon1.sprite = requirement1Resourceaux.icon;
                    }

                    if (Data.Instance.RESOURCES.TryGetValue(resource3.requirements[1].resourceNameKey, out Resource requirement2Resource))
                    {
                        requirementIcon2.sprite = requirement2Resource.icon;
                    }
                    requirementIcon1.gameObject.SetActive(true);
                    requirementIcon.gameObject.SetActive(false);
                    requirementIcon2.gameObject.SetActive(true);

                    if (Data.Instance.INVENTORY.TryGetValue(resource3.requirements[0].resourceNameKey, out int quantity3))
                    {
                        requirement1Text.SetText(GameManager.Instance.numToString(quantity3) + "/" + GameManager.Instance.numToString(resource3.requirements[0].quantity));
                        setTextColor(requirement1Text, quantity3, resource3.requirements[0].quantity);
                    }
                    else
                    {
                        requirement1Text.SetText("0/" + GameManager.Instance.numToString(resource3.requirements[0].quantity));
                        setTextColor(requirement1Text, 0, resource3.requirements[0].quantity);
                    }

                    if (Data.Instance.INVENTORY.TryGetValue(resource3.requirements[1].resourceNameKey, out int quantity2))
                    {
                        requirement2Text.SetText(GameManager.Instance.numToString(quantity2) + "/" + resource3.requirements[1].quantity);
                        setTextColor(requirement2Text, quantity2, resource3.requirements[1].quantity);
                    }
                    else
                    {
                        requirement2Text.SetText("0/" + GameManager.Instance.numToString(resource3.requirements[1].quantity));
                        setTextColor(requirement2Text, 0, resource3.requirements[1].quantity);
                    }
                    requirementText.gameObject.SetActive(false);
                    requirement1Text.gameObject.SetActive(true);
                    requirement2Text.gameObject.SetActive(true);
                }
                else
                {
                    //Only 1 requirement

                    if (Data.Instance.RESOURCES.TryGetValue(resource3.requirements[0].resourceNameKey, out Resource requirement1Resource))
                    {
                        requirementIcon.sprite = requirement1Resource.icon;
                    }
                    requirementIcon.gameObject.SetActive(true);
                    requirementIcon1.gameObject.SetActive(false);
                    requirementIcon2.gameObject.SetActive(false);

                    if (Data.Instance.INVENTORY.TryGetValue(resource3.requirements[0].resourceNameKey, out int quantity))
                    {
                        requirementText.SetText(GameManager.Instance.numToString(quantity) + "/" + GameManager.Instance.numToString(resource3.requirements[0].quantity));
                        setTextColor(requirementText, quantity, resource3.requirements[0].quantity);
                    }
                    else
                    {
                        requirementText.SetText("0/" + GameManager.Instance.numToString(resource3.requirements[0].quantity));
                        setTextColor(requirementText, 0, resource3.requirements[0].quantity);
                    }
                    requirementText.gameObject.SetActive(true);
                    requirement1Text.gameObject.SetActive(false);
                    requirement2Text.gameObject.SetActive(false);
                }

            }
            else
            {
                //It's free
                requirementIcon.gameObject.SetActive(false);
                requirementIcon1.gameObject.SetActive(false);
                requirementIcon2.gameObject.SetActive(false);

                requirementText.SetText("free");
                requirementText.color = new Color(1, 1, 1, 1);
                requirementText.gameObject.SetActive(true);
                requirement1Text.gameObject.SetActive(false);
                requirement2Text.gameObject.SetActive(false);
            }
        }
        #endregion
    }

    public void updateNumResource(int num)
    {
        numResources.SetText("x" + num.ToString());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(area.center, area.size);
    }

    public void setTextColor(TextMeshProUGUI text, int inventoryQuantity, int requirementQuantity)
    {
        if(inventoryQuantity >= requirementQuantity)
        {
            text.color = new Color(0, 1, 0, 1);
        }
        else
        {
            text.color = new Color(1, 0, 0, 1);
        }
    }
}
