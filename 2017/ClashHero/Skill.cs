
using UnityEngine;
using System.Collections;

public class Skill //: MonoBehaviour
{
    //public static readonly Skill Instance = new Skill();

    public long uID = 0;

	CharicManager kCharicManager = null;

    Charic kSkiller;
    Charic kSkillee;    
    ArrayList kTargetArray;             // 타겟 리스트.
    
    public int iSkill_index = 0;		// skill table index
    //public TableInfo_skill kTable = null;
    

    public float fStartTime;   
    public float fFireTime;
	//public bool bFire = false;

    //public delegate void 	FireFunction(long value);
    //public FireFunction		fireFunction = null;

    public bool bActive = false;

    public Skill(CharicManager _m)
    {
        kCharicManager = _m;
    }


	public void Init( Charic _skiller, Charic _skillee)
    {
        kSkiller = _skiller;
		kSkillee = _skillee;
		iSkill_index = 0;

        //kTable = CGame.Instance.kTable.Get_TableInfo_skill(_skill_index);
        //if (kTable == null)
		//    return;
                
		kTargetArray = new ArrayList();

        fStartTime 			= Time.time;
        //kStartPos 			= kSkiller.transform.position;
        //transform.position 	= kStartPos;

        //fFireTime = fStartTime + kTable.live_time;
        //fireFunction        = _fireFunction;
        //if(fireFunction == null)   fireFunction = FireFunctionNormal;		


        bActive = true;
    }

	public void Fire()
    {
		FireFunction(kSkiller, kSkillee);
/*		
        if (iSkill_index == 0) return;
        kTargetArray = MakeTargetPlayer(kSkiller, iSkill_index);
        if (kTargetArray.Count == 0)
        {
            Debug.Log("Skill Fire : " + iSkill_index + "  >>> no target ");
            return;
        }
        foreach (Charic skillee in kTargetArray)
        {
			FireFunction(kSkiller, skillee);
        }
*/        
    }

	void FireFunction(Charic _skiller, Charic _skillee)
	{
		//스킬 계산

		//데미지 계산
		int iDamage = _skiller.ap_cur - _skillee.dp_cur;
		if (iDamage <= 0)
			iDamage = 1;

		_skillee.hp_cur -= iDamage;
		if (_skillee.hp_cur <= 0) _skillee.hp_cur = 0; // is die		

		// skillee act ------------------------------------
		if (_skillee.hp_cur > 0) 
		{
			Hashtable hash = new Hashtable (); hash.Add ("skill", iSkill_index); hash.Add ("damage", iDamage);
			_skillee.Act_set (Charic.eAct.hit, hash); 
		} 
		else 
		{
			Hashtable hash = new Hashtable (); hash.Add ("skill", iSkill_index); hash.Add ("damage", iDamage);
			_skillee.Act_set (Charic.eAct.die, hash);
		}
	}

	ArrayList MakeTargetPlayer(Charic _skiller, int _skill_index)
	{
		ArrayList kTaretArray = new ArrayList();

		kTaretArray.Clear();

		ArrayList kArray = new ArrayList();
		kArray = kCharicManager.FindTarget(_skiller ); //

		foreach (Charic player in kArray)
		{
			kTaretArray.Add(player);
		}

		//if( kTaretArray.Count == 0) {
		//	print ("no target");
		//}

		return kTaretArray;
	}
}

//---------------------------------------------------------------------------------------------------------------------------------------
public class SkillManager
{
    //public static readonly SkillManager Instance = new SkillManager();
        
    ArrayList kSkillList = new ArrayList();

    CharicManager kCharicManager;

    public SkillManager(CharicManager _cm)
    {
        kCharicManager = _cm;
    }


    public Skill GetSkillByUID(long _skill_uid)
    {
        foreach (Skill kInfo in kSkillList)
        {
            if (kInfo.uID == _skill_uid)
                return kInfo;
        }
        return null;
    }

    public void Skill_remove(Skill kInfo)
    {
        kInfo = null;
        kSkillList.Remove(kInfo);
    }

    public void Skill_remove_all()
    {
        foreach (Skill kInfo in kSkillList)
        {
            Skill_remove(kInfo);
        }
        kSkillList.Clear();
    }


	// 스킬 시작 ----------------------------------------------------------------------
	public void Skill_start(Charic _skiller, Charic _skillee )
	{
		//if (_skill_index == 0) return;
		if (_skiller == null) return;
		if (_skillee.IsDie()) return;

		Skill kSkill = new Skill(kCharicManager);
		kSkillList.Add(kSkill); //스킬추가.

		kSkill.Init(_skiller, _skillee);
		kSkill.Fire(); 
	}



}







