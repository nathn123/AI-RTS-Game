using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

public class MapGen : MonoBehaviour {

	// Use this for initialization
    public GameObject Walkable, Grass, Trees, Swamp, Water, OutOfBounds, Entrance,
                    Barracks, School, Storage, Sawmill, Blacksmith, Turf, Smelter, House,
                    Mine, Quarry, Market;
    int tilesize = 32;
    static char[,] AiMap, PrevMap;
    GameObject[,] GameMap;
    bool Initialised = false;
	public void Start () {
	
	}
	
	// Update is called once per frame
	public void Update () {
        if (Initialised)
            DrawMap(AiMap, GameMap);
	
	}
    public void LoadMap()
    {
       var path =  EditorUtility.OpenFilePanel("Select a Map", @"\Assets\Maps", "map");
       if (path.Length == 0)
           return;
       StreamReader Mapreader = new StreamReader(path);
       Mapreader.ReadLine();
       var htext = Mapreader.ReadLine().Split(' ')[1];
       var wtext = Mapreader.ReadLine().Split(' ')[1];
       Mapreader.ReadLine();
       var height  = Int32.Parse(htext);
       var width = Int32.Parse(wtext);
       Char[] mapline = new char[width];
       Char[,] FinalMap = new char[height,width];
       for (int i = 0; i < height; ++i)
       {
           mapline = Mapreader.ReadLine().ToCharArray();
           for (int j = 0; j < width; ++j)
           {
               FinalMap[i, j] = mapline[j];
           }
       }
       AiMap = FinalMap;
       DrawMap(FinalMap, GameMap = CreateGrid(tilesize, height, width));
    }
    public GameObject[,] CreateGrid(int TileSize, int height, int width)
    {
        // create a grid of sprites based on tilesize
        GameObject[,] mapGrid = new GameObject[height,width];
        for(int i = 0 ; i< height; ++i)
            for(int j = 0; j<width;++j)
            {
                mapGrid[i, j] = new GameObject();
                mapGrid[i, j].AddComponent<SpriteRenderer>();
                mapGrid[i, j].transform.position = new Vector2(i * TileSize, j * TileSize);
            }
        return mapGrid;
    }
    public void DrawMap(Char[,]AiMap,GameObject[,]VisualMap)
    {
        if (!Initialised)
            PrevMap = new char[AiMap.GetLength(0),AiMap.GetLength(1)];
        //will need to be updated to reflect changes i.e building, tree cutting
        int height = 512; // map height
        int width = 512;
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
               if(PrevMap[i,j] != AiMap[i,j])
               {
                   GameObject.DestroyImmediate(VisualMap[i, j]);
                   switch (AiMap[i, j])
                   {
                       case '.':
                           VisualMap[i, j] = GameObject.Instantiate(Walkable, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           // walkable tile NOT GRASS
                           break;
                       case 'G':
                           VisualMap[i, j] = GameObject.Instantiate(Grass, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           // walkable tile IS GRASS
                           break;
                       case '@':
                           VisualMap[i, j] = GameObject.Instantiate(OutOfBounds, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           // OUT OF BOUNDS
                           break;
                       case 'O':
                           VisualMap[i, j] = GameObject.Instantiate(OutOfBounds, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           // OUT OF BOUNDS
                           break;
                       case 'T':
                           VisualMap[i, j] = GameObject.Instantiate(Trees, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           // TREES
                           break;
                       case 'S':
                           VisualMap[i, j] = GameObject.Instantiate(Swamp, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           // SWAMP
                           break;
                       case 'W':
                           VisualMap[i, j] = GameObject.Instantiate(Water, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           // WATER
                           break;
                       case 'E':
                           VisualMap[i, j] = GameObject.Instantiate(Entrance, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           //Building Entrance Universal and Walkable
                           break;
                       case '1':
                           VisualMap[i, j] = GameObject.Instantiate(Barracks, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           // Barracks
                           break;
                       case '2':
                           VisualMap[i, j] = GameObject.Instantiate(School, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           //School
                           break;
                       case '3':
                           VisualMap[i, j] = GameObject.Instantiate(Storage, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           //Storage
                           break;
                       case '4':
                           VisualMap[i, j] = GameObject.Instantiate(Sawmill, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           //sawmill
                           break;
                       case '5':
                           VisualMap[i, j] = GameObject.Instantiate(Blacksmith, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           //blacksmith
                           break;
                       case '6':
                           VisualMap[i, j] = GameObject.Instantiate(Turf, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           //Turf
                           break;
                       case '7':
                           VisualMap[i, j] = GameObject.Instantiate(Smelter, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           //Smelter
                           break;
                       case '8':
                           VisualMap[i, j] = GameObject.Instantiate(House, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           //HOuse
                           break;
                       case '9':
                           VisualMap[i, j] = GameObject.Instantiate(Mine, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           //Mine
                           break;
                       case '0':
                           VisualMap[i, j] = GameObject.Instantiate(Quarry, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           //Quarry
                           break;
                       case '-':
                           VisualMap[i, j] = GameObject.Instantiate(Market, new Vector3(i, j, -1), Quaternion.identity) as GameObject;//sprite goes here
                           //Market
                           break;
                   }
                }
            }
        }
        if (!Initialised)
        {
            Initialised = true;
            PrevMap = AiMap;
        }
        PrevMap = AiMap;
    }

    public static Char[,] GetMap()
    {
        return AiMap;
    }

    public static void UpdateMap(Vector2 Pos, char Val)
    {
        AiMap[(int)Pos.x, (int)Pos.y] = Val;
    }

    public Vector2 GetPosition(Vector2 position)
    {
        //uses the relative pos coordinate of map tile, to the unity scale position
        return GameMap[(int)position.x, (int)position.y].transform.position;
    }


}
