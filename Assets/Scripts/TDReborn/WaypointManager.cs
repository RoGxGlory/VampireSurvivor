using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    public static WaypointManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Automatically finds all waypoints in the WaypointsContainer
        Transform container = GameObject.Find("WaypointManager").transform;
        foreach (Transform child in container)
        {
            waypoints.Add(child);
        }
    }

    public Transform GetWaypoint(int index)
    {
        if (index >= 0 && index < waypoints.Count)
            return waypoints[index];
        return null;
    }
}