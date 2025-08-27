using UnityEngine;
using System.Collections;

public class CharicManager : MonoBehaviour 
{
	public ArrayList kCharicList = new ArrayList();

    public int uid_seed = 0;
    /*
        GameObject kRoot; 

        pr*ivate static CharicManager s_Instance = null;
        public static CharicManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance //= new CharicManager();
                        = FindObjectOfType(typeof(CharicManager)) as CharicManager;
                }
                return s_Instance;
            }
        }

        void Awake()
        {
            if (s_Instance != null)
            {
                Debug.LogError("Cannot have two instances of CharicManager.");
                return;
            }
            s_Instance = this;

            DontDestroyOnLoad(this);
            Debug.Log("CharicManager Awake");
        }
        // Use this for initialization
        void Start()
        {

            kRoot = GameObject.Find("root_charic");
            Debug.Log("CharicManager Start");
        }
        //
        void Update () {

            //if( Input.GetKeyDown(KeyCode.A) )
            //{	
                //Vector3 pos = new Vector3(2, 0, 0);
                //AddClone( 1 , pos );
            //}
        }
    */

    //init 
    public CharicManager()
    {
        uid_seed = 100;

    }

    // add charic

    public Charic Charic_add(int _cuid, int _table_index, Charic.eType _type)
    {
        Charic kCharic = new Charic();

        switch ((Charic.eType)_type)
        {
            case Charic.eType.Hero:
            case Charic.eType.Enemy:
            default:
                {
                    kCharic.ID = uid_seed++; //_cuid 자동 생성.
                    kCharic.kType = (Charic.eType)_type;				
					
					kCharic.kTable = CGameTable.Instance.Get_TableInfo_charic(_table_index);
					kCharic.kGO = CGame.Instance.GameObject_from_prefab ("prefabs/" + kCharic.kTable.resource, null);
					kCharic.kGO.name = "charic_" + _table_index;								

                    //kCharic.kEC = (EffectController)kGO.AddComponent<EffectController>();
                    //kCharic.kEC.kGameObject = kCharic.gameObject;
                }
                break;
        }

        //Charic_gameobject_set( kCharic ); //client


        kCharicList.Add(kCharic);
        return kCharic;
    }


    //remove
    public void Charic_remove(Charic _charic)
    {        
        //Destroy(_charic.kGo);
        kCharicList.Remove(_charic);
    }    
    public void Charic_remove_all()
    {
        //foreach (Charic obj in kCharicList)
            //Destroy(obj.kGo);

        kCharicList.Clear();
    }
    //find
    public Charic Charic_find(int _uid)
    {
        foreach (Charic obj in kCharicList)
        {			
            //if (obj == null) continue;
			if (obj.ID == _uid) {
				return obj;
			}
        }
        return null;
    }

	public void Charic_update()
	{
		foreach (Charic obj in kCharicList) 
		{
			obj.Charic_update ();
		}
	}


