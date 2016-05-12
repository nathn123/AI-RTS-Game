using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VillageManager {

    public GameObject MaleVillager;
    public GameObject FemaleVillager;

    PathPlanning PathPlanner;
    TaskPlanning TaskPlanner;
    List<ObjectiveTasks> CurrentObj;
    int MaxTasks = 10;
    public AI_Bias CurrentBias;
    bool Initialised;
    List<Villager> Villagers;
    List<Building> Buildings;
    float starttime, allowedtime;
    //List<> Tasks;

    char[,] Map, AiMap;

    // rather than global objects, have a list of maximum concurrent tasks
    // each task is deceided based upon certain conditions

    public enum ObjectiveTypes
    {
        IncreasePopulation, // Chosen when the number of labourers in less than 10 % pop increased untill labourers more than 25%
        IncreaseRawResources, // chosen when No of resources is less than 10 x no of villagers 
        IncreaseAdvResources, // chosen when rifles less than 2 x riflemen
        ScoutArea, // only active after pop is more than 10 , then activated when ???????? probs not gonna add it
        IncreaseHousing, // when villagers / 2 less than current number of houses 
        IncreaseEducation, // when specialists are less than 10 % of pop
        IncreaseMoney, // when No of resources is more than 20 x no of villagers
        TradeResources, // kinda same as above might need rework ... will need rework
        GoToWar, // not implementing yet
        None 
    }
    public enum TaskObjectives
    {

    }
    public struct GameState
    {
        // current state of game world // copy of the stored world state makes more sense
        public List<Villager> Villagers;
        public List<Building> OwnedLocations;
        public List<Vector2> TreeLocations;
    }
    public struct GoalState
    {
        // what we want at the end
        public List<Building.BuildingType> NewBuildings;
        public List<Villager.Items> NewItems;
        public List<Villager.Skills> NewSkills;
        // only storing new values otherwise we would have to calculate the future gamestate too much effort
        public void AddNewSkills(Villager.Skills Skills)
        {
            if (NewSkills == null)
                NewSkills = new List<Villager.Skills>();
            NewSkills.Add(Skills);
        }
        public void AddNewItems(Villager.Items Items)
        {
            if (NewItems == null)
                NewItems = new List<Villager.Items>();
            NewItems.Add(Items);
        }
        public void AddNewBuildings(Building.BuildingType Buildings)
        {
            if (NewBuildings == null)
                NewBuildings = new List<Building.BuildingType>();
            NewBuildings.Add(Buildings);
        }
    }
    public enum AI_Bias
    {
        Balanced, // No bias
        Economic, // focus on stocks of resources
        Aggressive, // focus on war
        Explorer, // focus on exploration, possible trade or war
        Turtle // focus on Defence of home but not attack of others
    }
    struct ObjectiveTasks
    {
        public int TaskID, PathID;
        public List<TaskPlanning.Task> Stepsneeded;
        public ObjectiveTypes Type;
        public bool complete;
        public void SetSteps(List<TaskPlanning.Task> newSteps)
        {
            Stepsneeded = newSteps;
        }
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public void Update () 
    {
        // this should be for updating the state of the game in the eyes of the planner
        // do we implement fog of war ????
        // if we want a scouting we need it 
        // how do we do it ............ 
        // duplicate maps updated with villager FOV  - large memory footprint ???? although only double char arrays 
        if (!Initialised)
            return;

        UpdateMap();
        starttime = Time.time;
        // after each step  we need to do to ensure it has time availiable
        if (checktime())
            return; 

        //do we have an objective set
        if (CurrentObj.Count < MaxTasks)
            GenerateNewTask();
        if (checktime())
            return;
        // must have a global objective / now we need to set sub-objectives ????

        ObjectiveCompleted();
        Debug.Log("Villager Manager run");
        TaskPlanner.Update();
        PathPlanner.Update();
	}
        bool checktime()
    {
        //if (Time.time - starttime > allowedtime)
        //    return true;
        //return false;
        return false;
    }
    public void Initialise(Vector2 StartingPos,int VillageNum,AI_Bias bias,ref char[,] AiMap_)
    {
        if(VillageNum == 0)
        {
            MaleVillager = Resources.Load<GameObject>("Characters/Prefabs/Male 1");
            FemaleVillager = Resources.Load<GameObject>("Characters/Prefabs/Female 1");
        }
        else if (VillageNum == 1)
        {
            MaleVillager = Resources.Load<GameObject>(Application.dataPath + "Characters/Prefabs/Male 2.prefab");
            FemaleVillager = Resources.Load<GameObject>(Application.dataPath + "Characters/Prefabs/Female 2.prefab");
        }
        else if (VillageNum == 2)
        {
            MaleVillager = Resources.Load<GameObject>(Application.dataPath + "Characters/Prefabs/Male 3.prefab");
            FemaleVillager = Resources.Load<GameObject>(Application.dataPath + "Characters/Prefabs/Female 3.prefab");
        }
        else if (VillageNum == 3)
        {
            MaleVillager = Resources.Load<GameObject>(Application.dataPath + "Characters/Prefabs/Male 4.prefab");
            FemaleVillager = Resources.Load<GameObject>(Application.dataPath + "Characters/Prefabs/Female 4.prefab");
        }
        TaskPlanner = new TaskPlanning();
        TaskPlanner.Initialise();
        PathPlanner = new PathPlanning();
        PathPlanner.Initialise();
        CurrentObj = new List<ObjectiveTasks>();
        CurrentBias = bias;
        AiMap = AiMap_;
        Villagers = new List<Villager>();
        Buildings = new List<Building>();
        
        Villagers.Add((GameObject.Instantiate(MaleVillager, new Vector3(StartingPos.x, StartingPos.y), Quaternion.identity) as GameObject).GetComponent<Villager>() );
        Villagers.Add( (GameObject.Instantiate(FemaleVillager, new Vector3(StartingPos.x, StartingPos.y), Quaternion.identity) as GameObject).GetComponent<Villager>());
        Initialised = true;
    }

    void UpdateMap()
    {
        foreach(var villager in Villagers)
        {
            var position = villager.GetComponent<Villager>().Position;
            var range = villager.GetComponent<Villager>().FOV;
            for(int i = -range; i<range;++i)
            {
                for(int j = -range; j<range;++j)
                {
                    Map[(int)position.x + i, (int)position.y + j] = AiMap[(int)position.x + i, (int)position.y + j];
                }
            }
        }
    }

    void PassGoal(GoalState goal)
    {
        ObjectiveTasks newTask = new ObjectiveTasks();
        newTask.PathID = -1;
        newTask.TaskID = TaskPlanner.RequestTask(goal);
        CurrentObj.Add(newTask);
    }
    void RequestPath()
    {
        for (int i = 0; i < CurrentObj.Count; i++)
        {
            for(int j= 0; j < CurrentObj[i].Stepsneeded.Count;j++)
            {
                // check to see if path is needed
                if (CurrentObj[i].Stepsneeded[j].PathID != -1)
                    continue; // means a path has been asked for
                if (CurrentObj[i].Stepsneeded[j].Path.Start.x == float.MaxValue)
                    CurrentObj[i].Stepsneeded[j].SetComplete(true);
                //add the path
                CurrentObj[i].Stepsneeded[j].SetPathID(PathPlanner.AddPath(CurrentObj[i].Stepsneeded[j].Path));
            }
        }
    }
    void CheckRetrieveTask()
    {
        for (int i = 0; i < CurrentObj.Count; i++)
        {
            if(CurrentObj[i].complete)
            {
                //task recieved we need to check for paths now
                for(int j= 0; j < CurrentObj[i].Stepsneeded.Count; j++)
                {
                    if (PathPlanner.PathReady(CurrentObj[i].Stepsneeded[j].PathID))
                        CurrentObj[i].Stepsneeded[j].SetPath( PathPlanner.GetPath(CurrentObj[i].Stepsneeded[j].PathID));
                }
            }
            else if (CurrentObj[i].Stepsneeded == null)
                if (TaskPlanner.TaskReady(CurrentObj[i].TaskID))
                    CurrentObj[i].SetSteps(TaskPlanner.GetPlan(CurrentObj[i].TaskID));
        }
    }

    void GenerateNewTask()
    {

        int Pop = 0, RRes = 0, ARes = 0, Scout = 0, Hous = 0, Edu = 0, Money = 0, Trade = 0, War = 0;
        // first we count the number of objectives already set with specific types, so we dont focus only on one type all the time
        int ObjCap = Mathf.CeilToInt(MaxTasks *0.4f); // maximum of 40% of tasks dedicated to one objective type
        foreach(var obj in CurrentObj)
        {
            if (obj.Type == ObjectiveTypes.GoToWar)
                War++;
            else if (obj.Type == ObjectiveTypes.IncreaseAdvResources)
                ARes++;
            else if (obj.Type == ObjectiveTypes.IncreaseEducation)
                Edu++;
            else if (obj.Type == ObjectiveTypes.IncreaseHousing)
                Hous++;
            else if (obj.Type == ObjectiveTypes.IncreaseMoney)
                Money++;
            else if (obj.Type == ObjectiveTypes.IncreasePopulation)
                Pop++;
            else if (obj.Type == ObjectiveTypes.IncreaseRawResources)
                RRes++;
            else if (obj.Type == ObjectiveTypes.ScoutArea)
                Scout++;
            else if (obj.Type == ObjectiveTypes.TradeResources)
                Trade++;
        }
        // now we take stock of what we have, to deciede what we need
        int TotPop = 0, Labs = 0, Rifle = 0, Trader = 0, BlacksP = 0, Mine = 0, Lumber = 0, Carp = 0,
            Turf = 0, House = 0, Scho = 0, Barr = 0, Stora = 0, MineB = 0, Smelt = 0, Quar = 0, Saw = 0, BlacksB = 0, Mark = 0;
        foreach(var villager in Villagers)
        {
            TotPop++; // total population
            if (villager.Skill == Villager.Skills.Labourer)
                Labs++;
            else if (villager.Skill == Villager.Skills.Blacksmith)
                BlacksP++;
            else if (villager.Skill == Villager.Skills.Carpenter)
                Carp++;
            else if (villager.Skill == Villager.Skills.Lumberjack)
                Lumber++;
            else if (villager.Skill == Villager.Skills.Miner)
                Mine++;
            else if (villager.Skill == Villager.Skills.Rifleman)
                Rifle++;
            else if (villager.Skill == Villager.Skills.Trader)
                Trader++;
        }
        foreach(var building in Buildings)
        {
            if (building.Type == Building.BuildingType.Barracks)
                Barr++;
            else if (building.Type == Building.BuildingType.Blacksmith)
                BlacksB++;
            else if (building.Type == Building.BuildingType.House)
                House++;
            else if (building.Type == Building.BuildingType.Market_Stall)
                Mark++;
            else if (building.Type == Building.BuildingType.Mine)
                MineB++;
            else if (building.Type == Building.BuildingType.Quarry)
                Quar++;
            else if (building.Type == Building.BuildingType.Sawmill)
                Saw++;
            else if (building.Type == Building.BuildingType.School)
                Scho++;
            else if (building.Type == Building.BuildingType.Smelter)
                Smelt++;
            else if (building.Type == Building.BuildingType.Storage)
                Stora++;
            else if (building.Type == Building.BuildingType.Turf_Hut)
                Turf++;
        }
        // and finally we take stock of the items
        int stone = 0 , wood = 0 , iron = 0 , timber = 0, ore = 0, coal = 0, axe = 0, cart = 0, RifleI = 0, MoneyI = 0, goods = 0;

        foreach (var villager in Villagers)
        {
            if (villager.Inventory == Villager.Items.Axe)
                axe++;
            else if (villager.Inventory == Villager.Items.Cart)
                cart++;
            else if (villager.Inventory == Villager.Items.Coal)
                coal++;
            else if (villager.Inventory == Villager.Items.Goods)
                goods++;
            else if (villager.Inventory == Villager.Items.Iron)
                iron++;
            else if (villager.Inventory == Villager.Items.Money)
                MoneyI++;
            else if (villager.Inventory == Villager.Items.Ore)
                ore++;
            else if (villager.Inventory == Villager.Items.Rifle)
                RifleI++;
            else if (villager.Inventory == Villager.Items.Stone)
                stone++;
            else if (villager.Inventory == Villager.Items.Timber)
                timber++;
            else if (villager.Inventory == Villager.Items.Wood)
                wood++;
        }
        foreach(var building in Buildings)
        {
            foreach(var item in building.Items)
            {
                if (item == Villager.Items.Axe)
                    axe++;
                else if (item == Villager.Items.Cart)
                    cart++;
                else if (item == Villager.Items.Coal)
                    coal++;
                else if (item == Villager.Items.Goods)
                    goods++;
                else if (item == Villager.Items.Iron)
                    iron++;
                else if (item == Villager.Items.Money)
                    MoneyI++;
                else if (item == Villager.Items.Ore)
                    ore++;
                else if (item == Villager.Items.Rifle)
                    RifleI++;
                else if (item == Villager.Items.Stone)
                    stone++;
                else if (item == Villager.Items.Timber)
                    timber++;
                else if (item == Villager.Items.Wood)
                    wood++;
            }
        }
        //IncreasePopulation, // Chosen when the number of labourers in less than 10 % pop increased untill labourers more than 25%
        //IncreaseRawResources, // chosen when No of resources is less than 10 x no of villagers 
        //IncreaseAdvResources, // chosen when rifles less than 2 x riflemen
        //IncreaseHousing, // when villagers / 2 less than current number of houses 
        //IncreaseEducation, // when specialists are less than 10 % of pop // not needed as pddl will educate as needed
        //IncreaseMoney, // when No of resources is more than 20 x no of villagers
        //TradeResources, // kinda same as above might need rework ... will need rework
        //ScoutArea, // only active after pop is more than 10 , then activated when ???????? probs not gonna add it
        //GoToWar, // not implementing yet
        // now we are ready to gen the task
        // order task are in order of which i believe are important
        // continue to add tasks until the cap is reached

        int noneselected = 1; // this var is added to combat infinite loops, will reduce the requirements to activate a task until a task is met
        do
        {
            ObjectiveTasks newObjective = new ObjectiveTasks();
            GoalState newGoal = new GoalState();
            if(Labs < (TotPop *0.1f)/noneselected && Pop < ObjCap)
            {
                newObjective.Type = ObjectiveTypes.IncreasePopulation;
                for (int i = 0; i < 5; i++)
                    newGoal.AddNewSkills(Villager.Skills.Labourer);
                newObjective.TaskID = TaskPlanner.RequestTask(newGoal);
                CurrentObj.Add(newObjective);
                noneselected = 1;
            }
            else if ((wood < 10 / noneselected || stone < 10 / noneselected)&&RRes < ObjCap)
            {
                newObjective.Type = ObjectiveTypes.IncreaseRawResources;
                for (int i = 0; i < 5; i++)
                {
                    newGoal.AddNewItems(Villager.Items.Wood);
                    newGoal.AddNewItems(Villager.Items.Stone);
                }
                newObjective.TaskID = TaskPlanner.RequestTask(newGoal);
                CurrentObj.Add(newObjective);
                noneselected = 1;
            }
            else
                noneselected++;
        } while (CurrentObj.Count < MaxTasks || noneselected == 20);
    }

    void ObjectiveCompleted()
    {
        for (int i = 0; i < CurrentObj.Count; i++)
        {
            if (ObjectiveCompleted(CurrentObj[i]))
                CurrentObj.RemoveAt(i);
        }
    }
    bool ObjectiveCompleted(ObjectiveTasks Current)
    {
        if (Current.Stepsneeded == null)
            return false;
        foreach (var steps in Current.Stepsneeded)
            if (steps.complete != true)
                return false;
        return true;
    }

}
