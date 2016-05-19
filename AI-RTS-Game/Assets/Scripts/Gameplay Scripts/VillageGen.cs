using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VillageGen : MonoBehaviour {
    //min 1 - max 4
    public int NumofVillages;
    public List<Vector2> StartingLocations;
    public List<VillageManager> TaskExecutives;
    public TreeManager TreeMgr;
    public Scenario NewScenario;
    public GameObject Panel;
    public struct Scenario
    {
        public int Villagers, Turf, House, School, Barracks, Storage,
            Mine, Smelter, Quarry, Saw, BlackS, Market, Xpos, Ypos;
        public bool Created;

    }
	// Use this for initialization
	void Start () {
        TaskExecutives = new List<VillageManager>();
        TreeMgr = new TreeManager();
	
	}
    public void StartGame()
    {
        Panel.SetActive( false);
        char[,] AiMap = new char[1, 1];
        AiMap = MapGen.GetMap();
        TreeMgr.GenerateTrees(AiMap);
        for (int i = 0; i < NumofVillages; ++i)
        {
            VillageManager newmanager = new VillageManager();
            if(i == 0 && NewScenario.Created)
                newmanager.Initialise(StartingLocations[i], i, VillageManager.AI_Bias.Balanced,TreeMgr, ref AiMap,NewScenario);
            else
                newmanager.Initialise(StartingLocations[i], i, VillageManager.AI_Bias.Balanced, TreeMgr, ref AiMap);
            TaskExecutives.Add(newmanager);
        }
    }
    public void CreateScenario()
    {
        NewScenario = new Scenario();
        var inputs = Panel.GetComponentsInChildren<UnityEngine.UI.InputField>();
        foreach(var val in inputs)
        {
            if (val.name.Split('_')[0] == "Barrack")
              int.TryParse(val.text,out NewScenario.Barracks );
            else if (val.name.Split('_')[0] == "BlackS")
                int.TryParse(val.text, out NewScenario.BlackS);
            else if (val.name.Split('_')[0] == "House")
                int.TryParse(val.text, out NewScenario.House);
            else if (val.name.Split('_')[0] == "Market")
                int.TryParse(val.text, out NewScenario.Market);
            else if (val.name.Split('_')[0] == "Mine")
                int.TryParse(val.text, out NewScenario.Mine);
            else if (val.name.Split('_')[0] == "Quarry")
                int.TryParse(val.text, out NewScenario.Quarry);
            else if (val.name.Split('_')[0] == "Saw")
                int.TryParse(val.text, out NewScenario.Saw);
            else if (val.name.Split('_')[0] == "School")
                int.TryParse(val.text, out NewScenario.School);
            else if (val.name.Split('_')[0] == "Smelter")
                int.TryParse(val.text, out NewScenario.Smelter);
            else if (val.name.Split('_')[0] == "Storage")
                int.TryParse(val.text, out NewScenario.Storage);
            else if (val.name.Split('_')[0] == "Turf")
                int.TryParse(val.text, out NewScenario.Turf);
            else if (val.name.Split('_')[0] == "Vill")
                int.TryParse(val.text, out NewScenario.Villagers);
            else if (val.name.Split('_')[0] == "X")
                int.TryParse(val.text, out NewScenario.Xpos);
            else if (val.name.Split('_')[0] == "Y")
                int.TryParse(val.text, out NewScenario.Ypos);
            NewScenario.Created = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
        foreach (var village in TaskExecutives)
            village.Update();
        TreeMgr.Update();
	
	}
}
