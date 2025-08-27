using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CGame : MonoBehaviour
{

    public string sDevice = "";
    public string sLanguage = "English";	//language 0: korean 1: english 2: japanese 3: chinese
    public string sCountry = "A1";          // ISO 3166-1 alpha-2
    public string sMarket = "google";
    public string sMarket_id = "market_id";
    public string sMarket_token = "market_token";


    public CGameDef kDef;     //게임의 로직 클래스.
    //public CGameTable kTable; //텍스트 테이블 정보. //20170406



    public Player kPlayer;

    public bool bGameInit = false;

    public int SceneNumber_cur = 0;

    public GameObject Root_ui = null; //CGame.Instance.Root_ui = GameObject.Find("root_window"); //ui root

    private static CGame s_instance = null; //싱글톤
    public static CGame Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance //= new CGame();
                    = FindObjectOfType(typeof(CGame)) as CGame;
            }
            return s_instance;
        }
    }

    void Awake()
    {
        if (s_instance != null)
        {
            Debug.LogError("Cannot have two instances of CGame.");
            return;
        }
        s_instance = this;

        DontDestroyOnLoad(this);

        //게임 정의
        kDef = (CGameDef)gameObject.AddComponent(typeof(CGameDef));


    }

    void Start()
    {
        //초기화 처리.
        Root_ui = GameObject.Find("Canvas_window");

        CGameTable.Instance.InitTableData();
        CGameSnd.Instance.LoadSource();

        kPlayer = new Player();
        kPlayer.Init();

        //TableInfo_text t = CGame.Instance.kTable.Get_TableInfo_text(10000); print("" + t.Korean);
        //TableInfo_charic t = CGame.Instance.kTable.Get_TableInfo_charic(1005); print("" + t.name);
                
        //CGameSnd.instance.LoadSource();
        
        if (!PlayerPrefs.HasKey("project_init"))
        {
            kDef.LocalDB_init();
        }
        else
        {
            kDef.LocalDB_load();
        }
        
        bGameInit = true;  //게임 초기화 종료.
    }
	
	// Update is called once per frame
	void Update ()
    {		

	}

    //------------------------------------------------------------------------------------------------
    // 씬 변경을 위한 호출함수.
    public void SceneChange(int number)
    {
        SceneManager.LoadScene(number);

        SceneNumber_cur = number;
        if (SceneNumber_cur != 0)
        {
            //로딩시 화면처리.
            //CGame.Instance.Show_Window("Prefab/WindowLoading", null);

            //GameObject kLoading = (GameObject)Instantiate(Resources.Load("prefab/screen_loading", typeof(GameObject)));
            //kLoading.transform.parent = Camera.main.transform;
            //kLoading.transform.localPosition = new Vector3( 0, 0, 0.5f ); //카메라 바로 앞.
        }
    }

    // 윈도우 팝업 ---------------------------------------------------------------------------------------
    //CGame.Instance.Root_ui = GameObject.Find("Canvas_window"); //scene 초기화 할 때.
    //CGame.Instance.Window_notice("notice message", rt => { if (rt == "0") print("notice ok");  });
    public void Window_notice(string _msg, Action<string> _callback)
    {        
        GameObject go = GameObject.Instantiate(Resources.Load("prefabs/Window_notice"), Vector3.zero, Quaternion.identity) as GameObject;
        go.transform.parent = Root_ui.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        CWindowNotice w = go.GetComponent<CWindowNotice>();
        w.Show(_msg, _callback);
    }
    public void Window_ok(string _msg, Action<string> _callback)
    {
        GameObject go = GameObject.Instantiate(Resources.Load("prefabs/Window_ok"), Vector3.zero, Quaternion.identity) as GameObject;
        go.transform.parent = Root_ui.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        CWindowOk w = go.GetComponent<CWindowOk>();
        w.Show( _msg, _callback);
    }
    public void Window_yesno(string _title, string _msg, Action<string> _callback)
    {
        GameObject go = GameObject.Instantiate(Resources.Load("prefabs/Window_yesno"), Vector3.zero, Quaternion.identity) as GameObject;
        go.transform.parent = Root_ui.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        CWindowYesNo w = go.GetComponent<CWindowYesNo>();
        w.Show(_title, _msg, _callback);
    }

    //------------------------------------------------------------------------------------------------
    // 리소스 이미지 로드.
    public Texture2D GetResourceImage(string _imagename)
    {
        string imageName = _imagename; // "path/" + _imagename;
        Texture2D texture = (Texture2D)Resources.Load(imageName);
        return texture;
    }

    // GameObject 텍스처 변경.
    public void GameObject_set_texture(GameObject go, Texture2D _tx)
    {
        go.GetComponent<Renderer>().material.mainTexture = _tx;
        //go.GetComponent<Renderer>().material.color = new Color(1,1,1,1.0f);
    }

    // GameObject에 prefab을 로드
    public GameObject GameObject_from_prefab(string _prefab_name)
    {
        GameObject go = (GameObject)Instantiate(Resources.Load(_prefab_name, typeof(GameObject)));
        return go;
    }
    // GameObject에 prefab을 로드하여 어태치하기
    public GameObject GameObject_from_prefab(string _prefab_name, GameObject _parent)
    {
        GameObject go = (GameObject)Instantiate(Resources.Load(_prefab_name, typeof(GameObject)));
        if (_parent != null) go.transform.SetParent(_parent.transform);
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        return go;
    }
    // GameObject의 UI Image 의 sprite 변경
    public void GameObject_set_image(GameObject go, string _path) //"image/test"
    {
        //GameObject go = GameObject.FindGameObjectWithTag("userTag1");
        Image myImage = go.GetComponent<Image>();
        myImage.sprite = Resources.Load<Sprite>(_path) as Sprite;
    }

    // 객체의 이름을 통하여 자식 요소를 찾아서 리턴하는 함수 
    //UILabel _label = CGame.Instance.GameObject_get_child(obj, "_label").GetComponent<UILabel>();
    public GameObject GameObject_get_child(GameObject source, string strName)
    {
        Transform[] AllData = source.GetComponentsInChildren<Transform>(true); //비활성포함.

        GameObject target = null;

        foreach (Transform Obj in AllData)
        {
            if (Obj.name == strName)
            {
                target = Obj.gameObject;
                break;
            }
        }
        return target;
    }

    //객체에 붙은 Child를 제거
    public void GameObject_del_child(GameObject source)
    {
        Transform[] AllData = source.GetComponentsInChildren<Transform>(true); //비활성포함.
        foreach (Transform Obj in AllData)
        {
            if (Obj.gameObject != source) //자신 제외. 
            {
                Destroy(Obj.gameObject);
            }
        }
    }

	//------------------------------------------------------------------------------------------------
	//스크린 좌표
	public Vector3 GetScreenPosition()
	{
		Camera camera = Camera.main;
		Vector3 p = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane));
		return p;
	}

	//마우스 포인트에 타겟 피킹
	public GameObject GetRaycastObject()
	{
		RaycastHit hit;
		GameObject target = null;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //마우스 포인트 근처 좌표를 만든다.
		//마우스 근처에 오브젝트가 있는지 확인
		if (true == (Physics.Raycast(ray.origin, ray.direction * 1000, out hit)))   
		{			
			target = hit.collider.gameObject; //있으면 오브젝트를 저장한다.
		}
		return target;
	}
	public Vector3 GetRaycastObjectPoint()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (true == (Physics.Raycast(ray.origin, ray.direction * 1000, out hit)))   
		{			
			return hit.point;
		}
		return Vector3.zero;
	}

	// 2D 유닛 히트처리 부분.  레이를 쏴서 처리합니다. 
	public GameObject GetRaycastObject2D()
	{
		GameObject target = null;

		Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0);
		if (hit.collider != null)
		{
			//Debug.Log (hit.collider.name);  //이 부분을 활성화 하면, 선택된 오브젝트의 이름이 찍혀 나옵니다. 
			target = hit.collider.gameObject;  //히트 된 게임 오브젝트를 타겟으로 지정
		}
		return target;
	}

	public List<int> List_suffle( List<int> _list)
	{
		List<int> m_list1 = new List<int>();
		List<int> m_list2 = new List<int>();
		for (int i = 0; i < _list.Count; i++) {
			m_list1.Add( _list[i]);
		}

		for (int i = 0; i < _list.Count; i++) {
			int r = UnityEngine.Random.Range(0, m_list1.Count-1);
			m_list2.Add( m_list1[r] );
			m_list1.Remove( m_list1[r] );
		}
		return m_list2;
	}

	public void List_suffle_test()
	{
		List<int> m_list1 = new List<int>();
		List<int> m_list2 = new List<int>();

		m_list1.Add(1);
		m_list1.Add(2);
		m_list1.Add(3);
		m_list1.Add(4);
		m_list1.Add(5);
		for (int i = 0; i < m_list1.Count; i++) {

			print("" + m_list1[i]);
		}

		m_list2 = List_suffle(m_list1);

		for (int i = 0; i < m_list2.Count; i++) {

			print("" + m_list2[i]);
		}

	}
}
