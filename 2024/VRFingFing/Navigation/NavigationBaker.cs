using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


using Pathfinding;

public class NavigationBaker
{
    // public NavMeshSurface[] surfaces;
    //public Transform[] objectsToRotate;

    public AstarPath ApathMgr;
    protected GridGraph gridGraph;

    protected RecastGraph rg;

    void Start()
    {


        rg = ApathMgr.data.recastGraph;
        rg.SnapForceBoundsToScene();
        rg.forcedBoundsCenter += Vector3.up * 0.5f;
        ApathMgr.graphs[0].Scan();


        //SetNavigationGraph(GameManager.Instance.playMgr.currentStage.arr_tilemap[0]);
        // surfaces[0].BuildNavMesh();
        // Debug.Log("SNavMeshBuild!");
    }

    /// <summary>
    /// 4/2/2024-LYI
    /// 스테이지 생성 후 호출
    /// 그리드 스캔
    /// </summary>
    /// <param name="grid"></param>
    public void SetNavigationGraph(GridLayout grid)
    {
        gridGraph = ApathMgr.data.gridGraph;
        gridGraph.SetGridShape(InspectorGridMode.IsometricGrid);
        gridGraph.AlignToTilemap(grid);
        gridGraph.isometricAngle = 0;

        gridGraph.Scan();
    }

}
