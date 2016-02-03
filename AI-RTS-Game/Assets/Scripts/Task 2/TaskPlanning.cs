using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TaskPlanning : MonoBehaviour {

    List<Task> Tasks;
    public struct Task
    {
        public List<Villager.Items> ItemsRequired;
        public List<Villager.Skills> SkillsRequired; // .count = villagers required
        public Villager.Actions TasktoBeCompleted;
        public Vector2 Goal;
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public Task GetTask(int pathID)
    {
        return Tasks[pathID];
    }
    public bool TaskReady(int pathID)
    {
        //case to check if path is done then return result
        // if struct is used for path info should be easy
        return true;
    }
    public int RequestTask(VillageManager.GlobalObjectives objective)
    {
        // add the struct 
        // get the value added at and then return
        return 0;
    }
}
