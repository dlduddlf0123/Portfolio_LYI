using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Charic //: MonoBehaviour //유니티 객체를 쓰지 않아야 null 처리가 편하다.
{ 

    public int	ID = 0;			// unity GetInstanceID();

	public TableInfo_charic kTable = null;
	//public string   Name = "";
	public bool 	bActive = false; //활성여부, false 면 제거.

	public GameObject kGO; 		//client

	public Animator kAnimator;	   

	// move -----------------------------------
	public Vector3 move_target;
	NavMeshAgent kAgent;

	// target charic
	public Charic target_charic = null;


    // Type ------------------------------------
    public enum eType
    {
        None,
        Hero = 1,
        Enemy = 2,
    };
    public eType kType;


	// Ability ---------------------------------
	public int hp_cur = 0;          //생명력
	public int hp_max = 0;          //
	public int ap_cur = 0;          //공격력
	public int ap_max = 0;          //
	public int dp_cur = 0;          //방어력
	public int dp_max = 0;          //
	public float aspeed = 0;          //공속
	public float fAttackTime = 0;

	//public int target_type = 0;            // 0: 유닛 1: 건물 

    // Act -------------------------------------
    public enum eAct // animation + transform + state
    {
        None,
        appear,     // create, pos
        disappear,  // delete
        idle,
        walk,
		run,
        attack,     // target
        hit,        //
        die,
        Max
    };
    public eAct kAct;
    public float fActTime = 0.0F;


	CharicManager kCharicManager;
	SkillManager kSkillManager;

	//public HpBar kHpBar;

	public delegate void Callback_charic(Hashtable _data);
	public Callback_charic OnCallback_charic;

	// Use this for initialization
    void Start()
    {
    }      

	public void Charic_init(CharicManager _cm, SkillManager _sm)
    {
		kCharicManager = _cm;
		kSkillManager = _sm;

		// hp bar
		//GameObject go = null;
		//if(kType == eType.Hero) go = CGame.Instance.GameObject_from_prefab ("hp_bar/hp_bar1");
		//else 					go = CGame.Instance.GameObject_from_prefab ("hp_bar/hp_bar2");				
		//kHpBar = go.GetComponent<HpBar>();
		//kHpBar.pos_parent = kGO;
		//kHpBar.fValue = 1.0f;
		////kHpBar.gameObject.SetActive (false);

		//anim
		kAnimator = kGO.GetComponent<Animator> ();

        //move
		kAgent = kGO.GetComponent<NavMeshAgent>();
		if(kAgent != null)
		{		
			//if(kAgent.isOnNavMesh)	Debug.Log("isOnNavMesh " + kGO.transform.position);
			kAgent.enabled = false;
			kAgent.speed = 0;
			//kGO.transform.position = Random.onUnitSphere;

			kAgent.enabled = true; //초기화가 좀 문제 있네.
			kAgent.SetDestination(kGO.transform.position);

		}
		else
		{
			//Debug.Log("kAgent null " + ID);
		}

		//data table 
		hp_cur = kTable.hp;          //생명력
		hp_max = kTable.hp;          //
		ap_cur = kTable.ap;          //공격력
		ap_max = kTable.ap;          //
		dp_cur = kTable.dp;          //방어력
		dp_max = kTable.dp;          //
		aspeed = kTable.aspeed;      //공속
		fAttackTime = Time.time;
		//target_type = kTable.target_type;            // 0: 유닛 1: 건물

		bActive = true;

		//act
		Act_set(Charic.eAct.appear); 
    }

	public void Charic_update()
    {		
        Act_process();

		//kHpBar.fValue = (float)hp_cur / (float)hp_max;
    }

    // 호출 함수 사용
    //Hashtable hash = new Hashtable(); hash.Add("time", 1.0f); 
    //_charic.Act_set(Charic.eAct.appear, hash);
    // Hashtable 사용
    // float time = (float)args["time"]

	//--------------------------------------------------------------------------
    public void Act_set(eAct _act, Hashtable args = null, bool _force = true)
    {
        //동일 액션 회피.
        if (!_force)
            if (_act == kAct) return;

        //액션 우선순위 체크.
        switch (_act)
        {
            case eAct.hit:
                if (kAct == eAct.die) return;
                if (kAct == eAct.disappear) return;
                break;

            case eAct.die:
                if (kAct == eAct.die) return;
                if (kAct == eAct.disappear) return;
                break;
        }

        //액션 변경 ----------------------		
        eAct kAct_old = kAct;
        kAct = _act;

        switch (kAct)
        {
		case eAct.appear:			
			{			 
				//Hashtable data = new Hashtable() { { 0, ID }, { 1, "appear" } }; OnCallback_charic(data);
            	//kEC.SetColor(Color.white);					
        	}
        	break;
        case eAct.idle:
            {
				//print ("eAct.idle " + ID);

				// move
				if (kAgent != null) kAgent.speed = 0.0f; 

				// anim					
				kAnimator.SetBool("Idle", true);
            }
            break;
        case eAct.walk:
			{
				//print("eAct.walk " + ID);
				//print("eAct.walk " + kAgent.speed + " " + target_charic.ID);                    

				if (kAgent != null) kAgent.speed = kTable.speed; //speed		
                kAnimator.SetBool("Walk", true);
			}
            break;
        case eAct.attack:
            {
                //print("eAct.attack " + ID);

				if(target_charic != null && kTable.type != 1 ) //건물제외.
                    kGO.transform.LookAt(target_charic.kGO.transform);
                
				if (kAgent != null) kAgent.speed = 0.0f;

				string anim = "Melee Right Attack 01";
				if( kTable.index == (int)eHeroCode.ranger ) anim = "Melee Right Attack 01";
				if( kTable.index == (int)eHeroCode.magician ) anim = "Cast Spell";

				//attack sound
				if( kTable.index == (int)eHeroCode.king 		 ) CGameSnd.Instance.PlaySound(eSound.skill_arrow);
				if( kTable.index == (int)eHeroCode.dealer 		 ) CGameSnd.Instance.PlaySound(eSound.skill_sword);
				if( kTable.index == (int)eHeroCode.ranger 		 ) CGameSnd.Instance.PlaySound(eSound.skill_arrow);
				if( kTable.index == (int)eHeroCode.magician 	 ) CGameSnd.Instance.PlaySound(eSound.skill_magic);
				if( kTable.index == (int)eHeroCode.giant 		 ) CGameSnd.Instance.PlaySound(eSound.skill_punch);
				if( kTable.index == (int)eHeroCode.tanker 		 ) CGameSnd.Instance.PlaySound(eSound.skill_slap1);
				if( kTable.index == (int)eHeroCode.supporter 	 ) CGameSnd.Instance.PlaySound(eSound.skill_slap2);
				if( kTable.index == (int)eHeroCode.healer 		 ) CGameSnd.Instance.PlaySound(eSound.skill_magic);
				if( kTable.index == (int)eHeroCode.buffer 		 ) CGameSnd.Instance.PlaySound(eSound.skill_magic);


				kAnimator.Play(anim);

				kSkillManager.Skill_start (this, target_charic);
				fAttackTime = Time.time + aspeed; //다음 공격 시간.

                fActTime = Time.time + 0.5f;
            }
            break;
        case eAct.hit:
            {
                //print("eAct.hit " + ID);

                int skill_index = (int)args["skill"];
				int damage = (int)args["damage"];

				Vector3 pos = kGO.transform.position;

				if (kTable.index == (int)eHeroCode.king) CGameFx.Instance.PlayFx ("fx/fx_shot_red", pos); 
				else CGameFx.Instance.PlayFx ("fx/fx_hit", pos); 	
				//todo //데미지바 처리 

            }
            break;
        case eAct.die:
            {
				//print("eAct.die " + ID);

				kAnimator.Play("Die");


				//kHpBar.gameObject.SetActive (false);

				fActTime = Time.time + 0.8f;
            }
            break;
        case eAct.disappear:
            {				
				if (kTable.index == (int)eHeroCode.king) //king
					break; 

				Vector3 pos = kGO.transform.position;
				CGameFx.Instance.PlayFx ("fx/fx_die", pos); 

				bActive = false;
				kGO.SetActive (false);
            }
            break;
        }
    }

	//-------------------------------------------------------------------------- 
    void Act_process()
    {
        //if (kType == eType.Hero) Act_process_Hero();
        switch (kAct)
        {
            case eAct.appear:
				{
					Act_set(eAct.idle);
				}
			    break;
		    case eAct.idle:
			    {
                    // 타겟 갱신.
                    target_charic = null; //재갱신.
                    ArrayList targets = kCharicManager.FindTarget (this);
					if (targets.Count > 0)
                    {                            
                        target_charic = (Charic)targets[0];                            
                        //Debug.Log("target_charic " + target_charic.ID);
                    }

                    // 공격거리 접근 체크.
                    if (target_charic != null && 
						Vector3.Distance(kGO.transform.position, target_charic.kGO.transform.position) <= kTable.distance )
                    {
						//공속
					if( Time.time >= fAttackTime )
                        	Act_set(eAct.attack);
                    }
                    else if (target_charic != null )
                    {
                        Act_set(eAct.walk); //타겟을 찾았다 이동하라.
                    }

                }
			    break;
		    case eAct.walk:
			    {
                    //move
                    if (kAgent != null && target_charic != null)
                    {
                        move_target = target_charic.kGO.transform.position;
                        kAgent.SetDestination(move_target);
                    }

                    // 타겟 갱신.
                    target_charic = null; //재갱신.
                    ArrayList targets = kCharicManager.FindTarget(this);
                    if (targets.Count > 0)
                    {
                        target_charic = (Charic)targets[0];
                        //Debug.Log("target_charic " + target_charic.ID);
                    }

                    // 타겟이 없으면
                    if (target_charic == null)
                    {
                        Act_set(eAct.idle);
                    }
                    // 공격거리 접근.
					else if (Vector3.Distance(kGO.transform.position, target_charic.kGO.transform.position) <= kTable.distance)
                    {
						Act_set(eAct.idle);
                    }
                    
                }
			    break;
            case eAct.attack:
                {
                    if (Time.time >= fActTime)
                    {
                        Act_set(Charic.eAct.idle);
                    }
                }
                break;
            case eAct.hit:
                {
				    if (Time.time >= fActTime) 
				    {								
					    if (hp_cur <= 0)	Act_set (Charic.eAct.die);
					    else 				Act_set (Charic.eAct.idle);
				    }
                }
                break;
            case eAct.die:
                {
				    if( Time.time >= fActTime )
					    Act_set(Charic.eAct.disappear);
                }
                break;
        }
    }

    //-----------------------------------------------------------------------------

    public bool IsEnemy()
    {
        if (kType == eType.Enemy) return true;
        return false;
    }
    public bool IsIdle()
    {
        if (kAct == eAct.idle)
            return true;
        return false;
    }
    public bool IsDie()
    {
        if (kAct == eAct.die ||
            kAct == eAct.disappear
             )
        {
            return true;
        }
        return false;
    }


}




