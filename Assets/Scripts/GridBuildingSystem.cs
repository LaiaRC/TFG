 using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem current;

    public GridLayout gridLayout;
    public Tilemap MainTilemap;
    public Tilemap TempTilemap;
    public TileBase greenTile;
    public TileBase redTile;
    public TileBase whiteTile;
    public TileBase takenTile;
    public TextMeshProUGUI debugText;
   
    private static Dictionary<TileType, TileBase> tileBases = new Dictionary<TileType, TileBase>();
    private Construction temp;
    private Vector3 prevPos;
    private BoundsInt prevArea;
    private Touch touch;

    #region Unity Method

    private void Awake()
    {
        current = this;
        string tilePath = @"Sprites\Palettes\";
        if (!tileBases.ContainsKey(TileType.Empty))
        {
            tileBases.Add(TileType.Empty, null);
        }
        if (!tileBases.ContainsKey(TileType.White))
        {
            tileBases.Add(TileType.White, whiteTile);
        }
        if (!tileBases.ContainsKey(TileType.Green))
        {
            tileBases.Add(TileType.Green, greenTile);
        }
        if (!tileBases.ContainsKey(TileType.Red))
        {
            tileBases.Add(TileType.Red, redTile);
        }
    }

    private void Update()
    {

        if (!temp)
        {
            ClearArea();
            return;
        }
        
        if(Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);
        }

        if (!temp.placed)
        {
            //en el if -> || touch.phase == TouchPhase.Stationary (per fer que vagi on cliqui, pero no si clica buttons)
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                Vector3Int cellPos = gridLayout.LocalToCell(touchPos);

                if (prevPos != cellPos)
                {
                    temp.transform.localPosition = gridLayout.CellToLocalInterpolated(cellPos);
                    prevPos = cellPos;
                    FollowBuilding();
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {

                 temp.place();
                
            }
        }
    }
    #endregion

    #region Tilemap Management

    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }
        return array;
    }

    private static void FillTiles(TileBase[] arr, TileType type)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = tileBases[type];
        }
    }

    private static void SetTilesBlock(BoundsInt area, TileType type, Tilemap tilemap){
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArray = new TileBase[size];
        FillTiles(tileArray, type);
        tilemap.SetTilesBlock(area, tileArray);
    }

    #endregion

    #region Building Placement

    public void InitializeWithBuilding(GameObject building)
    {
        Vector3 position = new Vector3(0, 0, 0);
        
        if(Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            position = new Vector3(touch.position.x, touch.position.y, 0);
        }
        

        temp = Instantiate(building, Camera.main.ScreenToWorldPoint(position), Quaternion.identity).GetComponent<Construction>();

        FollowBuilding();
    }

    /*public void initializeWithObject(GameObject building, Vector3 pos)
    {
        pos.z = 0;
        pos.y -= building.transform.position.y / 2f;
        Vector3Int cellPos = gridLayout.WorldToCell(pos);
        Vector3 position = gridLayout.CellToLocalInterpolated(cellPos);

        temp = Instantiate(building, position, Quaternion.identity).GetComponent<Construction>();
        temp.gameObject.AddComponent<ObjectDrag>();
        FollowBuilding();
    }*/
    private void ClearArea()
    {
        TileBase[] toClear = new TileBase[prevArea.size.x * prevArea.size.y * prevArea.size.z];
        FillTiles(toClear, TileType.Empty);
        TempTilemap.SetTilesBlock(prevArea, toClear);
    }

    private void FollowBuilding()
    {
        ClearArea();

        Vector3Int positionInt = gridLayout.WorldToCell(temp.gameObject.transform.position);

        if(temp.constructionType == 2)
        {
            //Decoration boost
            temp.area.position = new Vector3Int((int)((temp.transform.position.x - temp.area.size.x / 1.8) + 1), (int)((temp.transform.position.y - temp.area.size.y / 5) - 3), (int)temp.transform.position.z);
        }
        else
        {
            //Building 
            temp.area.position = new Vector3Int((int)(temp.transform.position.x - temp.area.size.x/1.8), (int)(temp.transform.position.y - temp.area.size.y/5), (int)temp.transform.position.z);
        }

        BoundsInt buildingArea = temp.area;

        TileBase[] baseArray = GetTilesBlock(buildingArea, MainTilemap);

        int size = baseArray.Length;
        TileBase[] tileArray = new TileBase[size];

        for (int i = 0; i < baseArray.Length; i++)
        {
            if(baseArray[i] == tileBases[TileType.White])
            {
                tileArray[i] = tileBases[TileType.Green];
                
            }
            else
            {
                
                FillTiles(tileArray, TileType.Red);
                break;
            }
        }

        TempTilemap.SetTilesBlock(buildingArea, tileArray);
        prevArea = buildingArea;
    }

    public bool canTakeArea(BoundsInt area)
    {
        TileBase[] baseArray = GetTilesBlock(area, MainTilemap);
        foreach (var b in baseArray)
        {
            if (b != tileBases[TileType.White])
            {
                return false;
            }
        }

        return true;
    }

    public void takeArea(BoundsInt area)
    {
        SetTilesBlock(area, TileType.Empty, TempTilemap);
        SetTilesBlock(area, TileType.Green, MainTilemap);
    }
    #endregion

}

public enum TileType
{
    Empty,
    White,
    Green,
    Red
}


