using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeProducer : MonoBehaviour
{
    public string id;
    public string activeResource;
    public float activeResourceTime;
    public bool isProducing;
    public bool isPaused;
    public float time;
    public float timeLeft;
    public int isProducer;
    public int isConverter;
    public int isSummoningCircle;

    private bool enoughResources = false;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isProducing)
        {
            time += Time.deltaTime;
            timeLeft = activeResourceTime - time;
            if (timeLeft <= 0)
            {
                if (isSummoningCircle == 1)
                {
                    produceMonster();
                    //Debug.Log("active monster - " + activeResource); //check if works with other monsters (like num 6)
                }
                else
                {
                    produce();
                }
                
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
            if (!isPaused)
            {
                //Was producing but ran out of requirements to produce
                if (isSummoningCircle == 1)
                {
                    if (checkRequirementsToProduceMonster())
                    {
                        isProducing = true;
                    }
                }
                else
                {
                    if (checkRequirementsToProduce())
                    {
                        isProducing = true;
                    }
                }
            }
        }
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

                    int extraNumResources = 0; //Default value

                    //Check if boost
                    if(isProducer == 1)
                    {
                        if (Data.Instance.BOOSTS.TryGetValue(Data.PRODUCER_BOOST, out int prodNum))
                        {
                            extraNumResources = prodNum;
                        }
                    }else if (isConverter == 1)
                    {
                        if (Data.Instance.BOOSTS.TryGetValue(Data.CONVERTER_BOOST, out int convNum))
                        {
                            extraNumResources = convNum;
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

    public void produceMonster()
    {
        if (Data.Instance.MONSTERS.TryGetValue(activeResource, out MonsterInfo monster))
        {

            if (checkRequirementsToProduceMonster())
            {
                #region PAY REQUIREMENTS

                foreach (Requirement requirement in monster.requirements)
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
                    Data.Instance.updateInventory(activeResource, quantity); //update monster inventory
                }
                else
                {
                    Data.Instance.INVENTORY.Add(activeResource, 1);
                }
                #endregion
            }
        }
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

    public bool checkRequirementsToProduceMonster()
    {
        bool hasRequirements = false;
        if (Data.Instance.MONSTERS.TryGetValue(activeResource, out MonsterInfo monster))
        {
            foreach (Requirement requirement in monster.requirements)
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
        return hasRequirements;
    }
}
