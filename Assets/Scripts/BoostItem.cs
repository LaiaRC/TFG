using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoostItem : MonoBehaviour
{
    public string id;
    public string title;
    public string description;
    public Sprite icon;
    public GameObject buyButton;
    public GameObject requirementIcon;
    public GameObject requirementGroup;
    public TextMeshProUGUI requirementText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;
    public bool isUnlocked = false;
    public bool isPermanent = false;

    private Requirement requirement;

    public void Update()
    {
        if (GameManager.Instance.isMerchantOpen)
        {
            if (!isUnlocked)
            {
                updateRequirementText();
            }
            else if (!requirementText.text.Equals("Owned"))
            {
                requirementText.SetText("Owned");
                requirementText.color = new Color(1, 1, 1, 1);
            }
        }
    }

    public void setRequirement(Requirement requirement)
    {
        //Set UI
        titleText.SetText(title);
        descriptionText.SetText(description);
        iconImage.sprite = icon;
        
        this.requirement = requirement;
        if (Data.Instance.RESOURCES.TryGetValue(requirement.resourceNameKey, out Resource resource))
        {
            requirementIcon.GetComponent<Image>().sprite = resource.icon;
        }

        updateRequirementText();
    }

    public void updateRequirementText()
    {
        if (Data.Instance.INVENTORY.TryGetValue(requirement.resourceNameKey, out int quantity))
        {
            requirementText.SetText(GameManager.Instance.numToString(quantity) + "/" + GameManager.Instance.numToString(requirement.quantity));
            setTextColor(requirementText, quantity, requirement.quantity);
        }
        else
        {
            requirementText.SetText("0/" + GameManager.Instance.numToString(requirement.quantity));
            setTextColor(requirementText, 0, requirement.quantity);
        }
    }
    public void setToOwned()
    {
        buyButton.SetActive(false);
        requirementIcon.SetActive(false);
        requirementText.SetText("Owned");
        requirementText.color = new Color(1, 1, 1, 1);
        requirementText.alignment = TextAlignmentOptions.Center;
        isUnlocked = true;
    }

    public void buy()
    {
        if (checkRequirements())
        {
            GameManager.Instance.audioSource.clip = GameManager.Instance.sounds[GameManager.CONFIRM];
            GameManager.Instance.audioSource.Play();

            #region PAY REQUIREMENTS

            if (Data.Instance.INVENTORY.TryGetValue(requirement.resourceNameKey, out int requirementQuantity))
            {
                if (requirementQuantity >= requirement.quantity)
                {
                    requirementQuantity -= requirement.quantity;
                    Data.Instance.updateInventory(requirement.resourceNameKey, requirementQuantity);
                }
            }
            #endregion

            //Apply the permanent boost
            isUnlocked = true;

            if(id.Equals(Data.OFFLINE_MAXTIME_BOOST) || id.Equals(Data.OFFLINE_PRODUCTIVITY_BOOST) || id.Equals(Data.SCARES_BOOST) || id.Equals(Data.DROPS_BOOST))
            {
                Data.Instance.BOOSTS.Add(id, 0); //Because the boost is only applied when the decoration boost is in the map
            }
            else
            {
                Data.Instance.BOOSTS.Add(id, 1);
            }
            //GameManager apply boost
            GameManager.Instance.applyBoost(id);

            setToOwned();
        }
        else
        {
            GameManager.Instance.audioSource.clip = GameManager.Instance.sounds[GameManager.ERROR];
            GameManager.Instance.audioSource.Play();
        }
    }

    public bool checkRequirements()
    {
        if (Data.Instance.INVENTORY.TryGetValue(requirement.resourceNameKey, out int requirementQuantity))
        {
            if (requirementQuantity < requirement.quantity)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            //Player don't have the requirement resource to produce 
            return false;
        }
    }

    public void setTextColor(TextMeshProUGUI text, int inventoryQuantity, int requirementQuantity)
    {
        if (inventoryQuantity >= requirementQuantity)
        {
            text.color = new Color(0, 1, 0, 1);
        }
        else
        {
            text.color = new Color(1, 0, 0, 1);
        }
    }
}