/*

/*
        // fire, kSkillee, kSkilleeScript 확정 필요. ------------------------------------------------------------------------------------	
void FireFunction(Charic _skiller, Charic _skillee)
{
	//Debug.Log("Skill Fire : " + kSkiller.ID + " >>> " + _skillee.ID + "   skill:"+ iSkill_index  );
	//MyUtil.print("FireFunction  " + _skiller.iID + " " + iSkill_index + " " + kTable.effect_name + " " + kTable.kState_name);

	//----------------------------------------------------------------------------------
	if (_skiller == null) return;
	//if (_skiller.IsDie()) return;
	if (_skillee == null) return;

	int iHp_add = 0;

	// skllee state		
	//if( _skillee.kAct == CCharic.eAct.die || _skillee.kAct == CCharic.eAct.corpse)
	//	return;

	//----------------------------------------------------------------------------------
	bool bSuccess = true;
	if (bSuccess)
	{
		switch (kTable.effect_name)
		{
		// passive skill -------------------------------------------------	
		//"hp_buf"          생명력 최대치 증가
		//"ap_buf"          공격력 추가
		//"dp_buf"          방어력 추가
		//"speed_buf"       민첩성 증가, 수치
		//"attr_buf"        속성 추가 데미지 (속성가산에 추가)
		//"turn_buf"        스킬턴 증감, 턴수                
		//"shield_buf"      방어구, 횟수, (깨지면 다시 HP시작)

		case "hp_buf": //hp max 증가. 
			{
				float buf = kTable.effect_param1;
				if (buf > _skillee.kAbility.hp_buf) // 버프가 중첩될때 가장 큰것이 적용된다.
				{
					_skillee.kAbility.hp_buf = buf;
					_skillee.calc_hp_max();
					_skillee.kAbility.hp_cur = _skillee.kAbility.hp_max;
				}
			}
			break;
		case "ap_buf": //ap_cur 증가. 
			{
				float buf = kTable.effect_param1;
				if (buf > _skillee.kAbility.ap_buf) // 버프가 중첩될때 가장 큰것이 적용된다.
				{
					_skillee.kAbility.ap_buf = buf;
					_skillee.calc_ap_max();
					_skillee.kAbility.ap_cur = _skillee.kAbility.ap_max;
				}
			}
			break;
		case "dp_buf": //dp_cur 증가. 
			{
				float buf = kTable.effect_param1;
				if (buf > _skillee.kAbility.dp_buf) // 버프가 중첩될때 가장 큰것이 적용된다.
				{
					_skillee.kAbility.dp_buf = buf;
					_skillee.calc_dp_max();
					_skillee.kAbility.dp_cur = _skillee.kAbility.dp_max;
				}
			}
			break;
		case "speed_buf": //speed 증가
			_skillee.kAbility.speed_buf = kTable.effect_param1;
			_skillee.calc_speed();
			break;
		case "attr_buf":
			_skillee.kAbility.attr_buf = kTable.effect_param1;
			break;
		case "turn_buf": //skill turn
			_skillee.kAbility.turn_buf = (int)kTable.effect_param1;
			_skillee.Skill_turn_max_calc();
			break;
		case "shield_buf":
			_skillee.kAbility.shield_buf = (int)kTable.effect_param1;
			break;

			// active skill -------------------------------------------------
			//"ap_abs"      (공격) 공격(물리, 과학, 마법 공격)
			//"ap_per"      (공격) 공격력의 일정퍼센트 공격
			//"critical"    (치명) 방어력 무시 공격
			//"hp_abs"      (치료) 체력 추가 일정량
			//"hp_per"      (치료) 체력의 일정 퍼센트 추가
			//"turn_dn"     (가속) 스킬 가속 (남은 스킬턴 감소)
			//"turn_up"     (감속) 스킬 감속
			//"revival"     (부활) 일정 체력으로 부활

		case "ap_abs":    //todo
		case "ap_per":
		case "critical":
			{
				// calc damage ------------------------------------
				int damage = Damage_calc(_skiller, _skillee, kTable.effect_name, kTable.effect_param1);		//damage = 50; //test	
				iHp_add = _skillee.HP_add(-damage);
			}
			break;
		case "hp_abs":
			{
				int heal = (int)kTable.effect_param1; //heal
				iHp_add = _skillee.HP_add(heal);
			}
			break;
		case "hp_per":
			{
				int heal = (int)(_skillee.kAbility.hp_max * kTable.effect_param1); //heal
				iHp_add = _skillee.HP_add(heal);
			}
			break;
		case "turn_dn":
			{
				int turn = (int)kTable.effect_param1;
				_skillee.kAbility.skill_turn_cur -= turn;
				if (_skillee.kAbility.skill_turn_cur < 0) _skillee.kAbility.skill_turn_cur = 0;
			}
			break;
		case "turn_up":
			{
				int turn = (int)kTable.effect_param1;
				_skillee.kAbility.skill_turn_cur += turn;
				if (_skillee.kAbility.skill_turn_cur > _skillee.kAbility.skill_turn_max)
					_skillee.kAbility.skill_turn_cur = _skillee.kAbility.skill_turn_max;
			}
			break;
		case "revival":
			{
				//todo
			}
			break;                   

		}

		// state -------------------------------------------------------- 스킬 추가부여.
		//berserker,   //(강화) 공격시 일정시간 공격력 강화
		//through,     //(관통) 공격시 상대 방어력 무시
		//absorb,      //(흡수) 공격시 일정 데미지 흡수
		//shield,      //(보호) 공격받을때 일정시간 데미지 감소
		//nodamage,    //(무적) 공격받을때 일정시간 데미지 무시
		//goback,      //(반사) 공격받을때 일정 데미지 반사
		//phoenix,     //(불사) HP 0이 되면 1로 되살아남
		//recover,     //(회복) 일정시간 체력회복 (도트회복)
		//aggro,       //(도발) 일정시간 어그로

		//weakness,    //(약화) 일정시간 공격력 약화
		//confuse,     //(혼란) 일정시간 방어력 약화
		//paralysis,   //(마비) 일정기간 공격력 약화,방어력 상실
		//poison,      //(중독) 일정시간 체력감소 (도트데미지)
		//silence,     //(침묵) 일정기간 공격, 스킬 불가
		//blind,       //(실명) 일정기간 공격, 스킬 불가, 방어력 상실
		//freeze,      //(빙결) 일정시간 공격, 스킬 불가, 회복 불가
		//stone,       //(석화) 일정기간 공격, 스킬 불가, 회복 불가, 데미지 무시
		//timebomb,    //(폭탄) 일정시간후 데미지 발생

		switch (kTable.state_name)
		{
		case "berserker":  _skillee.Status_set(eStatus.berserker, kTable.state_turn);      break;
		case "through":    _skillee.Status_set(eStatus.through, kTable.state_turn);      break;
		case "absorb":     _skillee.Status_set(eStatus.absorb, kTable.state_turn);      break;
		case "shield":     _skillee.Status_set(eStatus.shield, kTable.state_turn);      break;
		case "nodamage":   _skillee.Status_set(eStatus.nodamage, kTable.state_turn);      break;
		case "goback":     _skillee.Status_set(eStatus.goback, kTable.state_turn);     break;
		case "phoenix":    _skillee.Status_set(eStatus.phoenix, kTable.state_turn);      break;
		case "recover":    _skillee.Status_set(eStatus.recover, kTable.state_turn);      break;
		case "aggro":      _skillee.Status_set(eStatus.aggro, kTable.state_turn);      break;
		case "weakness":   _skillee.Status_set(eStatus.weakness, kTable.state_turn);      break;
		case "confuse":    _skillee.Status_set(eStatus.confuse, kTable.state_turn);      break;
		case "paralysis":  _skillee.Status_set(eStatus.paralysis, kTable.state_turn);      break;
		case "poison":     _skillee.Status_set(eStatus.poison, kTable.state_turn);      break;
		case "silence":    _skillee.Status_set(eStatus.silence, kTable.state_turn);      break;
		case "blind":      _skillee.Status_set(eStatus.blind, kTable.state_turn);      break;
		case "freeze":     _skillee.Status_set(eStatus.freeze, kTable.state_turn);      break;
		case "stone":      _skillee.Status_set(eStatus.stone, kTable.state_turn);      break;
		case "timebomb":   _skillee.Status_set(eStatus.timebomb, kTable.state_turn);      break;
		}

		//MyUtil.print("Fire    " + _skiller.iID + " >>> " + _skillee.iID + " hp:" + _skillee.kAbility.hp_cur);

		// skillee act ------------------------------------
		if (_skillee.kAbility.hp_cur <= 0)	// is die
		{
			_skillee.kAbility.hp_cur = 0;
		}

		if (skilll_type != "passive")
		{
			Hashtable hash = new Hashtable(); hash.Add("skill", iSkill_index); hash.Add("hp_add", iHp_add);
			_skillee.Act_set(Charic.eAct.hit, hash); 
		}
	}
}

//----------------------------------------------------------------------------------------	
int Damage_calc(Charic _skiller, Charic _skillee, string _name, float _param1)
{
	int damage = 0;

	// ap ------------------------------------
	int ap = _skiller.kAbility.ap_cur;

	if (_name == "ap_abs") ap = (int)_param1;
	if (_name == "ap_per") ap = (int) (ap *_param1);

	if (_skiller.Status_is(eStatus.berserker)) ap = (int)(ap * 1.5f);
	if (_skiller.Status_is(eStatus.weakness)) ap = (int)(ap * 0.5f);
	if (_skiller.Status_is(eStatus.paralysis)) ap = (int)(ap * 0.5f);

	// dp ------------------------------------
	int dp = _skillee.kAbility.dp_cur;

	if (_name == "critical") dp = 0;

	if (_skiller.Status_is(eStatus.confuse)) dp = (int)(dp * 0.5f);
	if (_skiller.Status_is(eStatus.paralysis)) dp = 0;

	// damage ----------------------------------
	damage = (int)(ap - dp);
	if (damage < 0) damage = 0;
	//Debug.Log("DMG : " + damage + "    " + ap + "-" + dp);

	if (_skiller.Status_is(eStatus.shield)) ap = (int)(ap * 0.5f);
	if (_skillee.Status_is(eStatus.nodamage)) ap = 0;

	//attr -------------------------------------
	damage = CalcDamage_attr(_skiller, _skillee, damage);	//속성계산.

	return damage;
}
// fAttr_add 는 속성 공격력 증가수치. 이벤트시 사용. //1:red,2:blue,3:green
int CalcDamage_attr(Charic _skiller, Charic _skillee, int _damage)
{
	int cdamage = _damage;


	float fAttr_effct = 1.20f;
	fAttr_effct = fAttr_effct + _skillee.kAbility.attr_buf; //속성버프.

	float attr_add = 1.0f;
	if (_skiller.kAbility.attr == 2 && _skillee.kAbility.attr == 1) attr_add = fAttr_effct;
	if (_skiller.kAbility.attr == 3 && _skillee.kAbility.attr == 2) attr_add = fAttr_effct;
	if (_skiller.kAbility.attr == 1 && _skillee.kAbility.attr == 3) attr_add = fAttr_effct;

	cdamage = (int)(_damage * attr_add);

	return cdamage;
}

// skill level 을 반영한 이펙트 효과. //주인공의 무기스킬의 효과를 계산한다.

float GetSkillLevelEffect(float _value)
{
	float fReturnValue = _value;

	return fReturnValue;
}     
//-----------------------------------------------------------------------------------------------------
ArrayList MakeTargetPlayer(Charic _skiller, int _skill_index)
{
	ArrayList _TargetArray = new ArrayList();

	_TargetArray.Clear();

	TableInfo_skill kSkillT = CGame.Instance.kTable.Get_TableInfo_skill(_skill_index);
	if (kSkillT == null) return _TargetArray;
	if (kSkillT.effect_name == "0") return _TargetArray;


	// target flag
	int targetflag = kSkillT.target;
	int target = (targetflag / 100) % 10;		//대상  1:적군,2:자신,3:아군(자신포함),4:아군(자신제외)
	int select = (targetflag / 10) % 10;		//선택  0:랜덤,1:공격최강,2:공격최소,3:방어최강,4:방어최소,5:생명최강,6:생명최소,7:죽은자
	int range = (targetflag / 1) % 10;          //범위  0:1인,1:2인,2:3인,3:4인,4:5인

	//Debug.Log("" + target + " " + select + "  " + range);

	if (target == 2)	//자신.
	{
		_TargetArray.Add(_skiller);
	}
	else
	{
		ArrayList kArray = new ArrayList();
		kArray = kCharicManager.FindTarget(_skiller, targetflag);

		foreach (Charic player in kArray)
		{
			_TargetArray.Add(player);
		}
	}

	//if( _TargetArray.Count == 0) {
	//	print ("CheckFire_OneShot() : no target");
	//	return;
	//}

	return _TargetArray;
}
*/




