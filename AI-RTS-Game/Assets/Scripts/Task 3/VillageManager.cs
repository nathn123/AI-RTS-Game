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
    int VillageNum;
    public AI_Bias CurrentBias;
    bool Initialised;
    TreeManager TreeMgr;
    List<Villager> Villagers;
    List<Building> Buildings;
    List<Villager> AvailableVillagers;
    Vector2 StartingPos;
    float starttime, allowedtime;
    //List<> Tasks;

    char[,] AiMap;

    // rather than global objects, have a list of maximum concurrent tasks
    // each task is deceided based upon certain conditions

    public enum ObjectiveTypes
    {
        IncreasePopulation, // Chosen when the number of labourers in less than 10 % pop increased untill labourers more than 25%
        IncreaseAdvResources, // chosen when rifles less than 2 x riflemen
        ScoutArea, // only active after pop is more than 10 , then activated when ???????? probs not gonna add it
        IncreaseHousing, // when villagers / 2 less than current number of houses 
        IncreaseEducation, // when specialists are less than specialist building i.e 0 blacksmith pop but 1 blacksmith
        IncreaseSpecialistBuilding, // will build specialist buildings, such as blacksmith sawmill
        IncreaseMoney, // when No of resources is more than 20 x no of villagers
        TradeResources, // kinda same as above might need rework ... will need rework
        GoToWar, // not implementing yet
        None 
    }
    public struct GameState
    {
        // current state of game world // copy of the stored world state makes more sense
        public List<Villager> Villagers;
        public List<Building> OwnedLocations;
        public List<Tree> Trees;
    }
    public struct GoalState
    {
        // what we want at the end
        public Building.BuildingType NewBuildings;
        public Building Site;
        public List<Villager.Items> NewItems;
        public List<Villager.Skills> NewSkills;
        bool Run;
        // only storing new values otherwise we would have to calculate the future gamestate too much effort
        public GameState GameState; // moved to here as these are the objects needed for the task
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
        //public void AddNewBuildings(Building.BuildingType Buildings)
        //{
        //    if (NewBuildings == null)
        //        NewBuildings = new List<Building.BuildingType>();
        //    NewBuildings.Add(Buildings);
        //}
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
        public Building BuildingSite;
        public ObjectiveTypes Type;
        public bool complete;
        public void SetSteps(List<TaskPlanning.Task> newSteps)
        {
            if (Stepsneeded == null)
                Stepsneeded = new List<TaskPlanning.Task>();
            Stepsneeded.AddRange( newSteps);
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
        //update map
        AiMap = MapGen.GetMap();
        starttime = Time.time;
        // after each step  we need to do to ensure it has time availiable
        if (checktime())
            return; 

        //do we have an objective set
        CheckRetrieveTask();
        RequestPath();
        if (CurrentObj.Count < MaxTasks)
            GenerateNewTask();
        if (checktime())
            return;
        
        
        // must have a global objective / now we need to set sub-objectives ????
        foreach (var Objective in CurrentObj)
            if (ObjectiveReady(Objective))
                AssignTasks(Objective);
        ObjectiveCompleted();
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
    public void Initialise(Vector2 StartingPos_,int VillageNum_,AI_Bias bias,TreeManager TreeMgr_,ref char[,] AiMap_, VillageGen.Scenario NewScenario = new VillageGen.Scenario())
    {
        if(VillageNum_ == 0)
        {
            MaleVillager = Resources.Load<GameObject>("Characters/Prefabs/Male 1");
            FemaleVillager = Resources.Load<GameObject>("Characters/Prefabs/Female 1");
        }
        else if (VillageNum_ == 1)
        {
            MaleVillager = Resources.Load<GameObject>(Application.dataPath + "Characters/Prefabs/Male 2.prefab");
            FemaleVillager = Resources.Load<GameObject>(Application.dataPath + "Characters/Prefabs/Female 2.prefab");
        }
        else if (VillageNum_ == 2)
        {
            MaleVillager = Resources.Load<GameObject>(Application.dataPath + "Characters/Prefabs/Male 3.prefab");
            FemaleVillager = Resources.Load<GameObject>(Application.dataPath + "Characters/Prefabs/Female 3.prefab");
        }
        else if (VillageNum_ == 3)
        {
            MaleVillager = Resources.Load<GameObject>(Application.dataPath + "Characters/Prefabs/Male 4.prefab");
            FemaleVillager = Resources.Load<GameObject>(Application.dataPath + "Characters/Prefabs/Female 4.prefab");
        }
        VillageNum = VillageNum_;
        TaskPlanner = new TaskPlanning();
        TaskPlanner.Initialise();
        PathPlanner = new PathPlanning();
        PathPlanner.Initialise(ref AiMap_);
        CurrentObj = new List<ObjectiveTasks>();
        CurrentBias = bias;
        AiMap = AiMap_;
        Villagers = new List<Villager>();
        AvailableVillagers = new List<Villager>();
        Buildings = new List<Building>();
        TreeMgr = TreeMgr_;
        if (!NewScenario.Created)
        {
            Villagers.Add((GameObject.Instantiate(MaleVillager, new Vector3(StartingPos_.x, StartingPos_.y), Quaternion.identity) as GameObject).GetComponent<Villager>());
            Villagers.Add((GameObject.Instantiate(FemaleVillager, new Vector3(StartingPos_.x, StartingPos_.y), Quaternion.identity) as GameObject).GetComponent<Villager>());
            StartingPos = StartingPos_;
            foreach (var vill in Villagers)
                vill.Initialise(StartingPos_);
            AvailableVillagers.AddRange(Villagers);
            Initialised = true;
        }
        else
        {
            StartingPos = new Vector2(NewScenario.Xpos, NewScenario.Ypos);
            // gen scenario
            for (int i = 0; i < NewScenario.Barracks; i++)
                Buildings.Add(GenerateBuildingSite(Building.BuildingType.Barracks));
            for (int i = 0; i < NewScenario.BlackS; i++)
                Buildings.Add(GenerateBuildingSite(Building.BuildingType.Blacksmith));
            for (int i = 0; i < NewScenario.House; i++)
                Buildings.Add(GenerateBuildingSite(Building.BuildingType.House));
            for (int i = 0; i < NewScenario.Market; i++)
                Buildings.Add(GenerateBuildingSite(Building.BuildingType.Market));
            for (int i = 0; i < NewScenario.Mine; i++)
                Buildings.Add(GenerateBuildingSite(Building.BuildingType.Mine));
            for (int i = 0; i < NewScenario.Quarry; i++)
                Buildings.Add(GenerateBuildingSite(Building.BuildingType.Quarry));
            for (int i = 0; i < NewScenario.Saw; i++)
                Buildings.Add(GenerateBuildingSite(Building.BuildingType.Sawmill));
            for (int i = 0; i < NewScenario.School; i++)
                Buildings.Add(GenerateBuildingSite(Building.BuildingType.School));
            for (int i = 0; i < NewScenario.Smelter; i++)
                Buildings.Add(GenerateBuildingSite(Building.BuildingType.Smelter));
            for (int i = 0; i < NewScenario.Storage; i++)
                Buildings.Add(GenerateBuildingSite(Building.BuildingType.Storage));
            for (int i = 0; i < NewScenario.Turf; i++)
                Buildings.Add(GenerateBuildingSite(Building.BuildingType.Turf));
            for (int i = 0; i < NewScenario.Villagers; i++)
                Villagers.Add((GameObject.Instantiate(MaleVillager, new Vector3(StartingPos_.x, StartingPos_.y), Quaternion.identity) as GameObject).GetComponent<Villager>());
            
        }
    }


    void PassGoal(GoalState goal, ObjectiveTypes Type)
    {
        ObjectiveTasks newTask = new ObjectiveTasks();
        newTask.Type = Type;
        newTask.PathID = -1;
        newTask.TaskID = TaskPlanner.RequestTask(goal);
        newTask.BuildingSite = goal.Site;
        newTask.Stepsneeded = new List<TaskPlanning.Task>();
        Debug.Log("new task created Type : " + Type.ToString());
        CurrentObj.Add(newTask);
    }
    void RequestPath()
    {
        for (int i = 0; i < CurrentObj.Count; i++)
        {
            if (CurrentObj[i].Stepsneeded.Count == 0)
                return;
            for(int j= 0; j < CurrentObj[i].Stepsneeded.Count;j++)
            {
                // check to see if path is needed
                if (CurrentObj[i].Stepsneeded[j].PathID != -1)
                    continue; // means a path has been asked for
                if (CurrentObj[i].Stepsneeded[j].Path.Start.x == float.MaxValue || CurrentObj[i].Stepsneeded[j].Path.Start == CurrentObj[i].Stepsneeded[j].Path.End)
                {
                    CurrentObj[i].Stepsneeded[j].Path.Complete = true;
                    continue;
                }
                //add the path
                var test = PathPlanner.AddPath(CurrentObj[i].Stepsneeded[j].Path);
                CurrentObj[i].Stepsneeded[j].SetPathID(test);
            }
        }
    }
    void CheckRetrieveTask()
    {
        for (int i = 0; i < CurrentObj.Count; i++)
        {
            if(CurrentObj[i].Stepsneeded.Count > 0)
            {
                //task recieved we need to check for paths now
                for(int j= 0; j < CurrentObj[i].Stepsneeded.Count; j++)
                {
                    if (CurrentObj[i].Stepsneeded[j].PathID == -1 || CurrentObj[i].Stepsneeded[j].Path.Complete)
                        continue;
                    if (PathPlanner.PathReady(CurrentObj[i].Stepsneeded[j].PathID))
                        CurrentObj[i].Stepsneeded[j].SetPath(PathPlanner.GetPath(CurrentObj[i].Stepsneeded[j].PathID));
                }
            }
            else if (CurrentObj[i].Stepsneeded.Count == 0)
                if (TaskPlanner.TaskReady(CurrentObj[i].TaskID))
                    CurrentObj[i].SetSteps(TaskPlanner.GetPlan(CurrentObj[i].TaskID));
        }
    }

    void GenerateNewTask()
    {

        int Pop = 0, ARes = 0, Scout = 0, Hous = 0, Edu = 0,Spec = 0 , Money = 0, Trade = 0, War = 0;
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
            else if (obj.Type == ObjectiveTypes.IncreaseSpecialistBuilding)
                Spec++;
            else if (obj.Type == ObjectiveTypes.ScoutArea)
                Scout++;
            else if (obj.Type == ObjectiveTypes.TradeResources)
                Trade++;
        }
        // now we take stock of what we have, to deciede what we need
        int TotPop = 0, Labs = 0, Rifle = 0, Trader = 0, BlacksP = 0, Mine = 0, Lumber = 0, Carp = 0,
            Turf = 0, House = 0, Scho = 0, Barr = 0, Stora = 0, MineB = 0, Smelt = 0, Quar = 0, Saw = 0, BlacksB = 0, Mark = 0, TotBuild = 0;
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
            else if (building.Type == Building.BuildingType.Market)
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
            else if (building.Type == Building.BuildingType.Turf)
                Turf++;
        }
        TotBuild = Barr + BlacksB + House + Mark + MineB + Quar + Saw + Scho + Smelt + Stora + Turf;
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
        //IncreaseAdvResources, // chosen when rifles less than 2 x riflemen
        //ScoutArea, // only active after pop is more than 10 , then activated when ???????? probs not gonna add it
        //IncreaseHousing, // when villagers / 2 less than current number of houses 
        //IncreaseEducation, // when specialists are less than specialist building i.e 0 blacksmith pop but 1 blacksmith
        //IncreaseSpecialistBuilding, // will build specialist buildings, such as blacksmith sawmill
        //IncreaseMoney, // when No of resources is more than 20 x no of villagers
        //TradeResources, // kinda same as above might need rework ... will need rework
        //GoToWar, // not implementing yet
        // now we are ready to gen the task
        // order task are in order of which i believe are important
        // continue to add tasks until the cap is reached
    
        int noneselected = 1; // this var is added to combat infinite loops, will reduce the requirements to activate a task until a task is met
        
        bool NeedTree = false;
        while (CurrentObj.Count < MaxTasks && noneselected < 20 && AvailableVillagers.Count > 0)
        {
            List<Villager.Skills> NeededSkills = new List<Villager.Skills>();
            ObjectiveTypes newObj = new ObjectiveTypes();
            GoalState newGoal = new GoalState();
            newGoal.NewBuildings = Building.BuildingType.None;
            newGoal.NewSkills = new List<Villager.Skills>();
            // Need to add a case for FORCED waiting to breed
            if( TotPop <= Mathf.FloorToInt(TotBuild * 0.75f)&& (House > 0 || Turf > 0)) // Increase Pop
            {

                // we need to gen a current state / decide what is going to be used here.
                newObj = ObjectiveTypes.IncreasePopulation;
                newGoal.AddNewSkills(Villager.Skills.Labourer);
                NeededSkills.Add(Villager.Skills.Any);
                NeededSkills.Add(Villager.Skills.Any);
                Pop++;// this SHOULD continually fail the second check until 2 vills are avaliable
            }
            else if (ARes < ObjCap && BlacksB > 0 && BlacksP > 0) // IncreaseAdvResources -rifles /carts axes
            {
                newObj = ObjectiveTypes.IncreaseAdvResources;
                if(RifleI < Rifle )
                {
                    newGoal.NewItems.Add( Villager.Items.Rifle);
                    ARes++;
                    NeededSkills.Add(Villager.Skills.Blacksmith);
                }
            }
            else if (Turf+House < TotPop/2 && Hous < ObjCap) // IncreaseHousing
            {
                newObj = ObjectiveTypes.IncreaseHousing;
                if(Quar>0) // build house
                {
                    newGoal.NewBuildings = Building.BuildingType.House;
                    var building = GenerateBuildingSite(Building.BuildingType.House);
                    newGoal.Site = building;
                    NeededSkills.Add(Villager.Skills.Labourer);
                    NeededSkills.Add(Villager.Skills.Lumberjack);
                    NeededSkills.Add(Villager.Skills.Carpenter);
                    Buildings.Add(building);
                    Hous++;
                }
                else // build turf
                {
                    newGoal.NewBuildings = Building.BuildingType.Turf;
                    var building = GenerateBuildingSite(Building.BuildingType.Turf);
                    newGoal.Site = building;
                    NeededSkills.Add(Villager.Skills.Labourer);
                    Buildings.Add(building);
                    Hous++;
                }

            }
            else if (Edu < ObjCap) // IncreaseEducation
            {
                newObj = ObjectiveTypes.IncreaseEducation;
                // we want ratios of people
                // 25 % labour
                // 20% lumber
                // 20% miner
                // 20% blacksmith
                // 15% rifle
                if(Lumber < Mathf.CeilToInt(0.2f * TotPop))
                {
                    newGoal.NewSkills.Add(Villager.Skills.Lumberjack);
                    NeededSkills.Add(Villager.Skills.Labourer);
                    NeededSkills.Add(Villager.Skills.Any);
                    Edu++;
                }
                else if (Mine < Mathf.CeilToInt(0.2f * TotPop))
                {
                    newGoal.NewSkills.Add(Villager.Skills.Miner);
                    NeededSkills.Add(Villager.Skills.Labourer);
                    NeededSkills.Add(Villager.Skills.Any);
                    Edu++;
                }
                else if (BlacksP < Mathf.CeilToInt(0.2f * TotPop))
                {
                    newGoal.NewSkills.Add(Villager.Skills.Blacksmith);
                    NeededSkills.Add(Villager.Skills.Labourer);
                    NeededSkills.Add(Villager.Skills.Any);
                    Edu++;
                }
                else if (Rifle < Mathf.CeilToInt(0.15f * TotPop))
                {
                    newGoal.NewSkills.Add(Villager.Skills.Rifleman);
                    NeededSkills.Add(Villager.Skills.Labourer);
                    NeededSkills.Add(Villager.Skills.Any);
                    Edu++;
                }

            }
            else if (Spec < ObjCap) // IncreaseSpecialistBuilding
            {
                newObj = ObjectiveTypes.IncreaseSpecialistBuilding;
                if (Smelt > 0 && Quar > 0)
                {
                    if(Scho == 0) // build school
                    {
                        newGoal.NewBuildings = Building.BuildingType.School;
                        var building = GenerateBuildingSite(Building.BuildingType.School);
                        newGoal.Site = building;
                        NeededSkills.Add(Villager.Skills.Labourer);
                        NeededSkills.Add(Villager.Skills.Lumberjack);
                        NeededSkills.Add(Villager.Skills.Carpenter);
                        Buildings.Add(building);
                        Spec++;
                    }
                    else if (BlacksB <(BlacksP*2)) // build blacksmith
                    {
                        newGoal.NewBuildings = Building.BuildingType.Blacksmith;
                        var building = GenerateBuildingSite(Building.BuildingType.Blacksmith);
                        newGoal.Site = building;
                        NeededSkills.Add(Villager.Skills.Labourer);
                        NeededSkills.Add(Villager.Skills.Miner);
                        Buildings.Add(building);
                        Spec++;
                    }
                    else if(Saw < Mathf.CeilToInt(0.05f*TotPop)) // build sawmill
                    {
                        newGoal.NewBuildings = Building.BuildingType.Sawmill;
                        var building = GenerateBuildingSite(Building.BuildingType.Sawmill);
                        newGoal.Site = building;
                        NeededSkills.Add(Villager.Skills.Labourer);
                        NeededSkills.Add(Villager.Skills.Miner);
                        Buildings.Add(building);
                        Spec++;
                    }
                    else if (Barr < Mathf.CeilToInt(0.05f * TotPop))
                    {
                        newGoal.NewBuildings = Building.BuildingType.Barracks;
                        var building = GenerateBuildingSite(Building.BuildingType.Barracks);
                        newGoal.Site = building;
                        NeededSkills.Add(Villager.Skills.Labourer);
                        NeededSkills.Add(Villager.Skills.Carpenter);
                        NeededSkills.Add(Villager.Skills.Lumberjack);
                        Buildings.Add(building);
                        Spec++;
                    }
                    else if (Stora < Mathf.CeilToInt(0.05f * TotPop))
                    {
                        newGoal.NewBuildings = Building.BuildingType.Storage;
                        var building = GenerateBuildingSite(Building.BuildingType.Storage);
                        NeededSkills.Add(Villager.Skills.Labourer);
                        NeededSkills.Add(Villager.Skills.Carpenter);
                        NeededSkills.Add(Villager.Skills.Lumberjack);
                        newGoal.Site = building;
                        Buildings.Add(building);
                        Spec++;
                    }
                    else if (Mark < Mathf.CeilToInt(0.05f * TotPop))
                    {
                        newGoal.NewBuildings = Building.BuildingType.Market;
                        NeededSkills.Add(Villager.Skills.Carpenter);
                        NeededSkills.Add(Villager.Skills.Lumberjack);
                        var building = GenerateBuildingSite(Building.BuildingType.Market);
                        newGoal.Site = building;
                        Buildings.Add(building);
                        Spec++;
                    }
                }
                else if (Quar > 0) // build smelter
                {
                    newGoal.NewBuildings = Building.BuildingType.Smelter;
                    NeededSkills.Add(Villager.Skills.Labourer);
                    var building = GenerateBuildingSite(Building.BuildingType.Smelter);
                    newGoal.Site = building;
                    Buildings.Add(building);
                    Spec++;
                }
                else // build quarry
                {
                    newGoal.NewBuildings = Building.BuildingType.Quarry;
                    NeededSkills.Add(Villager.Skills.Labourer);
                    var building = GenerateBuildingSite(Building.BuildingType.Quarry);
                    newGoal.Site = building;
                    Buildings.Add(building);
                    Spec++;
                }
            }
            else
                noneselected++;
            if (NeededSkills.Count != 0)
                if (GenerateGameState(NeededSkills, ref newGoal.GameState, NeedTree))
                    PassGoal(newGoal, newObj);
                else noneselected++;

            if (noneselected >= 20)
                return; // no objective set
                
        } 

    }
    bool GenerateGameState(List<Villager.Skills> NeededSkills, ref GameState State, bool Trees)
    {
        // to generate a game state , we mean to generate a list of entities that will be used for a task villagers/ buildings etc
        // we will assume all buildings are free, as there is no way to run plans concurrently and be able to check if the building is free at the time it is needed in that task
        // first thing we need it to test is, if there are villagers avaliable
        int runcount = 0;
        do
        {
            if(NeededSkills.Count >0)
                for (int j = 0; j < AvailableVillagers.Count; j++)
                {
                    if (AvailableVillagers[j].Skill == NeededSkills[0] || NeededSkills[0] == Villager.Skills.Any)
                    {
                        runcount = 0;
                        if (State.Villagers == null)
                            State.Villagers = new List<Villager>();
                        State.Villagers.Add(AvailableVillagers[j]);
                        NeededSkills.RemoveAt(0);
                        AvailableVillagers.RemoveAt(j);
                        break;
                    }
                    else
                        runcount++;
                }
        } while (NeededSkills.Count > 0 && runcount < (NeededSkills.Count * 2) * AvailableVillagers.Count);
        if (NeededSkills.Count > 0)
        {
            AvailableVillagers.AddRange(State.Villagers);
            State.Villagers.Clear();
            return false;
        }
        runcount = 0;
        State.OwnedLocations = Buildings; // all buildings added

        if(Trees)
        {
            // here we find the closest tree to a villager
            foreach(var vill in State.Villagers)
            {
                if (vill.Skill == Villager.Skills.Lumberjack)
                   State.Trees.Add(TreeMgr.FindNearestTree(vill.Position));
            }
        }
        return true;
    }
    void ObjectiveCompleted()
    {
        
        for (int i = 0; i < CurrentObj.Count; i++)
        {
            if (ObjectiveCompleted(CurrentObj[i]))
            {
                List<Villager> VillagersNowFree = new List<Villager>();
                for (int j = 0; j < CurrentObj[i].Stepsneeded.Count;j++)
                {
                    if (!Buildings.Contains(CurrentObj[i].BuildingSite))
                     Buildings.Add(CurrentObj[i].BuildingSite);
                    for (int p = 0; p < CurrentObj[i].Stepsneeded[j].villager.Count; p++)
                        if (!VillagersNowFree.Contains(CurrentObj[i].Stepsneeded[j].villager[p]))
                            VillagersNowFree.Add(CurrentObj[i].Stepsneeded[j].villager[p]);
                }
                AvailableVillagers.AddRange(VillagersNowFree);
                    CurrentObj.RemoveAt(i);
            }
        }
    }
    Building GenerateBuildingSite(Building.BuildingType Type)
    {
        Building NewBuilding = new Building();
        char BuildingChar = ' ';
        Vector2 NewPos = new Vector2(), dimensions = new Vector2();
        // need to setup building site somewhere on the map
        if (Type == Building.BuildingType.Barracks || Type == Building.BuildingType.School || Type == Building.BuildingType.Storage || Type == Building.BuildingType.Sawmill || Type == Building.BuildingType.Blacksmith)
        {
            dimensions = new Vector2(5, 5);
        }
        else if (Type == Building.BuildingType.Turf || Type == Building.BuildingType.Smelter)
        {
            dimensions = new Vector2(1,1);
        }
        else if (Type == Building.BuildingType.House)
        {
            dimensions = new Vector2(2, 3);
        }
        else if (Type == Building.BuildingType.Mine || Type == Building.BuildingType.Quarry || Type == Building.BuildingType.Market)
        {
            dimensions = new Vector2(1, 1);
        }
        // now we search area near the centre of base
        int range = 5;
        bool FoundPos = false;
        do
	    {
            for (int i = -range; i < range; i++)
            {
                if (FoundPos)
                    break;
                for (int j = -range; j < range; j++)
                    if (CheckMapArea(new Vector2(i, j) + StartingPos, dimensions))
                    {
                        FoundPos = true;
                        NewPos = new Vector2(i, j) + StartingPos;
                        break;
                    }
            }
            range += 5;
	    } while (!FoundPos);
        if (Type == Building.BuildingType.Barracks)
            BuildingChar = '1';
        else if (Type == Building.BuildingType.School )
            BuildingChar = '2';
        else if (Type == Building.BuildingType.Storage)
            BuildingChar = '3';
        else if (Type == Building.BuildingType.Sawmill )
            BuildingChar = '4';
        else if (Type == Building.BuildingType.Blacksmith)
            BuildingChar = '5';
        else if (Type == Building.BuildingType.Turf )
            BuildingChar = '6';
        else if (Type == Building.BuildingType.Smelter)
            BuildingChar = '7';
        else if (Type == Building.BuildingType.House)
            BuildingChar = '8';
        else if (Type == Building.BuildingType.Mine )
            BuildingChar = '9';
        else if (Type == Building.BuildingType.Quarry )
            BuildingChar = '0';
        else if (Type == Building.BuildingType.Market)
            BuildingChar = '-';

        for (int i = 0; i < dimensions.x; i++)
            for (int j = 0; j < dimensions.y; j++)
            {
                MapGen.UpdateMap( new Vector2((int)NewPos.x + i, (int)NewPos.y + j) , BuildingChar);
            }
        MapGen.UpdateMap(new Vector2((int)NewPos.x, (int)NewPos.y),'E');
        NewBuilding.Initialise(NewPos,dimensions, Type);
        return NewBuilding;
    }
    bool CheckMapArea(Vector2 Pos, Vector2 Dimensions)
    {
        for (int i = 0; i < Dimensions.x; i++)
            for (int j = 0; j < Dimensions.y;j++)
            {
                if (!PathPlanning.Buildable(AiMap, new Vector2(Pos.x + i, Pos.y + j)))
                {
                    Debug.Log("MapSquareTaken = " + new Vector2(Pos.x + i, Pos.y + j).ToString() + "Char Val = " + AiMap[(int)Pos.x + i, (int)Pos.y + j].ToString());
                    return false;
                }
            }
                return true;
    }
    bool ObjectiveCompleted(ObjectiveTasks Current)
    {
        if (Current.Stepsneeded.Count == 0)
            return false;
        foreach (var steps in Current.Stepsneeded)
            if (steps.complete != true)
                return false;
        Current.complete = true;
        return true;
    }

    bool ObjectiveReady(ObjectiveTasks Current)
    {
        if (Current.Stepsneeded.Count == 0 || Current.Stepsneeded == null)
            return false;
        foreach (var Steps in Current.Stepsneeded)
            if (Steps.Path.Complete == false)
                return false;

        return true;
    }
    void AssignTasks(ObjectiveTasks CurrentObj)
    {
        bool villagerFree = true;
        // at this point all tasks and paths are ready
        for(int i = 0; i < CurrentObj.Stepsneeded.Count; i++)
        {
            // we will add a list of callbacks to the stepsneeded struct,
            // these callbacks when all true will update the step as complete
            if (CurrentObj.Stepsneeded[i].complete)
                continue; // move to next task
            foreach (var vill in CurrentObj.Stepsneeded[i].villager)
                if (vill.CurrentAction != Villager.Actions.None)
                {
                    villagerFree = false;
                    break;
                }
            if (!villagerFree)
                continue;
            for (int j = 0; j < CurrentObj.Stepsneeded[i].villager.Count;j++ )
            {
                var Vill = CurrentObj.Stepsneeded[i].villager[j];
                // here we are ready, the correct task is selected and all the villagers required are free
                if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Walk)
                {
                    // quck check due to position assignments in pddl , due to villager start being the vil pos and no comparison to objective pos
                    if (CurrentObj.Stepsneeded[i].Path.Start == CurrentObj.Stepsneeded[i].Path.End)
                    {
                        // no walk needed
                        CurrentObj.Stepsneeded[i].complete = true;
                        return;
                    }
                    // only one villager, give the path
                    Vill.AddPath(CurrentObj.Stepsneeded[i].Path.Path);
                    Vill.SetAction(CurrentObj.Stepsneeded[i].Action);
                   
                    
                }
                else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Build)
                {
                    if (CurrentObj.Stepsneeded[i].ToBeBuilt == Building.BuildingType.Barracks)            Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 30.0f);
                    else if (CurrentObj.Stepsneeded[i].ToBeBuilt == Building.BuildingType.Blacksmith)     Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 30.0f);
                    else if (CurrentObj.Stepsneeded[i].ToBeBuilt == Building.BuildingType.House)          Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 15.0f);
                    else if (CurrentObj.Stepsneeded[i].ToBeBuilt == Building.BuildingType.Market)         Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 5.0f);
                    else if (CurrentObj.Stepsneeded[i].ToBeBuilt == Building.BuildingType.Mine)           Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 50.0f);
                    else if (CurrentObj.Stepsneeded[i].ToBeBuilt == Building.BuildingType.Quarry)         Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 10.0f);
                    else if (CurrentObj.Stepsneeded[i].ToBeBuilt == Building.BuildingType.Sawmill)        Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 30.0f);
                    else if (CurrentObj.Stepsneeded[i].ToBeBuilt == Building.BuildingType.School)         Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 30.0f);
                    else if (CurrentObj.Stepsneeded[i].ToBeBuilt == Building.BuildingType.Smelter)        Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 20.0f);
                    else if (CurrentObj.Stepsneeded[i].ToBeBuilt == Building.BuildingType.Storage)        Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 20.0f);
                    else if (CurrentObj.Stepsneeded[i].ToBeBuilt == Building.BuildingType.Turf)           Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 10.0f);
                }
                else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Buy_Sell)
                    Vill.SetAction(CurrentObj.Stepsneeded[i].Action, 1.0f);
                else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Combat)
                    Vill.SetAction(CurrentObj.Stepsneeded[i].Action, 1.0f);
                else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Store)
                    Vill.SetAction(CurrentObj.Stepsneeded[i].Action, 1.0f);
                else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Cut_Tree)    Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].TreeInUse, 5.0f);
                else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Educate)        Vill.SetAction(CurrentObj.Stepsneeded[i].Action, 100.0f);
                else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Educate_Barracks)    Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 30.0f);
                else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Family)  Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 20.0f);
                else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Family_House) Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 20.0f);
                else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Make_Tool) Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse,CurrentObj.Stepsneeded[i].ItemRequired, 10.0f);
                else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Mine)       Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 5.0f);
                else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Pickup)    Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse,CurrentObj.Stepsneeded[i].ItemRequired);
                else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Putdown)   Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse,CurrentObj.Stepsneeded[i].ItemRequired);
                else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Quarry) Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 5.0f);
                else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Saw_Wood) Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 10.0f);
                else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Smelt) Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 5.0f);
                else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Train) Vill.SetAction(CurrentObj.Stepsneeded[i].Action,CurrentObj.Stepsneeded[i].BuildingInUse, 50.0f);
            }
            // we put special cases like ... skills to learn
            CurrentObj.Stepsneeded[i].villager[0].ActionCallback = CurrentObj.Stepsneeded[i].SetComplete;
            if(CurrentObj.Stepsneeded[i].Action == Villager.Actions.Train ||
               CurrentObj.Stepsneeded[i].Action == Villager.Actions.Educate)
            {
                // should be two JiC first found with labourer skill
                    if (CurrentObj.Stepsneeded[i].villager[0].Skill == Villager.Skills.Labourer)
                    {
                        CurrentObj.Stepsneeded[i].villager[0].LearnSkill(CurrentObj.Stepsneeded[i].SkillToBeLearnt, CurrentObj.Stepsneeded[i].villager[1]);
                    }
            }
            if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Family)
                CurrentObj.Stepsneeded[i].villager[0].Preg = AddVillager;
            else if (CurrentObj.Stepsneeded[i].Action == Villager.Actions.Family_House)
            {
                CurrentObj.Stepsneeded[i].villager[0].Preg = AddVillager;
                CurrentObj.Stepsneeded[i].villager[1].Preg = AddVillager;
            }
        }
    }
    public delegate void NewVillager(Vector2 Pos);

    public void AddVillager(Vector2 Pos)
    {
        
        var newVill = ((GameObject.Instantiate(MaleVillager, new Vector3(Pos.x, Pos.y), Quaternion.identity) as GameObject).GetComponent<Villager>());
        newVill.Initialise(Pos);
        Villagers.Add(newVill);
        AvailableVillagers.Add(newVill);
    }


}
