using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class miniGameManager : MonoBehaviour
{
    public Dictionary<string, ObjectPool> POOLS = new Dictionary<string, ObjectPool>();
    public Transform waypointParent;
    public List<Transform> waypoints;
    //public List<Transform> flags;
    public List<GameObject> flags;
    public List<GameObject> activeFlags;
    public List<GameObject> monsters;
    public bool flagsPlaced = false;
    public Dictionary<string, GameObject> MONSTERS = new Dictionary<string, GameObject>();
    public NavMeshSurface2d surface; 

    //UI
    public bool isOnCanvas = false;
    public bool isCardSelected = false; //invoke
    public List<Button> cardButtons;
    public string selectedCard = NONE;

    public int currentFlag = 0;
    public int maxFlags = 1; //can be upgraded
    private Vector3 flagPosition = new Vector3();
    private Vector3 touchPosWorld;


    //Card variables
    public static string NONE = "none";
    public static string FLAG_BUTTON = "FlagButton";
    public static string SKELETON_BUTTON = "SkeletonButton";
    public static string GHOST_BUTTON = "GhostButton";
    public static string ZOMBIE_BUTTON = "ZombieButton";
    public static string JACK_BUTTON = "JackOLanternButton";

    //Dictionary variables
    public static string SKELETON = "skeleton";
    public static string ZOMBIE = "zombie";
    public static string GHOST = "ghost";
    public static string JACK_LANTERN = "jackOLantern";


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
        if(selectedCard != NONE && !isOnCanvas && isCardSelected)
        {
            //Get touch position
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld, Camera.main.transform.forward);
                if (hitInformation.collider != null)
                {
                    //We should have hit something with a 2D Physics collider!
                    GameObject touchedObject = hitInformation.transform.gameObject;

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
                    }
                    else if(touchedObject.tag == "sewer")
                    {
                        if (selectedCard == ZOMBIE_BUTTON)
                        {
                            if (currentFlag > 0)
                            {
                                invokeMonster(ZOMBIE, touchPosWorld);
                            }
                        }
                    }
                    else if(touchedObject.tag == "tomb")
                    {
                        if (selectedCard == GHOST_BUTTON)
                        {
                            if (currentFlag > 0)
                            {
                                invokeMonster(GHOST, touchPosWorld);
                            }
                        }
                    }else if (touchedObject.tag == "walkableGround")
                    {
                        if (selectedCard == JACK_BUTTON)
                        {
                            invokeMonster(JACK_LANTERN, touchPosWorld);                            
                        }
                    }
                }

                if (selectedCard == FLAG_BUTTON && !flagsPlaced)
                {
                    placeFlag(touchPosWorld);
                }
                
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
        if(MONSTERS.TryGetValue(monsterName, out GameObject monster))
        {
            GameObject mons =  Instantiate(monster, new Vector3(position.x, position.y, 0), Quaternion.identity);
        }
    }

    public void setIsOnCanvas()
    {
        isOnCanvas = true;
    }
    public void selectCard()
    {
        isCardSelected = true;
        isOnCanvas = false;
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
}