// manager
/*
    public Skill Skill_add(int _skill_index)
    {
        if (_skill_index == 0)  return null;

        Skill kSkill = new Skill(kCharicManager);

        //kSkill.uID = MyUtil.GetUid();   //

        kSkill.kTable = CGame.Instance.kTable.Get_TableInfo_skill(_skill_index);
        if (kSkill.kTable == null)
        {
            //MyUtil.print("ERROR: AddSkill() Get_TableInfo_skill() : " + _skill_index);
            return null;
        };

        kSkillList.Add(kSkill);

        return kSkill;
    }

    // 스킬 시작 ----------------------------------------------------------------------
    public void Skill_start(Charic _skiller, int _skill_index, string _type )
    {
        if (_skill_index == 0) return;
        if (_skiller == null) return;
        if (_skiller.IsDie()) return;

        //MyUtil.print("Skill_start charic:" + _skiller.iID + " skill:" + _skill_index);

        Skill kSkill = Skill_add(_skill_index); //스킬추가.

        kSkill.Init(_skiller, _skill_index, _type);
        kSkill.Fire(); //for server                
    }
    public float Skill_time_get(int _skill_index )
    {
        TableInfo_skill kTable = CGame.Instance.kTable.Get_TableInfo_skill(_skill_index);
        return kTable.live_time;
    }

    public bool IsCanAttack( Charic _skiller, Charic _skillee, int _skill_index )
    {
        if (_skiller == null) return false;
        if (_skillee == null) return false;

        if (_skiller.IsDie()) return false;
        if (_skillee.IsDie()) return false;

        return true;
    }

    public bool IsSkillSelf(int _skill_index) //자신. 아군.
    {
        TableInfo_skill kTable = CGame.Instance.kTable.Get_TableInfo_skill(_skill_index);
        if (kTable.effect_name == "0") return false;

        // target flag
        int targetflag = kTable.target;
        int target = (targetflag / 100) % 10;		//대상  1:적군,2:자신,3:아군(자신포함),4:아군(자신제외)
        int select = (targetflag / 10) % 10;		//선택  0:랜덤,1:공격최강,2:공격최소,3:방어최강,4:방어최소,5:생명최강,6:생명최소,7:죽은자
        int range = (targetflag / 1) % 10;          //범위  0:1인,1:2인,2:3인,3:4인,4:5인

        if (target == 2 || target == 3 || target == 4)
            return true;

        return false;
    }

    public bool IsSkillSolo(int _skill_index) // 개별.
    {
        TableInfo_skill kTable = CGame.Instance.kTable.Get_TableInfo_skill(_skill_index);
        if (kTable.effect_name == "0") return false;

        // target flag
        int targetflag = kTable.target;
        int target = (targetflag / 100) % 10;		//대상  1:적군,2:자신,3:아군(자신포함),4:아군(자신제외)
        int select = (targetflag / 10) % 10;		//선택  0:랜덤,1:공격최강,2:공격최소,3:방어최강,4:방어최소,5:생명최강,6:생명최소,7:죽은자
        int range = (targetflag / 1) % 10;          //범위  0:1인,1:2인,2:3인,3:4인,4:5인

        if (range == 0)
            return true;

        return false;
    }



    public ArrayList TargetPlayer_get(Charic _skiller, int _skill_index)
    {
        ArrayList _TargetArray = new ArrayList();

        _TargetArray.Clear();

        TableInfo_skill kSkillT = CGame.Instance.kTable.Get_TableInfo_skill(_skill_index);
        if (kSkillT == null) return _TargetArray;
        if (kSkillT.effect_name == "0") return _TargetArray;


        // target flag
        int targetflag = kSkillT.target;
        int target = (targetflag / 100) % 10;		//대상  1:적군,2:자신,3:아군(자신포함),4:아군(자신제외)
        int select = (targetflag / 10) % 10;		//선택  0:랜덤,1:공격최강,2:공격최소,3:방어최강,4:방어최소,5:생명최강,6:생명최소,7:죽은자
        int range = (targetflag / 1) % 10;          //범위  1:1인,2:2인,3:3인,4:4인,5:5인

        
        if (target == 2) return _TargetArray; //자신인 경우는 타겟불필요
        if (select != 0) return _TargetArray; //랜덤
        if (range != 1) return _TargetArray;  //1인
        {
            int targetf = 100;
            if (target == 1) targetf = 105; //적군
            if (target == 3) targetf = 305; //아군
            if (target == 4) targetf = 405; //아군(자신제외)

            ArrayList kArray = new ArrayList();
            kArray = kCharicManager.FindTarget(_skiller, targetf);

            foreach (Charic player in kArray)
            {
                _TargetArray.Add(player);
            }
        }

        return _TargetArray;
    }

*/