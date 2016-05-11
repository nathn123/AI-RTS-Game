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
    List<Types> DomainTypes;
    public struct Task
    {
        public List<Villager.Items> ItemsRequired; // ref to the items helb by villagers in order of list below
        public List<Villager> villager; //ref to the villager or villagers performing the task
        public Villager.Actions Action;
        public Vector2 Start;
        public Vector2 Goal;
        public bool complete;
    }
    public struct Actions
    {
        public string ActionName; // name of action i.e build move sell in pddl
        public Villager.Actions ActionType; // action as denoted in code
        // setup villager params and building params numerical order ?????????? or
        public List<int> PersonParams; // which parameters in the action denote a person
        public List<int> LocationParams; // which parameters in the action denote a location
        public List<int> ItemParams; // which paramenters in the action denote an item
        //public Actions()
        //{
        //    PersonParams = new List<int>();
        //    LocationParams = new List<int>();
        //    ItemParams = new List<int>();
        //    ActionType = Villager.Actions.Build;
        //    ActionName = "-1";
        //}
    }
    public struct Types
    {
        public string PDDLType;
        public bool person;
        public bool location ;
        public bool item;
    }
    List<Actions> DomainActions;
	// Use this for initialization
	void Start () {
        problemfileloc =Application.dataPath+ @"/scripts/PDDL/problem.pddl";
        domainfileloc = Application.dataPath + @"/scripts/PDDL/domain.pddl";
        solutionfileloc = Application.dataPath + @"/scripts/PDDL/Solution.soln";
        GoalsToAcheive = new List<VillageManager.GoalState>();
        TempPlanStorage = new List<string[]>();
        Plans = new List<List<Task>>();
        DomainTypes = new List<Types>();
        DomainActions = new List<Actions>();
        LoadDomain();
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
    Villager FindVillager(string name)
    {
        foreach (var vill in UsableVillagers)
            if (vill.Key == name)
                return vill.Value;
        return null;
    }
    Vector2 FindBuilding(string name)
    {
        
        string prefix = name.Split('_')[0];
        string suffix = name.Split('_')[1];
        int num = int.Parse(suffix);
        if (prefix == "building")
            return currentGameState.OwnedLocations[num].Position;
        else if (prefix == "tree")
            return currentGameState.TreeLocations[num];

        return new Vector2(float.MaxValue,float.MaxValue);
    }
    Villager.Items FindItems(string name)
    {
        Villager.Items returnval = Villager.Items.Axe;
    if(name == "Stone")
        returnval = Villager.Items.Stone;
	else if(name == "Wood")
        returnval = Villager.Items.Wood;
	else if(name == "Iron")
        returnval = Villager.Items.Iron;
	else if(name == "Timber")
        returnval = Villager.Items.Timber;
	else if(name == "Ore")
        returnval = Villager.Items.Ore;
	else if(name == "Coal")
        returnval = Villager.Items.Coal;
	else if(name == "Money")
        returnval = Villager.Items.Money;
	else if(name == "Goods")
        returnval = Villager.Items.Goods;
	else if(name == "Axe")
        returnval = Villager.Items.Axe;
	else if(name == "Cart")
        returnval = Villager.Items.Cart;
	else if(name == "Rifle")
        returnval = Villager.Items.Rifle;

    return returnval;
    }
    public void InterpretSolution()
    {
        // here we interpret the actions and generate the tasks 
        for (int i = 0; i < TempPlanStorage.Count;i++ ) // each string array is 1 action N params
        {
            Task newTask = new Task();
            for (int j = 0; j < DomainActions.Count; j++)// if we first match to an action, we can limit the number of required searchs, i.e only search for buildings
                if (TempPlanStorage[i][0] == DomainActions[j].ActionName)
                {
                    newTask.Action = DomainActions[j].ActionType;
                    //1st we get the villagers involved
                    foreach (int paramloc in DomainActions[j].PersonParams)
                        newTask.villager.Add(FindVillager(TempPlanStorage[i][paramloc]));
                    foreach (int paramloc in DomainActions[j].ItemParams)
                        newTask.ItemsRequired.Add(FindItems(TempPlanStorage[i][paramloc]));
                    if(DomainActions[j].LocationParams.Count > 1)
                    {
                        // is move too action
                        newTask.Start = FindBuilding(TempPlanStorage[i][DomainActions[j].LocationParams[0]]);
                        newTask.Goal = FindBuilding(TempPlanStorage[i][DomainActions[j].LocationParams[1]]);
                        // otherwise goal is primary location
                    }
                    else
                        newTask.Goal = FindBuilding(TempPlanStorage[i][DomainActions[j].LocationParams[0]]);

                }

        }
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
        if (!File.Exists(domainfileloc))
            return false;
        //load types
        var typeActions = File.ReadAllLines(domainfileloc).Where(s => s.Contains(" - ")).ToArray();
        UnityEngine.Debug.Log(typeActions.Count().ToString());
        for(int i = 0; i < typeActions.Count(); i++)
        {
            Types newType = new Types();
            if (typeActions[i].Contains("person"))
            {
                newType.PDDLType = typeActions[i].Split('-')[0];
                newType.person = true;
            }
            else if (typeActions[i].Contains("location"))
            {
                newType.PDDLType = typeActions[i].Split('-')[0];
                newType.location = true;
            }
            else if (typeActions[i].Contains("item"))
            {
                newType.PDDLType = typeActions[i].Split('-')[0];
                newType.item = true;
            }
            else
                continue;
            DomainTypes.Add(newType);
        }
        var domainActions = File.ReadAllLines(domainfileloc).Where(s => s.Contains("(:action")).ToArray();
        var domainParams = File.ReadAllLines(domainfileloc).Where(s => s.Contains(":parameters")).ToArray();
        for (int i = 0; i < domainActions.Count(); i++)
        {
            Actions newAction = new Actions();
            var test = Villager.Actions.Cut_Tree.ToString();
            newAction.PersonParams = new List<int>();
            newAction.LocationParams = new List<int>();
            newAction.ItemParams = new List<int>();
            newAction.ActionName = domainActions[i].Split(' ')[1];
            if (Villager.Actions.Build.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Build;
            else if (Villager.Actions.Buy_Sell.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Buy_Sell;
            else if (Villager.Actions.Combat.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Combat;
            else if (Villager.Actions.Cut_Tree.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Cut_Tree;
            else if (Villager.Actions.Educate.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Educate;
            else if (Villager.Actions.Educate_Barracks.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Educate_Barracks;
            else if (Villager.Actions.Family.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Family;
            else if (Villager.Actions.Family_House.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Family_House;
            else if (Villager.Actions.Make_Tool.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Make_Tool;
            else if (Villager.Actions.Mine.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Mine;
            else if (Villager.Actions.Pickup.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Pickup;
            else if (Villager.Actions.Putdown.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Putdown;
            else if (Villager.Actions.Quarry.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Quarry;
            else if (Villager.Actions.Saw_Wood.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Saw_Wood;
            else if (Villager.Actions.Smelt.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Smelt;
            else if (Villager.Actions.Store.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Store;
            else if (Villager.Actions.Train.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Train;
            else if (Villager.Actions.Walk.ToString().Contains(newAction.ActionName))
                newAction.ActionType = Villager.Actions.Walk;

            var paramline = domainParams[i].Split('?');
            for (int j = 0; j < paramline.Count();j++)
            {
                if (paramline[j].Contains("person"))
                    newAction.PersonParams.Add(j);
                else if (paramline[j].Contains("location"))
                    newAction.LocationParams.Add(j);
                else if (paramline[j].Contains("item"))
                    newAction.ItemParams.Add(j);
            }
            DomainActions.Add(newAction);
        }
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
