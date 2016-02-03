using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

public class MapGen : MonoBehaviour {

	// Use this for initialization
    public GameObject Walkable,Grass, Trees, Swamp, Water, OutOfBounds;
    char[,] AiMap;
    GameObject[,] GameMap;
	public void Start () {
	
	}
	
	// Update is called once per frame
	public void Update () {
	
	}
    public void LoadMap()
    {
       var path =  EditorUtility.OpenFilePanel("Select a Map", @"\Assets\Maps", "map");
       if (path.Length == 0)
           return;
       StreamReader Mapreader = new StreamReader(path);
       Mapreader.ReadLine();
       var htext = Mapreader.ReadLine().Split(' ')[1];
       print(htext);
       var wtext = Mapreader.ReadLine().Split(' ')[1];
       Mapreader.ReadLine();
       var height  = Int32.Parse(htext);
       var width = Int32.Parse(wtext);
       Char[] mapline = new char[width];
       Char[,] FinalMap = new char[height,width];
       for (int i = 0; i < height; ++i)
       {
           mapline = Mapreader.ReadLine().ToCharArray();
           print(mapline.Length);
           for (int j = 0; j < width; ++j)
           {
               FinalMap[i, j] = mapline[j];
           }
       }
       AiMap = FinalMap;
       DrawMap(FinalMap, GameMap = CreateGrid(32, height, width));
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
        //will need to be updated to reflect changes i.e building, tree cutting
        int height = 512; // map height
        int width = 512;
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                GameObject.Destroy(VisualMap[i, j]);
                switch (AiMap[i, j])
                {
                    case '.':
                        VisualMap[i, j] = GameObject.Instantiate(Walkable,new Vector3(i*0.32f,j*0.32f,0),Quaternion.identity) as GameObject;//sprite goes here
                        // walkable tile NOT GRASS
                        break;
                    case 'G':
                         VisualMap[i, j] = GameObject.Instantiate(Grass,new Vector3(i*0.32f,j*0.32f,0),Quaternion.identity) as GameObject;//sprite goes here
                        // walkable tile IS GRASS
                        break;
                    case '@':
                         VisualMap[i, j] = GameObject.Instantiate(OutOfBounds,new Vector3(i*0.32f,j*0.32f,0),Quaternion.identity) as GameObject;//sprite goes here
                        // OUT OF BOUNDS
                        break;
                    case 'O':
                         VisualMap[i, j] = GameObject.Instantiate(OutOfBounds,new Vector3(i*0.32f,j*0.32f,0),Quaternion.identity) as GameObject;//sprite goes here
                        // OUT OF BOUNDS
                        break;
                    case 'T':
                         VisualMap[i, j] = GameObject.Instantiate(Trees,new Vector3(i*0.32f,j*0.32f,0),Quaternion.identity) as GameObject;//sprite goes here
                        // TREES
                        break;
                    case 'S':
                         VisualMap[i, j] = GameObject.Instantiate(Swamp,new Vector3(i*0.32f,j*0.32f,0),Quaternion.identity) as GameObject;//sprite goes here
                        // SWAMP
                        break;
                    case 'W':
                         VisualMap[i, j] = GameObject.Instantiate(Water,new Vector3(i*0.32f,j*0.32f,0),Quaternion.identity) as GameObject;//sprite goes here
                        // WATER
                        break;
                }
            }
        }
    }

    public void GetMap(ref char[,] map)
    {
        map = AiMap;
    }
}
