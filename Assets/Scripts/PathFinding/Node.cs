﻿using UnityEngine;
using System.Collections.Generic;

public class Node3D
{
    public List<Node> Nodes = new List<Node>();
    public Node3D(bool walkable, Vector3 worldPoint, int x, int y)
    {
        Nodes.Add(new Node(walkable, worldPoint, x, y, Nodes.Count));
    }

    public void NewTimeStep()
    {
        Nodes.Add(new Node(Nodes[0].Walkable, Nodes[0].WorldPosition, Nodes[0].GridX, Nodes[0].GridY, Nodes.Count));
    }

    public void DeleteTimeStep()
    {
        Nodes.RemoveAt(Nodes.Count - 1);
    }
}

[System.Serializable]
public class Node : IHeapItem<Node>
{
    public bool Walkable;
    public Vector3 WorldPosition;
    public int GCost;
    public int HCost;
    public Node Parent;
    private int _timeStep;
    private int _heapIndex;
    public int HeapIndex
    {
        get { return _heapIndex; }
        set { _heapIndex = value; }
    }

    public int FCost
    {
        get { return GCost + HCost; }
    }

    public int GridX;
    public int GridY;

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY, int timeStep)
    {
        Walkable = walkable;
        WorldPosition = worldPosition;
        GridX = gridX;
        GridY = gridY;
        _timeStep = timeStep;
    }

    public int CompareTo(Node other)
    {
        int compare = FCost.CompareTo(other.FCost);
        if (compare == 0)
        {
            compare = HCost.CompareTo(other.HCost);

        }
        return -compare;
    }
}
