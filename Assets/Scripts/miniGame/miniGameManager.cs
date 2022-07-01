using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class miniGameManager : MonoBehaviour
{
    public GameObject shieldManSpawn;
    
    public Dictionary<string, ObjectPool> POOLS = new Dictionary<string, ObjectPool>();
    public Transform waypointParent;
    public List<Transform> waypoints;
    //public List<Transform> flags;
    public List<GameObject> flags;
    public List<GameObject> activeFlags;
    public List<GameObject> monsters;
    public int numMonstersInvoked = 0;
    public bool flagsPlaced = false;
    public Dictionary<string, GameObject> MONSTERS = new Dictionary<string, GameObject>();
    public NavMeshSurface2d surface; 

    //UI
    public bool isOnCanvas = false;
    public bool isDragging = false;
    public bool isHolding = false;
    public bool isCardSelected = false; //invoke
    public List<Button> cardButtons;
    public string selectedCard = NONE;

    public int currentFlag = 0;
    public int maxFlags = 1; //can be upgraded
    private Vector3 flagPosition = new Vector3();
    private Vector3 touchPosWorld;
    private float holdTimer = 0;
    private float holdInvokationTimer = 0;
    private bool isTouching = false;

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
        //Fill monsters dictionary
        foreach (var monster in monsters)
        {
            MONSTERS.Add(monster.name, monster);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentFlag > 0 && !shieldManSpawn.activeInHierarchy && numMonstersInvoked > 10)
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
        else if(holdTimer < HOLD_TIME && isHolding) //check isHolding true to not set everytime isHolding to false
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
        if (MONSTERS.TryGetValue(monsterName, out GameObject monster))
        {
            GameObject mons =  Instantiate(monster, new Vector3(position.x, position.y, 0), Quaternion.identity);
            numMonstersInvoked++;
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
}
