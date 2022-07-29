using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectre : DecorationBoost
{
    public int numSpectres = 0;

    override
    public bool applyBoost()
    {
        //Get num of spectres
        if (Data.Instance.BUILDING_INVENTORY.TryGetValue(id, out int quantity))
        {
            numSpectres = quantity;
        }

        if (numSpectres < 3)
        {
            GameManager.Instance.offlineBoostTimeModifier += 0.15f;
        }
        else
        {
            GameManager.Instance.offlineBoostTimeModifier += 0.2f; //So the last upgrade gives +50%
        }
        boostApplied = true;
        return boostApplied;
    }
}
