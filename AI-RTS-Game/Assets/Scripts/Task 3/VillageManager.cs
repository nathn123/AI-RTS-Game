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
    List<GameObject> Villagers;
    char[,] Map, AiMap;

    public enum GlobalObjectives
    {
        IncreasePopulation,
        IncreaseRawResources,
        IncreaseAdvResources,
        ScoutArea,
        IncreaseHousing,
        IncreaseEducation,
        IncreaseMoney,
        TradeResources,
        GoToWar
    }
    public enum TaskObjectives
    {

    }
    public struct GameState
    {

    }
    public struct GoalState
    {

    }


    public enum AI_Bias
    {
        Balanced, // No bias
        Economic, // focus on stocks of resources
        Aggressive, // focus on war
        Explorer, // focus on exploration, possible trade or war
        Turtle // focus on Defence of home but not attack of others
    }

    struct Plans
    {
        public List<List<Vector2>> Paths;
        public List<Villager.Items> ItemsRequired;
        public List<Villager.Skills> SkillsRequired; // .count = villagers required
        public Villager.Actions TasktoBeCompleted;
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
        // after each step  we need to do to ensure it runs quickly
        if (checktime())
            return; 

	
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
        CurrentBias = bias;
        AiMap = AiMap_;
        Villagers = new List<GameObject>();
        Villagers.Add( GameObject.Instantiate(MaleVillager, new Vector3(StartingPos.x,StartingPos.y), Quaternion.identity) as GameObject);
        Villagers.Add( GameObject.Instantiate(FemaleVillager, new Vector3(StartingPos.x, StartingPos.y), Quaternion.identity) as GameObject);
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

}
