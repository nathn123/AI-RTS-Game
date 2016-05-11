using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VillageManager : MonoBehaviour {

    public GameObject MaleVillager;
    public GameObject FemaleVillager;

    PathPlanning PathPlanner;
    TaskPlanning TaskPlanner;
    public GlobalObjectives CurrentObj;
    public AI_Bias CurrentBias;
    bool Initialised;
    List<Villager> Villagers;
    List<Building> Buildings;
    float starttime, allowedtime;
    List<KeyValuePair<int,List<TaskPlanning.Task>>> Tasks;

    char[,] Map, AiMap;

    public enum GlobalObjectives
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
        List<Building.BuildingType> NewBuildings;
        List<Villager.Items> NewItems;
        List<Villager.Skills> NewSkills;
        // only storing new values otherwise we would have to calculate the future gamestate too much effort
    }
    public enum AI_Bias
    {
        Balanced, // No bias
        Economic, // focus on stocks of resources
        Aggressive, // focus on war
        Explorer, // focus on exploration, possible trade or war
        Turtle // focus on Defence of home but not attack of others
    }
    private struct ObjectiveTargets
    {

    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
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
        if (CurrentObj == GlobalObjectives.None)
            PickNewGlobalObjective();
        if (checktime())
            return;
        // must have a global objective / now we need to set sub-objectives ????

        if (ObjectiveCompleted())
            CurrentObj = GlobalObjectives.None;
	
	}
        bool checktime()
    {
        if (Time.time - starttime > allowedtime)
            return true;
        return false;
    }
    public void Initialise(Vector2 StartingPos,int VillageNum,AI_Bias bias,ref char[,] AiMap_)
    {
        if(VillageNum == 1)
        {
            MaleVillager = Resources.Load<GameObject>("Characters/Prefabs/Male 1.prefab");
            FemaleVillager = Resources.Load<GameObject>("Characters/Prefabs/Female 1.prefab");
        }
        else if (VillageNum == 2)
        {
            MaleVillager = Resources.Load<GameObject>("Characters/Prefabs/Male 2.prefab");
            FemaleVillager = Resources.Load<GameObject>("Characters/Prefabs/Female 2.prefab");
        }
        else if (VillageNum == 3)
        {
            MaleVillager = Resources.Load<GameObject>("Characters/Prefabs/Male 3.prefab");
            FemaleVillager = Resources.Load<GameObject>("Characters/Prefabs/Female 3.prefab");
        }
        else if (VillageNum == 4)
        {
            MaleVillager = Resources.Load<GameObject>("Characters/Prefabs/Male 4.prefab");
            FemaleVillager = Resources.Load<GameObject>("Characters/Prefabs/Female 4.prefab");
        }
        TaskPlanner = new TaskPlanning();
        PathPlanner = new PathPlanning();
        CurrentObj = GlobalObjectives.None;
        CurrentBias = bias;
        AiMap = AiMap_;
        Villagers = new List<Villager>();
        
        Villagers.Add((GameObject.Instantiate(MaleVillager, new Vector3(StartingPos.x, StartingPos.y), Quaternion.identity) as GameObject).GetComponent<Villager>() );
        Villagers.Add( (GameObject.Instantiate(FemaleVillager, new Vector3(StartingPos.x, StartingPos.y), Quaternion.identity) as GameObject).GetComponent<Villager>());
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
        var taskID = TaskPlanner.RequestTask(goal);
        KeyValuePair<int, List<TaskPlanning.Task>> newTask = new KeyValuePair<int, List<TaskPlanning.Task>>(taskID, null);
        Tasks.Add(newTask);
    }
    void CheckRetrieveTask()
    {
        for (int i = 0; i < Tasks.Count;i++ )
        {
            if (Tasks[i].Value == null)
                if (TaskPlanner.TaskReady(Tasks[i].Key))
                    Tasks[i] = new KeyValuePair<int, List<TaskPlanning.Task>>(Tasks[i].Key, TaskPlanner.GetPlan(Tasks[i].Key));
        }
    }

    void PickNewGlobalObjective()
    {

    }

    bool ObjectiveCompleted()
    {

        return true;
    }

}
