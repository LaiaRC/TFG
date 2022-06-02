using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject shopUI;
    public ShopManager shop;
    public float velocity;
    public Button buttonTime;
    public TextMeshProUGUI buttonTimeText;

    void Start()
    {
        shopUI.SetActive(false);
    }

    void Update()
    {
        //Detectar quin objecte cliquem
        if (Input.GetMouseButtonDown(0) && !GameManager.Instance.isOnCanvas)
        {
            #region DETECTAR OBJ CLICAT
            TerrenoEdificable[] terrenos = FindObjectsOfType<TerrenoEdificable>();
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0f);
            if (hit.collider != null)
            {
                if (hit.transform.gameObject.GetComponent<TerrenoEdificable>())
                {
                    TerrenoEdificable terrenoSelected = hit.transform.gameObject.GetComponent<TerrenoEdificable>();
                    if (terrenoSelected.isSelected)
                    {
                        terrenoSelected.isSelected = false;
                        if (!terrenoSelected.hasBuilding)
                        {
                            shopUI.SetActive(false);
                        }                    
                    }
                    else
                    {
                        foreach (TerrenoEdificable terreno in terrenos)
                        {
                                terreno.isSelected = false;
                        }
                        terrenoSelected.isSelected = true;
                        if (!terrenoSelected.hasBuilding)
                        {
                            shopUI.SetActive(true);
                            shop.setTerrenoEdificable(terrenoSelected);
                        }                        
                    }
                }                
            }
            else
            {
                foreach (TerrenoEdificable terreno in terrenos)
                {
                    terreno.isSelected = false;
                }
                shopUI.SetActive(false);
            }
            #endregion
        }
    }

    public void toggleTimeEscale()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 100;
            buttonTimeText.text = "Time x100";
            buttonTime.GetComponent<Image>().color = new Color(0, 1, 0, 1);
        }
        else
        {
            Time.timeScale = 1;
            buttonTimeText.text = "Time x1";
            buttonTime.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }
    
}
