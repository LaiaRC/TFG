using GameDevWare.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    string pathInventory;
    string pathConstructions;
    string pathPlayer;
    string pathMonsters;

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
    }
    #endregion    

    private void Start()
    {
        pathInventory = Application.persistentDataPath + "inventory.idk";
        pathConstructions = Application.persistentDataPath + "constructions.idk";
        pathPlayer = Application.persistentDataPath + "player.idk";
        pathMonsters = Application.persistentDataPath + "monsters.idk";
    }
    public void Save()
    {
        //Update buildings time
        foreach (var construction in GameManager.Instance.constructionsBuilt)
        {
            //Debug.Log(construction.GetComponent<Building>().id + construction.GetComponent<Building>().numTypeBuildings);

            float[] oldValue = Data.Instance.CONSTRUCTIONS[construction.GetComponent<Building>().id + construction.GetComponent<Building>().numTypeBuildings];
            oldValue[GameManager.POS_X] = construction.transform.position.x;
            oldValue[GameManager.POS_Y] = construction.transform.position.y;

            //Save lever or hiddenMonsterIndex depending if it's summoningCircle or not
            if (construction.GetComponent<Building>().id.Contains("summoningCircle"))
            {
                oldValue[GameManager.LEVEL] = GameManager.Instance.hidenMonsterIndex;
            }
            else
            {
                oldValue[GameManager.LEVEL] = construction.GetComponent<Building>().level;
            }

            oldValue[GameManager.ACTIVE_RESOURCE] = construction.GetComponent<Building>().getNumActiveResource();
            oldValue[GameManager.TIME_LEFT] = (construction.GetComponent<Building>().timeLeft);
            oldValue[GameManager.PRODUCING] = construction.GetComponent<Building>().isProducing ? 1 : 0;
            oldValue[GameManager.PAUSED] = construction.GetComponent<Building>().isPaused ? 1 : 0;
            oldValue[GameManager.NUM_TYPE] = construction.GetComponent<Building>().numTypeBuildings;
            oldValue[GameManager.ACTIVE_RESOURCE_TIME] = construction.GetComponent<Building>().activeResourceTime;
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

        Delete();
        
        var outputStreamInventory = new FileStream(pathInventory, FileMode.Create);
        var outputStreamConstructions = new FileStream(pathConstructions, FileMode.Create);      
        var outputStreamPlayer = new FileStream(pathPlayer, FileMode.Create);      
        var outputStreamMonsters = new FileStream(pathMonsters, FileMode.Create);      

        MsgPack.Serialize(Data.Instance.INVENTORY, outputStreamInventory);
        MsgPack.Serialize(Data.Instance.CONSTRUCTIONS, outputStreamConstructions);
        MsgPack.Serialize(Data.Instance.PLAYER, outputStreamPlayer);
        MsgPack.Serialize(Data.Instance.MONSTERS_STATS, outputStreamMonsters);

        outputStreamInventory.Position = 0; // rewind stream before copying/read
        outputStreamConstructions.Position = 0;
        outputStreamPlayer.Position = 0;
        outputStreamMonsters.Position = 0;

        GameManager.Instance.saved = true;

        outputStreamInventory.Close();
        outputStreamConstructions.Close();
        outputStreamPlayer.Close();
        outputStreamMonsters.Close();
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
            inputStreamConstructions.Close();
        }
        if (File.Exists(pathPlayer))
        {
            FileStream inputStreamPlayer = new FileStream(pathPlayer, FileMode.Open);
            Data.Instance.PLAYER = MsgPack.Deserialize(typeof(Dictionary<string, int>), inputStreamPlayer) as Dictionary<string, int>;
            
            inputStreamPlayer.Close();
        }
        if (File.Exists(pathMonsters))
        {
            FileStream inputStreamMonsters = new FileStream(pathMonsters, FileMode.Open);
            Data.Instance.MONSTERS_STATS = MsgPack.Deserialize(typeof(Dictionary<string, int[]>), inputStreamMonsters) as Dictionary<string, int[]>;
            inputStreamMonsters.Close();

            GameManager.Instance.setMonstersDictionary();
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
        if (File.Exists(pathPlayer))
        {
            File.Delete(pathPlayer);
        }
        GameManager.Instance.constructionsBuilt.Clear();
        Data.Instance.CONSTRUCTIONS.Clear();
        Data.Instance.MONSTERS_STATS.Clear();
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
        GameManager.Instance.constructionsBuilt.Clear();
        Data.Instance.CONSTRUCTIONS.Clear();
        Data.Instance.INVENTORY.Clear();
        Data.Instance.MONSTERS_STATS.Clear();
        Data.Instance.MONSTERS.Clear();
    }
}