/*

    public int HP_add(int _value)
    {
        if( IsDie() ) return 0 ;

        if (_value < 0) //damage
        {
            //if( kStatus.index == eStatus.nodam )	return 0;		
            //if( bTransform )				return 0; 		//transform no damage
        }

        int org = kAbility.hp_cur;

        kAbility.hp_cur += _value;
        if (kAbility.hp_cur > kAbility.hp_max) kAbility.hp_cur = kAbility.hp_max;
        if (kAbility.hp_cur < 0) kAbility.hp_cur = 0;

        int diff = kAbility.hp_cur - org;

        Hashtable data = new Hashtable() { { 0, ID }, { 1, "hp" }, { 2, diff } }; OnCallback_charic(data);
        
        return diff;
    }


// eStatus ------------------------------------- 	
public enum eStatus // hp, ap, damage, attr, attack,
{
    none,

    berserker,   //(강화) 공격시 일정시간 공격력 강화
    through,     //(관통) 공격시 상대 방어력 무시
    absorb,      //(흡수) 공격시 일정 데미지 흡수

    shield,      //(보호) 공격받을때 일정시간 데미지 감소
    nodamage,    //(무적) 공격받을때 일정시간 데미지 무시
    goback,      //(반사) 공격받을때 일정 데미지 반사

    phoenix,     //(불사) HP 0이 되면 1로 되살아남 //check
    recover,     //(회복) 일정시간 체력회복 (도트회복) //use,check
    aggro,       //(도발) 일정시간 어그로

    weakness,    //(약화) 일정시간 공격력 약화
    confuse,     //(혼란) 일정시간 방어력 약화
    paralysis,   //(마비) 일정기간 공격력 약화,방어력 상실
    poison,      //(중독) 일정시간 체력감소 (도트데미지) //check

    silence,     //(침묵) 일정기간 공격, 스킬 불가
    blind,       //(실명) 일정기간 공격, 스킬 불가, 방어력 상실
    freeze,      //(빙결) 일정시간 공격, 스킬 불가, 회복 불가
    stone,       //(석화) 일정기간 공격, 스킬 불가, 회복 불가, 데미지 무시

    timebomb,    //(폭탄) 일정시간후 데미지 발생 //check

    max
}

public class CStatus
{
    public eStatus index = eStatus.none;
    //public float  	fStatusTime = 0.0F;
    public int iTurn = 0;
    public float fValue = 0;
}


//---------------------------------------------------------------------------------------------------------
public void Status_init()
{
    kStatusArray.Clear();
}
//--------------------------------------------------------------------------------
// Status는 종류별 1개씩으로 한정한다. 새로운 상태가 세팅되면 신규상태로 데이타가 갱신된다.    
public void Status_set(eStatus _status, int _turn)
{
    //무적상태일때 일부 상태적용불가.
    //CStatus kStatus = Status_find(eStatus.nodam);
    //if (kStatus != null) {
    //    if (_status == eStatus.silent || _status == eStatus.poison || _status == eStatus.confuse)
    //        return;
    //}

    Status_set(_status, _turn, null);
}
// 상태는 사용즉시 1턴이며,  다음턴 시작시 차감후 0이되면 사라진다. -------------------
public void Status_set(eStatus _status, int _turn, Hashtable args)
{
    //print("Status_set " + _status.ToString() + " turn:" + kStatus.iTurn + " card:" + kCard.iCardIndex);

    CStatus kStatus = Status_find(_status);
    // 새로운 상태. create
    if (kStatus == null)    
    {
        kStatus = new CStatus();            
        kStatus.index = _status;
        kStatus.iTurn = _turn;

        kStatusArray.Add(kStatus);
    }
    //기존에 있던 상태는 카운트 재세팅.
    else
    {
        kStatus.index = _status;
        kStatus.iTurn = _turn;
    }

    // 추가됨.
    Status_added(kStatus);

    // 사용즉시 카운트. 추후에는 턴변화시 상태턴 같이 체크.
    kStatus.iTurn--;

    // 사용즉시 효과.
    switch (_status)
    {
        // 상태 해제 ----------------------------------------------------------------------------..
        //case eStatus.recover_all:
        //case eStatus.recover_poison:
        //case eStatus.recover_silent:
        //case eStatus.recover_confuse:
        //    {
        //        CStatus state = Status_find(eStatus.confuse); if (state != null) state.iTurn = 0;
        //        kStatus.iTurn = 0; //종료.
        //    }
        //    break;
    }


}

public void Status_added(CStatus state)
{
    //OnCallback_charic
}
public void Status_removed(CStatus state)
{
    //OnCallback_charic
}

// turn //턴을 차감한다. 0턴이 되기전까지 상태가 유지된다.----------------------------------
public void Status_turn_count()
{
    kRemoveArray.Clear();

    foreach (CStatus kStatus in kStatusArray)
    {
        kStatus.iTurn--;

        if (kStatus.iTurn <= 0) kRemoveArray.Add(kStatus);
    }

    //턴이 종료된 것 삭제.
    foreach (CStatus kStatus in kRemoveArray)
    {
        Status_removed(kStatus);
        kStatusArray.Remove(kStatus);
    }
}
// process //스킬상태에 자체의 효과 처리 -----------------------------------------------------
public void Status_process()  //카운팅된 턴을 기준으로처리하자.
{
    foreach (CStatus kStatus in kStatusArray)
    {
        if (kStatus.iTurn <= 0) continue;

        switch (kStatus.index)
        {
            case eStatus.none:
                {
                }
                break;
            case eStatus.poison:
                {
                    int damage = (int)(kAbility.hp_max / 10.0f);
                    HP_add(-damage);
                    //ShowDamagePoint(damage, "red"); // poision damage
                    //print("StatusProcess : poison: turn:" + kStatus.iTurn + " damage:" + damage);
                }
                break;
        }
    }
}

//----------------------------------------------------------------
CStatus Status_find(eStatus _status_index)
{
    foreach (CStatus kStatus in kStatusArray)
    {
        if (kStatus.index == _status_index)
            return kStatus;
    }
    return null;
}
// 상태가 활성인지 확인.
public bool Status_is(eStatus _status_index)
{
    foreach (CStatus kStatus in kStatusArray)
    {
        if (kStatus.index == _status_index)
        {
            if (kStatus.iTurn > 0) return true;
        }
    }
    return false;
}

//skill -----------------------------------------------------------------------
public void Skill_turn_max_calc() //턴 재 계산. turn_buf 고려.
{
    // skill active.
    TableInfo_skill skill_a = CGame.Instance.kTable.Get_TableInfo_skill(kCard.skill_a);
    if (skill_a != null)
    {
        kAbility.skill_turn_max = skill_a.turn - kAbility.turn_buf;
        kAbility.skill_turn_cur = kAbility.skill_turn_max;
    }
}
// skill turn //턴을 차감한다 ----------------------------------------------
public void Skill_turn_count()
{
    kAbility.skill_turn_cur--;

    if (kAbility.skill_turn_cur <= 0)
    {
        kAbility.skill_turn_cur = 0; // 액티브 스킬을 사용할수 있는 상태.
        //OnCallback_charic
    }
    Hashtable data = new Hashtable() { { 0, ID }, { 1, "turn" } }; OnCallback_charic(data);
}
//스킬사용하면 카운트 리셋.
public void Skill_turn_reset()
{
    Skill_turn_max_calc();
}

//buf 계산.------------------------------------------------------------
public int calc_hp_max()
{
    kAbility.hp_max = (int) (CGame.Instance.GetCard_hp(kCard) * kAbility.hp_buf);
    return kAbility.hp_max;
}
public int calc_ap_max()
{
    kAbility.ap_max = (int)(CGame.Instance.GetCard_ap(kCard) * kAbility.ap_buf);
    return kAbility.ap_max;
}
public int calc_dp_max()
{
    kAbility.dp_max = (int)(CGame.Instance.GetCard_dp(kCard) * kAbility.dp_buf);
    return kAbility.dp_max;
}
public int calc_speed()
{
    kAbility.speed = (int)(CGame.Instance.GetCard_sp(kCard) * kAbility.speed_buf);
    return kAbility.speed;
}

*/


