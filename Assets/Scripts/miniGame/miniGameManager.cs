using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class Drop
{
    public Sprite icon;
    public int quantity;
    public int level;
}

public class miniGameManager : MonoBehaviour
{
    public Dictionary<string, ObjectPool> POOLS = new Dictionary<string, ObjectPool>();
    public Dictionary<string, GameObject> MONSTERS = new Dictionary<string, GameObject>();
    public Dictionary<string, int> UNITS_MONSTERS = new Dictionary<string, int>();
    public Dictionary<string, Drop> DROPS = new Dictionary<string, Drop>();

    public Transform waypointParent;
    public List<Transform> waypoints;
    public List<Transform> adultWaypoints;
    public List<Transform> momWaypoints;
    public List<Transform> childWaypoints;
    public List<Transform> elderWaypoints;
    public List<Transform> shieldmanWaypoints;
    public List<Transform> swashbucklerWaypoints;
    public List<Transform> sorcererWaypoints;
    public List<GameObject> tombs;

    public GameObject outsideSpawnIndicator;
    public GameObject insideSpawnIndicator;
    public GameObject sewerSpawnIndicator;

    public GameObject adultSpawn;
    public GameObject swashbucklerSpawn;
    public GameObject sorcererSpawn;
    public GameObject elderSpawn;
    public GameObject shieldManSpawn;

    public List<GameObject> flags;
    public List<GameObject> activeFlags;
    public List<GameObject> monsters;
    public AudioSource audioSource;
    public AudioClip[] sounds;
    public int numMonstersInvoked = 0;
    public int numMonstersDied = 0;
    public int numMaxMonsters = 0;
    public int numScares = 0;
    public bool flagsPlaced = false;
    public NavMeshSurface2d surface;
    public bool gameOver = false;
    public float scaresModifier = 0;
    public float dropsModifier = 0;
    public bool firstMonster = true;
    public bool villagersMoveFree = false;
    public bool momMoveFree = false;
    public bool childMoveFree = false;
    public bool elderMoveFree = false;
    public bool hasReaper = false;

    //UI
    public Canvas canvas;

    public bool isOnCanvas = false;
    public bool isDragging = false;
    public bool isHolding = false;
    public bool isCardSelected = false; //invoke
    public string selectedCard = NONE;
    public GameObject gameOverPanel;
    public GameObject scareGroup;
    public GameObject dropGroup;
    public TextMeshProUGUI scaresText;
    public TextMeshProUGUI scaresPercentText;
    public TextMeshProUGUI dropPercentText;
    public TextMeshProUGUI realScaresText;
    public TextMeshProUGUI extraScaresText;
    public TextMeshProUGUI timer;
    public GameObject dropPrefab;
    public GameObject dropsGroup;
    public GameObject card;
    public GameObject infoPanel;
    public GameObject flagButton;
    public GameObject cardsGroup;
    public GameObject confirmUI;
    public Sprite skeletonIcon;

    public int currentFlag = 0;
    public int maxFlags = 3; //can be upgraded
    private Vector3 flagPosition = new Vector3();
    private Vector3 touchPosWorld;
    private float holdTimer = 0;
    private float holdInvokationTimer = 0;
    private bool isTouching = false;
    private float time = 0;

    public GameObject timerPanel;

    //Particle prefabs
    public GameObject sorcererProjectile;

    public GameObject villagerDeathParticles;
    public GameObject scareProjectile;

    private Button buttonSelected;

    //Game variables
    public float MINIGAME_MAX_TIME = 10f; //in seconds

    //UI variables
    public static float HOLD_TIME = 0.25f;

    public static float HOLD_INVOKATION_TIME = 0.1f;
    public static Byte SELECTED_R = 42;
    public static Byte SELECTED_G = 154;
    public static Byte SELECTED_B = 255;

    //Card variables
    public static string NONE = "none";

    public static string FLAG_BUTTON = "FlagButton";
    public static string SKELETON_BUTTON = "skeletonButton";
    public static string GHOST_BUTTON = "ghostButton";
    public static string ZOMBIE_BUTTON = "zombieButton";
    public static string JACK_BUTTON = "jackOLanternButton";
    public static string BAT_BUTTON = "batButton";
    public static string GOBLIN_BUTTON = "goblinButton";
    public static string VAMPIRE_BUTTON = "vampireButton";
    public static string WITCH_BUTTON = "witchButton";
    public static string CLOWN_BUTTON = "clownButton";
    public static string REAPER_BUTTON = "reaperButton";

