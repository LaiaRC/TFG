using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class testing : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject a;

    private GameObject a2;

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if(a2 != null)
            {
                Vector3 p = new Vector3(Camera.main.ScreenToWorldPoint(t.position).x, Camera.main.ScreenToWorldPoint(t.position).y, 0);
                a2.transform.position = p;
            }
            text.SetText("-> " + t.position + " " + t.phase + " " + t.radius);
        }
    }


    public void ins()
    {
       a2 = Instantiate(a, Vector2.zero, Quaternion.identity);
    }
}
