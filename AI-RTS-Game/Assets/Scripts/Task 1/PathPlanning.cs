using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathPlanning : MonoBehaviour {

    //struct with required path info
    struct PathInfo
    {
        public Vector2 Start;
        public Vector2 End;
        public List<Vector2> Path;
    }
    List<PathInfo> Paths;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public List<Vector2> GetPath(int pathID)
    {
        return Paths[pathID].Path;
    }
    public bool PathReady(int pathID)
    {
        //case to check if path is done then return result
        // if struct is used for path info should be easy
        return true;
    }
    public int AddPath(Vector2 Start, Vector3 End)
    {
        // add the struct 
        // get the value added at and then return
        return 0;
    }
}
