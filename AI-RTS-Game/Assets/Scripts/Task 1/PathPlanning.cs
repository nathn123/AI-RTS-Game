
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathPlanning
{

    //struct with required path info
    char[,] Map; // is stored as ref between all that read it to ensure that the latest copy is always avaliable
    public struct PathInfo
    {
        public Vector2 Start;
        public Vector2 End;
        public List<Vector2> Path;
        public bool Complete;

        public void SetPath(List<Vector2> NewPath)
        {
            Path = NewPath;
            Complete = true;
        }
    }
    public struct Node
    {
        public int cost;
        public float dist;
        public float total;
        public Vector2 Pos;
        public int parent; // refers to the number in the list
        public void CalcTotal()
        {
            total = cost + dist;
        }
    }
    List<Node> PotentialNodes;
    Dictionary<int,PathInfo> Paths;


    // Use this for initialization
    void Start()
    {

    }
    public void Initialise(ref char[,] AIMAP)
    {
        Map = AIMAP;
        Paths = new Dictionary<int, PathInfo>();
    }

    // Update is called once per frame
    public void Update()
    {
        Map = MapGen.GetMap();

        for (int i = 0; i < Paths.Count; i++)
            if (!Paths[i].Complete)
                StartPathPlanning(i);
    }

    public PathInfo GetPath(int pathID)
    {
        var temp = Paths[pathID];
        Paths.Remove(pathID);
        return temp;
    }
    public bool PathReady(int pathID)
    {
        //case to check if path is done then return result
        // if struct is used for path info should be easy
        if (!Paths.ContainsKey(pathID))
            return false;
        return Paths[pathID].Complete;
    }
    public int AddPath(PathInfo newPath)
    {
        // add the struct 
        // get the value added at and then return

        //special case as trees are not walkable so
        if (!Walkable(Map, newPath.End))
            newPath.End = ClosestWalkable(newPath.End);
        Paths.Add(Paths.Count,newPath);
        return Paths.Count - 1;
    }
    public static bool Walkable(char[,] map, Vector2 Pos)
    {
        if (Pos.x > map.Length || Pos.y > map.Length)
            return false;
        if (Pos.x < 0 || Pos.y < 0)
            return false;
        char Loc = map[(int)Pos.x, (int)Pos.y];
        if (Loc == '.' || Loc == 'G' || Loc == 'E')
        {
            // walkable tile NOT GRASS
            return true;
        }
        // EDIT THIS IF NEEDED
        return false;
    }
    public static bool Buildable(char[,] map, Vector2 Pos)
    {
        if (Pos.x > map.Length || Pos.y > map.Length)
            return false;
        if (Pos.x < 0 || Pos.y < 0)
            return false;
        char Loc = map[(int)Pos.x, (int)Pos.y];
        if (Loc == '.' || Loc == 'G')
        {
            // walkable tile NOT GRASS
            return true;
        }
        // EDIT THIS IF NEEDED
        return false;
    }
    Vector2 ClosestWalkable(Vector2 Pos)
    {
        Vector2 Closest = Pos;
        int range  = 1;
        bool done = false;
        while(Walkable(Map,Closest) )
        {
            range++;
            for (int i = -range; i < range; i++)
            {
                for (int j = -range; j < range; j++)
                {
                    if (Walkable(Map, new Vector2(Closest.x + i, Closest.y + j)))
                    {
                        Closest = new Vector2(Closest.x + i, Closest.y + j);
                        done = true;
                        break;

                    }
                }
                if (done)
                    break;
            }
        }

        return Closest;
    }
    Node GenerateNewNodes(Node NewNodeStart, List<Node> PrevNodes, Vector2 Goal)
    {
        List<Node> NewNodes = new List<Node>();
        Node Best = new Node(), Top = new Node(), Left = new Node(), Bot = new Node(), Right = new Node();
        // first gen ne nodes possible
        if (Walkable(Map,new Vector2(NewNodeStart.Pos.x,NewNodeStart.Pos.y + 1)))
            Top.Pos = new Vector2(NewNodeStart.Pos.x, NewNodeStart.Pos.y + 1);

        if (Walkable(Map, new Vector2(NewNodeStart.Pos.x,NewNodeStart.Pos.y - 1)))
            Bot.Pos = new Vector2(NewNodeStart.Pos.x, NewNodeStart.Pos.y - 1);

        if (Walkable(Map,new Vector2(NewNodeStart.Pos.x - 1,NewNodeStart.Pos.y)))
            Left.Pos = new Vector2(NewNodeStart.Pos.x - 1, NewNodeStart.Pos.y);

        if (Walkable(Map,new Vector2(NewNodeStart.Pos.x + 1,NewNodeStart.Pos.y)))
            Right.Pos = new Vector2(NewNodeStart.Pos.x + 1, NewNodeStart.Pos.y);
        Top.dist = CalculateHValue(Top, Goal);
        Bot.dist = CalculateHValue(Bot, Goal);
        Left.dist = CalculateHValue(Left, Goal);
        Right.dist = CalculateHValue(Right, Goal);

        Top.cost = NewNodeStart.cost + 1;
        Bot.cost = NewNodeStart.cost + 1;
        Left.cost = NewNodeStart.cost + 1;
        Right.cost = NewNodeStart.cost + 1;
        Top.CalcTotal();
        Bot.CalcTotal();
        Left.CalcTotal();
        Right.CalcTotal();
        bool duplicate = false;
        foreach (var node in PrevNodes)
        {
            if (Top.Pos == node.Pos)
            {
                duplicate = true;
                break;
            }
        }
        if (!duplicate)
            NewNodes.Add(Top);
        duplicate = false;
        foreach (var node in PrevNodes)
        {
            if (Bot.Pos == node.Pos)
            {
                duplicate = true;
                break;
            }
        }
        if (!duplicate)
            NewNodes.Add(Bot);
        duplicate = false;
        foreach (var node in PrevNodes)
        {
            if (Left.Pos == node.Pos)
            {
                duplicate = true;
                break;
            }
        }
        if (!duplicate)
            NewNodes.Add(Left);
        duplicate = false;
        foreach (var node in PrevNodes)
        {
            if (Right.Pos == node.Pos)
            {
                duplicate = true;
                break;
            }
        }
        if (!duplicate)
            NewNodes.Add(Right);
        Best.total = float.MaxValue;
        for (int i = 0; i < NewNodes.Count; i++)
        {
            if (NewNodes[i].total < Best.total)
                Best = NewNodes[i];
        }
        Best.parent = PrevNodes.Count;
        return Best;
    }
    float CalculateHValue(Node New, Vector2 Goal)
    {
        return Vector2.Distance(New.Pos, Goal);
    }
    bool Search(Vector2 Goal, Node NewNode, ref List<Node> PrevNodes)
    {
        Node CurrentNode = new Node();
        PrevNodes.Add(NewNode);
        CurrentNode = GenerateNewNodes(NewNode, PrevNodes, Goal);
        if (CurrentNode.Pos != Goal)
        {
            if (Search(Goal, CurrentNode, ref PrevNodes))
                return true;
        }
        else
        {
            PrevNodes.Add(CurrentNode);
            return true;
        }
        return false;
    }

    void StartPathPlanning(int PathID)
    {
        
        // first we create a starting node
        Node Start = new Node();
        List<Node> NodePaths = new List<Node>();
        Start.parent = -1;
        Start.Pos = Paths[PathID].Start;
        if (Search(Paths[PathID].End, Start, ref NodePaths))
        {
            PathInfo FinishedPath = new PathInfo();
            FinishedPath.Complete = true;
            FinishedPath.Path = NodesToPos(NodePaths);
            FinishedPath.Start = Paths[PathID].Start;
            FinishedPath.End = Paths[PathID].End;
            Paths[PathID] = FinishedPath;
        }
            
    }
    List<Vector2> NodesToPos(List<Node> Nodes)
    {
        List<Vector2> Positions = new List<Vector2>();

        for (int i = 0; i < Nodes.Count; i++)
            Positions.Add(Nodes[i].Pos);
            return Positions;
    }
}