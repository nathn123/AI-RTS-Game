using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


public class TaskPlanning : MonoBehaviour {

    List<List<Task>> Plans;
    List<VillageManager.GoalState> GoalsToAcheive;
    Process mFF;
    string problemfileloc, domainfileloc, solutionfileloc;
    public float allowedtime,starttime, currenttime;
    int TempPlanID;
    List<string[]> TempPlanStorage;
    VillageManager.GameState currentGameState;
    public struct Task
    {
        public Villager.Items ItemsRequired;
        public int villager; //ref to the villager value in state list
        public Villager.Actions Action;
        public Vector2 Start;
        public Vector2 Goal;
        public bool complete = false;
    }
	// Use this for initialization
	void Start () {
        problemfileloc = @"problem";
        domainfileloc = @"domain";
        solutionfileloc = @"/scripts/PDDL/Solution.soln";
	}
	
	// Update is called once per frame
	void Update () {
        starttime = Time.time;
        // after each step  we need to do to ensure it runs quickly
        if (TempPlanStorage != null)
            InterpretSolution();
        if (checktime())
            return;
        //check plans that need to be completed
        for (int i = 0; i < GoalsToAcheive.Count;++i)
        {
            //now we create the problem file for the goal
            if (CreateProblem(GoalsToAcheive[i]))
                RunPlan();
            LoadSolution(i);
            if (checktime())
                return;
            InterpretSolution();
            if (checktime())
                return;

        }
	
	}
    bool checktime()
    {
        if (Time.time - starttime > allowedtime)
            return true;
        return false;
    }
    public void GetGameState(VillageManager.GameState State)
    {
        currentGameState = State;
    }
    public List<Task> GetPlan(int planID)
    {
        return Plans[planID];
    }
    public bool TaskReady(int planID)
    {
        //case to check if path is done then return result
        // if struct is used for path info should be easy
        foreach (var task in Plans[planID])
            if (task.complete == false)
                return false;
        return true;
    }
    public int RequestTask(Task newTask)
    {
        // add the struct 
        // get the value added at and then return
        return 0;
    }
    public void InterpretSolution()
    {

        // at the end empty it
        TempPlanStorage = null;
    }
    void RunPlan()
    {
        // find metricFF file path / execute .batfile
        mFF.StartInfo.FileName = Application.dataPath + @"/scripts/PDDL/metric-ff.exe";
        mFF.StartInfo.Arguments = string.Format("-o {0}.pddl -f {1}.pddl", domainfileloc, problemfileloc);
        mFF.StartInfo.CreateNoWindow = true;
        mFF.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        // run the process, and wait until it has closed
        mFF.Start();
        mFF.WaitForExit();
    }
    bool LoadSolution(int ID)
    {
        if (!File.Exists(solutionfileloc))
            return false;
        var solution = File.ReadAllLines(solutionfileloc).Where(s => s.Contains(":")).ToArray();
        if (checktime())
            return false;
        List<string[]> croppedSolution = new List<string[]>();
        foreach(var action in solution)
        {
            //now we parse based upon the action
            // once the action is identified we complete the struct to be returned
            char[] seperatingchars = {' ','('};
            var actionline = action.Split(seperatingchars);
            // remove the extra i.e )
            int i = 0;
            for (i = 0; i < actionline.Count(); ++i)
                if (actionline[i].Contains(')') == true)
                    break;
            actionline[0].Remove(i - 1, (actionline.Count() - (i - 1)));
            croppedSolution.Add(actionline);
        }
        // now we should have a list of all required
        // temp storage and time check ????
        TempPlanID = ID;
        TempPlanStorage = croppedSolution;
        InterpretSolution();
        return true;
    }
    bool LoadDomain()
    {
        
        return true;
    }
    bool CreateProblem(VillageManager.GoalState Goal)
    {
        // now we need to encode the problemfile
        var writer = File.CreateText(problemfileloc);
        writer.WriteLine("(define(problem prob)");
        writer.WriteLine("(:domain dom)");
        writer.WriteLine("(:objects");
        //here we add in all the objects
        //we need access to the
        for()
        {

        }
        writer.Write(")");
        writer.WriteLine("(:init ");
        //here we loop through the initial state
        for()
        {
        }
        writer.Write(")");
        writer.WriteLine("(:goal ");
        //here we loop through the goal state
        for()
        {

        }
        writer.Write(")");
        writer.Write(")");
        return true;
    }
}