    //Dictionary variables
    public static string SKELETON = "skeleton";

    public static string ZOMBIE = "zombie";
    public static string GHOST = "ghost";
    public static string JACK_LANTERN = "jackOLantern";
    public static string BAT = "bat";
    public static string GOBLIN = "goblin";
    public static string VAMPIRE = "vampire";
    public static string WITCH = "witch";
    public static string CLOWN = "clown";
    public static string REAPER = "reaper";

    public static string LOLLIPOP = "lollipop";
    public static string RING = "ring";
    public static string BEER = "beer";
    public static string SWORD = "sword";
    public static string SHIELD = "shield";
    public static string STICK = "stick";
    public static string GEM = "gem";

    //Particles
    public static int CURRENT_PARTICLES = 5;

    public static string SORCERER_PROJECTILE = "sorcererProjectile";
    public static string VILLAGER_DEATH_PARTICLES = "villagerDeathParticles";
    public static string SCARE_PROJECTILE = "scareProjectile";

    //Audio variables
    public static int CLOSE = 0;
    public static int CONFIRM = 1;
    public static int ERROR = 2;
    public static int DEFAULT = 3;
    public static int DEFAULT2 = 4;
    public static int GAME_OVER = 5;
    public static int ACHIEVEMENT = 6;

    #region SINGLETON PATTERN

