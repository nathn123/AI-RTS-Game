using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Building {

    public enum BuildingType
    {
        buildingSite,
        Turf,
        House,
        School,
        Barracks,
        Storage,
        Mine,
        Smelter,
        Quarry,
        Sawmill,
        Blacksmith,
        Market,
        None
    };
    public BuildingType Type, FType;
    public Vector2 Position, Dimensions;
    public List<Villager.Items> Items;
    public List<Villager> VillgersNeeded;
    public bool IsBuilt;
    public void Initialise(Vector2 Pos,Vector2 Dims, BuildingType Ftype_)
    {
        Items = new List<Villager.Items>();
        Type = BuildingType.buildingSite;
        FType = Ftype_;
        Position = Pos;
        Dimensions = Dims;
        IsBuilt = false;
    }
	
    public void Built()
    {
        Items.Clear(); // building items used
        Type = FType;
        IsBuilt = true;
    }
    public bool PickUp(Villager.Items item)
    {
        if (!Items.Contains(item))
            return false;
        Items.Remove(item);
        return true;
    }
    public void DropOff(Villager.Items item)
    {
        Items.Add(item);
    }
    void PerformAction()
    {
        if (VillgersNeeded.Count == 0)
            return;
        var Action = VillgersNeeded[0].CurrentAction;

        if (Action == Villager.Actions.Family || Action == Villager.Actions.Family_House)
        {
            if(VillgersNeeded.Count >= 2)
            {
                VillgersNeeded[0].StartTimer();
                VillgersNeeded[1].StartTimer();
            }
        }
        else if(Action == Villager.Actions.Make_Tool)
        {

        }
        else if (Action == Villager.Actions.Educate_Barracks)
        {

        }
        else if (Action == Villager.Actions.Train)
        {

        }
        else if (Action == Villager.Actions.Saw_Wood)
        {

        }
        else if (Action == Villager.Actions.Smelt)
        {

        }
        else if (Action == Villager.Actions.Mine)
        {

        }
        else if (Action == Villager.Actions.Quarry)
        {

        }
        else if (Action == Villager.Actions.Buy_Sell)
        {

        }



    }
    public void VillagerArrived(Villager newVil)
    {
        VillgersNeeded.Add(newVil);
    }
    public bool ReadyToBuild()
    {
        if (FType == BuildingType.Barracks)
        {
            if (!Items.Contains(Villager.Items.Stone))
                return false;
            if (!Items.Contains(Villager.Items.Wood))
                return false;
            if (!Items.Contains(Villager.Items.Iron))
                return false;
            if (!Items.Contains(Villager.Items.Timber))
                return false;
        }
        else if (FType == BuildingType.Turf || FType == BuildingType.Quarry)
        {
            return true;
        }
        else if (FType == BuildingType.House)
        {
            if (!Items.Contains(Villager.Items.Stone))
                return false;
            if (!Items.Contains(Villager.Items.Wood))
                return false;
        }
        else if (FType == BuildingType.School)
        {
            if (!Items.Contains(Villager.Items.Stone))
                return false;
            if (!Items.Contains(Villager.Items.Wood))
                return false;
            if (!Items.Contains(Villager.Items.Iron))
                return false;
        }
        else if (FType == BuildingType.Storage)
        {
            if (!Items.Contains(Villager.Items.Stone))
                return false;
            if (!Items.Contains(Villager.Items.Wood))
                return false;
        }
        else if (FType == BuildingType.Mine)
        {
            if (!Items.Contains(Villager.Items.Wood))
                return false;
            if (!Items.Contains(Villager.Items.Iron))
                return false;
        }
        else if (FType == BuildingType.Smelter)
        {
            if (!Items.Contains(Villager.Items.Stone))
                return false;
        }
        else if (FType == BuildingType.Sawmill)
        {
            if (!Items.Contains(Villager.Items.Stone))
                return false;
            if (!Items.Contains(Villager.Items.Iron))
                return false;
            if (!Items.Contains(Villager.Items.Timber))
                return false;
        }
        else if (FType == BuildingType.Blacksmith)
        {
            if (!Items.Contains(Villager.Items.Stone))
                return false;
            if (!Items.Contains(Villager.Items.Iron))
                return false;
            if (!Items.Contains(Villager.Items.Timber))
                return false;
        }
        return true;
    }

    public void Saw()
    {
        Items.Remove(Villager.Items.Timber);
        Items.Add(Villager.Items.Wood);
    }
    public void Smelt()
    {
        Items.Remove(Villager.Items.Ore);
        Items.Remove(Villager.Items.Coal);
        Items.Add(Villager.Items.Iron);
    }
    public void Store(Villager.Items item)
    { }
}
