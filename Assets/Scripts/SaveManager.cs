using GameDevWare.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
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
    }
    #endregion

    string pathInventory;
    string pathConstructions;
    string pathPlayer;

    private void Start()
    {
        pathInventory = Application.persistentDataPath + "inventory.idk";
        pathConstructions = Application.persistentDataPath + "constructions.idk";
        pathPlayer = Application.persistentDataPath + "player.idk";
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
            oldValue[GameManager.LEVEL] = construction.GetComponent<Building>().level;
            oldValue[GameManager.ACTIVE_RESOURCE] = construction.GetComponent<Building>().getNumActiveResource();
            oldValue[GameManager.TIME_LEFT] = (construction.GetComponent<Building>().timeLeft);
            oldValue[GameManager.PRODUCING] = construction.GetComponent<Building>().isProducing ? 1 : 0;
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

        var outputStreamInventory = new FileStream(pathInventory, FileMode.Create);
        var outputStreamConstructions = new FileStream(pathConstructions, FileMode.Create);      
        var outputStreamPlayer = new FileStream(pathPlayer, FileMode.Create);      

        MsgPack.Serialize(Data.Instance.INVENTORY, outputStreamInventory);
        MsgPack.Serialize(Data.Instance.CONSTRUCTIONS, outputStreamConstructions);
        MsgPack.Serialize(Data.Instance.PLAYER, outputStreamPlayer);

        outputStreamInventory.Position = 0; // rewind stream before copying/read
        outputStreamConstructions.Position = 0;
        outputStreamPlayer.Position = 0;

        GameManager.Instance.saved = true;
    }

    public void Load()
    {
        if (File.Exists(pathInventory))
        {
            FileStream inputStreamInventory = new FileStream(pathInventory, FileMode.Open);
            Data.Instance.INVENTORY = MsgPack.Deserialize(typeof(Dictionary<string, int>), inputStreamInventory) as Dictionary<string, int>;
        }
        if (File.Exists(pathConstructions))
        {
            FileStream inputStreamConstructions = new FileStream(pathConstructions, FileMode.Open);
            Data.Instance.CONSTRUCTIONS = MsgPack.Deserialize(typeof(Dictionary<string, float[]>), inputStreamConstructions) as Dictionary<string, float[]>;

            GameManager.Instance.buildConstructions();
        }
        if (File.Exists(pathPlayer))
        {
            FileStream inputStreamPlayer = new FileStream(pathPlayer, FileMode.Open);
            Data.Instance.PLAYER = MsgPack.Deserialize(typeof(Dictionary<string, int>), inputStreamPlayer) as Dictionary<string, int>;

            GameManager.Instance.calculateOfflineTime();
        }
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
            GameManager.Instance.constructionsBuilt.Clear();
            Data.Instance.CONSTRUCTIONS.Clear();
            Data.Instance.INVENTORY.Clear();
        }
    }
}
