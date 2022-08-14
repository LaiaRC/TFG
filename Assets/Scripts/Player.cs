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

    private Vector3 touchPosWorld;

    private void Start()
    {
        shopUI.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance.dragging)
        {
            dragged = true;
        }

        //Detectar objecte clicat (mobile)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);

            RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);
            if (hitInformation.collider != null)
            {
                //We should have hit something with a 2D Physics collider!
                GameObject touchedObject = hitInformation.transform.gameObject;

                if (touchedObject.GetComponent<Building>() != null && !dragged && !GameManager.Instance.isOnCanvas)
                {
                    //Building touched
                    if (touchedObject.GetComponent<Building>().placed)
                    {
                        touchedObject.GetComponent<Building>().showBuildingInterior();
                        if (GameManager.Instance.isOnBuildingMode)
                        {
                            GameManager.Instance.toggleBuildMode();
                            GameManager.Instance.closeShop();
                        }
                    }
                }
                else if (touchedObject.GetComponent<SummoningCircle>() != null && !dragged && !GameManager.Instance.isOnCanvas) //maybe no cal
                {
                    //Summoning circle building touched
                    if (touchedObject.GetComponent<SummoningCircle>().placed)
                    {
                        touchedObject.GetComponent<SummoningCircle>().showBuildingInterior();
                        if (GameManager.Instance.isOnBuildingMode)
                        {
                            GameManager.Instance.toggleBuildMode();
                            GameManager.Instance.closeShop();
                        }
                    }
                }
                else if (touchedObject.GetComponent<BoostShop>() != null && !dragged && !GameManager.Instance.isOnCanvas)
                {
                    touchedObject.GetComponent<BoostShop>().showShop();
                    if (GameManager.Instance.isOnBuildingMode)
                    {
                        GameManager.Instance.toggleBuildMode();
                        GameManager.Instance.closeShop();
                    }
                }
                else if (touchedObject.tag.Equals("portal") && !dragged && !GameManager.Instance.isOnCanvas)
                {
                    GameManager.Instance.showPortalDialog();
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
            buttonTimeText.text = "Time x600";
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