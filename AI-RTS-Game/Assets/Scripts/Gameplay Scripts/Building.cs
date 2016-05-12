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
    public Vector2 Position;
    public List<Villager.Items> Items;
	// Use this for initialization
	void Start () {
        Items = new List<Villager.Items>();
        Type = BuildingType.buildingSite;
        this.gameObject.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 1);
	
	}
    public void Initialise(Vector2 Pos, List<Villager.Items> BuildingItems)
    {
        Position = Pos;
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
}
