using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public RectTransform healthBarPanel;
    public Slider healthBar;
    public Transform target;
    public RectTransform targetCanvas;
    public Image fill;
    public Image background;

    public void Start()
    {
        healthBar = GetComponent<Slider>();
    }

    public void Update()
    {
        if (target != null && targetCanvas != null)
        {
            //Fem que la healthBar segueixi el monstre
            RepositionHealthBar();
        }
    }

    public void setTarget(Transform target)
    {
        this.target = target;
    }

    public void setMaxValue(int value)
    {
        healthBar.maxValue = value;
    }

    public void setMinValue(int value)
    {
        healthBar.minValue = value;
    }

    public void setValue(int value)
    {
        healthBar.value = value;
    }

    private void RepositionHealthBar()
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(target.position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * targetCanvas.sizeDelta.x) - (targetCanvas.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * targetCanvas.sizeDelta.y) - (targetCanvas.sizeDelta.y * 0.5f)) + 20f);
        //now you can set the position of the ui element
        healthBarPanel.anchoredPosition = WorldObject_ScreenPosition;
    }

    public void setColor(Color fillColor,Color backgroundColor)
    {
        fill.color = fillColor;
        background.color = backgroundColor;
    }
}
