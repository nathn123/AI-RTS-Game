using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Building : MonoBehaviour {

    public enum BuildingType
    {
        buildingSite,
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
    };
    public BuildingType Type;
    public Vector2 Position, Dimensions;
    public List<Villager.Items> Items;
	// Use this for initialization
	void Start () {
        Items = new List<Villager.Items>();
        Type = BuildingType.buildingSite;
        this.gameObject.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -1);
	
	}
    public void Initialise(Vector2 Pos,Vector2 Dims, List<Villager.Items> BuildingItems)
    {
        Position = Pos;
        Dimensions = Dims;
        Items.AddRange(BuildingItems);
    }
	
    public void Built(BuildingType type)
    {
        Items.Clear(); // building items used
        Type = type;
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

    public bool ReadyToBuild(Building.BuildingType Type)
    {
        if(Type == BuildingType.Barracks)
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
        else if (Type == BuildingType.Turf_Hut || Type == BuildingType.Quarry)
        {
            return true;
        }
        else if (Type == BuildingType.House)
        {
            if (!Items.Contains(Villager.Items.Stone))
                return false;
            if (!Items.Contains(Villager.Items.Wood))
                return false;
        }
        else if (Type == BuildingType.School)
        {
            if (!Items.Contains(Villager.Items.Stone))
                return false;
            if (!Items.Contains(Villager.Items.Wood))
                return false;
            if (!Items.Contains(Villager.Items.Iron))
                return false;
        }
        else if (Type == BuildingType.Storage)
        {
            if (!Items.Contains(Villager.Items.Stone))
                return false;
            if (!Items.Contains(Villager.Items.Wood))
                return false;
        }
        else if (Type == BuildingType.Mine)
        {
            if (!Items.Contains(Villager.Items.Wood))
                return false;
            if (!Items.Contains(Villager.Items.Iron))
                return false;
        }
        else if (Type == BuildingType.Smelter)
        {
            if (!Items.Contains(Villager.Items.Stone))
                return false;
        }
        else if (Type == BuildingType.Sawmill)
        {
            if (!Items.Contains(Villager.Items.Stone))
                return false;
            if (!Items.Contains(Villager.Items.Iron))
                return false;
            if (!Items.Contains(Villager.Items.Timber))
                return false;
        }
        else if (Type == BuildingType.Blacksmith)
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
}
