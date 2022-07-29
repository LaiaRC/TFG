using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodMoonTower : DecorationBoost
{
    public int numBloodMoonTower = 0;

    override
    public bool applyBoost()
    {
        //Get affected buildings
        foreach (GameObject building in GameManager.Instance.constructionsBuilt)
        {
            if (building.GetComponent<SummoningCircle>() != null)
            {
                affectedBuildings.Add(building);
            }
        }

        //Get num of active blood moon towers
        if (Data.Instance.BUILDING_INVENTORY.TryGetValue(id, out int quantity))
        {
            numBloodMoonTower = quantity;
        }

        //Apply boost to affected buildings
        for (int i = 0; i < affectedBuildings.Count; i++)
        {
            if (numBloodMoonTower < 3)
            {
                affectedBuildings[i].GetComponent<SummoningCircle>().timeModifier += 0.15f;
                affectedBuildings[i].GetComponent<SummoningCircle>().updateActiveMonsterTime();
            }
            else
            {
                affectedBuildings[i].GetComponent<SummoningCircle>().timeModifier += 0.2f; //So the last upgrade gives -50%
                affectedBuildings[i].GetComponent<SummoningCircle>().updateActiveMonsterTime();
            }
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
