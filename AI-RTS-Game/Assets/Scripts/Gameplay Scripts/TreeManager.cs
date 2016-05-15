using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeManager : MonoBehaviour {

    Tree[,] TotalTrees;
    List<Tree> ActiveTrees;
    float LastRefresh;
	// Use this for initialization
	void Start () {
        
	
	}
	public void GenerateTrees(char[,] Map)
    {

        TotalTrees = new Tree[Map.GetLength(0),Map.GetLength(1)];
        for(int i = 0; i < Map.GetLength(0); i++)
        {
            for(int j = 0; j < Map.GetLength(1);j++)
            {
                if (Map[i, j] == 'T')
                    TotalTrees[i, j] = new Tree(i,j);
            }
        }
        ActiveTrees = new List<Tree>();
    }
	// Update is called once per frame
	public void Update () {

        if ((Time.time - LastRefresh) >= 10.0f)
        {
            foreach (var Tree in ActiveTrees)
                Tree.Replace();
            LastRefresh = Time.time;
        }
        foreach(var tree in TotalTrees)
        {
            if (tree.Full() && ActiveTrees.Contains(tree))
                ActiveTrees.Remove(tree);
            else if (!tree.Full() && !ActiveTrees.Contains(tree))
                ActiveTrees.Add(tree);
        }
	}
    public Tree FindNearestTree(Vector2 Pos)
    {
        int Range = 20;
        Tree ClosestTree = null;
        float bestDistVal = float.MaxValue;
        for(int i = -Range; i < Range; i++)
        {
            for(int j = -Range; j < Range; j++)
            {
                if (TotalTrees[(int)Pos.x + i, (int)Pos.y + j] == null)
                    continue;
                if(Vector2.Distance(Pos, TotalTrees[(int)Pos.x + i, (int)Pos.y + j].Pos) < bestDistVal)
                {
                    ClosestTree = TotalTrees[(int)Pos.x + i, (int)Pos.y + j];
                    bestDistVal = Vector2.Distance(Pos, TotalTrees[(int)Pos.x + i, (int)Pos.y + j].Pos);
                }
            }
        }
        return ClosestTree;
    }
}
