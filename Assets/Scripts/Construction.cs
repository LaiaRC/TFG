using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{
    public string id;
    public string construction_name;
    public List<RequirementList> production_cost;
    public int numType = 0;

    [HideInInspector]
    public int constructionType; //0 -> general building / 1 -> summoning circle / 2 -> decoration boost

    public Vector3 position;
    public BoundsInt tempArea;
    public bool placed;
    public BoundsInt area;
    public GameObject confirmUI;
    protected Vector3 origin;

    
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
                
                //Update amount of buildings in inventory of constructions
                if (Data.Instance.BUILDING_INVENTORY.TryGetValue(id, out int quantity))
                {
                    quantity += 1;
                    Data.Instance.BUILDING_INVENTORY[id] = quantity;
                    numType = quantity;
                }
                else
                {
                    //It's the first building (of this type)
                    Data.Instance.BUILDING_INVENTORY.Add(id, 1);
                    numType = 1;
                }

                //Save to dictionary
                saveConstructionToDictionary();

                GameManager.Instance.constructionsBuilt.Add(this.gameObject);

                //Update shop price
                GameManager.Instance.updateShopPrices(id);
            }
        }
    }

    public virtual void saveConstructionToDictionary()
    {
        //Decoration Boost case (only need position)
        
        if (!Data.Instance.CONSTRUCTIONS.ContainsKey(id + numType))
        {
            Data.Instance.CONSTRUCTIONS.Add(id + numType, new float[] { transform.position.x, transform.position.y, 0, 0, 0, 0, 0, numType, 0, 2, 0, 0});
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

    void isOnCanvasOff()
    {
        GameManager.Instance.isOnCanvas = false;
    }

    #endregion
}
