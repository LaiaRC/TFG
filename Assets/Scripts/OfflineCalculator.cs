using GameDevWare.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OfflineCalculator : MonoBehaviour
{
    public GameObject fakeProducer;   

    string pathConstructions;
    string pathInventory;
    string pathPlayer;

    private float offlineTime = 0;

    [HideInInspector]
    public float REDUCTION_FACTOR = 1;
    [HideInInspector]
    public float LOAD_TIME = 10f;
    [HideInInspector]
    public float TIME_ESCALE_MULTIPLIER = 100;
    public int numType;
    public List<GameObject> buildings;

    public DateTime localDate;

    public List<FakeProducer> constructionsBuilt = new List<FakeProducer>();
    private bool offlineBoostApplied = false;

    private int offlineBoostTime;

    private static float ANIM_TIME = 3f; 

    public static OfflineCalculator Instance;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        pathConstructions = Application.persistentDataPath + "constructions.idk";
        pathInventory = Application.persistentDataPath + "inventory.idk";
        pathPlayer = Application.persistentDataPath + "player.idk";
    }

    // Start is called before the first frame update
    void Start()
    {
        //Load data
        if (File.Exists(pathConstructions))
        {
            FileStream inputStreamConstructions = new FileStream(pathConstructions, FileMode.Open);
            Data.Instance.CONSTRUCTIONS = MsgPack.Deserialize(typeof(Dictionary<string, float[]>), inputStreamConstructions) as Dictionary<string, float[]>;

            inputStreamConstructions.Close();
        }
        if (File.Exists(pathInventory))
        {
            FileStream inputStreamInventory = new FileStream(pathInventory, FileMode.Open);
            Data.Instance.INVENTORY = MsgPack.Deserialize(typeof(Dictionary<string, int>), inputStreamInventory) as Dictionary<string, int>;
            inputStreamInventory.Close();
        }
        if (File.Exists(pathPlayer))
        {
            FileStream inputStreamPlayer = new FileStream(pathPlayer, FileMode.Open);
            Data.Instance.PLAYER = MsgPack.Deserialize(typeof(Dictionary<string, int>), inputStreamPlayer) as Dictionary<string, int>;

            inputStreamPlayer.Close();
        }

        //Load boosts
        loadBoostsDictionary();

        if (Data.Instance.CONSTRUCTIONS.Count > 0)
        {           
            Data.Instance.setBuildings(buildings);
            instantiateFakeProducers();
            
            calculateOfflineTime();
            applyBoostTime();
        }
        else
        {
            Invoke(nameof(loadMainScene), ANIM_TIME);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void calculateOfflineTime()
    {
        DateTime previousDateTime = new DateTime(Data.Instance.PLAYER["Year"], Data.Instance.PLAYER["Month"], Data.Instance.PLAYER["Day"], Data.Instance.PLAYER["Hour"], Data.Instance.PLAYER["Minute"], Data.Instance.PLAYER["Second"]);

        updateLocalDate();

        offlineTime = (float)(localDate - previousDateTime).TotalSeconds;

        //Calculate max time out depending on boost
        float maxTimeOut = 4 * 3600; //Default value -> 4h

        if (Data.Instance.BOOSTS.TryGetValue(Data.OFFLINE_MAXTIME_BOOST, out int quantity))
        {
            for (int i = 0; i < quantity; i++)
            {
                maxTimeOut += 3600; // +1h for each boost
            }
        }

        if (offlineTime > maxTimeOut)
        {
            offlineTime = maxTimeOut;
        }
        LOAD_TIME = 20f;
        TIME_ESCALE_MULTIPLIER = 100;
        REDUCTION_FACTOR = 100;
    }

    public void updateLocalDate()
    {
        localDate = DateTime.Now;
    }

    public void instantiateFakeProducers()
    {
        foreach (KeyValuePair<string, float[]> buildingInfo in Data.Instance.CONSTRUCTIONS)
        {
            if (buildingInfo.Value[Data.CONSTRUCTION_TYPE] == 0 || buildingInfo.Value[Data.CONSTRUCTION_TYPE] == 1)
            {
                //It's a building
                FakeProducer aux = Instantiate(fakeProducer).GetComponent<FakeProducer>();

                if (buildingInfo.Value[Data.CONSTRUCTION_TYPE] == 0)
                {
                    //General building
                    if (Data.Instance.BUILDINGS.TryGetValue(buildingInfo.Key.Substring(0, buildingInfo.Key.Length - 1), out GameObject building))
                    {
                        aux.activeResource = building.GetComponent<Building>().resources[(int)buildingInfo.Value[Data.ACTIVE_RESOURCE]];
                    }
                } else if (buildingInfo.Value[Data.CONSTRUCTION_TYPE] == 1) {
                    //Summoning circle
                    if (Data.Instance.BUILDINGS.TryGetValue(buildingInfo.Key.Substring(0, buildingInfo.Key.Length - 1), out GameObject building))
                    {
                        aux.activeResource = building.GetComponent<SummoningCircle>().getStringMonster((int)buildingInfo.Value[Data.ACTIVE_RESOURCE]);
                    }
                }
                aux.activeResourceTime = buildingInfo.Value[Data.ACTIVE_RESOURCE_TIME];
                if (buildingInfo.Value[Data.PRODUCING] == 0)
                {
                    aux.isProducing = false;
                }
                else
                {
                    aux.isProducing = true;
                }
                if (buildingInfo.Value[Data.PAUSED] == 0)
                {
                    aux.isPaused = false;
                }
                else
                {
                    aux.isPaused = true;
                }
                aux.time = buildingInfo.Value[Data.ACTIVE_RESOURCE_TIME] - buildingInfo.Value[Data.TIME_LEFT];
                aux.timeLeft = buildingInfo.Value[Data.TIME_LEFT];
                aux.activeResourceTime = buildingInfo.Value[Data.ACTIVE_RESOURCE_TIME];
                aux.isProducer = (int)buildingInfo.Value[Data.IS_PRODUCER];
                aux.isConverter = (int)buildingInfo.Value[Data.IS_CONVERTER];
                aux.isSummoningCircle = (int)buildingInfo.Value[Data.CONSTRUCTION_TYPE]; //1 -> summoning circle
                aux.id = buildingInfo.Key;

                constructionsBuilt.Add(aux);
            }
        }
    }

    public void setReducedResourcesTime()
    {
        foreach (FakeProducer building in constructionsBuilt)
        {
            if (building.isProducing)
            {
                building.activeResourceTime = building.activeResourceTime / REDUCTION_FACTOR;
                building.time = building.time / REDUCTION_FACTOR;
            }
        }
    }

    public void setOriginalResourcesTime()
    {
        foreach (FakeProducer building in constructionsBuilt)
        {
            if (building.isProducing)
            {
                building.activeResourceTime = building.activeResourceTime * REDUCTION_FACTOR;
                building.time = building.time * REDUCTION_FACTOR;
            }
        }
    }

    public void applyBoostTime()
    {
        if (!offlineBoostApplied)
        {
            setReducedResourcesTime();
            Time.timeScale = TIME_ESCALE_MULTIPLIER;

            //Check offline productivity boost

            float offlineProductivityBoost = 0.25f; //Default value

            if (Data.Instance.BOOSTS.TryGetValue(Data.OFFLINE_PRODUCTIVITY_BOOST, out int quantity))
            {
                for (int i = 0; i < quantity; i++)
                {
                    offlineProductivityBoost += 0.25f;
                }
            }

            //Formula per calcular el temps que passa tenint en compte el multiplier escale, el reduction factor i el offline time (per compensar el canvi d'escala del time.timescale) i el 6 de la formula esta patillat (sistema hexadecimal(no))
            Invoke("stopOfflineProduction", (((offlineTime/(TIME_ESCALE_MULTIPLIER*REDUCTION_FACTOR)) + (offlineTime / (TIME_ESCALE_MULTIPLIER * REDUCTION_FACTOR))/6) * TIME_ESCALE_MULTIPLIER) * offlineProductivityBoost);
        }
    }

    public void stopOfflineProduction()
    {
        setOriginalResourcesTime();
        Time.timeScale = 1;
        offlineBoostApplied = true;        

        //Save inventory and update time left
        SaveManager.Instance.SaveInventory();
        save();
        Invoke(nameof(loadMainScene), 5f);        
    }

    public void loadMainScene()
    {
        SceneManager.LoadScene("globalView");
    }

    public void loadBoostsDictionary()
    {
        Data.Instance.BOOSTS.Clear(); //just in case

        foreach (KeyValuePair<string, float[]> construction in Data.Instance.CONSTRUCTIONS)
        {
            if (construction.Value[Data.CONSTRUCTION_TYPE] == 2)
            {
                //It's a decoration boost

                //Get id
                string id = construction.Key.Substring(0, construction.Key.Length - 1);
                if (Data.Instance.BOOSTS.TryGetValue(id, out int quantity))
                {
                    Data.Instance.BOOSTS[id] += 1;
                }
                else
                {
                    Data.Instance.BOOSTS.Add(id, 1);
                }
            }
        }
    }

    public void save()
    {
        //Update time left in constructions dictionary
        foreach (FakeProducer fakeProducer in constructionsBuilt)
        {
            float[] oldValue = Data.Instance.CONSTRUCTIONS[fakeProducer.id];

            oldValue[Data.TIME_LEFT] = fakeProducer.activeResourceTime - fakeProducer.time; //Not directly timeLeftSave because it hasn't got the time to be recalculated
        }

        if (File.Exists(pathConstructions))
        {
            File.Delete(pathConstructions);
        }

        var outputStreamConstructions = new FileStream(pathConstructions, FileMode.Create);
        MsgPack.Serialize(Data.Instance.CONSTRUCTIONS, outputStreamConstructions);
        outputStreamConstructions.Position = 0;
        outputStreamConstructions.Close();
    }
}
