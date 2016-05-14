using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathPlanning {

    //struct with required path info
    char[,] Map; // is stored as ref between all that read it to ensure that the latest copy is always avaliable
    public struct PathInfo
    {
        public Vector2 Start;
        public Vector2 End;
        public List<Vector2> Path;
        public bool Complete;
    }

    List<PathInfo> Paths;


	// Use this for initialization
	void Start () {
	
	}
    public void Initialise(ref char[,]AIMAP)
    {
        Map = AIMAP;
    }
	
	// Update is called once per frame
	public void Update () {
	
	}

    public PathInfo GetPath(int pathID)
    {
        var temp = Paths[pathID];
        return temp;
    }
    public bool PathReady(int pathID)
    {
        //case to check if path is done then return result
        // if struct is used for path info should be easy
        return Paths[pathID].Complete;
    }
    public int AddPath(PathInfo newPath)
    {
        // add the struct 
        // get the value added at and then return
        Paths.Add(newPath);
        return Paths.Count-1;
    }
    bool Walkable(char Loc)
    {
        if(Loc == '.' || Loc == 'G')
        {
            // walkable tile NOT GRASS
            return true;
        }
        // EDIT THIS IF NEEDED
        return false; 
                    
    }
}
