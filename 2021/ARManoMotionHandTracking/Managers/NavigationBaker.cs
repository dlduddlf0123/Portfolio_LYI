using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

public class NavigationBaker : MonoBehaviour
{
   // public NavMeshSurface[] surfaces;
    //public Transform[] objectsToRotate;

    public AstarPath ApathMgr;
    protected RecastGraph rg;
    void Start()
    {
        rg = ApathMgr.data.recastGraph;
        rg.SnapForceBoundsToScene();
        rg.forcedBoundsCenter += Vector3.up * 0.5f;
        ApathMgr.graphs[0].Scan();
       // surfaces[0].BuildNavMesh();
       // Debug.Log("SNavMeshBuild!");
    }
}
