using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Villager : MonoBehaviour {

    //possible to move these enums elsewhere
    public enum Skills
    {
        Labourer,
        Rifleman,
        Trader,
        Blacksmith,
        Miner,
        Lumberjack,
        Carpenter
    };
    public enum Actions
    {
        Family,
        Family_House,
        Educate,
        Educate_Barracks,
        Train,
        Cut_Tree,
        Mine,
        Store,
        Smelt,
        Quarry,
        Saw_Wood,
        Make_Tool,
        Buy_Sell,
        Combat
    };
    public enum Items
    {
        Stone,
        Wood,
        Iron,
        Timber,
        Ore,
        Coal,
        Axe,
        Cart, // possible should be a special case as it allows more carry capacity
        Rifle,
        Money,
        Goods

    };
    public enum Map_Items
    {
        Tree,
        Turf_Hut,
        House,
        School,
        Barracks,
        Storage,
        Mine,
        Smelter,
        Quarry,
        Sawmill,
        Blacksmith,
        Market_Stall
    }
    public List<Vector2> Path;
    public Vector2 Position;
    public Vector2 Goal;
    public Actions CurrentAction;
    public Skills Skill;
    public bool ActionComplete;
    public float MoveSpeed;
    public float CurrentActionTime;
    public float ActionTime;
    public int FOV;
    public Items Inventory;
	// Use this for initialization
	void Start () {
        Path = new List<Vector2>();
        Skill = Skills.Labourer;
	}
	
	// Update is called once per frame
	void Update () 
    {
        // if it has a path it should move
        if (Path.Count > 0)
            Move();
        // if at path goal then do Action
        if (Position == Goal)
            DoAction();
	}
    public bool AddPath(List<Vector2> newPath)
    {
        if (newPath.Count < 1)
            return false;
        Vector2 newPosition = newPath[0];
        // first check that this path is possible
        // i.e. that the path starts at or next to the current position
        if (!CheckPath(newPath[0]))
            return false;
        // now add to current path
        Path.AddRange(newPath);
        return true;
    }
    bool CheckPath(Vector2 startingpos)
    {
        if (new Vector2(startingpos.x + 1, startingpos.y) == Position) // right
            return true;
        if (new Vector2(startingpos.x + 1, startingpos.y + 1) == Position) // up right
            return true;
        if (new Vector2(startingpos.x + 1, startingpos.y - 1) == Position) // down right
            return true;
        if (new Vector2(startingpos.x - 1, startingpos.y) == Position) // left
            return true;
        if (new Vector2(startingpos.x - 1, startingpos.y - 1) == Position) // down left
            return true;
        if (new Vector2(startingpos.x - 1, startingpos.y + 1) == Position) // up left
            return true;
        if (startingpos == Position) // centre
            return true;
        if (new Vector2(startingpos.x, startingpos.y - 1) == Position) // down
            return true;
        if (new Vector2(startingpos.x, startingpos.y + 1) == Position) // up
            return true;

        return false;
    }
    void Move()
    {
        // move at a set speed towards next path goal
        // assume that path is correctly calculated
        var directionVector = Path[0] - Position;
        directionVector.Normalize();
        Position += directionVector * MoveSpeed;
        // once arrived remove path from goal
        if (Position == Path[0])
            Path.RemoveAt(0);
    }
    // possible abstraction from this class
    // especially due to the creation of villagers requiring 2 people, creating more than allowed
    // should we move this to a building script ??????
    public void DoAction() 
    {
        // not yet implemented
        // checks on the current square - prerequisites for action
        if (CurrentAction == Actions.Buy_Sell && ActionComplete == false)
        {
            // first check that square is correct
            if (!CheckMap(Map_Items.Market_Stall, Position))
                return; // if the building required doesnt exist then return
            // second check the required people are here
            //third check the required resources are here
        }
        // this one doesnt require a building but requires enemies present
       // if (CurrentAction == Actions.Combat && ActionComplete == false)
       // {
            // first check that square is correct
      //      if (!CheckMap(Map_Items.Turf_Hut, Position))
       //         return; // if the building required doesnt exist then return
            // second check the required people are here
            //third check the required resources are here
      //  }
        if (CurrentAction == Actions.Cut_Tree && ActionComplete == false)
        {
            // first check that square is correct
            if (!CheckMap(Map_Items.Tree, Position))
                return; // if the building required doesnt exist then return
            // second check the required people are here
            //third check the required resources are here        
        }
        if (CurrentAction == Actions.Educate && ActionComplete == false)
        {
            // second check the required people are here

            //third check the required resources are here

        }
        if (CurrentAction == Actions.Educate_Barracks && ActionComplete == false)
        {
            // first check that square is correct
            if (!CheckMap(Map_Items.Barracks, Position))
                return; // if the building required doesnt exist then return
            // second check the required people are here
            //third check the required resources are here
        }
        if (CurrentAction == Actions.Family && ActionComplete == false)
        {
            // first check that square is correct
            if (!CheckMap(Map_Items.Turf_Hut, Position))
                return; // if the building required doesnt exist then return
            // second check the required people are here
            //third check the required resources are here
        }
        if (CurrentAction == Actions.Family_House && ActionComplete == false)
        {
            // first check that square is correct
            if (!CheckMap(Map_Items.House, Position))
                return; // if the building required doesnt exist then return
            // second check the required people are here
            //third check the required resources are here
        }
        if (CurrentAction == Actions.Make_Tool && ActionComplete == false)
        {
            // first check that square is correct
            if (!CheckMap(Map_Items.Blacksmith, Position))
                return; // if the building required doesnt exist then return
            // second check the required people are here
            //third check the required resources are here
        }
        if (CurrentAction == Actions.Mine && ActionComplete == false)
        {
            // first check that square is correct
            if (!CheckMap(Map_Items.Mine, Position))
                return; // if the building required doesnt exist then return
            // second check the required people are here
            //third check the required resources are here
        }
        if (CurrentAction == Actions.Quarry && ActionComplete == false)
        {
            // first check that square is correct
            if (!CheckMap(Map_Items.Quarry, Position))
                return; // if the building required doesnt exist then return
            // second check the required people are here
            //third check the required resources are here
        }
        if (CurrentAction == Actions.Saw_Wood && ActionComplete == false)
        {
            // first check that square is correct
            if (!CheckMap(Map_Items.Sawmill, Position))
                return; // if the building required doesnt exist then return
            // second check the required people are here
            //third check the required resources are here
        }
        if (CurrentAction == Actions.Smelt && ActionComplete == false)
        {
            // first check that square is correct
            if (!CheckMap(Map_Items.Smelter, Position))
                return; // if the building required doesnt exist then return
            // second check the required people are here
            //third check the required resources are here
        }
        if (CurrentAction == Actions.Store && ActionComplete == false)
        {
            // first check that square is correct
            if (!CheckMap(Map_Items.Storage, Position))
                return; // if the building required doesnt exist then return
            // second check the required people are here
            //third check the required resources are here
        }
        if (CurrentAction == Actions.Train && ActionComplete == false)
        {
            // first check that square is correct
            if (!CheckMap(Map_Items.School, Position))
                return; // if the building required doesnt exist then return
            // second check the required people are here
            //third check the required resources are here
        }
        // fourth start the timer
        if (CurrentActionTime >= ActionTime)
            ActionComplete = true;
        // finally once time is complete remove required resources if needed
        // add produced resources
        if (CurrentAction == Actions.Buy_Sell && ActionComplete == true)
        { }
        if (CurrentAction == Actions.Combat && ActionComplete == true)
        { }
        if (CurrentAction == Actions.Cut_Tree && ActionComplete == true)
        { }
        if (CurrentAction == Actions.Educate && ActionComplete == true)
        { }
        if (CurrentAction == Actions.Educate_Barracks && ActionComplete == true)
        { }
        if (CurrentAction == Actions.Family && ActionComplete == true)
        { }
        if (CurrentAction == Actions.Family_House && ActionComplete == true)
        { }
        if (CurrentAction == Actions.Make_Tool && ActionComplete == true)
        { }
        if (CurrentAction == Actions.Mine && ActionComplete == true)
        { }
        if (CurrentAction == Actions.Quarry && ActionComplete == true)
        { }
        if (CurrentAction == Actions.Saw_Wood && ActionComplete == true)
        { }
        if (CurrentAction == Actions.Smelt && ActionComplete == true)
        { }
        if (CurrentAction == Actions.Store && ActionComplete == true)
        { }
        if (CurrentAction == Actions.Train && ActionComplete == true)
        { }
    }
    public bool CheckMap(Map_Items Check ,Vector2 pos)
    {
        char[,] map = new char[1,1]; // temp until map storage is decieded
        // if the map location contains the equavalent enum then return true
        if (Check == Map_Items.Barracks && map[(int)pos.x, (int)pos.y] == 'B')
            return true;
        if (Check == Map_Items.Blacksmith && map[(int)pos.x, (int)pos.y] == 'L')
            return true;
        if (Check == Map_Items.House && map[(int)pos.x, (int)pos.y] == 'H')
            return true;
        if (Check == Map_Items.Market_Stall && map[(int)pos.x, (int)pos.y] == 'M')
            return true;
        if (Check == Map_Items.Mine && map[(int)pos.x, (int)pos.y] == 'N')
            return true;
        if (Check == Map_Items.Quarry && map[(int)pos.x, (int)pos.y] == 'Q')
            return true;
        if (Check == Map_Items.Sawmill && map[(int)pos.x, (int)pos.y] == 'S')
            return true;
        if (Check == Map_Items.School && map[(int)pos.x, (int)pos.y] == 'C')
            return true;
        if (Check == Map_Items.Smelter && map[(int)pos.x, (int)pos.y] == 'M')
            return true;
        if (Check == Map_Items.Storage && map[(int)pos.x, (int)pos.y] == 'T')
            return true;
        if (Check == Map_Items.Turf_Hut && map[(int)pos.x, (int)pos.y] == 'U')
            return true;

        return false;
    }

}