    /*
    void Charic_gameobject_set(Charic _charic) //client
    {
        TableInfo_card c_table = CGame.Instance.kTable.Get_TableInfo_card(_charic.table_index);
        //if (c_table == null)

        string sPrefabName = c_table.resource; //card image

        GameObject kGO = null;
        kGO = (GameObject)Instantiate(Resources.Load(sPrefabName, typeof(GameObject)));
        if (kGO == null)
        {
            Debug.Log("ERROR: Charic_gameobject_set() Resources.Load() : " + sPrefabName);
        }

        //kGO.tag = "Charic";	// tagManager
        kGO.name = "Charic_" + _charic.ID;
        kGO.transform.parent = kRoot.transform;
        kGO.transform.position = new Vector3(0, 0, 0);

        //_charic.kGo = kGO;
    }
    
    // 전투객체로 필요한 정보를 세팅한다.-------------------------------------------------
    //public void SetCharicInfo(MyPlayer _player, ItemInfo _card, Charic _charic)
    public void Charic_battle_init(Charic _charic, CardInfo _card)
    {
        if (_card == null) return;
        if (_card.card_index == 0) return;

        _charic.SetCardInfo(_card);  //cardinfo

        //stat init 
        
        _charic.kAbility.hp_max = _charic.calc_hp_max(); //
        _charic.kAbility.hp_cur = _charic.kAbility.hp_max;

        _charic.kAbility.ap_max = _charic.calc_ap_max();
        _charic.kAbility.ap_cur = _charic.kAbility.ap_max;

        _charic.kAbility.dp_max = _charic.calc_dp_max();
        _charic.kAbility.dp_cur = _charic.kAbility.dp_max;

        _charic.kAbility.speed = _charic.calc_speed();

        _charic.kAbility.attr = _card.attr;
        _charic.kAbility.clss = _card.clss;
        _charic.kAbility.star = _card.star;

        // skill turn reset
        _charic.kAbility.skill_turn_cur = 0;
        _charic.Skill_turn_max_calc(); //스킬의 턴을 초기화.

        //state
        _charic.Status_init();
        
        Debug.Log(_charic.ID + " " + _charic.kAbility.hp_cur + " " + _charic.kAbility.ap_cur + " " + _charic.kAbility.dp_cur);

        _charic.bActive = true;
    }
    */

    //타겟 선택 --------------------------------------------------------------------------------------
    
    public class SortunitClass
    {
        public float m_value1 { get; set; }
        public float m_value2 { get; set; }
        public Charic m_charic { get; set; }
    }

    public class SortunitClassCompare : IComparer
    {
        public int Compare(object x, object y)
        {
            return Compare((SortunitClass)x, (SortunitClass)y);
        }
        public int Compare(SortunitClass x, SortunitClass y)
        {
            //return x.m_value.CompareTo( y.m_value ); // 작은 순서대로 정렬.
            //return y.m_value.CompareTo( x.m_value ); // 큰 순서대로 정렬.
            
            int v = x.m_value1.CompareTo(y.m_value1);   // 작은 순서
            if (v == 0)
                v = x.m_value2.CompareTo(y.m_value2);   //m_value 이 같을땐 	m_value2.
            return v;
        }
    }

    // 필요한 캐릭터를 반환 ------------------------------------------ 20170413
    public ArrayList FindTarget(Charic _charic)
    {
        ArrayList TargetArray = new ArrayList();
        ArrayList SortArray = new ArrayList(); //조건에 맞추어 정렬.

        foreach (Charic kCharic in kCharicList)
        {
            if (kCharic.bActive == false) continue;
            if (kCharic.ID == _charic.ID) continue; //자신제외
            if (kCharic.kType == _charic.kType) continue; //아군 제외.
            if (kCharic.IsDie()) continue;
            
            float fDistance = Vector3.Distance(kCharic.kGO.transform.position, _charic.kGO.transform.position);

            SortArray.Add(new SortunitClass() { m_value1 = fDistance, m_charic = kCharic });
        }

        if (SortArray.Count > 0)
        {
            // 작은 순서대로 정렬.
            SortArray.Sort(new SortunitClassCompare());

            foreach (SortunitClass sort in SortArray)
            {
                TargetArray.Add(sort.m_charic);
            }
        }

        return TargetArray;
    }

    
}

