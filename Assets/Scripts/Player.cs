using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject shopUI;
    public float velocity;
    public Button buttonTime;
    public TextMeshProUGUI buttonTimeText;
    public bool dragged = false;

    Vector3 touchPosWorld;

    void Start()
    {
        shopUI.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance.dragging)
        {
            dragged = true;
        }

        //Detectar objecte clicat (mobile)
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);

            RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);
            if (hitInformation.collider != null)
            {                
                //We should have hit something with a 2D Physics collider!
                GameObject touchedObject = hitInformation.transform.gameObject;

                if(touchedObject.GetComponent<Building>() != null && !dragged)
                {
                    //Building touched
                    if (touchedObject.GetComponent<Building>().placed)
                    {
                        touchedObject.GetComponent<Building>().showBuildingInterior();
                    }
                }     
            }
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            dragged = false;
        }
    }

    public void toggleTimeEscale()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 100; //600
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
