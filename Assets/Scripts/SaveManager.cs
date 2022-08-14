using GameDevWare.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private string pathInventory;
    private string pathConstructions;
    private string pathPlayer;
    private string pathMonsters;
    private string pathBoosts;

    #region SINGLETON PATTERN

    public static SaveManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        pathInventory = Application.persistentDataPath + "inventory.idk";
        pathConstructions = Application.persistentDataPath + "constructions.idk";
        pathPlayer = Application.persistentDataPath + "player.idk";
        pathMonsters = Application.persistentDataPath + "monsters.idk";
        pathBoosts = Application.persistentDataPath + "boosts.idk";
    }

    #endregion SINGLETON PATTERN

    private void Start()
    {
        pathInventory = Application.persistentDataPath + "inventory.idk";
        pathConstructions = Application.persistentDataPath + "constructions.idk";
        pathPlayer = Application.persistentDataPath + "player.idk";
        pathMonsters = Application.persistentDataPath + "monsters.idk";
        pathBoosts = Application.persistentDataPath + "boosts.idk";
    }

    public void Save()
    {
        //Update buildings time
        foreach (var construction in GameManager.Instance.constructionsBuilt)
        {
            //Debug.Log(construction.GetComponent<Building>().id + construction.GetComponent<Building>().numTypeBuildings);

            float[] oldValue = Data.Instance.CONSTRUCTIONS[construction.GetComponent<Construction>().id + construction.GetComponent<Construction>().numType];
            oldValue[GameManager.POS_X] = construction.transform.position.x;
            oldValue[GameManager.POS_Y] = construction.transform.position.y;
            oldValue[GameManager.NUM_TYPE] = construction.GetComponent<Construction>().numType;
            oldValue[GameManager.CONSTRUCTION_TYPE] = construction.GetComponent<Construction>().constructionType;

            //Check if it's decoration boost
            if (construction.GetComponent<DecorationBoost>() != null)
            {
                //It's decoration boost, put all to 0
                oldValue[GameManager.LEVEL] = 0;
                oldValue[GameManager.ACTIVE_RESOURCE] = 0;
                oldValue[GameManager.PRODUCING] = 0;
                oldValue[GameManager.PAUSED] = 0;
                oldValue[GameManager.ACTIVE_RESOURCE_TIME] = 0;
                oldValue[GameManager.IS_PRODUCER] = 0;
                oldValue[GameManager.IS_CONVERTER] = 0;
            }
            else
            {
                //It's a building

                //Save level or hiddenMonsterIndex depending if it's summoningCircle or not
                if (construction.GetComponent<Construction>().id.Contains("summoningCircle"))
                {
                    oldValue[GameManager.LEVEL] = GameManager.Instance.hidenMonsterIndex;
                }
                else
                {
                    oldValue[GameManager.LEVEL] = construction.GetComponent<Building>().level;
                }

                oldValue[GameManager.ACTIVE_RESOURCE] = construction.GetComponent<Building>().getNumActiveResource(); //Check if works with summoning circle
                oldValue[GameManager.TIME_LEFT] = (construction.GetComponent<Building>().timeLeft);
                oldValue[GameManager.PRODUCING] = construction.GetComponent<Building>().isProducing ? 1 : 0;
                oldValue[GameManager.PAUSED] = construction.GetComponent<Building>().isPaused ? 1 : 0;
                oldValue[GameManager.ACTIVE_RESOURCE_TIME] = construction.GetComponent<Building>().activeResourceTime;
                oldValue[GameManager.IS_PRODUCER] = construction.GetComponent<Building>().isProducer;
                oldValue[GameManager.IS_CONVERTER] = construction.GetComponent<Building>().isConverter;

                //Debug.Log("SM - " + construction.GetComponent<Construction>().construction_name + " - time left - " + oldValue[GameManager.TIME_LEFT]);
            }
            /*Debug.Log(oldValue[GameManager.POS_X]);
            Debug.Log(oldValue[GameManager.POS_Y]);
            Debug.Log(oldValue[GameManager.LEVEL]);
            Debug.Log(oldValue[GameManager.ACTIVE_RESOURCE]);
            Debug.Log(oldValue[GameManager.TIME_LEFT]);
            Debug.Log(oldValue[GameManager.PRODUCING]);
            Debug.Log(oldValue[GameManager.NUM_TYPE]);
            Debug.Log(oldValue[GameManager.ACTIVE_RESOURCE_TIME]);
            Data.Instance.CONSTRUCTIONS[construction.GetComponent<Building>().id + construction.GetComponent<Building>().numTypeBuildings] = oldValue;*/
        }

        //Update current time
        GameManager.Instance.updateLocalDate();
        Data.Instance.PLAYER["Hour"] = GameManager.Instance.localDate.Hour;
        Data.Instance.PLAYER["Minute"] = GameManager.Instance.localDate.Minute;
        Data.Instance.PLAYER["Second"] = GameManager.Instance.localDate.Second;
        Data.Instance.PLAYER["Day"] = GameManager.Instance.localDate.Day;
        Data.Instance.PLAYER["Month"] = GameManager.Instance.localDate.Month;
        Data.Instance.PLAYER["Year"] = GameManager.Instance.localDate.Year;
        Data.Instance.PLAYER["Tuto"] = GameManager.Instance.isTutoDone;
        Data.Instance.PLAYER["isRestart"] = 0; //only 1 when coming from minigame

        //Update inventory old value (same as current values)
        foreach (var item in Data.Instance.INVENTORY.ToList())
        {
            if (!item.Key.Contains("Old") && Data.Instance.INVENTORY.TryGetValue(item.Key + "Old", out int oldQuantity))
            {
                Data.Instance.INVENTORY[item.Key + "Old"] = item.Value; //Update old quantity to current quantity
            }
            else if (!item.Key.Contains("Old"))
            {
                Data.Instance.INVENTORY.Add(item.Key + "Old", item.Value);
            }
        }

        Delete();

        var outputStreamInventory = new FileStream(pathInventory, FileMode.Create);
        var outputStreamConstructions = new FileStream(pathConstructions, FileMode.Create);
        var outputStreamPlayer = new FileStream(pathPlayer, FileMode.Create);
        var outputStreamMonsters = new FileStream(pathMonsters, FileMode.Create);
        var outputStreamBoosts = new FileStream(pathBoosts, FileMode.Create);

        MsgPack.Serialize(Data.Instance.INVENTORY, outputStreamInventory);
        MsgPack.Serialize(Data.Instance.CONSTRUCTIONS, outputStreamConstructions);
        MsgPack.Serialize(Data.Instance.PLAYER, outputStreamPlayer);
        MsgPack.Serialize(Data.Instance.MONSTERS_STATS, outputStreamMonsters);
        MsgPack.Serialize(Data.Instance.BOOSTS, outputStreamBoosts);

        outputStreamInventory.Position = 0; // rewind stream before copying/read
        outputStreamConstructions.Position = 0;
        outputStreamPlayer.Position = 0;
        outputStreamMonsters.Position = 0;
        outputStreamBoosts.Position = 0;

        GameManager.Instance.saved = true;

        outputStreamInventory.Close();
        outputStreamConstructions.Close();
        outputStreamPlayer.Close();
        outputStreamMonsters.Close();
        outputStreamBoosts.Close();
    }

    public void SaveInventory()
    {
        //Save minigame drops to inventory
        if (File.Exists(pathInventory))
        {
            File.Delete(pathInventory);
        }
        var outputStreamInventory = new FileStream(pathInventory, FileMode.Create);
        MsgPack.Serialize(Data.Instance.INVENTORY, outputStreamInventory);
        outputStreamInventory.Position = 0; // rewind stream before copying/read
        outputStreamInventory.Close();
    }

    public void SaveTuto()
    {
        //Save minigame drops to inventory
        if (File.Exists(pathPlayer))
        {
            File.Delete(pathPlayer);
        }
        var outputStreamPlayer = new FileStream(pathPlayer, FileMode.Create);
        MsgPack.Serialize(Data.Instance.PLAYER, outputStreamPlayer);
        outputStreamPlayer.Position = 0; // rewind stream before copying/read
        outputStreamPlayer.Close();
    }

    public void Load()
    {
        if (File.Exists(pathInventory))
        {
            FileStream inputStreamInventory = new FileStream(pathInventory, FileMode.Open);
            Data.Instance.INVENTORY = MsgPack.Deserialize(typeof(Dictionary<string, int>), inputStreamInventory) as Dictionary<string, int>;
            inputStreamInventory.Close();
        }
        if (File.Exists(pathConstructions))
        {
            FileStream inputStreamConstructions = new FileStream(pathConstructions, FileMode.Open);
            Data.Instance.CONSTRUCTIONS = MsgPack.Deserialize(typeof(Dictionary<string, float[]>), inputStreamConstructions) as Dictionary<string, float[]>;

            GameManager.Instance.buildConstructions();
            //GameManager.Instance.loadBoosts();

            inputStreamConstructions.Close();
        }
        if (File.Exists(pathPlayer))
        {
            FileStream inputStreamPlayer = new FileStream(pathPlayer, FileMode.Open);
            Data.Instance.PLAYER = MsgPack.Deserialize(typeof(Dictionary<string, int>), inputStreamPlayer) as Dictionary<string, int>;

            inputStreamPlayer.Close();

            //update is tuto done
            if (Data.Instance.PLAYER.TryGetValue("Tuto", out int isDone))
            {
                GameManager.Instance.isTutoDone = isDone;
            }
            //update is tuto done
            if (Data.Instance.PLAYER.TryGetValue("isRestart", out int isRestart))
            {
                GameManager.Instance.isRestart = isRestart;
            }
        }
        if (File.Exists(pathMonsters))
        {
            FileStream inputStreamMonsters = new FileStream(pathMonsters, FileMode.Open);
            Data.Instance.MONSTERS_STATS = MsgPack.Deserialize(typeof(Dictionary<string, int[]>), inputStreamMonsters) as Dictionary<string, int[]>;
            inputStreamMonsters.Close();

            GameManager.Instance.setMonstersDictionary();
        }
        if (File.Exists(pathBoosts))
        {
            FileStream inputStreamBoosts = new FileStream(pathBoosts, FileMode.Open);
            Data.Instance.BOOSTS = MsgPack.Deserialize(typeof(Dictionary<string, int>), inputStreamBoosts) as Dictionary<string, int>;
            GameManager.Instance.applyAllBoosts();
            GameManager.Instance.boostShop.GetComponent<BoostShop>().checkBoosts();

            inputStreamBoosts.Close();
        }
        GameManager.Instance.calculateOfflineTime();
        GameManager.Instance.saved = false;
    }

    public void Delete()
    {
        if (File.Exists(pathInventory))
        {
            File.Delete(pathInventory);
        }
        if (File.Exists(pathConstructions))
        {
            File.Delete(pathConstructions);
        }
        if (File.Exists(pathPlayer))
        {
            File.Delete(pathPlayer);
        }
        if (File.Exists(pathMonsters))
        {
            File.Delete(pathMonsters);
        }
    }

    public void Restart()
    {
        //Borrar tot menys inventory (nomes te drops i scares)
        if (File.Exists(pathConstructions))
        {
            File.Delete(pathConstructions);
        }
        GameManager.Instance.constructionsBuilt.Clear();
        Data.Instance.CONSTRUCTIONS.Clear();
        foreach (KeyValuePair<string, int[]> stat in Data.Instance.MONSTERS_STATS)
        {
            stat.Value[1] = 1;
        }

        Data.Instance.MONSTERS.Clear();
    }

    public void DeleteDebug() //Nomes per debug, eventualment es pot borrar i utilitzar l'altre
    {
        if (File.Exists(pathInventory))
        {
            File.Delete(pathInventory);
        }
        if (File.Exists(pathConstructions))
        {
            File.Delete(pathConstructions);
        }
        if (File.Exists(pathPlayer))
        {
            File.Delete(pathPlayer);
        }
        if (File.Exists(pathMonsters))
        {
            File.Delete(pathMonsters);
        }
        GameManager.Instance.constructionsBuilt.Clear();
        Data.Instance.CONSTRUCTIONS.Clear();
        Data.Instance.INVENTORY.Clear();
        //Data.Instance.PLAYER.Clear();
        Data.Instance.MONSTERS_STATS.Clear();
        Data.Instance.MONSTERS.Clear();
        Data.Instance.BOOSTS.Clear();
    }
}