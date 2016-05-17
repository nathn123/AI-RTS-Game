using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VillageGen : MonoBehaviour {
    //min 1 - max 4
    public int NumofVillages;
    public List<Vector2> StartingLocations;
    public List<VillageManager> TaskExecutives;
    public TreeManager TreeMgr;
	// Use this for initialization
	void Start () {
        TaskExecutives = new List<VillageManager>();
        TreeMgr = new TreeManager();
	
	}
    public void StartGame()
    {
        char[,] AiMap = new char[1, 1];
        this.gameObject.GetComponent<MapGen>().GetMap(ref AiMap);
        TreeMgr.GenerateTrees(AiMap);
        for (int i = 0; i < NumofVillages; ++i)
        {
            VillageManager newmanager = new VillageManager();
            newmanager.Initialise(StartingLocations[i], i, VillageManager.AI_Bias.Balanced,TreeMgr, ref AiMap);
            TaskExecutives.Add(newmanager);
        }
    }
	
	// Update is called once per frame
	void Update () {
        foreach (var village in TaskExecutives)
            village.Update();
        TreeMgr.Update();
	
	}
}
