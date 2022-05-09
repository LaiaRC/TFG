using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI debugInventoryInfo;
    public Animator buildModeAnimator;
    public bool isbuildingMode = false;
    public bool isOnCanvas;


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
        foreach (KeyValuePair<string, int> inventoryResource in Data.Instance.INVENTORY)
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

    public void toggleBuildMode()
    {
        if (isbuildingMode){
            buildModeAnimator.Play("buildPanelClosing");
            isbuildingMode = false;
        }
        else
        {
            buildModeAnimator.Play("buildPanelOpening");
            isbuildingMode = true;
        }
    }

    public void pointerEnter()
    {
        isOnCanvas = true;
    }

    public void pointerExit()
    {
        isOnCanvas = false;
    }

}
