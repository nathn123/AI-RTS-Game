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
    List<KeyValuePair<string,Villager>> UsableVillagers;
    public struct Task
    {
        public Villager.Items ItemsRequired;
        public int villager; //ref to the villager value in state list
        public Villager.Actions Action;
        public Vector2 Start;
        public Vector2 Goal;
        public bool complete;
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
        
        for(int i = 0; i < currentGameState.Villagers.Count; i++)
            if(!currentGameState.Villagers[i].ActionComplete)
            {
                KeyValuePair<string,Villager> prekey = new KeyValuePair<string,Villager>("villager_" + i.ToString(),currentGameState.Villagers[i]);
                UsableVillagers.Add(prekey);
            }
        // all buildings are considered usable
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
    public int RequestTask(VillageManager.GoalState newTask)
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
        //we need access to the current game state to define the objects
        for (int i = 0; i < UsableVillagers.Count;i++)
        {
            writer.WriteLine(UsableVillagers[i].Key+" - person");
        }
       
        for (int i = 0; i < currentGameState.OwnedLocations.Count; i++)
        {
            if(currentGameState.OwnedLocations[i].Type == Building.BuildingType.Barracks)
            {
                writer.WriteLine("building_"+i.ToString() + " - Barracks");
            }
            else if(currentGameState.OwnedLocations[i].Type == Building.BuildingType.Blacksmith)
            {
                writer.WriteLine("building_"+i.ToString() + " - Blacksmith");
            }
            else if(currentGameState.OwnedLocations[i].Type == Building.BuildingType.House)
            {
                writer.WriteLine("building_"+i.ToString() + " - house");
            }
            else if(currentGameState.OwnedLocations[i].Type == Building.BuildingType.Market_Stall)
            {
                writer.WriteLine("building_"+i.ToString() + " - Market_Stall");
            }
            else if(currentGameState.OwnedLocations[i].Type == Building.BuildingType.Mine)
            {
                writer.WriteLine("building_"+i.ToString() + " - Mine");
            }
            else if(currentGameState.OwnedLocations[i].Type == Building.BuildingType.Quarry)
            {
                writer.WriteLine("building_"+i.ToString() + " - Quarry");
            }
            else if(currentGameState.OwnedLocations[i].Type == Building.BuildingType.Sawmill)
            {
                writer.WriteLine("building_"+i.ToString() + " - Sawmill");
            }
            else if(currentGameState.OwnedLocations[i].Type == Building.BuildingType.School)
            {
                writer.WriteLine("building_"+i.ToString() + " - School");
            }
            else if(currentGameState.OwnedLocations[i].Type == Building.BuildingType.Smelter)
            {
                writer.WriteLine("building_"+i.ToString() + " - Smelter");
            }
            else if(currentGameState.OwnedLocations[i].Type == Building.BuildingType.Storage)
            {
                writer.WriteLine("building_"+i.ToString() + " - Storage");
            }
            else if(currentGameState.OwnedLocations[i].Type == Building.BuildingType.Turf_Hut)
            {
                writer.WriteLine("building_"+i.ToString() + " - turfhut");
            }

        }
        writer.Write(")");
        writer.WriteLine("(:init ");
        //here we loop through the initial state and set up the predicates such as which items are where
        for (int i = 0; i < UsableVillagers.Count;i++)
        {
            string skill = "";
            string item = "";
            writer.WriteLine(UsableVillagers[i].Key+" - person");
            if (UsableVillagers[i].Value.Skill == Villager.Skills.Blacksmith)
                skill = "BlacksmithS";
            else if (UsableVillagers[i].Value.Skill == Villager.Skills.Carpenter)
                skill = "Carpenter";
            else if (UsableVillagers[i].Value.Skill == Villager.Skills.Labourer)
                skill = "Labourer";
            else if (UsableVillagers[i].Value.Skill == Villager.Skills.Lumberjack)
                skill = "Lumberjack";
            else if (UsableVillagers[i].Value.Skill == Villager.Skills.Miner)
                skill = "Miner";
            else if (UsableVillagers[i].Value.Skill == Villager.Skills.Rifleman)
                skill = "Rifleman";
            else if (UsableVillagers[i].Value.Skill == Villager.Skills.Trader)
                skill = "Trader";
            else
                continue;
            writer.WriteLine("(has-"+skill+" "+UsableVillagers[i].Key+")");
            if(UsableVillagers[i].Value.Inventory == Villager.Items.Stone)
                item = "Stone";
            else if(UsableVillagers[i].Value.Inventory == Villager.Items.Wood)
                item = "Wood";
            else if(UsableVillagers[i].Value.Inventory == Villager.Items.Iron)
                item = "Iron";
            else if(UsableVillagers[i].Value.Inventory == Villager.Items.Timber)
                item = "Timber";
            else if(UsableVillagers[i].Value.Inventory == Villager.Items.Ore)
                item = "Ore";
            else if(UsableVillagers[i].Value.Inventory == Villager.Items.Coal)
                item = "Coal";
            else if(UsableVillagers[i].Value.Inventory == Villager.Items.Money)
                item = "Money";
            else if(UsableVillagers[i].Value.Inventory == Villager.Items.Goods)
                item = "Goods";
            else if(UsableVillagers[i].Value.Inventory == Villager.Items.Axe)
                item = "Axe";
            else if(UsableVillagers[i].Value.Inventory == Villager.Items.Cart)
                item = "Cart";
            else if(UsableVillagers[i].Value.Inventory == Villager.Items.Rifle)
                item = "Rifle";
            else
                continue;

            writer.WriteLine("(has-"+item+" "+UsableVillagers[i].Key+")");
        }
       
        for (int i = 0; i < currentGameState.OwnedLocations.Count; i++)
        {
            foreach(var Item in currentGameState.OwnedLocations[i].Items)
            {
                string item;
                if(Item == Villager.Items.Stone)
                    item = "Stone";
                else if(Item == Villager.Items.Wood)
                    item = "Wood";
                else if(Item == Villager.Items.Iron)
                    item = "Iron";
                else if(Item == Villager.Items.Timber)
                    item = "Timber";
                else if(Item == Villager.Items.Ore)
                    item = "Ore";
                else if(Item == Villager.Items.Coal)
                    item = "Coal";
                else if(Item == Villager.Items.Money)
                    item = "Money";
                else if(Item == Villager.Items.Goods)
                    item = "Goods";
                else if(Item == Villager.Items.Axe)
                    item = "Axe";
                else if(Item == Villager.Items.Cart)
                    item = "Cart";
                else if(Item == Villager.Items.Rifle)
                    item = "Rifle";
                else
                    continue;
                writer.WriteLine("(has-"+item+" "+"building_"+i.ToString()+")");
            }

        }
        writer.Write(")");
        writer.WriteLine("(:goal ");
        //here we loop through the goal state
        writer.WriteLine("(and");
        //for()
        //{

        //}
        writer.Write(")");
        writer.Write(")");
        writer.Write(")");
        return true;
    }
}
