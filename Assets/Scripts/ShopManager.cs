using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[System.Serializable]

public class ShopManager : MonoBehaviour
{
    public int coins;
    public TextMeshProUGUI coinsTxt;
    private TerrenoEdificable terreno_selected;
    public List<GameObject> buildings;


    void Start()
    {
        Data.Instance.setBuildings(buildings);
    }

    void Update()
    {
      
    }

    public void Buy(string buildingToBuild)
    {
        if (!terreno_selected.hasBuilding)
        {
            if (Data.Instance.BUILDINGS.TryGetValue(buildingToBuild, out GameObject building))
            {
                if (building.GetComponent<Building>().production_cost.Count > 0)
                {
                    bool enoughResource = false;
                    foreach (Requirement requeriment in building.GetComponent<Building>().production_cost)
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
                        if (!enoughResource) return;
                    }
                    if (enoughResource)
                    {
                        foreach (Requirement requeriment in building.GetComponent<Building>().production_cost)
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
                        GameManager.Instance.buildBuilding(building.GetComponent<Building>(), terreno_selected);
                    }
                }
                else
                {
                    GameManager.Instance.buildBuilding(building.GetComponent<Building>(), terreno_selected);
                }
            }
        }        
    }

    public void setTerrenoEdificable(TerrenoEdificable terrenoEdificable) {
        this.terreno_selected = terrenoEdificable;
    }
}
