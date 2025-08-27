using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CMap : MonoBehaviour {

    static bool gridActive = true;

    static int objLength = 0;

    static float gridX = 10.5f;    //가로
    static float gridZ = gridX;    //세로

    static float gridYmin = -5.5f;
    static float gridYmax = 10.5f;

    static Transform[] obj2;

    static bool[,,] cubePos;

    private static CMap s_instance = null; //싱글톤
    public static CMap Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance //= new CGame();
                    = FindObjectOfType(typeof(CMap)) as CMap;
            }
            return s_instance;
        }
    }

    static CMap()
    {
        EditorApplication.update += Cubic;
    }


    /*
    오브젝트를 화면에 옮길 때 포지션을 정수로 변경해준다.
    프리팹이나 오브젝트를 생성하는 등의 동작으로 하이어라키창에 등록이 된다.
    하이어라키 창에 등록된 오브젝트의 포지션을 조작한다?

    타입으로 검사하기
    GameObject[] obj = FindObjectsOfType(typeof(GameObject)) as GameObject[];

    태그로 검사하기
    GameObject[] obj = GameObject.FindGameObjectsWithTag("Cube");

    Selection을 이용해 하이어라키창에서 현재 선택된 오브젝트를 받아온다.
    한개의 객체를 받아온다. 계층구조의 경우 같이 적용이 되나 동시에 2개이상의 객체를 받아올 수 없다.
    sGameObject obj = Selection.activeGameObject;

    화면에 있는 선택된 게임오브젝트를 모두 받아온다
    GameObject[] obj = Selection.gameObjects;
    */
    //객체 움직임 제한함수
    static void Cubic()
    {
        //화면에 있는 선택된 오브젝트들의 위치값을 받아온다.
        Transform[] obj = Selection.transforms;

        //예외처리) 오브젝트가 아무것도 선택되있지 않을때 돌려보냄.
        if (obj.Length == 0 || gridActive == false)
        {
            GameObject grid = GameObject.Find("Grid");
            if (grid.transform.GetChild(1) != null)
            {
                grid.transform.GetChild(1).gameObject.SetActive(false);
            }
            return;
        }
        else if (objLength != obj.Length)
        {
            objLength = 0;
            
            if (gridActive == false) { return; }

            Grid();

        }

        //라인을 클릭하면 클릭 해제시키기
        if (Selection.activeGameObject.tag == "Line")
        {
            Selection.objects = new UnityEngine.Object[0];
        }

        //현재 선택한 오브젝트들 루프
        for (int i = 0; i < obj.Length; i++)
        {
            //오브젝트의 위치가 그리드를 벗어나면 근처의 한계점으로 위치를 제한한다.
            //X좌표 제한
            if (obj[i].transform.position.x > gridX)
            {
                obj[i].transform.position = new Vector3(gridX - 0.5f, obj[i].transform.position.y, obj[i].transform.position.z);
            }
            else if (obj[i].transform.position.x < -gridX)
            {
                obj[i].transform.position = new Vector3(-gridX + 0.5f, obj[i].transform.position.y, obj[i].transform.position.z);
            }
            //Y좌표 제한
            else if (obj[i].transform.position.y > gridYmax)
            {
                obj[i].transform.position = new Vector3(obj[i].transform.position.x, gridYmax - 0.5f, obj[i].transform.position.z);
            }
            else if (obj[i].transform.position.y < gridYmin)
            {
                obj[i].transform.position = new Vector3(obj[i].transform.position.x, gridYmin + 0.5f, obj[i].transform.position.z);
            }
            //Z좌표 제한
            else if (obj[i].transform.position.z > gridZ)
            {
                obj[i].transform.position = new Vector3(obj[i].transform.position.x, obj[i].transform.position.y, gridZ - 0.5f);
            }
            else if (obj[i].transform.position.z < -gridZ)
            {
                obj[i].transform.position = new Vector3(obj[i].transform.position.x, obj[i].transform.position.y, -gridZ + 0.5f);
            }
            else
            {
                //소수점이 나왔을 때 반올림 Mathf.Round
                obj[i].transform.position = new Vector3(
                    Mathf.Round(obj[i].transform.position.x),
                    Mathf.Round(obj[i].transform.position.y),
                    Mathf.Round(obj[i].transform.position.z));
            }

        }
        
    }
    //그리드 그리기
    //256*256크기라면 해당 사이즈만큼 격자를 그려준다
    //라인렌더러를 이용해서 각 위치에 해당하는 길이만큼 그려준다.
    static void Grid()
    {
        GameObject grid = GameObject.Find("Grid");

        //Grid객체 하위에 객체 생성이 안돼있으면 생성
        if (grid.transform.GetChild(1).childCount == 0)
        {
            cubePos = new bool[(int)gridYmax,(int)gridX, (int)gridZ];

            GameObject lines = grid.transform.GetChild(1).gameObject;
            //lines.transform.SetParent(grid.transform);

            LineRenderer line = GameObject.Find("Line").GetComponent<LineRenderer>();
            
            for (int x = 0; x < gridX * 2 + 1; x++)
            {
                LineRenderer lr = Instantiate(line);
                lr.SetPosition(0, new Vector3(x - gridX, -0.5f, -gridX));
                lr.SetPosition(1, new Vector3(x - gridX, -0.5f, gridX));
                lr.transform.parent = lines.transform;
                if (x % 5 == 0)
                {
                    lr.startWidth = 0.1f;
                    lr.endWidth = 0.1f;
                }
            }
            for (int z = 0; z < gridZ * 2 + 1; z++)
            {
                LineRenderer lr = Instantiate(line);
                lr.SetPosition(0, new Vector3(-gridZ, -0.5f, z - gridZ));
                lr.SetPosition(1, new Vector3(gridZ, -0.5f, z - gridZ));
                lr.transform.parent = lines.transform;
                if (z % 5 == 0)
                {
                    lr.startWidth = 0.1f;
                    lr.endWidth = 0.1f;
                }
            }
        }

        grid.transform.GetChild(1).gameObject.SetActive(true);
    }


    /// <summary>
    /// 버튼 동작들
    /// </summary>
    //좌회전.반시계.-90도
    public void LeftRotate()
    {
        Transform[] obj = Selection.transforms;
        for (int i = 0; i < obj.Length; i++)
        {
            Vector3 rot = obj[i].transform.eulerAngles;
            rot = new Vector3(0, rot.y - 90, 0);
            obj[i].transform.rotation = Quaternion.Euler(rot);
        }

    }
    //우회전.시계.+90도
    public void RightRotate()
    {
        Transform[] obj = Selection.transforms;
        for (int i = 0; i < obj.Length; i++)
        {
            Vector3 rot = obj[i].transform.eulerAngles;
            rot = new Vector3(0, rot.y + 90, 0);
            obj[i].transform.rotation = Quaternion.Euler(rot);
        }
    }
    //그리드 지우기
    public void Clear()
    {
        Debug.Log("Clear");
        GameObject grid = GameObject.Find("Grid");

        for (int i = 0; i < grid.transform.childCount; i++)
        {
            DestroyImmediate(grid.transform.GetChild(i).gameObject);
        }

    }

    //그리드 토글
    public void ToggleGrid()
    {
        if (gridActive == true)
        {
            gridActive = false;
            Debug.Log("그리드 꺼짐");
        }
        else if (gridActive == false)
        {
            gridActive = true;
            Debug.Log("그리드 켜짐");
        }
    }

    //큐브 높이조절
    public void FloorUp()
    {
        Transform[] obj = Selection.transforms;
        for (int i = 0; i < obj.Length; i++)
        {
            obj[i].transform.position = new Vector3(
                obj[i].transform.position.x,
                obj[i].transform.position.y + 1,
                obj[i].transform.position.z);
        }
    }
    public void FloorDown()
    {
        Transform[] obj = Selection.transforms;
        for (int i = 0; i < obj.Length; i++)
        {
            obj[i].transform.position = new Vector3(
                obj[i].transform.position.x,
                obj[i].transform.position.y - 1,
                obj[i].transform.position.z);
        }
    }
    public void GridUp()
    {
        GameObject lines = GameObject.Find("lines");
        LineRenderer[] lr = lines.GetComponentsInChildren<LineRenderer>();
        
        for (int i = 0; i < lr.Length; i++)
        {
            lr[i].SetPosition(0, new Vector3(lr[i].GetPosition(0).x, lr[i].GetPosition(0).y + 1, lr[i].GetPosition(0).z));
            lr[i].SetPosition(1, new Vector3(lr[i].GetPosition(1).x, lr[i].GetPosition(1).y + 1, lr[i].GetPosition(1).z));
        }
    }
    public void GridDown()
    {
        GameObject lines = GameObject.Find("lines");
        LineRenderer[] lr = lines.GetComponentsInChildren<LineRenderer>();

        for (int i = 0; i < lr.Length; i++)
        {
            lr[i].SetPosition(0, new Vector3(lr[i].GetPosition(0).x, lr[i].GetPosition(0).y - 1, lr[i].GetPosition(0).z));
            lr[i].SetPosition(1, new Vector3(lr[i].GetPosition(1).x, lr[i].GetPosition(1).y - 1, lr[i].GetPosition(1).z));
        }
    }

    public int GridFloor()
    {
        GameObject lines = GameObject.Find("lines");
        LineRenderer lr = lines.transform.GetChild(0).GetComponent<LineRenderer>();
        return (int)(lr.GetPosition(0).y+0.5f);
    }

}
