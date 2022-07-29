using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class Drop
{
    public Sprite icon;
    public int quantity;
    public int level;
}

public class miniGameManager : MonoBehaviour
{
    public GameObject shieldManSpawn;
    
    public Dictionary<string, ObjectPool> POOLS = new Dictionary<string, ObjectPool>();
    public Dictionary<string, GameObject> MONSTERS = new Dictionary<string, GameObject>();
    public Dictionary<string, int> UNITS_MONSTERS = new Dictionary<string, int>();
    public Dictionary<string, Drop> DROPS = new Dictionary<string, Drop>();

    public Transform waypointParent;
    public List<Transform> waypoints;
    public List<GameObject> flags;
    public List<GameObject> activeFlags;
    public List<GameObject> monsters;
    public int numMonstersInvoked = 0;
    public int numScares = 0;
    public bool flagsPlaced = false;
    public NavMeshSurface2d surface;
    public bool gameOver = false;
    public float scaresModifier = 0;
    public float dropsModifier = 0;

    //UI
    public bool isOnCanvas = false;
    public bool isDragging = false;
    public bool isHolding = false;
    public bool isCardSelected = false; //invoke
    public List<Button> cardButtons;
    public string selectedCard = NONE;
    public GameObject gameOverPanel;
    public GameObject scareGroup;
    public GameObject dropGroup;
    public TextMeshProUGUI scaresText;
    public TextMeshProUGUI scaresPercentText;
    public TextMeshProUGUI dropPercentText;
    public TextMeshProUGUI realScaresText;
    public TextMeshProUGUI extraScaresText;
    public GameObject dropPrefab;
    public GameObject dropsGroup;

    public int currentFlag = 0;
    public int maxFlags = 1; //can be upgraded
    private Vector3 flagPosition = new Vector3();
    private Vector3 touchPosWorld;
    private float holdTimer = 0;
    private float holdInvokationTimer = 0;
    private bool isTouching = false;
    private float time = 0;

    //Particle prefabs
    public GameObject sorcererProjectile;
    public GameObject villagerDeathParticles;
    public GameObject scareProjectile;

    //Game variables
    public static float MINIGAME_MAX_TIME = 30f; //in seconds

    //UI variables
    public static float HOLD_TIME = 0.25f;
    public static float HOLD_INVOKATION_TIME = 0.25f;

    //Card variables
    public static string NONE = "none";
    public static string FLAG_BUTTON = "FlagButton";
    public static string SKELETON_BUTTON = "SkeletonButton";
    public static string GHOST_BUTTON = "GhostButton";
    public static string ZOMBIE_BUTTON = "ZombieButton";
    public static string JACK_BUTTON = "JackOLanternButton";
    public static string BAT_BUTTON = "BatButton";
    public static string GOBLIN_BUTTON = "GoblinButton";
    public static string VAMPIRE_BUTTON = "VampireButton";
    public static string WITCH_BUTTON = "WitchButton";
    public static string CLOWN_BUTTON = "ClownButton";

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

    public static string LOLLIPOP = "lollipop";
    public static string RING = "ring";
    public static string BEER = "beer";
    public static string SWORD = "sword";
    public static string SHIELD = "shield";
    public static string STICK = "stick";
    public static string GEM = "gem";

    //BOOSTS
    public static string SCARE_BOOST = "mageGuardian";
    public static string DROP_BOOST = "demonLord";

    //Particles
    public static int CURRENT_PARTICLES = 5;
    public static string SORCERER_PROJECTILE = "sorcererProjectile";
    public static string VILLAGER_DEATH_PARTICLES = "villagerDeathParticles";
    public static string SCARE_PROJECTILE = "scareProjectile";


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
    #endregion


