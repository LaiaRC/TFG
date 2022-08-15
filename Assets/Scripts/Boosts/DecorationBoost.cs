using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DecorationBoost : Construction
{
    public List<GameObject> affectedBuildings;
    public List<Sprite> affectedBuildingsIcons;
    public string type; //General or Ranged
    public bool boostApplied = false;

    // Start is called before the first frame update
    void Start()
    {
        //Save boost decoration position
        position = transform.position;
        constructionType = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (placed && !boostApplied)
        {
            if (Data.Instance.BOOSTS.TryGetValue(id, out int quantity))
            {
                Data.Instance.BOOSTS[id] += 1;
            }
            else
            {
                Data.Instance.BOOSTS.Add(id, 1);
            }

            //If it's summoning circle boost update the time
            if (id.Equals(Data.SUMMONING_BOOST))
            {
                foreach (GameObject building in GameManager.Instance.constructionsBuilt)
                {
                    if (building.GetComponent<SummoningCircle>() != null)
                    {
                        building.GetComponent<SummoningCircle>().updateActiveMonsterTime();
                    }
                }
            }

            //Check if it's producer or converter boost and update numResource of affected buildings
            if (id.Equals(Data.PRODUCER_BOOST))
            {
                foreach (GameObject building in GameManager.Instance.constructionsBuilt)
                {
                    if(building.GetComponent<GeneralBuilding>() != null  && building.GetComponent<GeneralBuilding>().isProducer == 1)
                    {
                        building.GetComponent<Building>().updateNumResource(Data.Instance.BOOSTS[id] + 1);
                    }
                }
            }
            if (id.Equals(Data.CONVERTER_BOOST))
            {
                foreach (GameObject building in GameManager.Instance.constructionsBuilt)
                {
                    if (building.GetComponent<GeneralBuilding>() != null && building.GetComponent<GeneralBuilding>().isConverter == 1)
                    {
                        building.GetComponent<Building>().updateNumResource(Data.Instance.BOOSTS[id] + 1);
                    }
                }
            }

            boostApplied = true;
        }
    }

    public virtual bool applyBoost() {
        return boostApplied;
    }    
}
