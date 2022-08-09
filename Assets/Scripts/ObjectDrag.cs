using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 startPos;
    private int deltaX, deltaY;
    
    // Start is called before the first frame update
    void Start()
    {
        startPos = Input.mousePosition;
        startPos = Camera.main.ScreenToWorldPoint(startPos);

        deltaX = (int)(startPos.x - transform.position.x);
        deltaY = (int)(startPos.y - transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        /*Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 pos = new Vector3((int)(mousePos.x - deltaX),(int)( mousePos.y - deltaY));

        Vector3Int cellPos = GridBuildingSystem.current.gridLayout.WorldToCell(pos);
        transform.position = new Vector3(Mathf.RoundToInt(GridBuildingSystem.current.gridLayout.CellToLocalInterpolated(cellPos).x), Mathf.RoundToInt(GridBuildingSystem.current.gridLayout.CellToLocalInterpolated(cellPos).y), Mathf.RoundToInt(GridBuildingSystem.current.gridLayout.CellToLocalInterpolated(cellPos).z));
    */
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            gameObject.GetComponent<Building>().checkPlacement();
            Destroy(this);
        }
    }
}
