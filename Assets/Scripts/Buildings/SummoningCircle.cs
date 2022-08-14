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

    #endregion building variables

    #region UI VARIABLES

    //Falta fer els tabs de seleccio de monster
    public TextMeshProUGUI monsterName;

    public Image monsterImage;
    public Image levelImage;
    public TextMeshProUGUI description;
    public GameObject summonRequirement1;
    public GameObject summonRequirement2;
    public TextMeshProUGUI statsTextColumn1;
    public TextMeshProUGUI statsTextColumn2;
    public List<GameObject> villagersIcons;
    public GameObject upgradeRequirement1;
    public GameObject upgradeRequirement2;
    public Image producingRequirementIcon1;
    public TextMeshProUGUI producingRequirementText1;
    public Image producingRequirementIcon2;
    public TextMeshProUGUI producingRequirementText2;
    public Image timeBarMonster;
    public TextMeshProUGUI timeTextMonster;
    public Image activeMonsterIcon;
    public TextMeshProUGUI noActiveMonsterText;
    public List<GameObject> tabs;
    public List<Sprite> tabBackgrounds;
    public Image monsterImagePortrait;
    public Sprite unlockedPortrait;
    public Sprite lockedPortrait;
    public GameObject upgradeMonsterButton;
    public Sprite questionMark;
    public GameObject unknownGroup;
    public GameObject levelImagePortrait;
    public GameObject infoGroup;
    public GameObject upgradeGroup;
    public GameObject unlockButton;
    public GameObject playButtonMonster;
    public GameObject pauseButtonMonster;
    public GameObject goToMerchantText;
    public GameObject timeBarGroup;

    #endregion UI VARIABLES

    public List<Sprite> numbersIcons;
    public Animator animator; //.play("book") to turn page

    public string activeMonster = NONE;
    public string selectedTab = NONE;
    public int selectedTabIndex = 0;
    public float activeMonsterTime = 0;
    public float timeModifier = 0;
    private float timeToUpdate = 0;
    private float timeToUpdateBar = 0;
    private bool timeTimeUpdated = false;

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

    #endregion MONSTER KEYS

    #region VILLAGER ICONS INDEX

    public static int CHILD = 0;
    public static int MOM = 1;
    public static int ADULT = 2;
    public static int SWASHBUCKLER = 3;
    public static int SHIELDMAN = 4;
    public static int ELDER = 5;
    public static int SORCERER = 6;

    #endregion VILLAGER ICONS INDEX

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

    #endregion MONSTER TABS INDEX

    #region TABS BACKGROUND INDEX

    public static int SELECTED = 0;
    public static int DESELECTED = 1;
    public static int DESELECTED_RED = 2;
    public static int SELECTED_RED = 3;
    public static int SELECTED_BLUE = 4;
    public static int DESELECTED_BLUE = 5;

    #endregion TABS BACKGROUND INDEX

    //Audio variables
    protected static int TURNING_PAGE = 7;

    private void Start()
    {
        //Set first monster to inventory
        if (!Data.Instance.INVENTORY.ContainsKey(Data.SKELETON))
        {
            Data.Instance.INVENTORY.Add(Data.SKELETON, 0);
        }

        constructionType = 1;

        if (activeMonster == NONE)
        {
            //If it's not producing (either in pause or first time building created, set default settings to skeleton)
            selectedTab = SKELETON;
            setUI(SKELETON);
        }
        else
        {
            //Put default UI to the monster that is producing
            selectedTab = activeMonster;
            setUI(activeMonster);

            if (Data.Instance.MONSTERS.TryGetValue(activeMonster, out MonsterInfo monster))
            {
                activeMonsterTime = monster.time - (monster.time * timeModifier);
                activeResourceTime = monster.time - (monster.time * timeModifier);
            }
        }
        canvasInterior.SetActive(false);

        //Save building position
        position = transform.position;

        //Apply boost
        updateActiveMonsterBoostStart();
    }

    private void Update()
    {
        //Update requirements (inventory) EACH SECOND
        if (placed)
        {
            if (isProducing)
            {
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
                timeBarMonster.fillAmount = timeLeft / activeMonsterTime;
                timeTextMonster.SetText("-");
                if (isPaused)
                {
                    playButtonMonster.gameObject.SetActive(true);
                    pauseButtonMonster.gameObject.SetActive(false);
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
                if (timeToUpdateBar >= 0.25f) //Update time bar every 0.25s
                {
                    #region TIME BAR

                    if (isProducing)
                    {
                        int hours = (int)timeLeft / 3600;
                        int minutes = (int)(timeLeft - (hours * 3600)) / 60;
                        int secondsLeft = (int)timeLeft - (hours * 3600) - (minutes * 60);
                        if (hours > 0)
                        {
                            if (minutes > 0)
                            {
                                timeTextMonster.SetText(hours.ToString() + "h " + minutes.ToString() + "m " + secondsLeft + "s");
                            }
                            else
                            {
                                timeTextMonster.SetText(hours.ToString() + "h " + secondsLeft + "s");
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
                                timeTextMonster.SetText(secondsLeft + "s");
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
                        timeBarMonster.fillAmount = timeLeft / activeMonsterTime;
                    }

                    #endregion TIME BAR

                    timeToUpdateBar = 0;
                }
            }

            #endregion UPDATE UI
        }
    }

    public void setUI(string monsterKey)
    {
        #region BOOK INFO

        if (Data.Instance.MONSTERS.TryGetValue(monsterKey, out MonsterInfo monsterInfo))
        {
            //Check if it's unknown
            if (!monsterInfo.isUnlocked && monsterKey != GameManager.Instance.hidenMonster)
            {
                //It's unknown
                monsterName.SetText("Unknown");
                monsterImagePortrait.sprite = unlockedPortrait;
                monsterImage.sprite = questionMark;
                monsterImage.color = new Color(1, 1, 1, 1);
                levelImagePortrait.gameObject.SetActive(false);

                description.SetText("This monster is yet to be discovered. Unlock the other monsters first");

                unknownGroup.SetActive(false);

                //Hide timeBar
                timeBarGroup.SetActive(false);
            }
            else
            {
                monsterName.SetText(monsterInfo.monsterName);
                levelImagePortrait.gameObject.SetActive(true); //just in case
                levelImage.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                unknownGroup.SetActive(true);

                //check if it's unlocked
                if (monsterInfo.isUnlocked)
                {
                    monsterImagePortrait.sprite = unlockedPortrait;
                    monsterImage.sprite = monsterInfo.icon;
                    monsterImage.color = new Color(1, 1, 1, 1);
                    unlockButton.SetActive(false);
                    goToMerchantText.SetActive(false);

                    //check if it's being produced
                    if (monsterInfo.id.Equals(activeMonster))
                    {
                        //is being produced
                        pauseButtonMonster.SetActive(true);
                        playButtonMonster.SetActive(false);
                        //Show timeBar
                        timeBarGroup.SetActive(true);
                    }
                    else
                    {
                        //is not being produced
                        pauseButtonMonster.SetActive(false);
                        playButtonMonster.SetActive(true);
                        //Hide timeBar
                        timeBarGroup.SetActive(false);
                    }
                }
                else
                {
                    monsterImagePortrait.sprite = lockedPortrait;
                    monsterImage.sprite = monsterInfo.icon;
                    monsterImage.color = new Color(0, 0, 0, 1);
                    unlockButton.SetActive(true);
                    goToMerchantText.SetActive(true);
                    playButtonMonster.SetActive(false);
                    pauseButtonMonster.SetActive(false);
                    //Hide timeBar
                    timeBarGroup.SetActive(false);
                }
                levelImage.sprite = numbersIcons[monsterInfo.upgradeLevel - 1];
                description.text = monsterInfo.description;

                if (monsterInfo.isUnlocked)
                {
                    summonRequirement1.SetActive(true);
                    summonRequirement2.SetActive(true);
                    unlockButton.SetActive(false);
                    goToMerchantText.SetActive(false);

                    //Summon requirement 1
                    if (Data.Instance.RESOURCES.TryGetValue(monsterInfo.requirements[0].resourceNameKey, out Resource resource))
                    {
                        summonRequirement1.transform.GetChild(0).GetComponent<Image>().sprite = resource.icon;
                    }

                    //Show current amount and needed
                    if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.requirements[0].resourceNameKey, out int quantity))
                    {
                        summonRequirement1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(GameManager.Instance.numToString(quantity) + "/" + GameManager.Instance.numToString(monsterInfo.requirements[0].quantity));
                    }
                    else
                    {
                        summonRequirement1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("0/" + GameManager.Instance.numToString(monsterInfo.requirements[0].quantity));
                    }

                    //Summon requirement 2
                    if (Data.Instance.RESOURCES.TryGetValue(monsterInfo.requirements[1].resourceNameKey, out Resource resource1))
                    {
                        summonRequirement2.transform.GetChild(0).GetComponent<Image>().sprite = resource1.icon;
                    }

                    //Show current amount and needed
                    if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.requirements[1].resourceNameKey, out int quantity1))
                    {
                        summonRequirement2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(GameManager.Instance.numToString(quantity1) + "/" + GameManager.Instance.numToString(monsterInfo.requirements[1].quantity));
                    }
                    else
                    {
                        summonRequirement2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("0/" + GameManager.Instance.numToString(monsterInfo.requirements[1].quantity));
                    }
                }
                else
                {
                    //show unlock requirements (only 1?)

                    //Hide other requirements
                    summonRequirement1.SetActive(false);
                    summonRequirement2.SetActive(false);
                    unlockButton.SetActive(true);
                    goToMerchantText.SetActive(true);
                }
                //only show >> if upgrading changes value (TODO)
                if (monsterInfo.upgradeLevel < 3)
                {
                    string aux = "";

                    //Level                    
                    aux = "Level: " + monsterInfo.upgradeLevel + " >> " + (monsterInfo.upgradeLevel + 1).ToString();
                    
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
                    statsTextColumn1.text = "Level: " + monsterInfo.upgradeLevel + "\nVelocity: " + monsterInfo.velocity[monsterInfo.upgradeLevel - 1] + "\nHealth: " + monsterInfo.health[monsterInfo.upgradeLevel - 1];
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

                //Show upgrade group if it's unlocked
                if (monsterInfo.isUnlocked)
                {
                    upgradeGroup.SetActive(true);

                    //Upgrade requirements
                    if (monsterInfo.upgradeLevel < 3)
                    {
                        upgradeGroup.transform.GetChild(2).gameObject.SetActive(false);
                        upgradeRequirement1.SetActive(true);
                        upgradeRequirement2.SetActive(true);
                        upgradeMonsterButton.SetActive(true);

                        //Summon requirement 1
                        if (Data.Instance.RESOURCES.TryGetValue(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][0].resourceNameKey, out Resource upResource))
                        {
                            upgradeRequirement1.transform.GetChild(0).GetComponent<Image>().sprite = upResource.icon;
                        }

                        //Show current amount and needed
                        if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][0].resourceNameKey, out int upQuantity))
                        {
                            upgradeRequirement1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(GameManager.Instance.numToString(upQuantity) + "/" + GameManager.Instance.numToString(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][0].quantity));
                        }
                        else
                        {
                            upgradeRequirement1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("0/" + GameManager.Instance.numToString(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][0].quantity));
                        }

                        //Summon requirement 2
                        if (Data.Instance.RESOURCES.TryGetValue(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][1].resourceNameKey, out Resource upResource2))
                        {
                            upgradeRequirement2.transform.GetChild(0).GetComponent<Image>().sprite = upResource2.icon;
                        }

                        //Show current amount and needed
                        if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][1].resourceNameKey, out int upQuantity2))
                        {
                            upgradeRequirement2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(GameManager.Instance.numToString(upQuantity2) + "/" + GameManager.Instance.numToString(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][1].quantity));
                        }
                        else
                        {
                            upgradeRequirement2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("0/" + GameManager.Instance.numToString(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][1].quantity));
                        }
                    }
                    else
                    {
                        //Fully upgraded
                        upgradeRequirement1.SetActive(false);
                        upgradeRequirement2.SetActive(false);
                        upgradeMonsterButton.SetActive(false);

                        upgradeGroup.transform.GetChild(2).gameObject.SetActive(true);
                    }
                }
                else
                {
                    //Hide upgrade group
                    upgradeGroup.SetActive(false);
                }
            }
        }

        #endregion BOOK INFO

        #region TABS

        if (!Data.Instance.BOOSTS.ContainsKey(Data.JACK_LANTERN))
        {
            GameManager.Instance.hidenMonster = Data.JACK_LANTERN;
            GameManager.Instance.hidenMonsterIndex = getIndexMonster(Data.JACK_LANTERN);
        }

        for (int i = 0; i < tabs.Count; i++) //Put all to deselected
        {
            if (i == tabs.Count - 1) //The reaper
            {
                tabs[i].GetComponent<Image>().sprite = tabBackgrounds[DESELECTED_RED];
            }
            else
            {
                tabs[i].GetComponent<Image>().sprite = tabBackgrounds[DESELECTED];
            }

            if (i == GameManager.Instance.hidenMonsterIndex)
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
        if (selectedTabIndex == REAPER_INDEX)
        {
            tabs[selectedTabIndex].GetComponent<Image>().sprite = tabBackgrounds[SELECTED_RED];
        }
        else
        {
            tabs[selectedTabIndex].GetComponent<Image>().sprite = tabBackgrounds[SELECTED];
        }

        //Sprite inside
        if (selectedTabIndex == GameManager.Instance.hidenMonsterIndex)
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
            if (i > GameManager.Instance.hidenMonsterIndex)
            {
                //It's unknown
                //tabs[i].transform.GetChild(0).GetComponent<Image>().sprite = questionMark;
                tabs[i].transform.GetChild(0).gameObject.SetActive(true);
                tabs[i].transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                if (Data.Instance.MONSTERS.TryGetValue(getHiddenMonster(i), out MonsterInfo monsterA))
                {
                    tabs[i].transform.GetChild(1).GetComponent<Image>().sprite = monsterA.icon;
                    tabs[i].transform.GetChild(0).gameObject.SetActive(false);
                    tabs[i].transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            //Set active monster tab background
        }
        if (activeMonster != NONE)
        {
            if (tabs[getIndexMonster(activeMonster)] == tabs[selectedTabIndex])
            {
                tabs[getIndexMonster(activeMonster)].GetComponent<Image>().sprite = tabBackgrounds[SELECTED_BLUE];
            }
            else
            {
                tabs[getIndexMonster(activeMonster)].GetComponent<Image>().sprite = tabBackgrounds[DESELECTED_BLUE];
            }
        }

        #endregion TABS

        //Refresh title
        //Invoke(nameof(updateTitleGroup), 0.2f);
    }

    public void goToMerchant()
    {
        //Has pressed "unlock" -> go to BoostShop
        audioSource.clip = sounds[DEFAULT];
        audioSource.Play();

        canvasInterior.SetActive(false);
        GameManager.Instance.boostShop.GetComponent<BoostShop>().showShopImmediate();
    }

    public void summon()
    {
        //Has pressed "summon"

        #region UNLOCKED CASE

        //Summon selected tab monster
        if (selectedTab != activeMonster) //check the monster isn't already being produced
        {
            activeMonster = selectedTab;
            if (Data.Instance.MONSTERS.TryGetValue(activeMonster, out MonsterInfo monster))
            {
                activeMonsterTime = monster.time - (monster.time * timeModifier);
                activeResourceTime = monster.time - (monster.time * timeModifier);
            }
            //Reset time to produce
            time = 0;

            //Slider config
            //timeBarMonster.fillAmount = activeMonsterTime;

            //change button to pause
            setPlayPauseButtons();

            setUI(selectedTab); //To show timeBar
        }

        #endregion UNLOCKED CASE

        play();
    }

    public void setPlayPauseButtons()
    {
        if (selectedTab.Equals(activeMonster))
        {
            if (isProducing)
            {
                playButtonMonster.SetActive(false);
                pauseButtonMonster.SetActive(true);
            }
            else
            {
                playButtonMonster.SetActive(true);
                pauseButtonMonster.SetActive(false);
            }
        }
        else
        {
            playButtonMonster.SetActive(true);
            pauseButtonMonster.SetActive(false);
        }
    }

    public void goToBoostShop()
    {
        GameManager.Instance.boostShop.GetComponent<BoostShop>().showShop();
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

            case 100:
                hidenMonster = NONE;
                break;
        }
        return hidenMonster;
    }

    public string getStringMonster(int monsterIndex)
    {
        string hidenMonster = "";
        switch (monsterIndex)
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

            case 100:
                hidenMonster = NONE;
                break;
        }
        return hidenMonster;
    }

    public int getIndexMonster(string monsterString)
    {
        int hidenMonster = 0;
        switch (monsterString)
        {
            case "skeleton":
                hidenMonster = 0;
                break;

            case "jackOLantern":
                hidenMonster = 1;
                break;

            case "bat":
                hidenMonster = 2;
                break;

            case "goblin":
                hidenMonster = 3;
                break;

            case "ghost":
                hidenMonster = 4;
                break;

            case "clown":
                hidenMonster = 5;
                break;

            case "zombie":
                hidenMonster = 6;
                break;

            case "vampire":
                hidenMonster = 7;
                break;

            case "witch":
                hidenMonster = 8;
                break;

            case "reaper":
                hidenMonster = 9;
                break;

            case "none":
                hidenMonster = 100;
                break;
        }
        return hidenMonster;
    }

    public void setSelectedTab(string selectedMonster)
    {
        GameManager.Instance.debugInventoryInfo.SetText(activeMonster + " T: " + ((int)(activeMonsterTime/60)).ToString()+ "m TL: " + timeLeft.ToString());
        
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

            #endregion SET TAB INDEX

            //Turn page animation
            audioSource.clip = sounds[TURNING_PAGE];
            audioSource.Play();
            animator.Play("book");
            Invoke(nameof(hideInfoGroup), 0.25f);
            Invoke(nameof(showInfoGroup), 0.4f); //When animation ends
        }
    }

    public void setTabIndex(string selectedMonster) //Used in buildConstructions in GM
    {
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

        #endregion SET TAB INDEX
    }

    public void showInfoGroup()
    {
        infoGroup.SetActive(true);
    }

    public void hideInfoGroup()
    {
        infoGroup.SetActive(false);
        setUI(selectedTab);
        //Set buttons
        setPlayPauseButtons();
    }

    public override void pause()
    {
        audioSource.clip = sounds[PAUSE];
        audioSource.Play();

        isProducing = false;
        isPaused = true;
        timeBarMonster.fillAmount = timeLeft / activeMonsterTime;
        setPlayPauseButtons();
    }

    public override void play()
    {
        audioSource.clip = sounds[PLAY];
        audioSource.Play();

        isProducing = true;
        isPaused = false;
        setPlayPauseButtons();
    }

    public override void produce()
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

                #endregion PAY REQUIREMENTS

                #region ADD TO INVENTORY

                if (Data.Instance.INVENTORY.TryGetValue(activeMonster, out int quantity))
                {
                    quantity += 1;
                    Data.Instance.updateInventory(activeMonster, quantity); //update monster inventory
                }
                else
                {
                    Data.Instance.INVENTORY.Add(activeMonster, 1);
                }

                #endregion ADD TO INVENTORY
            }
        }
    }

    public override bool checkRequirementsToProduce()
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
                    summonRequirement1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(GameManager.Instance.numToString(quantity) + "/" + GameManager.Instance.numToString(monsterInfo.requirements[0].quantity));
                }
                else
                {
                    summonRequirement1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("0/" + GameManager.Instance.numToString(monsterInfo.requirements[0].quantity));
                }

                //Summon requirement 2
                //Show current amount and needed
                if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.requirements[1].resourceNameKey, out int quantity1))
                {
                    summonRequirement2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(GameManager.Instance.numToString(quantity1) + "/" + GameManager.Instance.numToString(monsterInfo.requirements[1].quantity));
                }
                else
                {
                    summonRequirement2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("0/" + GameManager.Instance.numToString(monsterInfo.requirements[1].quantity));
                }

                #endregion SUMMON COST (SELECTED TAB)
            }
            else
            {
                #region UNLOCK COST

                //show unlock requirements (only 1?)
                //Summon requirement 1
                //Show current amount and needed
                if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.unlockRequirements[0].resourceNameKey, out int quantity))
                {
                    summonRequirement1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(GameManager.Instance.numToString(quantity) + "/" + GameManager.Instance.numToString(monsterInfo.unlockRequirements[0].quantity));
                }
                else
                {
                    summonRequirement1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("0/" + GameManager.Instance.numToString(monsterInfo.unlockRequirements[0].quantity));
                }

                #endregion UNLOCK COST
            }

            #region UPGRADE COST (SELECTED TAB)

            //Upgrade requirement 1

            if (monsterInfo.upgradeLevel < 3)
            {
                //Show current amount and needed
                if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][0].resourceNameKey, out int upQuantity))
                {
                    upgradeRequirement1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(GameManager.Instance.numToString(upQuantity) + "/" + GameManager.Instance.numToString(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][0].quantity));
                }
                else
                {
                    upgradeRequirement1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("0/" + GameManager.Instance.numToString(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][0].quantity));
                }

                //Upgrade requirement 2
                //Show current amount and needed
                if (Data.Instance.INVENTORY.TryGetValue(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][1].resourceNameKey, out int upQuantity2))
                {
                    upgradeRequirement2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(GameManager.Instance.numToString(upQuantity2) + "/" + GameManager.Instance.numToString(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][1].quantity));
                }
                else
                {
                    upgradeRequirement2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("0/" + GameManager.Instance.numToString(monsterInfo.upgradeRequirements[monsterInfo.upgradeLevel - 1][1].quantity));
                }
            }

            #endregion UPGRADE COST (SELECTED TAB)
        }

        if (Data.Instance.MONSTERS.TryGetValue(activeMonster, out MonsterInfo mInfo))
        {
            #region TIMEGROUP (ACTIVE RESOURCE)

            //Summon requirement 1
            //Show current amount and needed
            if (Data.Instance.INVENTORY.TryGetValue(mInfo.requirements[0].resourceNameKey, out int quantity))
            {
                producingRequirementText1.SetText(GameManager.Instance.numToString(quantity) + "/" + GameManager.Instance.numToString(mInfo.requirements[0].quantity));
            }
            else
            {
                producingRequirementText1.SetText("0/" + GameManager.Instance.numToString(mInfo.requirements[0].quantity));
            }

            //Summon requirement 2
            //Show current amount and needed
            if (Data.Instance.INVENTORY.TryGetValue(mInfo.requirements[1].resourceNameKey, out int quantity1))
            {
                producingRequirementText2.SetText(GameManager.Instance.numToString(quantity1) + "/" + GameManager.Instance.numToString(mInfo.requirements[1].quantity));
            }
            else
            {
                producingRequirementText2.SetText("0/" + GameManager.Instance.numToString(mInfo.requirements[1].quantity));
            }

            #endregion TIMEGROUP (ACTIVE RESOURCE)
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
                    audioSource.clip = sounds[UPGRADE];
                    audioSource.Play();

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

                    #endregion PAY REQUIREMENTS

                    monster.upgradeLevel++;

                    //Update monster stats dictionary
                    Data.Instance.MONSTERS_STATS[selectedTab] = new int[] { 1, monster.upgradeLevel };

                    setUI(selectedTab);
                }
                else
                {
                    audioSource.clip = sounds[ERROR];
                    audioSource.Play();
                }
            }
            else
            {
                audioSource.clip = sounds[ERROR];
                audioSource.Play();
            }
        }
    }

    public override int getNumActiveResource() //Get index of activeMonster
    {
        return getIndexMonster(activeMonster);
    }

    public void updateActiveMonsterTime()
    {
        if (Data.Instance.BOOSTS.TryGetValue(Data.SUMMONING_BOOST, out int quantity))
        {
            switch (quantity)
            {
                case 1:
                    timeModifier = 0.15f;
                    break;

                case 2:
                    timeModifier = 0.3f;
                    break;

                case 3:
                    timeModifier = 0.5f;
                    break;
            }
        }
        else
        {
            timeModifier = 0;
        }

        if (activeMonster != NONE)
        {
            //Get proportion
            float proportion = timeLeft / activeMonsterTime;

            if (Data.Instance.MONSTERS.TryGetValue(activeMonster, out MonsterInfo monster))
            {
                activeMonsterTime = monster.time - (monster.time * timeModifier);
                activeResourceTime = monster.time - (monster.time * timeModifier);
            }
            time = activeMonsterTime - (activeMonsterTime * proportion);
        }
    }

    public void updateActiveMonsterBoostStart()
    {
        if (Data.Instance.BOOSTS.TryGetValue(Data.SUMMONING_BOOST, out int quantity))
        {
            switch (quantity)
            {
                case 1:
                    timeModifier = 0.15f;
                    break;

                case 2:
                    timeModifier = 0.3f;
                    break;

                case 3:
                    timeModifier = 0.5f;
                    break;
            }
        }
        else
        {
            timeModifier = 0;
        }

        if (activeMonster != NONE)
        {
            //Get proportion
            float proportion = timeLeft / activeMonsterTime;

            if (Data.Instance.MONSTERS.TryGetValue(activeMonster, out MonsterInfo monster))
            {
                activeMonsterTime = monster.time - (monster.time * timeModifier);
                activeResourceTime = monster.time - (monster.time * timeModifier);
            }
        }
    }

    public override void saveConstructionToDictionary()
    {
        if (!Data.Instance.CONSTRUCTIONS.ContainsKey(id + numType))
        {
            Data.Instance.CONSTRUCTIONS.Add(id + numType, new float[] { transform.position.x, transform.position.y, level, getNumActiveResource(), timeLeft, isProducing ? 1 : 0, isPaused ? 1 : 0, numType, activeResourceTime, 1, 0, 0 });
        }
    }
}