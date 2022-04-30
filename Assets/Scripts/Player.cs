using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject shopUI;
    public bool isOnCanvas;
    public ShopManager shop;
    public float velocity; 

    private Camera cam;

    void Start()
    {
        shopUI.SetActive(false);
        cam = Camera.main;
    }

    void Update()
    {
        #region DETECTAR OBJECTE CLICAT
        //Detectar quin objecte cliquem
        if (Input.GetMouseButtonDown(0) && !isOnCanvas)
        {
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
        }
        #endregion

        #region CAMERA CONTROLS
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
        cam.transform.Translate(new Vector3(xAxis, yAxis));



        #endregion
    }

    public void pointerEnter()
    {
        isOnCanvas = true;
    }

    public void pointerExit()
    {
        isOnCanvas = false;
    }
}
