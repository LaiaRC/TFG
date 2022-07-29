using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gargoyle : DecorationBoost
{
    override
    public bool applyBoost()
    {
        //Get affected buildings
        foreach (GameObject building in GameManager.Instance.constructionsBuilt)
        {
            if (building.GetComponent<GeneralBuilding>() != null && building.GetComponent<GeneralBuilding>().isProducer)
            {
                affectedBuildings.Add(building);
            }
        }

        //Apply boost to affected buildings
        foreach (GameObject building in affectedBuildings)
        {
            building.GetComponent<GeneralBuilding>().extraNumResources += 1;
        }

        if (affectedBuildings.Count > 0)
        {
            boostApplied = true;
        }
        else
        {
            //There where no buildings to apply the boost
            boostApplied = false;
        }

        return boostApplied;
    }
}
