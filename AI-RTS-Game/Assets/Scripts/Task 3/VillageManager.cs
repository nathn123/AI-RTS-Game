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
    public enum TaskObjectives
    {

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

        starttime = Time.time;
        // after each step  we need to do to ensure it has time availiable
        if (checktime())
            return; 

        //do we have an objective set
        CheckRetrieveTask();
        if (CurrentObj.Count < MaxTasks)
            GenerateNewTask();
        if (checktime())
            return;
        RequestPath();
        
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
    public void Initialise(Vector2 StartingPos,int VillageNum_,AI_Bias bias,TreeManager TreeMgr_,ref char[,] AiMap_)
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
        Villagers.Add((GameObject.Instantiate(MaleVillager, new Vector3(StartingPos.x, StartingPos.y), Quaternion.identity) as GameObject).GetComponent<Villager>() );
        Villagers.Add( (GameObject.Instantiate(FemaleVillager, new Vector3(StartingPos.x, StartingPos.y), Quaternion.identity) as GameObject).GetComponent<Villager>());
        foreach (var vill in Villagers)
            vill.Initialise();
        AvailableVillagers.AddRange(Villagers);
        Initialised = true;
    }


    void PassGoal(GoalState goal, ObjectiveTypes Type)
    {
        ObjectiveTasks newTask = new ObjectiveTasks();
        newTask.Type = Type;
        newTask.PathID = -1;
        newTask.TaskID = TaskPlanner.RequestTask(goal);
        Debug.Log("new task created Type : " + Type.ToString());
        CurrentObj.Add(newTask);
    }
    void RequestPath()
    {
        for (int i = 0; i < CurrentObj.Count; i++)
        {
            if (CurrentObj[i].Stepsneeded == null)
                return;
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
            if(Labs < TotPop *0.25f && Pop < ObjCap && House > 0) // Increase Pop
            {

                // we need to gen a current state / decide what is going to be used here.
                newObj = ObjectiveTypes.IncreasePopulation;
                for (int i = 0; i < 5; i++)
                    newGoal.AddNewSkills(Villager.Skills.Labourer);
                NeededSkills.Add(Villager.Skills.Any);
                NeededSkills.Add(Villager.Skills.Any);
                noneselected = 1;
                Pop++;
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
                    newGoal.NewBuildings = Building.BuildingType.Turf_Hut;
                    var building = GenerateBuildingSite(Building.BuildingType.Turf_Hut);
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
                if(Smelt > 0)
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
                        newGoal.NewBuildings = Building.BuildingType.Market_Stall;
                        NeededSkills.Add(Villager.Skills.Carpenter);
                        NeededSkills.Add(Villager.Skills.Lumberjack);
                        var building = GenerateBuildingSite(Building.BuildingType.Market_Stall);
                        newGoal.Site = building;
                        Buildings.Add(building);
                        Spec++;
                    }
                }
                else // build smelter
                {
                    newGoal.NewBuildings = Building.BuildingType.Smelter;
                    NeededSkills.Add(Villager.Skills.Labourer);
                    var building = GenerateBuildingSite(Building.BuildingType.Smelter);
                    newGoal.Site = building;
                    Buildings.Add(building);
                    Spec++;
                }
            }
            else
                noneselected++;
            if(GenerateGameState(NeededSkills,ref newGoal.GameState,NeedTree))
                PassGoal(newGoal, newObj);
                
        } 
        if (noneselected >= 20)
            return; // no objective set
    }
    bool GenerateGameState(List<Villager.Skills> NeededSkills, ref GameState State, bool Trees)
    {
        // to generate a game state , we mean to generate a list of entities that will be used for a task villagers/ buildings etc
        // we will assume all buildings are free, as there is no way to run plans concurrently and be able to check if the building is free at the time it is needed in that task
        // first thing we need it to test is, if there are villagers avaliable
        int runcount = 0;
        do
        {
            for (int j = 0; j < AvailableVillagers.Count; j++)
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
        } while (NeededSkills.Count > 0 && runcount > (NeededSkills.Count * 2) * AvailableVillagers.Count);
        if (NeededSkills.Count > 1)
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
                CurrentObj.RemoveAt(i);
        }
    }
    Building GenerateBuildingSite(Building.BuildingType Type)
    {
        Building NewBuilding = new Building();
        Vector2 NewPos = new Vector2(), dimensions = new Vector2();
        // need to setup building site somewhere on the map
        if (Type == Building.BuildingType.Barracks || Type == Building.BuildingType.School || Type == Building.BuildingType.Storage || Type == Building.BuildingType.Sawmill || Type == Building.BuildingType.Blacksmith)
        {
            dimensions = new Vector2(5, 5);
        }
        else if (Type == Building.BuildingType.Turf_Hut || Type == Building.BuildingType.Smelter)
        {
            dimensions = new Vector2(1,1);
        }
        else if (Type == Building.BuildingType.House)
        {
            dimensions = new Vector2(2, 3);
        }
        else if (Type == Building.BuildingType.Mine || Type == Building.BuildingType.Quarry || Type == Building.BuildingType.Market_Stall)
        {
            dimensions = new Vector2(1, 1);
        }
        // now we search area near the centre of base
        int range = 20;
        bool FoundPos = false;
        do
	    {
            for (int i = -range; i < range; i++)
            {
                if (FoundPos)
                    break;
                for (int j = -range; i < range; j++)
                    if (CheckMapArea(new Vector2(i, j), dimensions))
                    {
                        FoundPos = true;
                        NewPos = new Vector2(i, j);
                        break;
                    }
            }
            range += 20;
	    } while (!FoundPos);
        NewBuilding.Initialise(NewPos,dimensions, new List<Villager.Items>());
        return NewBuilding;
    }
    bool CheckMapArea(Vector2 Pos, Vector2 Dimensions)
    {
        return true;
    }
    bool ObjectiveCompleted(ObjectiveTasks Current)
    {
        if (Current.Stepsneeded == null)
            return false;
        foreach (var steps in Current.Stepsneeded)
            if (steps.complete != true)
                return false;
        Current.complete = true;
        return true;
    }

}
