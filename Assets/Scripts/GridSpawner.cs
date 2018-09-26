using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour {

    public GameObject GO_ObjectToSpawn = null;
    public Transform T_StartPoint;
    public Transform T_Parent = null;
    public Vector3 V3_Dimensions = new Vector3();
    public Vector3 V3_Spacing = new Vector3();

    private Transform T_SpawnSpots = null;
    private Queue<Vector3> QueuedSpawns = null;

    private bool B_Init = false;
    private bool B_Spawning = false;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        QueuedSpawns = new Queue<Vector3>();
        B_Init = true;
    }

    public void Spawn(Transform t_StartPoint = null)
    {
        if (!B_Init) Init();
        Vector3 v3_CurrentSpawnSpot = new Vector3();
        if (t_StartPoint == null)
            v3_CurrentSpawnSpot = Vector3.zero;
        else
        {
            v3_CurrentSpawnSpot.x = t_StartPoint.position.x;
            v3_CurrentSpawnSpot.y = t_StartPoint.position.y;
            v3_CurrentSpawnSpot.z = t_StartPoint.position.z;
        }

        for (int i_Level = 0; i_Level < V3_Dimensions.y; i_Level++) // Y
        {
            for (int i_Depth = 0; i_Depth < V3_Dimensions.x; i_Depth++) // X
            {
                for (int i_Position = 0; i_Position < V3_Dimensions.z; i_Position++) // Z
                {
                    QueuedSpawns.Enqueue(v3_CurrentSpawnSpot);
                    v3_CurrentSpawnSpot.z += V3_Spacing.z;
                }
                v3_CurrentSpawnSpot.z = T_StartPoint.position.z;
                v3_CurrentSpawnSpot.x += V3_Spacing.x;
            }
            v3_CurrentSpawnSpot.x = T_StartPoint.position.x;
            v3_CurrentSpawnSpot.y += V3_Spacing.y;
        }
        StartSpawns();
    }

    public void MultiGridSpawn(Transform t_SpawnSpots, GameObject go_SpawningObject, Vector3 v3_Dimensions, Vector3 v3_Spacing, Transform t_Parent = null)
    {
        if (!B_Init) Init();
        GO_ObjectToSpawn = go_SpawningObject;
        V3_Dimensions = v3_Dimensions;
        V3_Spacing = v3_Spacing;
        T_Parent = t_Parent;
        T_SpawnSpots = t_SpawnSpots;

        for (int i = 0; i < T_SpawnSpots.childCount; i++)
        {
            T_StartPoint = T_SpawnSpots.GetChild(i);
            Spawn(T_StartPoint);
        }       
    }

    private void StartSpawns()
    {
        if (!B_Spawning)
        {
            B_Spawning = true;
            StartCoroutine(SpawnQueued());
        }
    }

    private IEnumerator SpawnQueued()
    {
        while (QueuedSpawns.Count > 0)
        {
            Vector3 v3_CurrentPos = QueuedSpawns.Dequeue();
            Instantiate(GO_ObjectToSpawn, v3_CurrentPos, Quaternion.identity, T_Parent);
            yield return null;
        }
        B_Spawning = false;
    }
}