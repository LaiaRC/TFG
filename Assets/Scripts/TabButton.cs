using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerClickHandler
{
    public TabGroup tabGroup;
    [NonSerialized] public Image background;

    private void Awake()
    {
        background = GetComponent<Image>();
        tabGroup.subscribe(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.onTabSelected(this);
    }
}
