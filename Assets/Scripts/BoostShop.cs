using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostShop : MonoBehaviour
{
    public GameObject canvas;
    public List<GameObject> boostItems;
    public List<GameObject> boostMonsterItems;
    public Sprite questionMark;

    public Dictionary<string, Requirement> BOOSTS = new Dictionary<string, Requirement>();

    #region BOOSTS SHOP KEYS
    public static string CRYPT = "crypt";
    public static string MAGIC_WORKSHOP = "magicWorkshop";
    public static string DEEP_FOREST = "deepForest";
    public static string HELLFIRE = "hellfire";
    public static string GRAVEYARD = "graveyard";
    public static string FOREST = "forest";
    public static string VEGETABLE_PATCH = "vegetablePatch";
    public static string SWAMP = "swamp";
    public static string WELL = "well";
    public static string ABANDONED_HOSPITAL = "abandonedHospital";
    public static string CRYPT2 = "crypt2";
    #endregion

    private int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        canvas.SetActive(false);

        //Fill BOOST dictionary
        BOOSTS.Add(CRYPT, new Requirement(Data.LOLLIPOP, 1));
        BOOSTS.Add(MAGIC_WORKSHOP, new Requirement(Data.RING, 50));
        BOOSTS.Add(DEEP_FOREST, new Requirement(Data.BEER, 50));
        BOOSTS.Add(HELLFIRE, new Requirement(Data.SHIELD, 50));

        BOOSTS.Add(VEGETABLE_PATCH, new Requirement(Data.LOLLIPOP, 50));
        BOOSTS.Add(SWAMP, new Requirement(Data.RING, 50));
        BOOSTS.Add(WELL, new Requirement(Data.BEER, 50));
        BOOSTS.Add(GRAVEYARD, new Requirement(Data.SWORD, 50));
        BOOSTS.Add(FOREST, new Requirement(Data.SHIELD, 50));        
        BOOSTS.Add(ABANDONED_HOSPITAL, new Requirement(Data.STICK, 50));
        BOOSTS.Add(CRYPT2, new Requirement(Data.GEM, 50));

        BOOSTS.Add(Data.JACK_LANTERN, new Requirement(Data.LOLLIPOP, 5));
        BOOSTS.Add(Data.BAT, new Requirement(Data.LOLLIPOP, 20));
        BOOSTS.Add(Data.GOBLIN, new Requirement(Data.RING, 15));
        BOOSTS.Add(Data.GHOST, new Requirement(Data.RING, 40));
        BOOSTS.Add(Data.CLOWN, new Requirement(Data.BEER, 30));
        BOOSTS.Add(Data.ZOMBIE, new Requirement(Data.SWORD, 50));
        BOOSTS.Add(Data.VAMPIRE, new Requirement(Data.SHIELD, 50));
        BOOSTS.Add(Data.WITCH, new Requirement(Data.STICK, 50));
        BOOSTS.Add(Data.REAPER, new Requirement(Data.GEM, 50));

        BOOSTS.Add(Data.MERCHANT_BOOST, new Requirement(Data.LOLLIPOP, 10));
        BOOSTS.Add(Data.OFFLINE_MAXTIME_BOOST, new Requirement(Data.LOLLIPOP, 25));
        BOOSTS.Add(Data.OFFLINE_PRODUCTIVITY_BOOST, new Requirement(Data.RING, 50));
        BOOSTS.Add(Data.SCARES_BOOST, new Requirement(Data.BEER, 40));
        BOOSTS.Add(Data.DROPS_BOOST, new Requirement(Data.SWORD, 50));

        foreach (KeyValuePair<string, Requirement> boost in BOOSTS)
        {
            for (int i = 0; i < boostItems.Count; i++)
            {
                if (boostItems[i].GetComponent<BoostItem>().id.Equals(boost.Key))
                {
                    boostItems[i].GetComponent<BoostItem>().setRequirement(boost.Value);
                    boostItems[i].GetComponent<BoostItem>().isPermanent = true; //All boosts from this shop are permanent
                }
            }
        }

        setMonsterTabs();        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void showShop()
    {
        if (!GameManager.Instance.isOnCanvas && !GameManager.Instance.isDialogOpen)
        {
            canvas.SetActive(true);
            GameManager.Instance.isOnCanvas = true;
        }
    }

    public void showShopImmediate()
    {
        canvas.SetActive(true);
        GameManager.Instance.isOnCanvas = true;
    }

    public void hideShop()
    {
        canvas.SetActive(false);
        Invoke("isOnCanvasOff", 0.1f);
    }

    void isOnCanvasOff()
    {
        GameManager.Instance.isOnCanvas = false;
    }

    public void updateMonsterPanels()
    {
        for (int i = 0; i < boostMonsterItems.Count; i++)
        {
            for (int j = 0; j < GameManager.Instance.unlockedMonsters.Count; j++)
            {
                if (boostMonsterItems[i].GetComponent<BoostItem>().id.Equals(GameManager.Instance.unlockedMonsters[j]))
                {
                    //monster is unlocked
                    boostMonsterItems[i].GetComponent<BoostItem>().titleText.SetText(boostMonsterItems[i].GetComponent<BoostItem>().title);
                    boostMonsterItems[i].GetComponent<BoostItem>().descriptionText.SetText(boostMonsterItems[i].GetComponent<BoostItem>().description);
                    boostMonsterItems[i].GetComponent<BoostItem>().iconImage.sprite = boostMonsterItems[i].GetComponent<BoostItem>().icon;
                    boostMonsterItems[i].GetComponent<BoostItem>().iconImage.color = new Color(1,1,1,1);
                    boostMonsterItems[i].GetComponent<BoostItem>().requirementGroup.SetActive(true);
                }
            }

            //Check if it's hiden
            if (boostMonsterItems[i].GetComponent<BoostItem>().id.Equals(GameManager.Instance.hidenMonster))
            {
                boostMonsterItems[i].GetComponent<BoostItem>().titleText.SetText(boostMonsterItems[i].GetComponent<BoostItem>().title);
                boostMonsterItems[i].GetComponent<BoostItem>().descriptionText.SetText(boostMonsterItems[i].GetComponent<BoostItem>().description);
                boostMonsterItems[i].GetComponent<BoostItem>().iconImage.sprite = boostMonsterItems[i].GetComponent<BoostItem>().icon;
                boostMonsterItems[i].GetComponent<BoostItem>().iconImage.color = new Color(0,0,0,1);
                boostMonsterItems[i].GetComponent<BoostItem>().requirementGroup.SetActive(true);
            }
        }
    }

    public void setMonsterTabs()
    {
        if (GameManager.Instance.hidenMonster=="")
        {
            GameManager.Instance.hidenMonster = "jackOLantern";
        }
        //Put monsters to unknown
        for (int i = 0; i < boostMonsterItems.Count; i++)
        {
            boostMonsterItems[i].GetComponent<BoostItem>().iconImage.sprite = questionMark;
            boostMonsterItems[i].GetComponent<BoostItem>().titleText.SetText("Unkown");
            boostMonsterItems[i].GetComponent<BoostItem>().descriptionText.SetText("Unlock previous monsters to discover this new creature");
            boostMonsterItems[i].GetComponent<BoostItem>().requirementGroup.SetActive(false);
            boostMonsterItems[i].GetComponent<BoostItem>().buyButton.SetActive(false);
        }

        foreach (KeyValuePair<string, MonsterInfo> monster in Data.Instance.MONSTERS)
        {
            for (int i = 0; i < boostMonsterItems.Count; i++)
            {
                if (boostMonsterItems[i].GetComponent<BoostItem>().id.Equals(monster.Key))
                {
                    if (monster.Value.isUnlocked)
                    {
                        boostMonsterItems[i].GetComponent<BoostItem>().iconImage.sprite = boostMonsterItems[i].GetComponent<BoostItem>().icon;
                        boostMonsterItems[i].GetComponent<BoostItem>().iconImage.color = new Color(1,1,1,1);
                        boostMonsterItems[i].GetComponent<BoostItem>().titleText.SetText(boostMonsterItems[i].GetComponent<BoostItem>().title);
                        boostMonsterItems[i].GetComponent<BoostItem>().descriptionText.SetText(boostMonsterItems[i].GetComponent<BoostItem>().description);
                        boostMonsterItems[i].GetComponent<BoostItem>().requirementGroup.SetActive(true);
                        boostMonsterItems[i].GetComponent<BoostItem>().buyButton.SetActive(false);
                    }
                    else
                    {
                        if (monster.Key.Equals(GameManager.Instance.hidenMonster))
                        {
                            boostMonsterItems[i].GetComponent<BoostItem>().iconImage.sprite = boostMonsterItems[i].GetComponent<BoostItem>().icon;
                            boostMonsterItems[i].GetComponent<BoostItem>().iconImage.color = new Color(0, 0, 0, 1);
                            boostMonsterItems[i].GetComponent<BoostItem>().titleText.SetText(boostMonsterItems[i].GetComponent<BoostItem>().title);
                            boostMonsterItems[i].GetComponent<BoostItem>().descriptionText.SetText(boostMonsterItems[i].GetComponent<BoostItem>().description);
                            boostMonsterItems[i].GetComponent<BoostItem>().requirementGroup.SetActive(true);
                            boostMonsterItems[i].GetComponent<BoostItem>().buyButton.SetActive(true);
                        }
                        else
                        {
                            boostMonsterItems[i].GetComponent<BoostItem>().iconImage.sprite = questionMark;
                            boostMonsterItems[i].GetComponent<BoostItem>().iconImage.color = new Color(1, 1, 1, 1);
                            boostMonsterItems[i].GetComponent<BoostItem>().titleText.SetText("Unkown");
                            boostMonsterItems[i].GetComponent<BoostItem>().descriptionText.SetText("Unlock previous monsters to discover this new creature");
                            boostMonsterItems[i].GetComponent<BoostItem>().requirementGroup.SetActive(false);
                            boostMonsterItems[i].GetComponent<BoostItem>().buyButton.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    public void checkBoosts()
    {
        //check if boost has already been applied
        foreach (KeyValuePair<string, int> boost in Data.Instance.BOOSTS)
        {
            for (int i = 0; i < boostItems.Count; i++)
            {
                if (boost.Key.Equals(boostItems[i].GetComponent<BoostItem>().id))
                {
                    //The boost is applied (it is only added in the dictionary if applied)
                    boostItems[i].GetComponent<BoostItem>().setToOwned();
                }
            }
        }
        setMonsterTabs();
    }
}
