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
    }

    // Update is called once per frame
    void Update()
    {
        if (placed && !boostApplied)
        {
            GameManager.Instance.applyBoost(id);
        }
    }

    public virtual bool applyBoost() {
        return boostApplied;
    }    
}
