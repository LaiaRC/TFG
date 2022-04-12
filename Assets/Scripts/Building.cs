using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Requirement
{
    public string resourceNameKey;
    public int quantity;
}

public class UpgradeRequirement
{
    public string resourceNameKey;
    public int quantity;
    public int level;
}
public class Building : MonoBehaviour
{
    
    public string building_name; //nom del building
    public List<Requirement> production_cost;
    public List<List<Requirement>> upgrade_cost;
    public List<string> resources; //Llista dels recursos que produeix el building
    public int level; //nivell de millora del building -> new resource unlocked
    public bool isProducing = false; 
    public int maxLevel = 1;
    public string initialActiveResource;
    public GameObject buildingInterior;
    public bool isSelected;

    private string activeResource; //Quina resource s'esta produïnt
    private float activeResourceTime;
    private float timeToNextItem = 0;

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
    }

    public void produce()
    {
        if (Data.Instance.RESOURCES.TryGetValue(activeResource, out Resource resource))
        {

            #region CHECK REQUIREMENTS
            foreach (Requirement requirement in resource.requirements)
            {
                if(Data.Instance.INVERTORY.TryGetValue(requirement.resourceNameKey, out int requirementQuantity))
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
                if (Data.Instance.INVERTORY.TryGetValue(requirement.resourceNameKey, out int requirementQuantity))
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
            if (Data.Instance.INVERTORY.TryGetValue(activeResource, out int quantity))
            {
                quantity += 1;
                Data.Instance.updateInventory(activeResource, quantity);
            }
            else
            {
                Data.Instance.INVERTORY.Add(activeResource, 1);
            }
        }
    }
    public void setActiveResource(string activeResource)
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
    }

    public void upgrade()
    {
        if (level < maxLevel)
        {
            level++;
        }
    }
}
