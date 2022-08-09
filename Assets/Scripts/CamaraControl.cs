using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraControl : MonoBehaviour
{
    Vector3 touchStart;
    Vector3 touchStartZoom;
    public float zoomOutMin = 7;
    public float zoomOutMax = 22;
    public float zoomFactor = 0.01f;
    public float zoomThreshold = 0.5f;
    public float maxX = 20;
    public float maxY = 20;

    private Vector3 initialPos;
    private bool isFirstTouch = false;
    private bool isFirstTouchZoom = false;
    private bool isTwoFingered = false;

    private void Awake()
    {
        initialPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!GameManager.Instance.isOnCanvas && !GameManager.Instance.draggingFromShop && !GameManager.Instance.draggingItemShop && !GameManager.Instance.isDialogOpen)
        {
            if(Input.touchCount <= 0)
            {
                isFirstTouch = true;
                isFirstTouchZoom = true;
                isTwoFingered = false;
            }
            if (Input.touchCount > 0 && Input.touchCount < 2)
            {
                Touch touch = Input.GetTouch(0);
                if (!isTwoFingered)
                {
                    if (isFirstTouch)
                    {
                        touchStart = Camera.main.ScreenToWorldPoint(touch.position);
                        isFirstTouch = false;
                    }


                    Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(touch.position);
                    Camera.main.transform.position += direction;
                }
            }
            if (Input.touchCount == 2)
            {
                isTwoFingered = true;
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);
                if ((Mathf.Abs(touchZero.deltaPosition.x) > zoomThreshold && Mathf.Abs(touchOne.deltaPosition.x) > zoomThreshold) || (Mathf.Abs(touchZero.deltaPosition.y) > zoomThreshold && Mathf.Abs(touchOne.deltaPosition.y) > zoomThreshold)) {
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                    float difference = currentMagnitude - prevMagnitude;

                    Vector3 zero = Camera.main.ScreenToWorldPoint(touchZero.position);
                    Vector3 one = Camera.main.ScreenToWorldPoint(touchOne.position);

                    if (isFirstTouchZoom)
                    {
                        touchStartZoom = Camera.main.ScreenToWorldPoint(new Vector3((touchZero.position.x + touchOne.position.x) / 2, (touchZero.position.y + touchOne.position.y) / 2, transform.position.z));
                        isFirstTouchZoom = false;
                    }

                    if (GameManager.Instance != null && GameManager.Instance.dragging)
                    {
                        Vector3 direction = touchStartZoom - Camera.main.ScreenToWorldPoint(new Vector3((touchZero.position.x + touchOne.position.x) / 2, (touchZero.position.y + touchOne.position.y) / 2, transform.position.z));
                        Camera.main.transform.position = Vector3.Lerp(transform.position, transform.position + direction, 0.075f);
                    }
                    zoom(difference * zoomFactor);
                }
            }
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -maxX, maxX), Mathf.Clamp(transform.position.y, -maxY, maxY), Mathf.Clamp(transform.position.z, -10, -10));
        }
    }
    void zoom(float increment)
    {
        
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(maxX * 2, maxY * 2));
    }
}