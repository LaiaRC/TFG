using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TerrenoEdificable : MonoBehaviour
{
    public bool isSelected = false;
    public bool hasBuilding = false;
    public Building building; //(x saber el tipus)
    public GameObject building_prefab;

    private bool isBuilt = false;

    private void Update()
    {
        if (isSelected)
        {
            if (hasBuilding && isBuilt)
            {
                building.isSelected = true;
            }
        }

        if (hasBuilding && !isBuilt)
        {
            building = Instantiate(building_prefab,transform.position,Quaternion.identity).GetComponent<Building>(); //posicio del terrenoEdificable, sense rotacio
            isBuilt = true;
        }
    }

    
}
