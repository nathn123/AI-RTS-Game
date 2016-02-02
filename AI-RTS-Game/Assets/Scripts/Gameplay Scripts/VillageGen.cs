using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VillageGen : MonoBehaviour {
    //min 1 - max 4
    public int NumofVillages;
    public List<Vector2> StartingLocations;
    public List<VillageManager> TaskExecutives;
	// Use this for initialization
	void Start () {
        for(int i=0; i<NumofVillages;++i)
        {
            VillageManager newmanager = new VillageManager();
            newmanager.Initialise(StartingLocations[i],i);
            TaskExecutives.Add(newmanager);
        }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
