using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class SummoningCircle : Building
{
    /*public string building_name; //nom del building
    public string id;
    public List<Requirement> production_cost;
    public bool isProducing = false;

    public Vector3 position;
    public BoundsInt tempArea;
    public string activeResource; //Quina resource s'esta produïnt
    public int numTypeBuildings = 0;
    public float timeLeft;
    public float time = 0; //comptador de temps fins el activeResourceTime
    public bool isPaused = false;

    public bool placed;
    public BoundsInt area;
    public GameObject confirmUI;
    public GameObject canvasInterior;

    public float activeResourceTime = 0;   //temps que triga a fer-se el active resource
    private Vector3 origin;
    private bool showUI = false;
    private bool enoughResources = false;

    #region UI variables
    public TextMeshProUGUI upgradeText1;
    public TextMeshProUGUI upgradeText2;
    public Image upgradeIcon1;
    public Image upgradeIcon2;
    public TextMeshProUGUI resourceTimeText;
    public Image activeResourceIcon;
    public Button playButton;
    public Button pauseButton;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI maxText;
    public Button upgradeButton;
    public Slider timeBar;
    public Image requirementIcon1;
    public Image requirementIcon2;
    public TextMeshProUGUI requirement1Text;
    public TextMeshProUGUI requirement2Text;

    #endregion*/
    


    private void Start()
    {
        canvasInterior.SetActive(false);
    }

    void Update()
    {
        
    }
}
