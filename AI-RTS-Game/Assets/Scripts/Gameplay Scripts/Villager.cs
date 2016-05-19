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
        Carpenter,
        Any
    };
    public enum Actions
    {
        None,
        Walk,
        Pickup,
        Putdown,
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
        Combat,
        Build
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
        Goods,
        Empty

    };
    public enum Map_Items
    {
        Tree
    }
    public List<Vector2> Path;
    public Vector2 Position;
    public Vector2 Goal;

    public Actions CurrentAction;
    public Building LocationAction;
    public Items ItemAction;
    public Tree ToCut;
    public VillageManager.NewVillager Preg;
    public TaskPlanning.Task.ActionComplete ActionCallback;
    public Skills Skill;
    public bool ActionComplete;
    bool Initialised = false, Timer = false;
    public float MoveSpeed = 1.0f;
    public float CurrentActionTime;
    public float ActionTime;
    public int FOV;
    public Items Inventory;
	// Use this for initialization
	void Start () {

        
	}
    public void Initialise(Vector2 Pos)
    {
        Path = new List<Vector2>();
        CurrentAction = Actions.None;
        Skill = Skills.Labourer;
        Inventory = Items.Empty;
        Position = Pos;
        Initialised = true;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (!Initialised)
            return;
        // if it has a path it should move
        if (Path.Count > 0)
            Move();
        if (CurrentAction == Actions.Walk && Path.Count == 0)
        {
            CurrentAction = Actions.None;
            ActionComplete = true;
        }
        // if at path goal then do Action
        if (CurrentAction != Actions.Walk && CurrentActionTime >= ActionTime)
            DoAction();

        if (Timer)
            CurrentActionTime += Time.deltaTime;
        transform.position = new Vector3(Position.x, Position.y, -2);
	}
    public bool AddPath(List<Vector2> newPath)
    {
        Path.AddRange(newPath);
        CurrentAction = Actions.Walk;
        return true;
    }
    public void SetAction(Villager.Actions NewAction,float ActionTime_ = 0)
    {
        CurrentActionTime = 0;
        CurrentAction = NewAction;
        ActionTime = ActionTime_;
        Timer = true;
    }
    public void SetAction(Villager.Actions NewAction,Tree TreeInUse, float ActionTime_ = 0)
    {
        CurrentActionTime = 0;
        CurrentAction = NewAction;
        ActionTime = ActionTime_;
        ToCut = TreeInUse;
        Timer = true;
    }
    public void SetAction(Villager.Actions NewAction, Building Location, Items Item, float ActionTime_ = 0)
    {
        Timer = true;
        CurrentActionTime = 0;
        ActionTime = ActionTime_;
        // for pick up and put down Is instant
        if (NewAction == Actions.Pickup)
            if (Location.PickUp(Item))
                Inventory = Item;
        else if (NewAction == Actions.Putdown)
        {
            Location.DropOff(Item);
            Inventory = Items.Empty;
        }
        //blacksmith production
        if(Item == Items.Axe)
        { }
        else if (Item == Items.Cart)
        { }
        else if (Item == Items.Rifle)
        { }

    }
    public void SetAction(Villager.Actions NewAction,Building Location,float ActionTime_ = 0)
    {
        ActionTime = ActionTime_;
        CurrentActionTime = 0;
        CurrentAction = NewAction;
        LocationAction = Location;
        Timer = true;
    }
    public void LearnSkill(Villager.Skills newSkills, Villager Teach)
    {
        Skill = newSkills;
        Teach.StartTimer();
        StartTimer();
    }
    //public delegate void StartTimer();
    //private StartTimer Callback;
    public void StartTimer()
    {
        Timer = true;
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
        Debug.Log("Villager Action");
        // not yet implemented
        // checks on the current square - prerequisites for action
        if(ActionComplete)
        {
            CurrentAction = Actions.None;
            if(ActionCallback != null)
                ActionCallback();
            ActionCallback = null;
            ActionComplete = false;
            LocationAction = null;
            Timer = false;
            
            return;
        }


        if (CurrentAction == Actions.Buy_Sell)
        {

        }
        if (CurrentAction == Actions.Combat)
        {

        }
        if (CurrentAction == Actions.Build)
        {
            LocationAction.Built();
            ActionComplete = true;
        }
        if (CurrentAction == Actions.Cut_Tree)
        {
            // need to add a ref to the physical tree
            if (ToCut != null)
                ToCut.CUT();
            Inventory = Items.Timber;
            ActionComplete = true;
        }
        if (CurrentAction == Actions.Educate)
        {
            // need a ref to the other student
            ActionComplete = true; // the reason for this is because we already assume both are correct in POS
            // and timer has started and conculded to get to this case , and skill has been assigned
        }
        if (CurrentAction == Actions.Educate_Barracks)
        {
            ActionComplete = true; // as above
        }
        if (CurrentAction == Actions.Family)
        {
            if (Preg != null)
                Preg(Position);
            Preg = null;
            ActionComplete = true;
        }
        if (CurrentAction == Actions.Family_House)
        {
            if (Preg != null)
                Preg(Position);
            Preg = null;
            ActionComplete = true;
        }
        if (CurrentAction == Actions.Make_Tool)
        {
            LocationAction.DropOff(ItemAction); // add the item to the smithy
            ActionComplete = true;
        }
        if (CurrentAction == Actions.Mine)
        {
            Inventory = ItemAction;
            ActionComplete = true;
        }
        if (CurrentAction == Actions.Quarry)
        {
            Inventory = ItemAction;
            ActionComplete = true;
        }
        if (CurrentAction == Actions.Saw_Wood)
        {
            LocationAction.Saw();
            ActionComplete = true;
        }
        if (CurrentAction == Actions.Smelt)
        {
            LocationAction.Smelt();
            ActionComplete = true;
        }
        if (CurrentAction == Actions.Store)
        {
            LocationAction.Store(ItemAction);
            ActionComplete = true;
        }
        if (CurrentAction == Actions.Train)
        {
            ActionComplete = true; // the reason for this is because we already assume both are correct in POS
            // and timer has started and conculded to get to this case , and skill has been assigned
        }
    }

}

