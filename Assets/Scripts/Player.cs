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

    Vector3 touchPosWorld;
    TouchPhase touchPhase = TouchPhase.Ended;

    void Start()
    {
        shopUI.SetActive(false);
    }

    void Update()
    {
        //Detectar objecte clicat (mobile)
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == touchPhase)
        {
            touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);

            RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);
            if (hitInformation.collider != null)
            {                
                //We should have hit something with a 2D Physics collider!
                GameObject touchedObject = hitInformation.transform.gameObject;

                if(touchedObject.GetComponent<Building>() != null)
                {
                    //Building touched
                    if (touchedObject.GetComponent<Building>().placed)
                    {
                        touchedObject.GetComponent<Building>().showBuildingInterior();
                    }
                }     
            }
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
