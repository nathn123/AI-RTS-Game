using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


public class TaskPlanning : MonoBehaviour {

    List<Task> Tasks;
    Process mFF;
    public struct Task
    {
        public List<Villager.Items> ItemsRequired;
        public List<Villager.Skills> SkillsRequired; // .count = villagers required
        public Villager.Actions TasktoBeCompleted;
        public Vector2 Goal;
    }
	// Use this for initialization
	void Start () {

        mFF.StartInfo.FileName = Application.dataPath + "metricFF/run.bat";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public Task GetTask(int taskID)
    {
        return Tasks[taskID];
    }
    public bool TaskReady(int taskID)
    {
        //case to check if path is done then return result
        // if struct is used for path info should be easy
        return true;
    }
    public int RequestTask(Task newTask)
    {
        // add the struct 
        // get the value added at and then return
        return 0;
    }
    void RunPlan()
    {
        // find metricFF file path / execute .batfile
    }
    bool InterpretPlan()
    {
        return true;
    }
    bool LoadDomain()
    {
        return true;
    }
    bool CreateProblem(int taskID)
    {
        return true;
    }
}