/*
 
     public ArrayList FindTarget(Charic _player, int targetflag)
    {
        // target flag
        int target = (targetflag / 100) % 10;	//대상  1:적군,2:자신,3:아군(자신포함),4:아군(자신제외)
        int select = (targetflag / 10) % 10;	//선택  0:랜덤,1:공격최강,2:공격최소,3:방어최강,4:방어최소,5:생명최강,6:생명최소,7:죽은자
        int range = (targetflag / 1) % 10;      //범위  1:1인,2:2인,3:3인,4:4인,5:5인

        ArrayList TargetArray = new ArrayList();
        ArrayList SortArray = new ArrayList();

        foreach (Charic kCharic in kCharicList)
        {
            if (kCharic.bActive == false) continue;

            if (target == 1) if (_player.kType == kCharic.kType ) continue; //적
            if (target == 2) if (_player.ID != kCharic.ID) continue; //자신
            if (target == 3) if (_player.kType != kCharic.kType) continue; //아군
            if (target == 4) if (_player.ID == kCharic.ID) continue; //자신제외                       
                      
            int iValue = Random.Range(1,6); //
            if (select == 1) iValue = 100000000 - kCharic.ap_cur; //
            if (select == 2) iValue = kCharic.ap_cur; //
            if (select == 3) iValue = 100000000 - kCharic.dp_cur; //
            if (select == 4) iValue = kCharic.dp_cur; //
            if (select == 5) iValue = 100000000 - kCharic.hp_cur; //
            if (select == 6) iValue = kCharic.hp_cur; //
            if (select == 7) if (!kCharic.IsDie()) continue; //죽은자
            if (select != 7) if (kCharic.IsDie()) continue;


            //int iValue = kCharic.kAbility.skill_turn_cur;   //스킬턴 작은대상 우선.
            SortArray.Add(new SortunitClass() { m_value1 = iValue, m_player = kCharic });
        }

        if (SortArray.Count > 0)
        {
            // 작은 순서대로 정렬.
            SortArray.Sort(new SortunitClassCompare());

            int count = 0;
            foreach (SortunitClass sort in SortArray)
            {   
                TargetArray.Add(sort.m_player);                

                count++; if (count >= range) break;
            }
            return TargetArray;

            //SortunitClass sort1 = (SortunitClass) SortArray[0];
        }

        return TargetArray;
    }



    public ArrayList FindTarget_our(Charic _player)
    {
        ArrayList TargetArray = new ArrayList();

        ArrayList SortArray = new ArrayList();

        foreach (Charic kCharic in kCharicList)
        {
            if (kCharic.bActive == false) continue;
            if (kCharic.IsDie()) continue;
            //if (kCharic.kCard.card_uid == _player.kCard.card_uid) continue; // 자신제외.

            if (_player.IsEnemy()) { if (!kCharic.IsEnemy()) continue; } //우리진영.
            else { if (kCharic.IsEnemy()) continue; }

            // insert in sort array
            int iValue = kCharic.ap_cur; //
            SortArray.Add(new SortunitClass() { m_value1 = iValue, m_player = kCharic });
        }

        if (SortArray.Count > 0)
        {
            // 작은 순서대로 정렬.
            SortArray.Sort(new SortunitClassCompare());
            foreach (SortunitClass sort in SortArray)
            {
                TargetArray.Add(sort.m_player);
                //print( sort.m_player.name  + " " + sort.m_value );
            }
            return TargetArray;
            //SortunitClass sort1 = (SortunitClass) SortArray[0];
        }

        return TargetArray;
    }
 
 
    //ArrayList kArray1 = new ArrayList();
    //kArray1 = kCharicManager.GetOrderArry();
    //---------------------------------------------------------------
    public ArrayList GetOrderArry()
    {
        ArrayList TargetArray = new ArrayList();
        ArrayList SortArray = new ArrayList();

        foreach (Charic kCharic in kCharicList)
        {
            if (kCharic.bActive == false) continue;
            if (kCharic.IsDie()) continue;

            // insert in sort array
			int iValue1 = 100000 - kCharic.aspeed ;//공속 우선. //큰 순서대로 정렬하려고 뺌.
            int iValue2 = kCharic.ID; 
            SortArray.Add(new SortunitClass() { m_value1 = iValue1, m_value2 = iValue2, m_player = kCharic });
            //Debug.Log("" + kCharic.ID + " " + kCharic.kAbility.speed);
        }

        if (SortArray.Count > 0)
        {
            SortArray.Sort(new SortunitClassCompare()); 

            foreach (SortunitClass sort in SortArray)
            {
                TargetArray.Add(sort.m_player);
                //Debug.Log("" + sort.m_player.ID + " " + sort.m_player.kAbility.speed);
            }
            return TargetArray;
        }

        return TargetArray;
    } 
 
 
 */


