using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class SummoningCircle : Building
{
    #region building variables
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
    public TextMeshProUGUI requirement2Text;*/

    #endregion

    #region UI VARIABLES
    //Falta fer els tabs de seleccio de monster
    public TextMeshProUGUI monsterName;
    public Image monsterImage;
    public Image levelImage;
    public TextMeshProUGUI description;
    public Image summonRequirementIcon1;
    public TextMeshProUGUI summonRequirementText1;
    public Image summonRequirementIcon2;
    public TextMeshProUGUI summonRequirementText2;
    public Image summonRequirementIcon3;
    public TextMeshProUGUI summonRequirementText3;
    public TextMeshProUGUI statsTextColumn1;
    public TextMeshProUGUI statsTextColumn2;
    public List<GameObject> villagersIcons;
    public Image upgradeRequirementIcon1;
    public TextMeshProUGUI upgradeRequirementText1;
    public Image upgradeRequirementIcon2;
    public TextMeshProUGUI upgradeRequirementText2;
    public Image upgradeRequirementIcon3;
    public TextMeshProUGUI upgradeRequirementText3;
    public Image producingRequirementIcon1;
    public TextMeshProUGUI producingRequirementText1;
    public Image producingRequirementIcon2;
    public TextMeshProUGUI producingRequirementText2;
    public Image producingRequirementIcon3;
    public TextMeshProUGUI producingRequirementText3;
    public Slider timeBarMonster;
    public TextMeshProUGUI timeTextMonster;
    public Image activeMonsterIcon;
    public TextMeshProUGUI noActiveMonsterText;
    public List<GameObject> tabs;
    public List<Sprite> tabBackgrounds;
    public Image monsterImagePortrait;
    public Sprite unlockedPortrait;
    public Sprite lockedPortrait;
    public TextMeshProUGUI summonButtonText;
    public GameObject upgradeMonsterButton;
    public Sprite questionMark;
    public GameObject unknownGroup;
    public GameObject levelImagePortrait;
    public GameObject infoGroup;
    #endregion

    public List<Sprite> numbersIcons;
    public Animator animator; //.play("book") to turn page

    private string activeMonster = NONE;
    private string selectedTab = NONE;
    private int selectedTabIndex = 0;
    private float activeMonsterTime = 0;
    private float timeToUpdate = 0;
    private float timeToUpdateBar = 0;
    private List<string> unlockedMonsters = new List<string>();
    private string hidenMonster = "skeleton"; //default value
    private int hidenMonsterIndex = 0;

    #region MONSTER KEYS

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
    public static string NONE = "none";
    #endregion

    #region VILLAGER ICONS INDEX
    public static int CHILD = 0;
    public static int MOM = 1;
    public static int ADULT = 2;
    public static int SWASHBUCKLER = 3;
    public static int SHIELDMAN = 4;
    public static int ELDER = 5;
    public static int SORCERER = 6;
    #endregion

    #region MONSTER TABS INDEX
    public static int SKELETON_INDEX = 0;
    public static int ZOMBIE_INDEX = 6;
    public static int GHOST_INDEX = 4;
    public static int JACK_LANTERN_INDEX = 1;
    public static int BAT_INDEX = 2;
    public static int GOBLIN_INDEX = 3;
    public static int VAMPIRE_INDEX = 7;
    public static int WITCH_INDEX = 8;
    public static int CLOWN_INDEX = 5;
    public static int REAPER_INDEX = 9;
    #endregion

    #region TABS BACKGROUND INDEX
    public static int SELECTED = 0;
    public static int DESELECTED = 1;
    public static int DESELECTED_RED = 2;
    public static int SELECTED_RED = 3;
    #endregion

    private void Start()
    {
        if (activeMonster == NONE)
        {
            //If it's not producing (either in pause or first time building created, set default settings to skeleton)
            selectedTab = SKELETON;
            setUI(SKELETON);
            setActiveMonsterUI(NONE);
        }
        else
        {
            //Put default UI to the monster that is producing
            selectedTab = activeMonster;
            setUI(activeMonster);
            setActiveMonsterUI(activeMonster);

            if (Data.Instance.MONSTERS.TryGetValue(activeMonster, out MonsterInfo monster))
            {
                activeMonsterTime = monster.time;
            }
            timeBarMonster.minValue = 0;
            timeBarMonster.maxValue = activeMonsterTime;

            //Save building position
            position = transform.position;
        }
        canvasInterior.SetActive(false);
    }

    void Update()
    {
        //Update requirements (inventory) EACH SECOND
        if (placed)
        {
            if (isProducing)
            {
                if (playButton.gameObject.activeInHierarchy)
                {
                    playButton.gameObject.SetActive(false);
                    pauseButton.gameObject.SetActive(true);
                }

                time += Time.deltaTime;
                timeLeft = activeMonsterTime - time;

                if (timeLeft <= 0)
                {
                    produce();
                    if (enoughResources)
                    {
                        time = 0;
                    }
                    else
                    {
                        isProducing = false;
                    }
                }
            }
            else
            {
                timeBarMonster.value = timeLeft;
                timeTextMonster.SetText("-");
                if (isPaused)
                {
                    playButton.gameObject.SetActive(true);
                    pauseButton.gameObject.SetActive(false);
                }
                else
                {
                    //Was producing but ran out of requirements to produce
                    if (checkRequirementsToProduce())
                    {
                        isProducing = true;
                    }
                }
            }
            #region UPDATE UI

            if (canvasInterior.activeInHierarchy)
            {
                timeToUpdate += Time.deltaTime;
                timeToUpdateBar += Time.deltaTime;
                if (timeToUpdate >= 1) //Update only each second
                {
                    updateUIInventory();
                    timeToUpdate = 0;
                }
                if(timeToUpdateBar >= 0.25f) //Update time bar every 0.25s
                {
                    #region TIME BAR

                    if (isProducing)
                    {
                        int hours = (int)timeLeft / 3600;
                        int minutes = (int)(timeLeft - (hours*3600))/60;
                        int secondsLeft = (int)timeLeft - (hours*3600) - (minutes * 60);
                        if (hours > 0)
                        {
                            if (minutes > 0)
                            {
                                timeTextMonster.SetText(hours.ToString() + "h " + minutes.ToString() + "m " + secondsLeft + "s");
                            }
                            else
                            {
                                timeTextMonster.SetText(hours.ToString() + "h " + timeLeft.ToString("F0") + "s");
                            }
                        }
                        else
                        {
                            if (minutes > 0)
                            {
                                timeTextMonster.SetText(minutes.ToString() + "m " + secondsLeft + "s");
                            }
                            else
                            {
                                timeTextMonster.SetText(timeLeft.ToString("F0") + "s");
                            }
                        }
                    }
                    else
                    {
                        timeTextMonster.SetText("-");
                    }

                    //Slider config
                    if (isProducing)
                    {
                        timeBarMonster.value = timeLeft;
                    }
                    #endregion
                    timeToUpdateBar = 0;
                }
            }
            #endregion
        }
    }

    public void setUI(string monsterKey)
    {
        #region BOOK INFO
        if (Data.Instance.MONSTERS.TryGetValue(monsterKey, out MonsterInfo monsterInfo)){

            //Check if it's unknown
            if (!monsterInfo.isUnlocked && monsterKey != hidenMonster)
            {
                //It's unknown
                monsterName.text = "Unknown";
                monsterImagePortrait.sprite = unlockedPortrait;
                monsterImage.sprite = questionMark;
                monsterImage.color = new Color(1, 1, 1, 1);
                levelImagePortrait.gameObject.SetActive(false);

                description.SetText("This monster is yet to be discovered. Unlock the other monsters first");

                unknownGroup.SetActive(false);
            }
            else
            {
                levelImagePortrait.gameObject.SetActive(true); //just in case
                unknownGroup.SetActive(true);

                monsterName.text = monsterInfo.monsterName;

                //check if it's unlocked
                if (monsterInfo.isUnlocked)
                {
                    monsterImagePortrait.sprite = unlockedPortrait;
                    monsterImage.sprite = monsterInfo.icon;
                    monsterImage.color = new Color(1, 1, 1, 1);
                }
                else
                {
                    monsterImagePortrait.sprite = lockedPortrait;
                    monsterImage.sprite = monsterInfo.icon;
                    monsterImage.color = new Color(0, 0, 0, 1);
                }
                levelImage.sprite = numbersIcons[monsterInfo.level[monsterInfo.upgradeLevel - 1] - 1];
                description.text = monsterInfo.description;

                if (monsterInfo.isUnlocked)
                {
                    summonRequirementIcon2.gameObject.SetActive(true);
                    summonRequirementText2.gameObject.SetActive(true);
                    summonRequirementIcon3.gameObject.SetActive(true);
                    summonRequirementText3.gameObject.SetActive(true);

                    //Summon requirement 1
                    if (Data.Instance.RESOURCES.TryGetValue(monsterInfo.requirements[0].resourceNameKey, out Resource resource))
                    {
                        summonRequirementIcon1.sprite = resource.icon;
                    }

                    //Show current amount and needed
                    if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.requirements[0].resourceNameKey, out int quantity))
                    {
                        summonRequirementText1.SetText(quantity + "/" + monsterInfo.requirements[0].quantity);
                    }
                    else
                    {
                        summonRequirementText1.SetText("0/" + monsterInfo.requirements[0].quantity);
                    }

                    //Summon requirement 2
                    if (Data.Instance.RESOURCES.TryGetValue(monsterInfo.requirements[1].resourceNameKey, out Resource resource1))
                    {
                        summonRequirementIcon2.sprite = resource1.icon;
                    }

                    //Show current amount and needed
                    if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.requirements[1].resourceNameKey, out int quantity1))
                    {
                        summonRequirementText2.SetText(quantity1 + "/" + monsterInfo.requirements[1].quantity);
                    }
                    else
                    {
                        summonRequirementText2.SetText("0/" + monsterInfo.requirements[1].quantity);
                    }

                    //Summon requirement 3 (MAY NOT HAVE)
                    if (monsterInfo.requirements.Count > 2)
                    {
                        if (Data.Instance.RESOURCES.TryGetValue(monsterInfo.requirements[2].resourceNameKey, out Resource resource2))
                        {
                            summonRequirementIcon3.sprite = resource2.icon;
                        }

                        //Show current amount and needed
                        if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.requirements[2].resourceNameKey, out int quantity2))
                        {
                            summonRequirementText3.SetText(quantity2 + "/" + monsterInfo.requirements[2].quantity);
                        }
                        else
                        {
                            summonRequirementText3.SetText("0/" + monsterInfo.requirements[2].quantity);
                        }
                    }
                    else
                    {
                        //Hide 3rth requirement
                        summonRequirementIcon3.gameObject.SetActive(false);
                        summonRequirementText3.gameObject.SetActive(false);
                    }
                    summonButtonText.SetText("Summon");
                    upgradeMonsterButton.GetComponent<Image>().color = new Color(0, 1, 0, 1);
                }
                else
                {
                    //show unlock requirements (only 1?)
                    //Summon requirement 1
                    if (Data.Instance.RESOURCES.TryGetValue(monsterInfo.unlockRequirements[0].resourceNameKey, out Resource resource))
                    {
                        summonRequirementIcon1.sprite = resource.icon;
                    }

                    //Show current amount and needed
                    if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.unlockRequirements[0].resourceNameKey, out int quantity))
                    {
                        summonRequirementText1.SetText(quantity + "/" + monsterInfo.unlockRequirements[0].quantity);
                    }
                    else
                    {
                        summonRequirementText1.SetText("0/" + monsterInfo.unlockRequirements[0].quantity);
                    }

                    //Hide other requirements
                    summonRequirementIcon2.gameObject.SetActive(false);
                    summonRequirementText2.gameObject.SetActive(false);
                    summonRequirementIcon3.gameObject.SetActive(false);
                    summonRequirementText3.gameObject.SetActive(false);

                    summonButtonText.SetText("Unlock");
                    upgradeMonsterButton.GetComponent<Image>().color = new Color(0, 1, 0, 0.5f);
                }
                //only show >> if upgrading changes value (TODO) 
                if (monsterInfo.upgradeLevel < 3)
                {
                    string aux = "";

                    //Level
                    if (monsterInfo.level[monsterInfo.upgradeLevel - 1] != monsterInfo.level[monsterInfo.upgradeLevel])
                    {
                        aux = "Level: " + monsterInfo.level[monsterInfo.upgradeLevel - 1] + " >> " + monsterInfo.level[monsterInfo.upgradeLevel];
                    }
                    else
                    {
                        aux = "Level: " + monsterInfo.level[monsterInfo.upgradeLevel - 1];
                    }

                    //Velocity
                    if (monsterInfo.velocity[monsterInfo.upgradeLevel - 1] != monsterInfo.velocity[monsterInfo.upgradeLevel])
                    {
                        aux += "\nVelocity: " + monsterInfo.velocity[monsterInfo.upgradeLevel - 1] + " >> " + monsterInfo.velocity[monsterInfo.upgradeLevel];
                    }
                    else
                    {
                        aux += "\nVelocity: " + monsterInfo.velocity[monsterInfo.upgradeLevel - 1];
                    }

                    //Health
                    if (monsterInfo.health[monsterInfo.upgradeLevel - 1] != monsterInfo.health[monsterInfo.upgradeLevel])
                    {
                        aux += "\nHealth: " + monsterInfo.health[monsterInfo.upgradeLevel - 1] + " >> " + monsterInfo.health[monsterInfo.upgradeLevel];
                    }
                    else
                    {
                        aux += "\nHealth: " + monsterInfo.health[monsterInfo.upgradeLevel - 1];
                    }

                    statsTextColumn1.text = aux;
                    aux = "";

                    //Damage
                    if (monsterInfo.damage[monsterInfo.upgradeLevel - 1] != monsterInfo.damage[monsterInfo.upgradeLevel])
                    {
                        aux = "Damage: " + monsterInfo.damage[monsterInfo.upgradeLevel - 1] + " >> " + monsterInfo.damage[monsterInfo.upgradeLevel];
                    }
                    else
                    {
                        aux = "Damage: " + monsterInfo.damage[monsterInfo.upgradeLevel - 1];
                    }

                    //Attack Rate
                    if (monsterInfo.attackRate[monsterInfo.upgradeLevel - 1] != monsterInfo.attackRate[monsterInfo.upgradeLevel])
                    {
                        aux += "\nAttack rate: " + monsterInfo.attackRate[monsterInfo.upgradeLevel - 1] + "s >> " + monsterInfo.attackRate[monsterInfo.upgradeLevel] + "s";
                    }
                    else
                    {
                        aux += "\nAttack rate: " + monsterInfo.attackRate[monsterInfo.upgradeLevel - 1] + "s";
                    }

                    //Range
                    if (monsterInfo.attackRange[monsterInfo.upgradeLevel - 1] != monsterInfo.attackRange[monsterInfo.upgradeLevel])
                    {
                        aux += "\nRange: " + monsterInfo.attackRange[monsterInfo.upgradeLevel - 1] + " >> " + monsterInfo.attackRange[monsterInfo.upgradeLevel];
                    }
                    else
                    {
                        aux += "\nRange: " + monsterInfo.attackRange[monsterInfo.upgradeLevel - 1];
                    }

                    statsTextColumn2.text = aux;
                }
                else
                {
                    //don't show >>
                    statsTextColumn1.text = "Level: " + monsterInfo.level[monsterInfo.upgradeLevel - 1] + "\nVelocity: " + monsterInfo.velocity[monsterInfo.upgradeLevel - 1] + "\nHealth: " + monsterInfo.health[monsterInfo.upgradeLevel - 1];
                    statsTextColumn2.text = "Damage: " + monsterInfo.damage[monsterInfo.upgradeLevel - 1] + "\nAttack rate: " + monsterInfo.attackRate[monsterInfo.upgradeLevel - 1] + "\nRange: " + monsterInfo.attackRange[monsterInfo.upgradeLevel - 1];
                }

                //depending on level show villagers
                for (int i = 0; i < villagersIcons.Count; i++) //Put all to false just in case
                {
                    villagersIcons[i].SetActive(false);
                }

                villagersIcons[CHILD].SetActive(true); //always true
                if (monsterInfo.level[monsterInfo.upgradeLevel - 1] > 1)
                {
                    villagersIcons[MOM].SetActive(true);
                }
                if (monsterInfo.level[monsterInfo.upgradeLevel - 1] > 2)
                {
                    villagersIcons[ADULT].SetActive(true);
                    villagersIcons[SWASHBUCKLER].SetActive(true);

                }
                if (monsterInfo.level[monsterInfo.upgradeLevel - 1] > 3)
                {
                    villagersIcons[SHIELDMAN].SetActive(true);
                    villagersIcons[ELDER].SetActive(true);

                }
                if (monsterInfo.level[monsterInfo.upgradeLevel - 1] > 4)
                {
                    villagersIcons[SORCERER].SetActive(true);

                }

                //Upgrade requirements
                if (monsterInfo.upgradeLevel < 3)
                {
                    upgradeRequirementIcon1.gameObject.SetActive(true);
                    upgradeRequirementIcon2.gameObject.SetActive(true);
                    upgradeRequirementIcon3.gameObject.SetActive(true);
                    upgradeRequirementText1.gameObject.SetActive(true);
                    upgradeRequirementText2.gameObject.SetActive(true);
                    upgradeRequirementText3.gameObject.SetActive(true);
                    upgradeMonsterButton.gameObject.SetActive(true);

                    //Summon requirement 1
                    if (Data.Instance.RESOURCES.TryGetValue(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][0].resourceNameKey, out Resource upResource))
                    {
                        upgradeRequirementIcon1.sprite = upResource.icon;
                    }

                    //Show current amount and needed
                    if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][0].resourceNameKey, out int upQuantity))
                    {
                        upgradeRequirementText1.SetText(upQuantity + "/" + monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][0].quantity);
                    }
                    else
                    {
                        upgradeRequirementText1.SetText("0/" + monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][0].quantity);
                    }

                    //Summon requirement 2
                    if (Data.Instance.RESOURCES.TryGetValue(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][1].resourceNameKey, out Resource upResource2))
                    {
                        upgradeRequirementIcon2.sprite = upResource2.icon;
                    }

                    //Show current amount and needed
                    if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][1].resourceNameKey, out int upQuantity2))
                    {
                        upgradeRequirementText2.SetText(upQuantity2 + "/" + monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][1].quantity);
                    }
                    else
                    {
                        upgradeRequirementText2.SetText("0/" + monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][1].quantity);
                    }

                    //Summon requirement 3 (MAY NOT HAVE)
                    if (monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1].Count > 2)
                    {
                        if (Data.Instance.RESOURCES.TryGetValue(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][2].resourceNameKey, out Resource upResource3))
                        {
                            upgradeRequirementIcon3.sprite = upResource3.icon;
                        }

                        //Show current amount and needed
                        if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][2].resourceNameKey, out int upQuantity3))
                        {
                            upgradeRequirementText3.SetText(upQuantity3 + "/" + monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][2].quantity);
                        }
                        else
                        {
                            upgradeRequirementText3.SetText("0/" + monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][2].quantity);
                        }
                    }
                    else
                    {
                        //Hide 3rth requirement
                        upgradeRequirementIcon3.gameObject.SetActive(false);
                        upgradeRequirementText3.gameObject.SetActive(false);
                    }
                }
                else
                {
                    //Fully upgraded
                    upgradeRequirementIcon1.gameObject.SetActive(false);
                    upgradeRequirementIcon2.gameObject.SetActive(false);
                    upgradeRequirementIcon3.gameObject.SetActive(false);
                    upgradeRequirementText2.gameObject.SetActive(false);
                    upgradeRequirementText3.gameObject.SetActive(false);
                    upgradeMonsterButton.gameObject.SetActive(false);

                    upgradeRequirementText1.SetText("Fully upgraded");
                }
            }
        }
        #endregion

        #region TABS
        for (int i = 0; i < tabs.Count; i++) //Put all to deselected 
        {
            if(i == tabs.Count - 1) //The reaper
            {
                tabs[i].GetComponent<Image>().sprite = tabBackgrounds[DESELECTED_RED];
            }
            else
            {
                tabs[i].GetComponent<Image>().sprite = tabBackgrounds[DESELECTED];
            }

            if (i == hidenMonsterIndex)
            {
                //Put color black
                tabs[i].transform.GetChild(1).GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
            }
            else
            {
                //Change alpha 
                tabs[i].transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            }
        }

        //Select tab
        
        //Portait
        if(selectedTabIndex == REAPER_INDEX)
        {
            tabs[selectedTabIndex].GetComponent<Image>().sprite = tabBackgrounds[SELECTED_RED];
        }
        else
        {
            tabs[selectedTabIndex].GetComponent<Image>().sprite = tabBackgrounds[SELECTED];
        }

        //Sprite inside
        if(selectedTabIndex == hidenMonsterIndex)
        {
            tabs[selectedTabIndex].transform.GetChild(1).GetComponent<Image>().color = new Color(0, 0, 0, 1);
        }
        else
        {
            tabs[selectedTabIndex].transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }

        //Set sprite inside portrait
        for (int i = 0; i < tabs.Count; i++)
        {
            if(i > hidenMonsterIndex)
            {
                //It's unknown
                //tabs[i].transform.GetChild(0).GetComponent<Image>().sprite = questionMark;
                tabs[i].transform.GetChild(0).gameObject.SetActive(true);
                tabs[i].transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                if(Data.Instance.MONSTERS.TryGetValue(getHiddenMonster(i), out MonsterInfo monsterA))
                {
                    tabs[i].transform.GetChild(1).GetComponent<Image>().sprite = monsterA.icon;
                    tabs[i].transform.GetChild(0).gameObject.SetActive(false);
                    tabs[i].transform.GetChild(1).gameObject.SetActive(true);
                }
            }
        }
        #endregion
    }

    public void setActiveMonsterUI(string monsterKey)
    {
        if (monsterKey != NONE)
        {
            producingRequirementIcon1.gameObject.SetActive(true);
            producingRequirementText1.gameObject.SetActive(true);
            producingRequirementIcon2.gameObject.SetActive(true);
            producingRequirementText2.gameObject.SetActive(true);
            producingRequirementIcon3.gameObject.SetActive(true);
            producingRequirementText3.gameObject.SetActive(true);

            pauseButton.gameObject.SetActive(true);
            playButton.gameObject.SetActive(false);
            noActiveMonsterText.gameObject.SetActive(false);

            timeBarMonster.gameObject.SetActive(true);
            activeMonsterIcon.gameObject.SetActive(true);

            if (Data.Instance.MONSTERS.TryGetValue(monsterKey, out MonsterInfo monsterInfo))
            {
                //Summon requirement 1
                if (Data.Instance.RESOURCES.TryGetValue(monsterInfo.requirements[0].resourceNameKey, out Resource resource))
                {
                    producingRequirementIcon1.sprite = resource.icon;
                }

                //Show current amount and needed
                if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.requirements[0].resourceNameKey, out int quantity))
                {
                    producingRequirementText1.SetText(quantity + "/" + monsterInfo.requirements[0].quantity);
                }
                else
                {
                    producingRequirementText1.SetText("0/" + monsterInfo.requirements[0].quantity);
                }

                //Summon requirement 2
                if (Data.Instance.RESOURCES.TryGetValue(monsterInfo.requirements[1].resourceNameKey, out Resource resource1))
                {
                    producingRequirementIcon2.sprite = resource1.icon;
                }

                //Show current amount and needed
                if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.requirements[1].resourceNameKey, out int quantity1))
                {
                    producingRequirementText2.SetText(quantity1 + "/" + monsterInfo.requirements[1].quantity);
                }
                else
                {
                    producingRequirementText2.SetText("0/" + monsterInfo.requirements[1].quantity);
                }

                //Summon requirement 3 (MAY NOT HAVE)
                if (monsterInfo.requirements.Count > 2)
                {
                    if (Data.Instance.RESOURCES.TryGetValue(monsterInfo.requirements[2].resourceNameKey, out Resource resource2))
                    {
                        producingRequirementIcon3.sprite = resource2.icon;
                    }

                    //Show current amount and needed
                    if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.requirements[2].resourceNameKey, out int quantity2))
                    {
                        producingRequirementText3.SetText(quantity2 + "/" + monsterInfo.requirements[2].quantity);
                    }
                    else
                    {
                        producingRequirementText3.SetText("0/" + monsterInfo.requirements[2].quantity);
                    }
                }
                else
                {
                    //Hide 3rth requirement
                    producingRequirementIcon3.gameObject.SetActive(false);
                    producingRequirementText3.gameObject.SetActive(false);
                }

                timeBarMonster.minValue = 0;
                timeBarMonster.maxValue = monsterInfo.time;
                timeBarMonster.value = monsterInfo.time; //S'hauria d'agafar el time left si s'ha guardat
                //timeTextMonster.text = "-";
                activeMonsterIcon.sprite = monsterInfo.icon;
            }
        }
        else
        {
            //Default values (no monster producing)
            //Summon requirement 1

            producingRequirementIcon1.gameObject.SetActive(false);
            producingRequirementText1.gameObject.SetActive(false);
            producingRequirementIcon2.gameObject.SetActive(false);
            producingRequirementText2.gameObject.SetActive(false);
            producingRequirementIcon3.gameObject.SetActive(false);
            producingRequirementText3.gameObject.SetActive(false);

            pauseButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(false);
            noActiveMonsterText.gameObject.SetActive(true);            

            timeBarMonster.gameObject.SetActive(false);
            timeTextMonster.text = "-";
            activeMonsterIcon.gameObject.SetActive(false);
        }
    }

    public void summon()
    {
        //check if has pressed "unlock" or "summon"
        if (Data.Instance.MONSTERS.TryGetValue(selectedTab, out MonsterInfo monsterAux))
        {
            if (!monsterAux.isUnlocked)
            {
                //Has pressed "unlock"
                if (checkRequirementsToUnlock())
                {
                    #region PAY REQUIREMENTS

                    foreach (Requirement requirement in monsterAux.unlockRequirements)
                    {
                        if (Data.Instance.INVENTORY.TryGetValue(requirement.resourceNameKey, out int requirementQuantity))
                        {
                            if (requirementQuantity >= requirement.quantity)
                            {
                                requirementQuantity -= requirement.quantity;
                                Data.Instance.updateInventory(requirement.resourceNameKey, requirementQuantity);
                            }
                        }
                    }
                    #endregion

                    monsterAux.isUnlocked = true;
                    unlockedMonsters.Add(selectedTab);
                    hidenMonster = getHiddenMonster(selectedTabIndex + 1);
                    hidenMonsterIndex = selectedTabIndex + 1;
                    setUI(selectedTab); //Update UI (unlocked)
                }
            }
            else
            {
                //Has pressed "summon"
                #region UNLOCKED CASE
                //Summon selected tab monster
                if (selectedTab != activeMonster) //check the monster isn't already being produced
                {
                    activeMonster = selectedTab;
                    if (Data.Instance.MONSTERS.TryGetValue(activeMonster, out MonsterInfo monster))
                    {
                        activeMonsterTime = monster.time;
                    }
                    //Reset time to produce
                    time = 0;

                    //Change sprite
                    setActiveMonsterUI(activeMonster);

                    //Slider config
                    timeBarMonster.minValue = 0;
                    timeBarMonster.maxValue = activeMonsterTime;
                    timeBarMonster.value = activeMonsterTime;

                    //Play when changing monster to produce
                    if (!isProducing)
                    {
                        play();
                    }

                    setActiveMonsterUI(activeMonster);
                }
                #endregion
            }
        }        
    }

    public string getHiddenMonster(int hidenMonsterIndex)
    {
        string hidenMonster = "";
        switch (hidenMonsterIndex)
        {
            case 0:
                hidenMonster = SKELETON;
                break;
            case 1:
                hidenMonster = JACK_LANTERN;
                break;
            case 2:
                hidenMonster = BAT;
                break;
            case 3:
                hidenMonster = GOBLIN;
                break;
            case 4:
                hidenMonster = GHOST;
                break;
            case 5:
                hidenMonster = CLOWN;
                break;
            case 6:
                hidenMonster = ZOMBIE;
                break;
            case 7:
                hidenMonster = VAMPIRE;
                break;
            case 8:
                hidenMonster = WITCH;
                break;
            case 9:
                hidenMonster = REAPER;
                break;
        }
        return hidenMonster;
    }

    

    public void setSelectedTab(string selectedMonster)
    {
        if (selectedTab != selectedMonster) //Check if it's already in that page
        {
            selectedTab = selectedMonster;
            #region SET TAB INDEX
            switch (selectedMonster)
            {
                case "skeleton":
                    selectedTabIndex = SKELETON_INDEX;
                    break;

                case "jackOLantern":
                    selectedTabIndex = JACK_LANTERN_INDEX;
                    break;

                case "bat":
                    selectedTabIndex = BAT_INDEX;
                    break;

                case "zombie":
                    selectedTabIndex = ZOMBIE_INDEX;
                    break;

                case "vampire":
                    selectedTabIndex = VAMPIRE_INDEX;
                    break;

                case "clown":
                    selectedTabIndex = CLOWN_INDEX;
                    break;

                case "ghost":
                    selectedTabIndex = GHOST_INDEX;
                    break;

                case "goblin":
                    selectedTabIndex = GOBLIN_INDEX;
                    break;

                case "witch":
                    selectedTabIndex = WITCH_INDEX;
                    break;

                case "reaper":
                    selectedTabIndex = REAPER_INDEX;
                    break;
            }
            #endregion

            //Turn page animation
            animator.Play("book");
            Invoke(nameof(hideInfoGroup), 0.25f);
            Invoke(nameof(showInfoGroup), 0.4f); //When animation ends
        }
    }

    public void showInfoGroup()
    {
        infoGroup.SetActive(true);
    }

    public void hideInfoGroup()
    {
        infoGroup.SetActive(false);
        setUI(selectedTab);
    }

    override
    public void pause()
    {
        isProducing = false;
        isPaused = true;
        timeBarMonster.value = timeLeft;
    }

    override
    public void produce()
    {
        if (Data.Instance.MONSTERS.TryGetValue(activeMonster, out MonsterInfo monster))
        {

            if (checkRequirementsToProduce())
            {
                #region PAY REQUIREMENTS

                foreach (Requirement requirement in monster.requirements)
                {
                    if (Data.Instance.INVENTORY.TryGetValue(requirement.resourceNameKey, out int requirementQuantity))
                    {
                        if (requirementQuantity >= requirement.quantity)
                        {
                            requirementQuantity -= requirement.quantity;
                            Data.Instance.updateInventory(requirement.resourceNameKey, requirementQuantity);
                        }
                    }
                }

                #endregion

                #region ADD TO INVENTORY
                if (Data.Instance.MONSTER_INVENTORY.TryGetValue(activeMonster, out int quantity))
                {
                    quantity += 1;
                    Data.Instance.updateMonsterInventory(activeMonster, quantity); //update monster inventory
                }
                else
                {
                    Data.Instance.MONSTER_INVENTORY.Add(activeMonster, 1);
                }
                #endregion
            }
        }
    }

    override
    public bool checkRequirementsToProduce()
    {
        bool hasRequirements = false;
        if (Data.Instance.MONSTERS.TryGetValue(activeMonster, out MonsterInfo monster))
        {
            foreach (Requirement requirement in monster.requirements)
            {
                if (Data.Instance.INVENTORY.TryGetValue(requirement.resourceNameKey, out int requirementQuantity))
                {
                    if (requirementQuantity < requirement.quantity)
                    {
                        enoughResources = false;
                        return false;
                    }
                    else
                    {
                        enoughResources = true;
                        hasRequirements = true;
                    }
                }
                else
                {
                    //Player don't have the requirement resource to produce 
                    enoughResources = false;
                    return false;
                }
            }
        }
        return hasRequirements;
    }

    public bool checkRequirementsToUnlock()
    {
        bool hasRequirements = false;
        if (Data.Instance.MONSTERS.TryGetValue(selectedTab, out MonsterInfo monster))
        {
            foreach (Requirement requirement in monster.unlockRequirements)
            {
                if (Data.Instance.INVENTORY.TryGetValue(requirement.resourceNameKey, out int requirementQuantity))
                {
                    if (requirementQuantity < requirement.quantity)
                    {
                        enoughResources = false;
                        return false;
                    }
                    else
                    {
                        enoughResources = true;
                        hasRequirements = true;
                    }
                }
                else
                {
                    //Player don't have the requirement resource to produce 
                    enoughResources = false;
                    return false;
                }
            }
        }
        return hasRequirements;
    }

    public bool checkRequirementsToUpgrade()
    {
        bool hasRequirements = false;
        if (Data.Instance.MONSTERS.TryGetValue(selectedTab, out MonsterInfo monster))
        {
            foreach (Requirement requirement in monster.upgradeRequirements[monster.upgradeLevel - 1])
            {
                if (Data.Instance.INVENTORY.TryGetValue(requirement.resourceNameKey, out int requirementQuantity))
                {
                    if (requirementQuantity < requirement.quantity)
                    {
                        enoughResources = false;
                        return false;
                    }
                    else
                    {
                        enoughResources = true;
                        hasRequirements = true;
                    }
                }
                else
                {
                    //Player don't have the requirement resource to produce 
                    enoughResources = false;
                    return false;
                }
            }
        }
        return hasRequirements;
    }

    public void updateUIInventory() //called each second (selectedTab x parametre)
    {
        string monsterKey = selectedTab;
        if (Data.Instance.MONSTERS.TryGetValue(monsterKey, out MonsterInfo monsterInfo))
        {
            if (monsterInfo.isUnlocked)
            {
                #region SUMMON COST (SELECTED TAB)
                //Summon requirement 1

                //Show current amount and needed
                if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.requirements[0].resourceNameKey, out int quantity))
                {
                    summonRequirementText1.SetText(quantity + "/" + monsterInfo.requirements[0].quantity);
                }
                else
                {
                    summonRequirementText1.SetText("0/" + monsterInfo.requirements[0].quantity);
                }

                //Summon requirement 2
                //Show current amount and needed
                if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.requirements[1].resourceNameKey, out int quantity1))
                {
                    summonRequirementText2.SetText(quantity1 + "/" + monsterInfo.requirements[1].quantity);
                }
                else
                {
                    summonRequirementText2.SetText("0/" + monsterInfo.requirements[1].quantity);
                }

                //Summon requirement 3 (MAY NOT HAVE)
                if (monsterInfo.requirements.Count > 2)
                {
                    //Show current amount and needed
                    if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.requirements[2].resourceNameKey, out int quantity2))
                    {
                        summonRequirementText3.SetText(quantity2 + "/" + monsterInfo.requirements[2].quantity);
                    }
                    else
                    {
                        summonRequirementText3.SetText("0/" + monsterInfo.requirements[2].quantity);
                    }
                }
                else
                {

                    //Hide 3rth requirement
                    summonRequirementIcon3.gameObject.SetActive(false);
                    summonRequirementText3.gameObject.SetActive(false);
                }
                #endregion
            }
            else
            {
                #region UNLOCK COST
                //show unlock requirements (only 1?)
                //Summon requirement 1
                //Show current amount and needed
                if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.unlockRequirements[0].resourceNameKey, out int quantity))
                {
                    summonRequirementText1.SetText(quantity + "/" + monsterInfo.unlockRequirements[0].quantity);

                }
                else
                {
                    summonRequirementText1.SetText("0/" + monsterInfo.unlockRequirements[0].quantity);
                }
                #endregion
            }

            #region UPGRADE COST (SELECTED TAB)
            //Upgrade requirement 1

            if (monsterInfo.upgradeLevel < 3)
            {
                //Show current amount and needed
                if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][0].resourceNameKey, out int upQuantity))
                {
                    upgradeRequirementText1.SetText(upQuantity + "/" + monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][0].quantity);
                }
                else
                {
                    upgradeRequirementText1.SetText("0/" + monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][0].quantity);
                }

                //Upgrade requirement 2
                //Show current amount and needed
                if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][1].resourceNameKey, out int upQuantity2))
                {
                    upgradeRequirementText2.SetText(upQuantity2 + "/" + monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][1].quantity);
                }
                else
                {
                    upgradeRequirementText2.SetText("0/" + monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][1].quantity);
                }

                //Upgrade requirement 3 (MAY NOT HAVE)
                if (monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1].Count > 2)
                {
                    //Show current amount and needed
                    if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][2].resourceNameKey, out int upQuantity3))
                    {
                        upgradeRequirementText3.SetText(upQuantity3 + "/" + monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][2].quantity);
                    }
                    else
                    {
                        upgradeRequirementText3.SetText("0/" + monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][2].quantity);
                    }
                }
                else
                {
                    //Hide 3rth requirement
                    upgradeRequirementIcon3.gameObject.SetActive(false);
                    upgradeRequirementText3.gameObject.SetActive(false);
                }
            }
            #endregion
        }

        if (Data.Instance.MONSTERS.TryGetValue(activeMonster, out MonsterInfo mInfo))
        {
            #region TIMEGROUP (ACTIVE RESOURCE)
            //Summon requirement 1
            //Show current amount and needed
            if (Data.Instance.INVENTORY.TryGetValue(mInfo.requirements[0].resourceNameKey, out int quantity))
            {
                producingRequirementText1.SetText(quantity + "/" + mInfo.requirements[0].quantity);
            }
            else
            {
                producingRequirementText1.SetText("0/" + mInfo.requirements[0].quantity);
            }

            //Summon requirement 2
            //Show current amount and needed
            if (Data.Instance.INVENTORY.TryGetValue(mInfo.requirements[1].resourceNameKey, out int quantity1))
            {
                producingRequirementText2.SetText(quantity1 + "/" + mInfo.requirements[1].quantity);
            }
            else
            {
                producingRequirementText2.SetText("0/" + mInfo.requirements[1].quantity);
            }

            //Summon requirement 3 (MAY NOT HAVE)
            if (mInfo.requirements.Count > 2)
            {
                //Show current amount and needed
                if (Data.Instance.INVENTORY.TryGetValue(mInfo.requirements[2].resourceNameKey, out int quantity2))
                {
                    producingRequirementText3.SetText(quantity2 + "/" + mInfo.requirements[2].quantity);
                }
                else
                {
                    producingRequirementText3.SetText("0/" + mInfo.requirements[2].quantity);
                }
            }
            #endregion
        }
    }

    public void upgradeMonster()
    {
        //Only enters here if can upgrade (button disappears when max level)
        if (Data.Instance.MONSTERS.TryGetValue(selectedTab, out MonsterInfo monster))
        {
            //check if monster is unlocked
            if (monster.isUnlocked)
            {
                if (checkRequirementsToUpgrade())
                {
                    #region PAY REQUIREMENTS

                    foreach (Requirement requirement in monster.upgradeRequirements[monster.upgradeLevel - 1])
                    {
                        if (Data.Instance.INVENTORY.TryGetValue(requirement.resourceNameKey, out int requirementQuantity))
                        {
                            if (requirementQuantity >= requirement.quantity)
                            {
                                requirementQuantity -= requirement.quantity;
                                Data.Instance.updateInventory(requirement.resourceNameKey, requirementQuantity);
                            }
                        }
                    }
                    #endregion

                    monster.upgradeLevel++;

                    setUI(selectedTab);
                }
            }
        }
    }
}