    public void Start()
    {
        gameOverPanel.SetActive(false);
        
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

        //Only to debug, fill units_monsters dictionary 

        /*foreach (KeyValuePair<string, int> monster in Data.Instance.INVENTORY)
        {
            for (int i = 0; i < GameManager.Instance.monstersKeys.Count; i++)
            {
                if (monster.Key.Equals(GameManager.Instance.monstersKeys[i]))
                {
                    //It's a monster
                    UNITS_MONSTERS.Add(monster.Key, monster.Value);
                }
            }
        }*/

        //Add all monsters
        UNITS_MONSTERS.Add(Data.SKELETON, 30);
        UNITS_MONSTERS.Add(Data.JACK_LANTERN, 30);
        UNITS_MONSTERS.Add(Data.ZOMBIE, 30);
        UNITS_MONSTERS.Add(Data.GHOST, 30);
        UNITS_MONSTERS.Add(Data.BAT, 30);
        UNITS_MONSTERS.Add(Data.GOBLIN, 30);
        UNITS_MONSTERS.Add(Data.VAMPIRE, 30);
        UNITS_MONSTERS.Add(Data.CLOWN, 30);
        UNITS_MONSTERS.Add(Data.WITCH, 30);


        //Get boosts
        if (Data.Instance.BUILDING_INVENTORY.TryGetValue(SCARE_BOOST, out int quantity))
        {
            for (int i = 0; i < quantity; i++)
            {
                scaresModifier += 0.5f;
            }
            scareGroup.SetActive(true);
        }
        else
        {
            //No scare boost
            scareGroup.SetActive(false);
        }

        if (Data.Instance.BUILDING_INVENTORY.TryGetValue(DROP_BOOST, out int quantity2))
        {
            for (int i = 0; i < quantity2; i++)
            {
                dropsModifier += 0.25f;
                Debug.Log(dropsModifier);
            }
            dropGroup.SetActive(true);
        }
        else
        {
            //No drop boost
            dropGroup.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time < MINIGAME_MAX_TIME)
        {
            if (currentFlag > 0 && !shieldManSpawn.activeInHierarchy && numMonstersInvoked > 20)
            {
                shieldManSpawn.SetActive(true);
            }

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
            #endregion

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
                        if (hitInformation.collider != null)
                        {
                            //We should have hit something with a 2D Physics collider!
                            GameObject touchedObject = hitInformation.transform.gameObject;
                            resolveTouchedObject(touchedObject);
                        }

                        if (selectedCard == FLAG_BUTTON && !flagsPlaced)
                        {
                            placeFlag(touchPosWorld);
                        }

                    }
                }
            }
        }
        else if(!gameOver)
        {
            gameOver = true;
            showStats();
        }
    }

    public void resolveTouchedObject(GameObject touchedObject)
    {
        if (touchedObject.tag == "spawnableGround")
        {
            if (selectedCard == SKELETON_BUTTON)
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
        }
        else if (touchedObject.tag == "sewer")
        {
            if (selectedCard == ZOMBIE_BUTTON)
            {
                if (currentFlag > 0)
                {
                    invokeMonster(ZOMBIE, touchPosWorld);
                }
            }
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
        }
    }

    public void placeFlag(Vector3 position)
    {
        if(currentFlag < maxFlags)
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
        if(currentFlag == 3)
        {
            flagsPlaced = true;
        }        
    }

    public void invokeMonster(string monsterName, Vector3 position)
    {
        if (UNITS_MONSTERS.TryGetValue(monsterName, out int quantity)) {
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
        if(selectedCard == cardSelected.name)
        {
            //deselect card
            selectedCard = NONE;

            //reset touch position
            touchPosWorld = Vector3.zero;

            cardSelected.GetComponent<Image>().color = new Color(1,1,1,1);
        }
        else
        {
            //Deselect all cards
            for (int i = 0; i < cardButtons.Count; i++)
            {
                cardButtons[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }

            //select card
            selectedCard = cardSelected.name;
            cardSelected.GetComponent<Image>().color = Color.green;
            Invoke("selectCard", 0.05f);
        }
    }

    public void onBeginDrag()
    {
        isDragging = true;
    }

    public void onEndDrag()
    {
        Invoke("endDrag", 0.5f); //sino just quan es deixa anar el drag ja compta isDragging false i es poden instanciar clowns i pumpkins
    }

    public void endDrag()
    {
        isDragging = false;
    }

    public void showStats()
    {
        scaresText.SetText((numScares + (int)(numScares * scaresModifier)).ToString());
        scaresPercentText.SetText("+" + scaresModifier*100 + "%");
        realScaresText.SetText("(" + numScares.ToString());
        extraScaresText.SetText(" + " + ((int)(numScares * scaresModifier)).ToString() + ")");
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
            itemObject.transform.GetChild(0).GetComponent<Image>().sprite = dropsSorted[i].icon;
            itemObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText((dropsSorted[i].quantity + (int)(dropsSorted[i].quantity*dropsModifier)).ToString());
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
        //Save drops to inventory
        foreach (KeyValuePair<string, Drop> drop in DROPS)
        {
            if (Data.Instance.INVENTORY.ContainsKey(drop.Key))
            {
                //Sumar al que ja te
                Data.Instance.INVENTORY[drop.Key] += drop.Value.quantity + (int)(drop.Value.quantity*dropsModifier);
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
            Data.Instance.INVENTORY[Data.SCARE] += numScares + (int)(numScares*scaresModifier);
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

        //Delete all except inventory
        SaveManager.Instance.Restart();

        //Go back to main scene
        SceneManager.LoadScene("globalView");
    }
}
