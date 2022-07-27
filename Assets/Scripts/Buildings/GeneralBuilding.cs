using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class GeneralBuilding : Building
{    
    public void Awake()
    {
        //Set all UI variables through code
        upgradeText1 = canvasInterior.transform.Find("UpgradeGroup").transform.Find("UpgradeImage").transform.Find("Requirement1Text").GetComponent<TextMeshProUGUI>();
        upgradeText2 = canvasInterior.transform.Find("UpgradeGroup").transform.Find("UpgradeImage").transform.Find("Requirement2Text").GetComponent<TextMeshProUGUI>();
        upgradeIcon1 = canvasInterior.transform.Find("UpgradeGroup").transform.Find("UpgradeImage").transform.Find("resourceIcon1").GetComponent<Image>();
        upgradeIcon2 = canvasInterior.transform.Find("UpgradeGroup").transform.Find("UpgradeImage").transform.Find("resourceIcon2").GetComponent<Image>();
        maxText = canvasInterior.transform.Find("UpgradeGroup").transform.Find("UpgradeImage").transform.Find("FullyUpgraded").GetComponent<TextMeshProUGUI>();
        upgradeButton = canvasInterior.transform.Find("UpgradeGroup").transform.Find("UpgradeImage").transform.Find("upgradeButton").GetComponent<Button>();

        resourceTimeText = canvasInterior.transform.Find("TimeGroup").transform.Find("activeResourceImage").transform.Find("timeText").GetComponent<TextMeshProUGUI>();
        activeResourceIcon = canvasInterior.transform.Find("TimeGroup").transform.Find("activeResourceImage").transform.Find("activeResourceIcon").GetComponent<Image>();
        timeBar = canvasInterior.transform.Find("TimeGroup").transform.Find("activeResourceImage").transform.Find("TimeBar").GetComponent<Slider>();

        playButton = canvasInterior.transform.Find("TimeGroup").transform.Find("TimeResourceImage").transform.Find("playButton").GetComponent<Button>();
        pauseButton = canvasInterior.transform.Find("TimeGroup").transform.Find("TimeResourceImage").transform.Find("pauseButton").GetComponent<Button>();
        requirementIcon = canvasInterior.transform.Find("TimeGroup").transform.Find("TimeResourceImage").transform.Find("RequirementIcon").GetComponent<Image>();
        requirementIcon1 = canvasInterior.transform.Find("TimeGroup").transform.Find("TimeResourceImage").transform.Find("RequirementIcon1").GetComponent<Image>();
        requirementIcon2 = canvasInterior.transform.Find("TimeGroup").transform.Find("TimeResourceImage").transform.Find("RequirementIcon2").GetComponent<Image>();
        requirementText = canvasInterior.transform.Find("TimeGroup").transform.Find("TimeResourceImage").transform.Find("RequirementText").GetComponent<TextMeshProUGUI>();
        requirement1Text = canvasInterior.transform.Find("TimeGroup").transform.Find("TimeResourceImage").transform.Find("Requirement1Text").GetComponent<TextMeshProUGUI>();
        requirement2Text = canvasInterior.transform.Find("TimeGroup").transform.Find("TimeResourceImage").transform.Find("Requirement2Text").GetComponent<TextMeshProUGUI>();

        levelText = canvasInterior.transform.Find("LevelGroup").transform.Find("LevelImage").transform.Find("NumberText").GetComponent<TextMeshProUGUI>();


        level1Background = canvasInterior.transform.Find("level1Background").GetComponent<Image>();
        level2Background = canvasInterior.transform.Find("level2Background").GetComponent<Image>();
        level3Background = canvasInterior.transform.Find("level3Background").GetComponent<Image>();

        level2Grid = transform.Find("Grid").transform.Find("Level2").gameObject;
        level3Grid = transform.Find("Grid").transform.Find("Level3").gameObject;

        resourcesButtons.Add(canvasInterior.transform.Find("resource1Button").gameObject);
        resourcesButtons.Add(canvasInterior.transform.Find("resource2Button").gameObject);
        resourcesButtons.Add(canvasInterior.transform.Find("resource3Button").gameObject);

        resourceButtonsIcons.Add(canvasInterior.transform.Find("resource1Button").transform.Find("Resource1Icon").GetComponent<Image>());
        resourceButtonsIcons.Add(canvasInterior.transform.Find("resource2Button").transform.Find("Resource2Icon").GetComponent<Image>());
        resourceButtonsIcons.Add(canvasInterior.transform.Find("resource3Button").transform.Find("Resource3Icon").GetComponent<Image>());

        level = 1;
        maxLevel = 3;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (level == 1)
        {
            activeResource = initialActiveResource;

            resourcesButtons[0].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            resourcesButtons[1].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
            resourcesButtons[2].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);

            resourceButtonsIcons[0].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            resourceButtonsIcons[1].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
            resourceButtonsIcons[2].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
        }
        if (Data.Instance.RESOURCES.TryGetValue(activeResource, out Resource resource))
        {
            activeResourceTime = resource.time;
        }
        canvasInterior.SetActive(false);
        playButton.gameObject.SetActive(false);
        setCanvasInterior();
        timeBar.minValue = 0;
        timeBar.maxValue = activeResourceTime;

        /*level1Background.gameObject.SetActive(true);
         level2Background.gameObject.SetActive(false);
         level3Background.gameObject.SetActive(false);*/

        //Save building position
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
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
                timeLeft = activeResourceTime - time;

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

                timeBar.value = timeLeft;
                resourceTimeText.SetText("-");
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
                //Update requirements to update building (inventory)
                if (level != maxLevel)
                {
                    if (Data.Instance.INVENTORY.TryGetValue(upgrade_cost[level - 1].list[0].resourceNameKey, out int quantity))
                    {
                        if (quantity > 1000)
                        {
                            string[] aux = (quantity / 1000f).ToString().Split(',');
                            if (aux.Length > 1)
                            {
                                upgradeText1.text = aux[0] + "." + aux[1].ToCharArray()[0] + "k/" + upgrade_cost[level - 1].list[0].quantity;
                            }
                            else
                            {
                                upgradeText1.text = aux[0] + "k/" + upgrade_cost[level - 1].list[0].quantity;
                            }
                        }
                        else
                        {
                            upgradeText1.text = quantity + "/" + upgrade_cost[level - 1].list[0].quantity;
                        }
                    }
                    else
                    {
                        //Set to 0
                        upgradeText1.text = "0/" + upgrade_cost[level - 1].list[0].quantity;
                    }

                    if (upgrade_cost[level - 1].list.Count > 1)
                    {
                        //Set upgrade requirement 2
                        if (Data.Instance.INVENTORY.TryGetValue(upgrade_cost[level - 1].list[1].resourceNameKey, out int quantity2))
                        {
                            upgradeText2.text = quantity2 + "/" + upgrade_cost[level - 1].list[1].quantity;
                        }
                        else
                        {
                            //Set to 0
                            upgradeText2.text = "0/" + upgrade_cost[level - 1].list[1].quantity;
                        }
                    }
                    else
                    {
                        if (upgradeText2.isActiveAndEnabled)
                        {
                            upgradeText2.gameObject.SetActive(false);
                        }
                    }
                }

                //Show resource producing and time to next
                if (Data.Instance.RESOURCES.TryGetValue(activeResource, out Resource aResource))
                {

                    if (isProducing)
                    {
                        int minutes = (int)timeLeft / 60;
                        int secondsLeft = (int)timeLeft - (minutes * 60);
                        if (minutes > 0)
                        {
                            resourceTimeText.SetText(minutes.ToString() + "m " + secondsLeft + "s");
                        }
                        else
                        {
                            resourceTimeText.SetText(timeLeft.ToString("F0") + "s");
                        }
                    }
                    else
                    {
                        resourceTimeText.SetText("-");
                    }
                }

                //Slider config
                if (isProducing)
                {
                    timeBar.value = timeLeft;
                }

                //Update requirements to produce resource (inventory)
                updateUITimeGroup();
            }
            #endregion
        }
    }
}