    public static miniGameManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        //Fill all targets with waypoints
        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints.Add(waypointParent.GetChild(i));
        }
    }

    #endregion SINGLETON PATTERN

    public void Start()
    {
        timerPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        cardsGroup.SetActive(false);
        confirmUI.SetActive(false);

        adultSpawn.SetActive(false);
        swashbucklerSpawn.SetActive(false);
        sorcererSpawn.SetActive(false);
        elderSpawn.SetActive(false);
        shieldManSpawn.SetActive(false);

        firstMonster = true;
        //Fill monsters dictionary
        foreach (var monster in monsters)
        {
            MONSTERS.Add(monster.name, monster);
        }

        //Fill POOLS dictionary with particles
        ObjectPool pool = new ObjectPool();
        pool.objectToPool = sorcererProjectile;
        pool.amountToPool = CURRENT_PARTICLES;
        pool.setup();
        POOLS.Add(SORCERER_PROJECTILE, pool);

        ObjectPool pool1 = new ObjectPool();
        pool1.objectToPool = villagerDeathParticles;
        pool1.amountToPool = CURRENT_PARTICLES;
        pool1.setup();
        POOLS.Add(VILLAGER_DEATH_PARTICLES, pool1);

        ObjectPool pool2 = new ObjectPool();
        pool2.objectToPool = scareProjectile;
        pool2.amountToPool = CURRENT_PARTICLES;
        pool2.setup();
        POOLS.Add(SCARE_PROJECTILE, pool2);

        //Fill units_monsters dictionary
        foreach (KeyValuePair<string, int> monster in Data.Instance.INVENTORY)
        {
            for (int i = 0; i < GameManager.Instance.monstersKeys.Count; i++)
            {
                if (monster.Key.Equals(GameManager.Instance.monstersKeys[i]))
                {
                    //It's a monster

                    //check that quantity is not 0
                    if (monster.Value > 0)
                    {
                        UNITS_MONSTERS.Add(monster.Key, monster.Value);
                        numMaxMonsters += monster.Value;
                    }
                }
            }
        }
        int maxLevel = 0;
        foreach (KeyValuePair<string, int> monster in UNITS_MONSTERS)
        {
            if (Data.Instance.MONSTERS.ContainsKey(monster.Key))
            {
                if (Data.Instance.MONSTERS[monster.Key].level[Data.Instance.MONSTERS[monster.Key].upgradeLevel - 1] > maxLevel)
                {
                    maxLevel = Data.Instance.MONSTERS[monster.Key].level[Data.Instance.MONSTERS[monster.Key].upgradeLevel - 1];
                }
            }
        }

        if (maxLevel == 6)
        {
            hasReaper = true;
        }

        if (maxLevel > 2)
        {
            switch (maxLevel)
            {
                case 3:
                    adultSpawn.SetActive(true);
                    swashbucklerSpawn.SetActive(true);
                    break;

                case 4:
                    //shieldManSpawn.SetActive(true);
                    adultSpawn.SetActive(true);
                    swashbucklerSpawn.SetActive(true);
                    elderSpawn.SetActive(true);
                    break;

                case 5:
                    adultSpawn.SetActive(true);
                    swashbucklerSpawn.SetActive(true);
                    elderSpawn.SetActive(true);
                    sorcererSpawn.SetActive(true);
                    break;

                default:
                    adultSpawn.SetActive(true);
                    swashbucklerSpawn.SetActive(true);
                    elderSpawn.SetActive(true);
                    sorcererSpawn.SetActive(true);
                    break;
            }
        }

        /*//Add all monsters
        UNITS_MONSTERS.Add(Data.SKELETON, 30);
        UNITS_MONSTERS.Add(Data.JACK_LANTERN, 30);
        UNITS_MONSTERS.Add(Data.ZOMBIE, 30);
        UNITS_MONSTERS.Add(Data.GHOST, 30);
        UNITS_MONSTERS.Add(Data.BAT, 30);
        UNITS_MONSTERS.Add(Data.GOBLIN, 30);
        UNITS_MONSTERS.Add(Data.VAMPIRE, 30);
        UNITS_MONSTERS.Add(Data.CLOWN, 30);
        UNITS_MONSTERS.Add(Data.WITCH, 30);*/

        //Get boosts
        if (Data.Instance.BOOSTS.TryGetValue(Data.SCARES_BOOST, out int quantity))
        {
            for (int i = 0; i < quantity; i++)
            {
                scaresModifier += 0.5f;
            }
            scareGroup.SetActive(true);
        }

        if (Data.Instance.BOOSTS.TryGetValue(Data.DROPS_BOOST, out int quantity2))
        {
            for (int i = 0; i < quantity2; i++)
            {
                dropsModifier += 0.25f;
            }
            dropGroup.SetActive(true);
        }
    }

    private void Update()
    {
        if (!gameOver)
        {
            #region DETECT USER HOLD

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                isTouching = true;
            }
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                isTouching = false;
            }

            if (isTouching)
            {
                holdTimer += Time.deltaTime;
            }
            else
            {
                holdTimer = 0;
            }
            if (holdTimer >= HOLD_TIME)
            {
                isHolding = true;
            }
            else if (holdTimer < HOLD_TIME && isHolding) //check isHolding true to not set everytime isHolding to false
            {
                isHolding = false;
            }

            #endregion DETECT USER HOLD

            if (selectedCard != NONE && !isOnCanvas && isCardSelected && !isDragging)
            {
                //If holds tap (invoke multiple monsters)
                if (isHolding)
                {
                    holdInvokationTimer += Time.deltaTime;
                    if (holdInvokationTimer >= HOLD_INVOKATION_TIME) //Invoke monster every 0.5 seconds
                    {
                        touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                        RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld, Camera.main.transform.forward);
                        if (hitInformation.collider != null)
                        {
                            //We should have hit something with a 2D Physics collider!
                            GameObject touchedObject = hitInformation.transform.gameObject;
                            resolveTouchedObject(touchedObject);
                        }
                        holdInvokationTimer = 0;
                    }
                }
                else
                {
                    //Get touch (tap) position
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                        RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld, Camera.main.transform.forward);
                        GameObject touchedObject;
                        if (hitInformation.collider != null)
                        {
                            //We should have hit something with a 2D Physics collider!
                            touchedObject = hitInformation.transform.gameObject;
                            resolveTouchedObject(touchedObject);
                            if (selectedCard == FLAG_BUTTON && !flagsPlaced && touchedObject.tag == "walkableGround")
                            {
                                placeFlag(touchPosWorld);
                            }
                        }
                    }
                }
            }
        }

        if (!firstMonster && !hasReaper)
        {
            if (time < MINIGAME_MAX_TIME && numMonstersDied < numMaxMonsters) //&& !gameOver ?
            {
                time += Time.deltaTime;
                timer.SetText(((int)((MINIGAME_MAX_TIME - time) / 60)).ToString() + ":" + ((MINIGAME_MAX_TIME - time) - (((int)((MINIGAME_MAX_TIME - time) / 60)) * 60)).ToString("0#"));
                if (currentFlag > 0 && !shieldManSpawn.activeInHierarchy && numMonstersInvoked > 20)
                {
                    shieldManSpawn.SetActive(true);
                }
            }
            else if (!gameOver)
            {
                audioSource.clip = sounds[GAME_OVER];
                audioSource.Play();

                timerPanel.SetActive(false);
                gameOver = true;
                Invoke("showStats", 5);
            }
        }
        else if (hasReaper)
        {
            timer.SetText("666");
        }
    }

    public void resolveTouchedObject(GameObject touchedObject)
    {
        if (touchedObject.tag == "spawnableGround")
        {
            if (selectedCard == SKELETON_BUTTON || (selectedCard == FLAG_BUTTON && flagsPlaced)) //The tuto card
            {
                if (currentFlag > 0)
                {
                    invokeMonster(SKELETON, touchPosWorld);
                }
            }
            if (selectedCard == ZOMBIE_BUTTON)
            {
                if (currentFlag > 0)
                {
                    invokeMonster(ZOMBIE, touchPosWorld);
                }
            }
            if (selectedCard == GHOST_BUTTON)
            {
                if (currentFlag > 0)
                {
                    invokeMonster(GHOST, touchPosWorld);
                }
            }
            if (selectedCard == BAT_BUTTON)
            {
                if (currentFlag > 0)
                {
                    invokeMonster(BAT, touchPosWorld);
                }
            }
            if (selectedCard == GOBLIN_BUTTON)
            {
                if (currentFlag > 0)
                {
                    invokeMonster(GOBLIN, touchPosWorld);
                }
            }
            if (selectedCard == VAMPIRE_BUTTON)
            {
                if (currentFlag > 0)
                {
                    invokeMonster(VAMPIRE, touchPosWorld);
                }
            }
            if (selectedCard == WITCH_BUTTON)
            {
                if (currentFlag > 0)
                {
                    invokeMonster(WITCH, touchPosWorld);
                }
            }
            if (selectedCard == REAPER_BUTTON)
            {
                if (currentFlag > 0)
                {
                    invokeMonster(REAPER, touchPosWorld);
                }
            }

            //Error audio
            if (selectedCard == JACK_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == CLOWN_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
        }
        else if (touchedObject.tag == "sewer")
        {
            if (selectedCard == GOBLIN_BUTTON)
            {
                if (currentFlag > 0)
                {
                    invokeMonster(GOBLIN, touchPosWorld);
                }
            }

            #region ERROR AUDIO
            if (selectedCard == SKELETON_BUTTON) //The tuto card
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == ZOMBIE_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == GHOST_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == BAT_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == VAMPIRE_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == WITCH_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == REAPER_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == JACK_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == CLOWN_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            #endregion
        }
        else if (touchedObject.tag == "tomb")
        {
            if (selectedCard == GHOST_BUTTON)
            {
                if (currentFlag > 0)
                {
                    invokeMonster(GHOST, touchPosWorld);
                }
            }

            #region ERROR AUDIO
            if (selectedCard == SKELETON_BUTTON) //The tuto card
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == ZOMBIE_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == GOBLIN_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == BAT_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == VAMPIRE_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == WITCH_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == REAPER_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == JACK_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == CLOWN_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            #endregion
        }
        else if (touchedObject.tag == "walkableGround")
        {
            if (selectedCard == JACK_BUTTON)
            {
                invokeMonster(JACK_LANTERN, touchPosWorld);
            }
            if (selectedCard == CLOWN_BUTTON)
            {
                invokeMonster(CLOWN, touchPosWorld);
            }

            #region ERROR AUDIO
            if (selectedCard == SKELETON_BUTTON) //The tuto card
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == ZOMBIE_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == GHOST_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == GOBLIN_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == BAT_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == VAMPIRE_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == WITCH_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            if (selectedCard == REAPER_BUTTON)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            #endregion
        }
    }

    public void placeFlag(Vector3 position)
    {
        audioSource.clip = sounds[DEFAULT2];
        audioSource.Play();

        if (currentFlag < maxFlags)
        {
            flags[currentFlag].transform.position = new Vector3(position.x, position.y, 0);
            flags[currentFlag].SetActive(true);
            activeFlags.Add(flags[currentFlag]);
            currentFlag++;

            //Update monsters flag
            Monster[] monsters = FindObjectsOfType<Monster>();

            foreach (Monster monster in monsters)
            {
                monster.updateFlags();
            }
        }
        if (currentFlag == 3)
        {
            flagButton.SetActive(false);
            infoPanel.SetActive(false);
            confirmUI.SetActive(true);
            selectedCard = NONE;
        }
        else
        {
            //change flag card
            flagButton.transform.GetChild(0).gameObject.SetActive(false);
            flagButton.transform.GetChild(1).gameObject.SetActive(false);
            flagButton.transform.GetChild(2).gameObject.SetActive(false);
            flagButton.transform.GetChild(currentFlag).gameObject.SetActive(true);
            flagButton.transform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText((maxFlags - currentFlag).ToString());
        }
    }

    public void invokeMonster(string monsterName, Vector3 position)
    {
        if (firstMonster)
        {
            firstMonster = false;
            villagersMoveFree = true;
            momMoveFree = false;
            childMoveFree = false;
            elderMoveFree = false;
        }
        if (UNITS_MONSTERS.TryGetValue(monsterName, out int quantity))
        {
            if (quantity > 0)
            {
                if (MONSTERS.TryGetValue(monsterName, out GameObject monster))
                {
                    GameObject monsterObj = Instantiate(monster, new Vector3(position.x, position.y, 0), Quaternion.identity);

                    //Set monster stats (not affecting the prefab)
                    if (Data.Instance.MONSTERS.TryGetValue(monsterName, out MonsterInfo monsterInfo))
                    {
                        monsterObj.GetComponent<Monster>().velocity = monsterInfo.velocity[monsterInfo.upgradeLevel - 1];
                        monsterObj.GetComponent<Monster>().health = monsterInfo.health[monsterInfo.upgradeLevel - 1];
                        monsterObj.GetComponent<Monster>().damage = monsterInfo.damage[monsterInfo.upgradeLevel - 1];
                        monsterObj.GetComponent<Monster>().attackRate = monsterInfo.attackRate[monsterInfo.upgradeLevel - 1];
                        monsterObj.GetComponent<Monster>().attackRange = monsterInfo.attackRange[monsterInfo.upgradeLevel - 1];
                        monsterObj.GetComponent<Monster>().level = monsterInfo.level[monsterInfo.upgradeLevel - 1];
                    }

                    numMonstersInvoked++;
                    UNITS_MONSTERS[monsterName] = quantity - 1;

                    if (Data.Instance.PLAYER.TryGetValue("Tuto", out int isDone))
                    {
                        if (isDone == 1)
                        {
                            //update card
                            for (int i = 0; i < cardsGroup.transform.childCount; i++)
                            {
                                if (cardsGroup.transform.GetChild(i).name.Contains(monsterName))
                                {
                                    cardsGroup.transform.GetChild(i).transform.Find("NumPanel").transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(UNITS_MONSTERS[monsterName].ToString());

                                    //check if quantity is 0
                                    if (UNITS_MONSTERS[monsterName] == 0)
                                    {
                                        cardsGroup.transform.GetChild(i).transform.Find("Icon").GetComponent<Image>().color = new Color32(255, 255, 255, 150);

                                        //deselect card
                                        selectedCard = NONE;
                                        outsideSpawnIndicator.SetActive(false);
                                        insideSpawnIndicator.SetActive(false);
                                        sewerSpawnIndicator.SetActive(false);
                                        foreach (GameObject s in tombs)
                                        {
                                            s.SetActive(false);
                                        }

                                        //reset touch position
                                        touchPosWorld = Vector3.zero;

                                        if (buttonSelected != null)
                                        {
                                            buttonSelected.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                                            buttonSelected.transform.Find("NumPanel").GetComponent<Image>().color = new Color(1, 1, 1, 1);
                                        }
                                    }
                                }
                            }                            
                        }
                        else
                        {
                            //update card
                            flagButton.transform.Find("NumPanel").transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(UNITS_MONSTERS[monsterName].ToString());

                            //update info panel
                            infoPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Monsters will scare villagers until their Scare Bar is full and you will get special drops once they pass out, but some villagers will attack your creatures!");
                        }
                    }
                }
            }
        }
    }

    public void setIsOnCanvas()
    {
        isOnCanvas = true;
    }

    public void setOutOffCanvas()
    {
        isOnCanvas = false;
    }

    public void selectCard()
    {
        isCardSelected = true;
    }

    public void toggleSelectedCard(Button cardSelected)
    {
        buttonSelected = cardSelected;
        if (selectedCard == cardSelected.name)
        {
            //check if quantity is 0
            bool isError = false;

            if (UNITS_MONSTERS.TryGetValue(getMonsterName(cardSelected.name), out int quantity))
            {
                if (quantity <= 0)
                {
                    isError = true;
                }
            }

            if (isError)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            else
            {

                audioSource.clip = sounds[DEFAULT2];
                audioSource.Play();

                //deselect card
                selectedCard = NONE;
                outsideSpawnIndicator.SetActive(false);
                insideSpawnIndicator.SetActive(false);
                sewerSpawnIndicator.SetActive(false);
                foreach (GameObject s in tombs)
                {
                    s.SetActive(false);
                }

                //reset touch position
                touchPosWorld = Vector3.zero;

                cardSelected.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                cardSelected.transform.Find("NumPanel").GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }
        else
        {
            //check if quantity is 0
            bool isError = false;

            if(UNITS_MONSTERS.TryGetValue(getMonsterName(cardSelected.name), out int quantity)){
                if(quantity <= 0)
                {
                    isError = true;
                }
            }

            if (isError)
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
            else
            {
                //Deselect all cards
                for (int i = 0; i < cardsGroup.transform.childCount; i++)
                {
                    cardsGroup.transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    cardsGroup.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }

                foreach (GameObject s in tombs)
                {
                    s.SetActive(false);
                }

                //select card
                selectedCard = cardSelected.name;

                audioSource.clip = sounds[DEFAULT];
                audioSource.Play();

                switch (cardSelected.name)
                {
                    case "skeletonButton":
                        outsideSpawnIndicator.SetActive(true);
                        insideSpawnIndicator.SetActive(false);
                        sewerSpawnIndicator.SetActive(false);
                        break;

                    case "ghostButton":
                        outsideSpawnIndicator.SetActive(true);
                        insideSpawnIndicator.SetActive(false);
                        sewerSpawnIndicator.SetActive(false);
                        foreach (GameObject s in tombs)
                        {
                            s.SetActive(true);
                        }
                        break;

                    case "zombieButton":
                        outsideSpawnIndicator.SetActive(true);
                        insideSpawnIndicator.SetActive(false);
                        sewerSpawnIndicator.SetActive(false);
                        break;

                    case "jackOLanternButton":
                        outsideSpawnIndicator.SetActive(false);
                        insideSpawnIndicator.SetActive(true);
                        sewerSpawnIndicator.SetActive(false);
                        break;

                    case "batButton":
                        outsideSpawnIndicator.SetActive(true);
                        insideSpawnIndicator.SetActive(false);
                        sewerSpawnIndicator.SetActive(false);
                        break;

                    case "goblinButton":
                        outsideSpawnIndicator.SetActive(true);
                        insideSpawnIndicator.SetActive(false);
                        sewerSpawnIndicator.SetActive(true);
                        break;

                    case "vampireButton":
                        outsideSpawnIndicator.SetActive(true);
                        insideSpawnIndicator.SetActive(false);
                        sewerSpawnIndicator.SetActive(false);
                        break;

                    case "witchButton":
                        outsideSpawnIndicator.SetActive(true);
                        insideSpawnIndicator.SetActive(false);
                        sewerSpawnIndicator.SetActive(false);
                        break;

                    case "clownButton":
                        outsideSpawnIndicator.SetActive(false);
                        insideSpawnIndicator.SetActive(true);
                        sewerSpawnIndicator.SetActive(false);
                        break;

                    case "reaperButton":
                        outsideSpawnIndicator.SetActive(true);
                        insideSpawnIndicator.SetActive(false);
                        sewerSpawnIndicator.SetActive(false);
                        break;
                }

                cardSelected.GetComponent<Image>().color = new Color32(SELECTED_R, SELECTED_G, SELECTED_B, 255);
                cardSelected.transform.Find("NumPanel").GetComponent<Image>().color = new Color32(SELECTED_R, SELECTED_G, SELECTED_B, 255);
                Invoke("selectCard", 0.05f);
            }
        }
    }

    public void onBeginDrag()
    {
        isDragging = true;
    }

    public void onEndDrag()
    {
        Invoke("endDrag", 0.25f); //sino just quan es deixa anar el drag ja compta isDragging false i es poden instanciar clowns i pumpkins
    }

    public void endDrag()
    {
        isDragging = false;
    }

    public void showStats()
    {
        audioSource.clip = sounds[DEFAULT];
        audioSource.Play();

        scaresText.SetText((numScares + (int)(numScares * scaresModifier)).ToString()); //fer funcio minigame manager que passi a 1.3k (copiar de gameManager)
        scaresPercentText.SetText("+" + scaresModifier * 100 + "%");
        realScaresText.SetText("(" + numScares.ToString());
        extraScaresText.SetText(" + " + ((int)(numScares * scaresModifier)).ToString());
        dropPercentText.SetText("+" + dropsModifier * 100 + "% villager's drops");
        List<Drop> dropsSorted = new List<Drop>();
        foreach (KeyValuePair<string, Drop> drop in DROPS)
        {
            dropsSorted.Add(drop.Value);
        }

        //sort drops obtained by level of villager
        dropsSorted.Sort((p1, p2) => p1.level.CompareTo(p2.level));
        for (int i = 0; i < dropsSorted.Count; i++)
        {
            GameObject itemObject = Instantiate(dropPrefab, dropsGroup.transform);
            itemObject.transform. Find("Panel").transform.GetChild(0).transform.Find("Icon").GetComponent<Image>().sprite = dropsSorted[i].icon;
            itemObject.transform.Find("Panel").transform.GetChild(0).transform.Find("Quantity").GetComponent<TextMeshProUGUI>().SetText((dropsSorted[i].quantity + (int)(dropsSorted[i].quantity * dropsModifier)).ToString());
        }

        gameOverPanel.SetActive(true);
    }

    public GameObject poolParticle(string objectToPool, Vector3 position)
    {
        GameObject particle = null;
        if (POOLS.TryGetValue(objectToPool, out ObjectPool pool))
        {
            int currentParticles = 0;
            foreach (GameObject go in pool.pooledObjects)
            {
                if (go.activeInHierarchy)
                {
                    currentParticles++;
                }
            }
            if (currentParticles < CURRENT_PARTICLES)
            {
                GameObject temp = pool.GetPooledObject();
                temp.SetActive(true);
                temp.transform.position = position;
                particle = temp;
            }
            else
            {
                //modify quantity to pull
                pool.amountToPool++;
                pool.setup();

                GameObject temp = pool.GetPooledObject();
                temp.SetActive(true);
                temp.transform.position = position;
                particle = temp;

                CURRENT_PARTICLES++;

                //Fill POOLS dictionary with particles
                //ObjectPool pool = new ObjectPool();
                /*pool.objectToPool = sorcererProjectile;
                pool.amountToPool = INITIAL_PARTICLES;
                pool.setup();
                POOLS.Add(SORCERER_PROJECTILE, pool);*/
            }
        }
        return particle;
    }

    public void close()
    {
        audioSource.clip = sounds[CLOSE];
        audioSource.Play();
        
        //Save drops to inventory
        foreach (KeyValuePair<string, Drop> drop in DROPS)
        {
            if (Data.Instance.INVENTORY.ContainsKey(drop.Key))
            {
                //Sumar al que ja te
                Data.Instance.INVENTORY[drop.Key] += drop.Value.quantity + (int)(drop.Value.quantity * dropsModifier);
            }
            else
            {
                Data.Instance.INVENTORY.Add(drop.Key, drop.Value.quantity + (int)(drop.Value.quantity * dropsModifier));
            }
        }

        //Save scares to inventory
        if (Data.Instance.INVENTORY.ContainsKey(Data.SCARE))
        {
            //Sumar al que ja te
            Data.Instance.INVENTORY[Data.SCARE] += numScares + (int)(numScares * scaresModifier);
        }
        else
        {
            Data.Instance.INVENTORY.Add(Data.SCARE, numScares + (int)(numScares * scaresModifier));
        }

        //Borrar totes resources inventory menys scares i drops

        foreach (var item in Data.Instance.INVENTORY.ToList())
        {
            bool isDrop = false;
            for (int i = 0; i < GameManager.Instance.dropsKeys.Count; i++)
            {
                if (item.Key.Equals(GameManager.Instance.dropsKeys[i]))
                {
                    isDrop = true;
                }
            }
            if (!isDrop)
            {
                //Delete from inventory
                Data.Instance.INVENTORY.Remove(item.Key);
            }
        }

        //Save inventory
        SaveManager.Instance.SaveInventory();

        //Save player (tuto)

        //Save tuto done and restart
        if (Data.Instance.PLAYER.TryGetValue("Tuto", out int isDone))
        {
            Data.Instance.PLAYER["Tuto"] = 1;
        }

        if (Data.Instance.PLAYER.TryGetValue("isRestart", out int isRestart))
        {
            Data.Instance.PLAYER["isRestart"] = 1;
        }
        SaveManager.Instance.SaveTuto();

        //Delete all except inventory
        SaveManager.Instance.Restart();

        //Go back to main scene
        SceneManager.LoadScene("loadingScene");
    }

    public void confirmFlags()
    {
        audioSource.clip = sounds[CONFIRM];
        audioSource.Play();

        flagsPlaced = true;
        confirmUI.SetActive(false);

        //Put all card to invisible
        for (int i = 0; i < cardsGroup.transform.childCount; i++)
        {
            cardsGroup.transform.GetChild(i).gameObject.SetActive(false);
        }

        if (Data.Instance.PLAYER.TryGetValue("Tuto", out int isDone))
        {
            if (isDone == 1)
            {
                //Load only cards that player has
                foreach (KeyValuePair<string, int> monster in UNITS_MONSTERS)
                {
                    for (int i = 0; i < cardsGroup.transform.childCount; i++)
                    {
                        if (cardsGroup.transform.GetChild(i).name.Contains(monster.Key))
                        {
                            //set units num
                            cardsGroup.transform.GetChild(i).transform.Find("NumPanel").transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(monster.Value.ToString());

                            //Make that card visible
                            cardsGroup.transform.GetChild(i).gameObject.SetActive(true);
                        }
                    }
                }

                cardsGroup.SetActive(true);
            }
            else
            {
                //restart colors
                flagButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                flagButton.transform.GetChild(3).GetComponent<Image>().color = new Color(1, 1, 1, 1);

                //Explain minigame

                //Get skeleton num units
                if (UNITS_MONSTERS.TryGetValue("skeleton", out int quantity))
                {
                    flagButton.transform.GetChild(0).GetComponent<Image>().sprite = skeletonIcon;
                    flagButton.transform.GetChild(0).gameObject.SetActive(true);
                    flagButton.transform.GetChild(1).gameObject.SetActive(false);
                    flagButton.transform.GetChild(2).gameObject.SetActive(false);
                    flagButton.transform.Find("NumPanel").transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(quantity.ToString());
                }

                //Set info text
                infoPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Select the monster card and touch anywhere in the green area to invoke it.");

                flagButton.SetActive(true);
                infoPanel.SetActive(true);
            }
        }
    }

    public void cancelFlags()
    {
        audioSource.clip = sounds[CLOSE];
        audioSource.Play();

        foreach (GameObject flag in flags)
        {
            flag.SetActive(false);
        }
        currentFlag = 0;
        flagButton.transform.GetChild(0).gameObject.SetActive(true);
        flagButton.transform.GetChild(1).gameObject.SetActive(false);
        flagButton.transform.GetChild(2).gameObject.SetActive(false);
        flagButton.transform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(maxFlags.ToString());

        //restart colors
        flagButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        flagButton.transform.GetChild(3).GetComponent<Image>().color = new Color(1, 1, 1, 1);

        confirmUI.SetActive(false);
        Invoke("restartFlags", 0.05f);
    }

    public void restartFlags()
    {
        flagButton.SetActive(true);
        infoPanel.SetActive(true);
    }

    public string getMonsterName(string buttonName)
    {
        string name = "";

        switch (buttonName)
        {
            case "skeletonButton":
                name = SKELETON;
                break;
            case "jackOLanternButton":
                name = JACK_LANTERN;
                break;
            case "batButton":
                name = BAT;
                break;
            case "goblinButton":
                name = GOBLIN;
                break;
            case "ghostButton":
                name = GHOST;
                break;
            case "clownButton":
                name = CLOWN;
                break;
            case "zombieButton":
                name = ZOMBIE;
                break;
            case "vampireButton":
                name = VAMPIRE;
                break;
            case "witchButton":
                name = WITCH;
                break;
            case "reaperButton":
                name = REAPER;
                break;
        }

        return name;
    }
}