using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


public class TaskPlanning  {

    List<List<Task>> Plans;
    List<VillageManager.GoalState> GoalsToAcheive;
    Process mFF;
    string problemfileloc, domainfileloc, solutionfileloc;
    public float allowedtime,starttime, currenttime;
    int TempPlanID;
    List<string[]> TempPlanStorage;
    //VillageManager.GameState currentGameState;
    List<Types> DomainTypes;
    public struct Task
    {
        public List<Villager.Items> ItemsRequired; // ref to the items held by villagers in order of list below
        public List<Villager> villager; //ref to the villager or villagers performing the task
        public Villager.Actions Action;
        public PathPlanning.PathInfo Path;
        public int PathID;
        public bool complete;

        public void SetPath(PathPlanning.PathInfo newPath)
        {
            Path = newPath;
        }
        public void SetPathID(int newID)
        {
            PathID = newID;
        }
        public void SetComplete(bool comp)
        {
            complete = comp;
        }
    }
    public struct Actions
    {
        public string ActionName; // name of action i.e build move sell in pddl
        public Villager.Actions ActionType; // action as denoted in code
        // setup villager params and building params numerical order ?????????? or
        public List<int> PersonParams; // which parameters in the action denote a person
        public List<int> LocationParams; // which parameters in the action denote a location
        public List<int> ItemParams; // which paramenters in the action denote an item
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
	public void Initialise () {
        problemfileloc =Application.dataPath+ @"/scripts/PDDL/problem.pddl";
        domainfileloc = Application.dataPath + @"/scripts/PDDL/domain.pddl";
        solutionfileloc = Application.dataPath + @"/scripts/PDDL/ffSolution.soln";
        GoalsToAcheive = new List<VillageManager.GoalState>();
        TempPlanStorage = new List<string[]>();
        Plans = new List<List<Task>>();
        DomainTypes = new List<Types>();
        DomainActions = new List<Actions>();
        LoadDomain();
	}
	
	// Update is called once per frame
	public void Update () {
        starttime = Time.time;
        
        // after each step  we need to do to ensure it runs quickly
        //if (TempPlanStorage != null)
        //    InterpretSolution(GoalsToAcheive[TempPlanID]);
        //if (checktime())
        //    return;
        //check plans that need to be completed
        for (int i = 0; i < GoalsToAcheive.Count;++i)
        {
            //now we create the problem file for the goal
            if (CreateProblem(GoalsToAcheive[i]))
                RunPlan();
            LoadSolution(i);
            if (checktime())
                return;
            InterpretSolution(GoalsToAcheive[TempPlanID]);
            if (checktime())
                return;
        }
	
	}
    bool checktime()
    {
        //if (Time.time - starttime > allowedtime)
        //    return true;
        //return false;
        return false;
    }
    //public void GetGameState(VillageManager.GameState State)
    //{
    //    currentGameState = State;
        
    //    // VILLAGERS are pre selected
    //    //for(int i = 0; i < currentGameState.Villagers.Count; i++)
    //    //    if(!currentGameState.Villagers[i].ActionComplete)
    //    //    {
    //    //        KeyValuePair<string,Villager> prekey = new KeyValuePair<string,Villager>("villager_" + i.ToString(),currentGameState.Villagers[i]);
    //    //        UsableVillagers.Add(prekey);
    //    //    }
    //    // all buildings are considered usable
    //}
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
        List<Task> newTaskList = new List<Task>();
        GoalsToAcheive.Add(newTask);
        var intref = GoalsToAcheive.Count - 1;
        Plans.Insert(intref, newTaskList);
        
