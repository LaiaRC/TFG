using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TabGroup : MonoBehaviour
{
    public Sprite tabIdle1;
    public Sprite tabActive1;
    public Sprite tabIdle2;
    public Sprite tabActive2;

    public List<TabButton> tabButtons = new List<TabButton>();
    public List<GameObject> objectsToSwap = new List<GameObject>();
    public TextMeshProUGUI categoryTitle;
    public RectTransform scrollContent;

    [NonSerialized] public TabButton selectedTab;

    private void Start()
    {
        onTabSelected(tabButtons[0]);
    }

    public void subscribe(TabButton button)
    {
        tabButtons.Add(button);
    }

    private void resetTabs()
    {
        //Harcoded 
        tabButtons[0].background.sprite = tabIdle1;
        tabButtons[1].background.sprite = tabIdle2;
    }

    public void onTabSelected(TabButton button)
    {
        selectedTab = button;
        resetTabs();
        if (button.name.Equals("BuildingTab"))
        {
            button.background.sprite = tabActive1;
            categoryTitle.SetText("Constructions");
            scrollContent.sizeDelta = new Vector2(scrollContent.sizeDelta.x, 3479);
        }
        else if (button.name.Equals("DarkMarket"))
        {
            button.background.sprite = tabActive2;
            categoryTitle.SetText("Dark Market");
            scrollContent.sizeDelta = new Vector2(scrollContent.sizeDelta.x, 1845);
        }

        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
    }
}
