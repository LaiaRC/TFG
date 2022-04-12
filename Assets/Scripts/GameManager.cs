using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI debugInventoryInfo;

    private string info = "";

    #region SINGLETON PATTERN
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    #endregion

    private void Update()
    {
        foreach (KeyValuePair<string, int> inventoryResource in Data.Instance.INVERTORY)
        {
            info += "\n -" + inventoryResource.Key + ": " + inventoryResource.Value;
        }
        debugInventoryInfo.text = info;
        info = "";
    }

    public bool buildBuilding(Building building, TerrenoEdificable terrenoEdificable)
    {
        if (!terrenoEdificable.hasBuilding)
        {
            terrenoEdificable.building = building;
            terrenoEdificable.building_prefab = building.gameObject;
            terrenoEdificable.hasBuilding = true;
            return true;
        }
        else
        {
            return false;
        }
    }


}
