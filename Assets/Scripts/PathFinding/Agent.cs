﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [HideInInspector]
    public Vector3[] Path;
    public Transform Target;
    [Range(1, 50)]
    public float Speed = 5;
    private int _waypointIndex;
    private Grid _grid;
    
    public List<Plaque> Plaques;
    public List<Professor> Professors { get; set; }

    private void Awake()
    {
        _grid = GameObject.Find("A*").GetComponent<Grid>();
        Professors = new List<Professor>(Plaques.Count);
        foreach (var p in Plaques)
        {
            Professors.Add(p.Professor);
        }
    }

	private void Update () {
	    if (Input.GetKeyDown(KeyCode.F1))
	    {
            ResetPath();
	        PathRequestManager.RequestPath(transform.position, Target.position, OnPathFound, this);
	    }
	    if (Input.GetKeyDown(KeyCode.F3))
	    {
	        StopPath();
	    }
	}

    private void ResetPath()
    {
        _waypointIndex = 0;
    }
    /// <summary>
    /// Method that moves the target towards its target. It is called automatically 
    /// by the path request manager once the path has been found for the request.
    /// </summary>
    /// <param name="newPath">The path</param>
    /// <param name="pathFound">The result of calculating a path</param>
    public void OnPathFound(Vector3[] newPath, bool pathFound)
    {
        if (!pathFound) return;
        Path = newPath;
        StopCoroutine("FollowPath");
        StartCoroutine("FollowPath");
    }

    public void StopPath()
    {
        StopCoroutine("FollowPath");
    }
    /// <summary>
    /// Moves the agent towards the waypoint it's supposed to be processing in the path.
    /// The waypoint starts from 0, and goes on proceeding through the path until the waypointindex
    /// has reached the length of the path, meaning the agent has reached the last waypoint in the path.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FollowPath()
    {
        if (Path.Length > 0)
        {
            var currentWaypoint = Path[0];
            while (true)
            {
                if (Path.Length > _grid.TimeStepWindow && _waypointIndex == _grid.TimeStepWindow)
                {
                    _grid.ResetGridReservations(this);
                    ResetPath();
                    PathRequestManager.RequestPath(transform.position, Target.position, OnPathFound, this);
                    yield break;
                }
                if (transform.position == currentWaypoint)
                {
                    _waypointIndex++;
                    // target has reached target
                    if (_waypointIndex >= Path.Length)
                    {
                        yield break;
                    }
                    currentWaypoint = Path[_waypointIndex];
                }
//                transform.position = currentWaypoint;
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, Speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (Path == null) return;
        for (int i = _waypointIndex; i < Path.Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(Path[i], Vector3.one * 2);
        }
    }
}