//-------------------------------------------------------------------------------------------------------
// 클라이언트는 캐릭(전투주체)를 override 하자.
// act 변화, stat 변화, skill 변화, status 변화 등 디스플레이 관련 부분.
//kdw add --------------------------------------------------------------------------------------------
//charic.OnCallback_charic = OnCallback_charic;
//void OnCallback_charic(Hashtable _data) //OnServerCallback
//{
//print("OnCallback_charic : " + _data);
//string _rt = (string)_data[0];
//switch (_rt)
//{
//    case "JoinLobby": //master server
//}
//}

//    void OnCallback_charic(Hashtable _data) //
//    {
//        print("OnCallback_charic : " + _data);
//        string _rt = (string)_data[1];
//        switch (_rt)
//        {            case "": break;        }
//    }
//    //Hashtable data = new Hashtable() { { 0, ID }, { 1, "init" } }; OnCallback_charic(data);

//-------------------------------------------------------------------------------------------------------
/*
    // State -------------------------------------
    //public ArrayList kStatusArray = new ArrayList();
    //public ArrayList kRemoveArray = new ArrayList();

    // Motion -------------------------------------
    public enum eMotion
    {
        None,
        idle,
        walk,
        attack,
        hit,
        die,
        corpse,
        Max
    };
    public eMotion kMotion = eMotion.None;

    // Move -----------------------------------
    //CMove kMove = null;
    //public float moveSpeed { get { return kMove.fMoveSpeed; } set { kMove.fMoveSpeed = value; } }
  void Awake()
    {
        kCamera = (Camera)GameObject.Find("Main Camera").GetComponent("Camera");

        //kMove = (CMove)gameObject.GetComponent("CMove");
        //if (kMove == null)
        //    kMove = (CMove)gameObject.AddComponent(typeof(CMove));

        //foreach (CStatus kStatus in kStatusArray)
        //    kStatus.index = eStatus.none;
    }

    // Use this for initialization
    void Start ()
    {
    }

    // Update is called once per frame
    void Update () {

        Act_process();
        //Status_process();
        //Motion_process();
        //Move_process();
    }
*/