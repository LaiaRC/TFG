using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class ShopItemHolder : MonoBehaviour
{
    private ShopItem Item;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image iconImage;
    //[SerializeField] private Image currencyImage;
    [SerializeField] private TextMeshProUGUI resourceText1;
    [SerializeField] private TextMeshProUGUI resourceText2;
    [SerializeField] private Image resource1Icon;
    [SerializeField] private Image resource2Icon;

    private string idResource1 = "";
    private string resourceText1Aux = "0";
    private string idResource2 = "";
    private string resourceText2Aux = "0";
    private bool isInitialized = false;
    private int result = 0;
    private bool isDragActivated = false;
    private bool hasReachedLimit = false;
    private int currentQuantity = 0;

    private void Update()
    {
        if (GameManager.Instance.isOnBuildingMode)
        {
            //Update cost & amount text
            if (isInitialized)
            {
                updateTextCost(Item.resourceText1, resourceText1, resource1Icon.sprite.name);
                updateTextCost(Item.resourceText2, resourceText2, resource2Icon.sprite.name);

                if(Data.Instance.BUILDING_INVENTORY.TryGetValue(Item.id, out int quantity))                {
                    if(currentQuantity != quantity)
                    {
                        currentQuantity = quantity;
                        updateTextAmount(currentQuantity);
                    }
                }
            }

            //Activate/Deactivate if an item can be dragged 
            if (GameManager.Instance.checkRequirements(Item.id) && !isDragActivated && !hasReachedLimit)
            {
                activateDrag();
            }
            if(!GameManager.Instance.checkRequirements(Item.id) && isDragActivated && !hasReachedLimit)
            {
                deactivateDrag();
            }
        }
    }

    public void initialize(ShopItem item)
    {
        Item = item;

        iconImage.sprite = Item.icon;
        titleText.text = Item.name;
        //descriptionText.text = Item.description;
        amountText.text = currentQuantity.ToString() + "/" + Item.maxQuantity;

        //Hide icons if there is no requirement
        if(Item.resource1Icon != null)
        {
            resource1Icon.sprite = Item.resource1Icon;
        }
        else
        {
            resource1Icon.color = new Color(1,1,1,0);
        }

        if (Item.resource2Icon != null)
        {
            resource2Icon.sprite = Item.resource2Icon;
        }
        else
        {
            resource2Icon.color = new Color(1, 1, 1, 0);
        }

        //set cost text
        updateTextCost(Item.resourceText1, resourceText1, resource1Icon.sprite.name);
        updateTextCost(Item.resourceText2, resourceText2, resource2Icon.sprite.name);

        unlockItem();
        isInitialized = true;
    }

    public void unlockItem()
    {
        iconImage.gameObject.AddComponent<ShopItemDrag>().initialize(Item);
        deactivateDrag();
    }

    public void updateTextCost(string text, TextMeshProUGUI textShop, string iconId)
    {
        if (text != "")
        {
            if (Data.Instance.INVENTORY.TryGetValue(iconId, out int quantity))
            {
                if (quantity > 0)
                {
                    int.TryParse(text, out result);
                    textShop.text = quantity.ToString() + "/" + text;

                    //Set color of text
                    if (!hasReachedLimit)
                    {
                        if (quantity >= result)
                        {
                            textShop.color = new Color(0, 1, 0, 1);
                        }
                        else
                        {
                            textShop.color = new Color(1, 0, 0, 1);
                        }
                    }
                    else
                    {
                        textShop.color = new Color(1, 1, 1, 1);
                    }
                }
                else
                {
                    textShop.text = "0" + "/" + text;
                    if (!hasReachedLimit)
                    {
                        textShop.color = new Color(1, 0, 0, 1);
                    }
                    else
                    {
                        textShop.color = new Color(1, 1, 1, 1);
                    }
                }
            }
            else
            {
                textShop.text = "0" + "/" + text;
                if (!hasReachedLimit)
                {
                    textShop.color = new Color(1, 0, 0, 1);
                }
                else
                {
                    textShop.color = new Color(1, 1, 1, 1);
                }
            }
        }
        else
        {
            if(textShop.name == resourceText1.name)
            {
                textShop.text = "free";
                if (!hasReachedLimit)
                {
                    textShop.color = new Color(0, 1, 0, 1);
                }
                else
                {
                    textShop.color = new Color(1,1,1,1);
                }
            }
            else
            {
                textShop.text = "";
            }           
        }
    }

    public void updateTextAmount(int currentQuantity)
    {
        amountText.text = currentQuantity.ToString() + "/" + Item.maxQuantity;
        if(currentQuantity == Item.maxQuantity)
        {
            hasReachedLimit = true;
            deactivateDrag();
            amountText.color = new Color(1, 0, 0, 1);
        }
    }
    
    public void activateDrag()
    {
        isDragActivated = true;
        iconImage.gameObject.GetComponent<ShopItemDrag>().enabled = true;
        iconImage.color = new Color(1, 1, 1, 1);
    }
    public void deactivateDrag()
    {
        isDragActivated = false;
        iconImage.gameObject.GetComponent<ShopItemDrag>().enabled = false;
        iconImage.color = new Color(1, 1, 1, 0.25f);
    }

    public void callDescriptionDialog()
    {
        GameManager.Instance.fillDescriptionDialog(Item.name, Item.description, Item.icon, Item.produces1, Item.produces2, Item.produces3);
        GameManager.Instance.showDescriptionDialog();
    }
}
