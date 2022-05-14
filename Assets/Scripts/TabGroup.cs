using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TabGroup : MonoBehaviour
{
    public Sprite tabIdle;
    public Sprite tabActive;

    public List<TabButton> tabButtons = new List<TabButton>();
    public List<GameObject> objectsToSwap = new List<GameObject>();

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
        foreach (var button in tabButtons)
        {
            if(selectedTab != null && button == selectedTab)
            {
                continue;
            }
            button.background.sprite = tabIdle;
        }
    }

    public void onTabSelected(TabButton button)
    {
        selectedTab = button;
        resetTabs();
        button.background.sprite = tabActive;

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