        // add the struct 
        // get the value added at and then return
        return intref;
    }
    Villager FindVillager(string name,VillageManager.GoalState Goal)
    {
        string prefix = name.Split('_')[0];
        string suffix = name.Split('_')[1];
        int num = int.Parse(suffix);
        return Goal.GameState.Villagers[num];
    }
    Vector2 FindBuilding(string name, VillageManager.GoalState Goal)
    {
        
        string prefix = name.Split('_')[0];
        string suffix = name.Split('_')[1];
        int num = int.Parse(suffix);
        if (prefix == "building")
            return Goal.GameState.OwnedLocations[num].Position;
        else if (prefix == "tree")
            return Goal.GameState.Trees[num].Pos;
        else if (name.Contains("buildingSite"))
            return Goal.Site.Position;

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
    public void InterpretSolution(VillageManager.GoalState Goal)
    {
        // here we interpret the actions and generate the tasks
		List<Task> TotalTasks = new List<Task> ();
        for (int i = 0; i < TempPlanStorage.Count;i++ ) // each string array is 1 action N params
        {
            Task newTask = new Task();
            for (int j = 0; j < DomainActions.Count; j++)// if we first match to an action, we can limit the number of required searchs, i.e only search for buildings
                if (TempPlanStorage[i][0].Equals(DomainActions[j].ActionName, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    newTask.Action = DomainActions[j].ActionType;
                    //1st we get the villagers involved
                    foreach (int paramloc in DomainActions[j].PersonParams)
                        newTask.villager.Add(FindVillager(TempPlanStorage[i][paramloc], Goal));
                    foreach (int paramloc in DomainActions[j].ItemParams)
                        newTask.ItemsRequired.Add(FindItems(TempPlanStorage[i][paramloc]));
                    if(DomainActions[j].LocationParams.Count > 1)
                    {
                        // is move too action
                        newTask.Path.Start = FindBuilding(TempPlanStorage[i][DomainActions[j].LocationParams[0]], Goal);
                        newTask.Path.End = FindBuilding(TempPlanStorage[i][DomainActions[j].LocationParams[1]], Goal);
                        newTask.Path.Complete = false;
                        // otherwise goal is primary location
                    }
                    else
                        newTask.Path.End = FindBuilding(TempPlanStorage[i][DomainActions[j].LocationParams[0]], Goal);
				TotalTasks.Add(newTask);
                }

        }
            // at the end empty it
            TempPlanStorage = null;
    }
    void RunPlan()
    {
        if (mFF == null)
        {
            mFF = new Process();
            // find metricFF file path / execute .batfile
            mFF.StartInfo.FileName = Application.dataPath + @"/scripts/PDDL/metric-ff.exe";
            mFF.StartInfo.Arguments = string.Format("-o {0}.pddl -f {1}.pddl", domainfileloc, problemfileloc);
            mFF.StartInfo.CreateNoWindow = true;
            mFF.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        }
        // run the process, and wait until it has closed
        mFF.Start();
        mFF.WaitForExit();
    }
    bool LoadSolution(int ID)
    {
        if (!File.Exists(solutionfileloc))
            return false;
        var solution = File.ReadAllLines(solutionfileloc).Where(s => s.Contains(":")).ToArray();
       // if (checktime())
       //     return false;
        List<string[]> croppedSolution = new List<string[]>();
        foreach(var action in solution)
        {
            //now we parse based upon the action
            // once the action is identified we complete the struct to be returned
            char[] seperatingchars = {' ','(',')'};
            var actionline = action.Split(seperatingchars).ToList();
            // remove the extra stuff

            int i = 0;
            for (i = 0; i < actionline.Count(); ++i)
			{
				if (actionline[i].Contains(':')  ||actionline[i].Contains('[') || actionline[i] == "")
				{
					actionline.RemoveAt(i);
					i--;
				}
			}
			if(actionline[0].Equals( ""))
				return false;
            croppedSolution.Add(actionline.ToArray());
        }
        // now we should have a list of all required
        // temp storage and time check ????
        TempPlanID = ID;
        TempPlanStorage = croppedSolution;
        InterpretSolution(GoalsToAcheive[ID]);
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
        for (int i = 0; i < Goal.GameState.Villagers.Count; i++)
        {
            writer.WriteLine("villager_" + i.ToString() + " - person");
			writer.WriteLine("Start_" + i.ToString() + " - location");
        }
        for (int i = 0; i < Goal.GameState.OwnedLocations.Count; i++)
        {
            writer.WriteLine("building_" + i.ToString() + " - location");
        }
        if (Goal.NewBuildings != Building.BuildingType.None )
            writer.WriteLine("buildingSite" + " - location");
        if(Goal.GameState.Trees != null)
            for (int i = 0; i < Goal.GameState.Trees.Count; i++)
            {
                writer.WriteLine("Tree_" + i.ToString() + "- Tree");
            }
        writer.Write(")");
        writer.WriteLine("(:init ");
        //here we loop through the initial state and set up the predicates such as which items are where
        for (int i = 0; i < Goal.GameState.Villagers.Count; i++)
        {
            string skill = "";
            string item = "";
           // writer.WriteLine(Goal.GameState.Villagers[i].Key + " - person");
            if (Goal.GameState.Villagers[i].Skill == Villager.Skills.Blacksmith)
                skill = "BlacksmithS";
            else if (Goal.GameState.Villagers[i].Skill == Villager.Skills.Carpenter)
                skill = "Carpenter";
            else if (Goal.GameState.Villagers[i].Skill == Villager.Skills.Labourer)
                skill = "Labourer";
            else if (Goal.GameState.Villagers[i].Skill == Villager.Skills.Lumberjack)
                skill = "Lumberjack";
            else if (Goal.GameState.Villagers[i].Skill == Villager.Skills.Miner)
                skill = "Miner";
            else if (Goal.GameState.Villagers[i].Skill == Villager.Skills.Rifleman)
                skill = "Rifleman";
            else if (Goal.GameState.Villagers[i].Skill == Villager.Skills.Trader)
                skill = "Trader";
            else
                continue;
            writer.WriteLine("(has-"+skill+" "+"villager_"+i.ToString()+")");
            if (Goal.GameState.Villagers[i].Inventory == Villager.Items.Stone)
                item = "Stone";
            else if (Goal.GameState.Villagers[i].Inventory == Villager.Items.Wood)
                item = "Wood";
            else if(Goal.GameState.Villagers[i].Inventory == Villager.Items.Iron)
                item = "Iron";
            else if(Goal.GameState.Villagers[i].Inventory == Villager.Items.Timber)
                item = "Timber";
            else if(Goal.GameState.Villagers[i].Inventory == Villager.Items.Ore)
                item = "Ore";
            else if(Goal.GameState.Villagers[i].Inventory == Villager.Items.Coal)
                item = "Coal";
            else if(Goal.GameState.Villagers[i].Inventory == Villager.Items.Money)
                item = "Money";
            else if(Goal.GameState.Villagers[i].Inventory == Villager.Items.Goods)
                item = "Goods";
            else if(Goal.GameState.Villagers[i].Inventory == Villager.Items.Axe)
                item = "Axe";
            else if(Goal.GameState.Villagers[i].Inventory == Villager.Items.Cart)
                item = "Cart";
            else if(Goal.GameState.Villagers[i].Inventory == Villager.Items.Rifle)
                item = "Rifle";
            else
                item  = "";
			if(item != "")
            	writer.WriteLine("(has-" + item + " " + "villager_" + i.ToString() + ")");
            writer.WriteLine("(at villager_"+i.ToString()+" Start_"+i.ToString() + ")"); // so all villagers start at an arbitrary possition based on their current position at time of planning
        }
       
        for (int i = 0; i < Goal.GameState.OwnedLocations.Count; i++)
        {
            string location = "";
            if (Goal.GameState.OwnedLocations[i].Type == Building.BuildingType.Barracks)
                location = "Barracks";
            else if (Goal.GameState.OwnedLocations[i].Type == Building.BuildingType.Blacksmith)
                location = "Blacksmith";
            else if (Goal.GameState.OwnedLocations[i].Type == Building.BuildingType.House)
                location = "house";
            else if (Goal.GameState.OwnedLocations[i].Type == Building.BuildingType.Market_Stall)
                location = "Market_Stall";
            else if (Goal.GameState.OwnedLocations[i].Type == Building.BuildingType.Mine)
                location = "Mine";
            else if (Goal.GameState.OwnedLocations[i].Type == Building.BuildingType.Quarry)
                location = "Quarry";
            else if (Goal.GameState.OwnedLocations[i].Type == Building.BuildingType.Sawmill)
                location = "Sawmill";
            else if (Goal.GameState.OwnedLocations[i].Type == Building.BuildingType.School)
                location = "School";
            else if (Goal.GameState.OwnedLocations[i].Type == Building.BuildingType.Smelter)
                location = "Smelter";
            else if (Goal.GameState.OwnedLocations[i].Type == Building.BuildingType.Storage)
                location = "Storage";
            else if (Goal.GameState.OwnedLocations[i].Type == Building.BuildingType.Turf_Hut)
                location = "turfhut";
            else if (Goal.GameState.OwnedLocations[i].Type == Building.BuildingType.buildingSite)
                continue;
            writer.WriteLine("(is-" + location + " building_" + i.ToString() + ")");
            foreach(var Item in Goal.GameState.OwnedLocations[i].Items)
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
		if(Goal.NewBuildings != Building.BuildingType.None)
			writer.WriteLine("(is-BuildingSite buildingSite)");

        writer.Write(")");
        writer.WriteLine("(:goal ");
        //here we loop through the goal state

        writer.WriteLine("(and");
        if (Goal.NewBuildings != Building.BuildingType.None)
        {

            if (Goal.NewBuildings == Building.BuildingType.Barracks)
            {
                writer.WriteLine("(is-Barracks buildingSite)");
            }
            else if (Goal.NewBuildings == Building.BuildingType.Turf_Hut)
            {
                writer.WriteLine("(is-turfhut buildingSite)");
            }
            else if (Goal.NewBuildings == Building.BuildingType.Quarry)
            {
                writer.WriteLine("(is-Quarry buildingSite)");
            }
            else if (Goal.NewBuildings == Building.BuildingType.House)
            {
                writer.WriteLine("(is-House buildingSite)"); ;
            }
            else if (Goal.NewBuildings == Building.BuildingType.School)
            {
                writer.WriteLine("(is-School buildingSite)");
            }
            else if (Goal.NewBuildings == Building.BuildingType.Storage)
            {
                writer.WriteLine("(is-Storage buildingSite)");
            }
            else if (Goal.NewBuildings == Building.BuildingType.Mine)
            {
                writer.WriteLine("(is-Mine buildingSite)");
            }
            else if (Goal.NewBuildings == Building.BuildingType.Smelter)
            {
                writer.WriteLine("(is-Smelter buildingSite)");
            }
            else if (Goal.NewBuildings == Building.BuildingType.Sawmill)
            {
                writer.WriteLine("(is-Sawmill buildingSite)");
            }
            else if (Goal.NewBuildings == Building.BuildingType.Blacksmith)
            {
                writer.WriteLine("(is-Blacksmith buildingSite)");
            }
        }
        else if(Goal.NewSkills != null)
        {
            //labourers will be assigned skills here based on number of new skills
            if (Goal.NewSkills.Contains(Villager.Skills.Labourer))
            {
                // means they want to hump
                writer.WriteLine("(isParent villager_0)");
                writer.WriteLine("(isParent villager_1)");
            }
            else
            {
                // here we learn existing skills so its any labourer taught by ANYONE
                List<string> StudentName = new List<string>();
                List<string> ItemName = new List<string>();
                for (int i = 0; i < Goal.NewSkills.Count; i++)
                {
                    if (Goal.GameState.Villagers[i].Skill == Villager.Skills.Labourer)
                        StudentName.Add("villager_" + i.ToString());
                }
                foreach (var skill in Goal.NewSkills)
                {
                    if (skill == Villager.Skills.Blacksmith)
                        ItemName.Add("BlacksmithS");
                    else if (skill == Villager.Skills.Carpenter)
                        ItemName.Add("Carpenter");
                    else if (skill == Villager.Skills.Lumberjack)
                        ItemName.Add("Lumberjack");
                    else if (skill == Villager.Skills.Miner)
                        ItemName.Add("Miner");
                    else if (skill == Villager.Skills.Rifleman)
                        ItemName.Add("Rifleman");
                    else if (skill == Villager.Skills.Trader)
                        ItemName.Add("Trader");
                }
                for (int i = 0; i < Goal.NewSkills.Count; i++)
                    writer.WriteLine("(has-" + ItemName[i] + StudentName[i] + ")");
            }
        }
        else if(Goal.NewItems != null)
        {
            //check whether raw or derivative resources
            // there are no storage facilities, and so raw resources will only be collected when we require the tools / buildings
            if (Goal.NewItems.Contains(Villager.Items.Rifle))
            {
                for (int i = 0; i < Goal.GameState.OwnedLocations.Count; i++)
                {
                  if(Goal.GameState.OwnedLocations[i].Type == Building.BuildingType.Blacksmith)
                    {
                        writer.WriteLine("(has-Rifle " + "building_" + i.ToString() + ")");
                        break;
                    }
                }
            }
            else if (Goal.NewItems.Contains(Villager.Items.Axe))
            {
                for (int i = 0; i < Goal.GameState.OwnedLocations.Count; i++)
                {
                    if (Goal.GameState.OwnedLocations[i].Type == Building.BuildingType.Blacksmith)
                    {
                        writer.WriteLine("(has-Axe " + "building_" + i.ToString() + ")");
                        break;
                    }
                }
            }
            else if (Goal.NewItems.Contains(Villager.Items.Cart))
            {
                for (int i = 0; i < Goal.GameState.OwnedLocations.Count; i++)
                {
                    if (Goal.GameState.OwnedLocations[i].Type == Building.BuildingType.Blacksmith)
                    {
                        writer.WriteLine("(has-Cart " + "building_" + i.ToString() + ")");
                        break;
                    }
                }
            }
        }
        writer.Write(")");
        writer.Write(")");
        writer.Write(")");
        writer.Dispose();
        return true;
    }
}
