using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public Dictionary<string, GameObject> BUILDINGS = new Dictionary<string, GameObject>();
    public Dictionary<string, Resource> RESOURCES = new Dictionary<string, Resource>();
    public Dictionary<string, int> INVENTORY = new Dictionary<string, int>();


    #region RESOURCES KEYS
    public static string BONE = "bone";
    public static string ROTTEN_FLESH = "rottenFlesh";
    public static string COFFIN = "coffin";
    public static string TOMBSTONE = "tombstone";    
    #endregion


    #region SINGLETON PATTERN
    public static Data Instance;

    //crear diccionaris publics de inventari i de crafteos

    //(x accedir desde altres scripts fer Data.Instance.inventaryDictionary. eetc
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        #region RESOURCES 
        Requirement[] requirements1;

        //Bone
        requirements1 = new Requirement[0];
        RESOURCES.Add(BONE, new Resource("Bone", BONE, true, 3, true, requirements1));

        //Rotten Flesh
        Requirement[] requirements2;
        Requirement requirement2 = new Requirement();

        requirements2 = new Requirement[1];

        requirement2.resourceNameKey = BONE;
        requirement2.quantity = 5;
        requirements2[0] = requirement2;

        RESOURCES.Add(ROTTEN_FLESH, new Resource("Rotten flesh", ROTTEN_FLESH, false, 5, true, requirements2));

        //Coffin
        Requirement[] requirements3;
        Requirement requirement3A = new Requirement();
        Requirement requirement3B = new Requirement();

        requirements3 = new Requirement[2];

        requirement3A.resourceNameKey = BONE;
        requirement3A.quantity = 5;
        requirements3[0] = requirement3A;

        requirement3B.resourceNameKey = ROTTEN_FLESH;
        requirement3B.quantity = 5;
        requirements3[1] = requirement3B;

        RESOURCES.Add(COFFIN, new Resource("Coffin", COFFIN, false, 5, true, requirements3));


        //Tombstone
        Requirement[] requirements4;
        Requirement requirement4A = new Requirement();
        Requirement requirement4B = new Requirement();
        Requirement requirement4C = new Requirement();

        requirements4 = new Requirement[3];

        requirement4A.resourceNameKey = BONE;
        requirement4A.quantity = 5;
        requirements4[0] = requirement4A;

        requirement4B.resourceNameKey = ROTTEN_FLESH;
        requirement4B.quantity = 5;
        requirements4[1] = requirement4B;

        requirement4C.resourceNameKey = COFFIN;
        requirement4C.quantity = 5;
        requirements4[2] = requirement4C;

        RESOURCES.Add(TOMBSTONE, new Resource("Tombstone", TOMBSTONE, false, 5, true, requirements4));
        #endregion
    }
    #endregion

    //Al start de la shopManager es passa el array de buldings (prefabs) omplert manualment cap aqui i es guarda en el diccionari
    public void setBuildings(List<GameObject> buildings)
    {
        foreach(GameObject building in buildings)
        {
            BUILDINGS.Add(building.GetComponent<Building>().building_name, building);
        }
    }

    public void updateInventory(string key, int quantity)
    {
        INVENTORY.Remove(key);
        INVENTORY.Add(key, quantity);
    }
}
