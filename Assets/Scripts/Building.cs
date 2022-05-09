using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    public List<GameObject> spriteResources; //Llista amb els sprites de cada resource 
    public int level; //nivell de millora del building -> new resource unlocked
    public bool isProducing = false; 
    public int maxLevel;
    public string initialActiveResource;
    public GameObject buildingInterior;
    public bool isSelected;
    public TextMeshProUGUI upgradeText;
    public TextMeshProUGUI resourceTimeText;
    public TextMeshProUGUI pauseText;

    private string activeResource; //Quina resource s'esta produïnt
    private float activeResourceTime;
    private float timeToNextItem = 0;
    private string upgradeTextAux = "";
    private string upgradeTextAux2 = "";
    private string resourceTimeAux = "";
    private float timeLeft;

    private void Start()
    {
        activeResource = initialActiveResource;
        if(Data.Instance.RESOURCES.TryGetValue(activeResource, out Resource resource))
        {
            activeResourceTime = resource.time;
        }
        buildingInterior.SetActive(false);
    }

    void Update()
    {
        if(Time.time >= timeToNextItem && isProducing)
        {
            produce();
        }
        if (isSelected)
        {
            showBuildingInterior();
        }
        else
        {
            hideBuildingInterior();
        }       

        //Requirements to update building
        foreach (Requirement requirement in upgrade_cost[level - 1].list)
        {
            if(Data.Instance.RESOURCES.TryGetValue(requirement.resourceNameKey, out Resource resource))
            {
                if(Data.Instance.INVENTORY.TryGetValue(requirement.resourceNameKey, out int quantity))
                {
                    if(level < maxLevel)
                    {
                        upgradeTextAux2 = "Level " + level + " -> " + (level + 1) + "\n";
                        upgradeTextAux += quantity + "/" + requirement.quantity + " " + resource.resourceName + "\n";
                    }
                    else
                    {
                        upgradeTextAux2 = " Level " + level + "\nMax level";
                    }
                }
                else
                {
                    if (level < maxLevel)
                    {
                        upgradeTextAux2 = "Level " + level + " -> " + (level + 1) + "\n";
                        upgradeTextAux += "0/" + requirement.quantity + " " + resource.resourceName + "\n";
                    }
                    else
                    {
                        upgradeTextAux2 = " Level " + level + "\nMax level";
                    }
                }
            }            
        }
        upgradeText.text = upgradeTextAux2 + upgradeTextAux;
        upgradeTextAux = "";
        upgradeTextAux2 = "";

        //Show resource producing and time to next
        if (Data.Instance.RESOURCES.TryGetValue(activeResource, out Resource aResource))
        {
            if ((timeToNextItem - Time.time) >= 0 && isProducing)
            {
                resourceTimeAux = aResource.resourceName + " - " + (timeToNextItem - Time.time).ToString("F0");
            }
            else
            {
                resourceTimeAux = aResource.resourceName + " - -";
            }
        }
        resourceTimeText.text = resourceTimeAux;
        resourceTimeAux = "";
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
                    foreach (GameObject sprite in spriteResources)
                    {
                        sprite.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0.25f);
                    }
                    spriteResources[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                }
            }
        }
    }

    public void showBuildingInterior()
    {
        buildingInterior.SetActive(true);
    }

    public void hideBuildingInterior()
    {
        buildingInterior.SetActive(false);
    }

    public void exitBuildingInterior()
    {
        isSelected = false;
    }

    public void togglePause()
    {
        isProducing = !isProducing;
        if (isProducing)
        {
            //En Play
            pauseText.text = "Pause";
            timeToNextItem = Time.time + timeLeft;            
        }
        else
        {
            //En Pause
            pauseText.text = "Play";
            timeLeft = timeToNextItem - Time.time;
        }

        //If is mid production, reset
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
                }
            }
        }
    }
}
