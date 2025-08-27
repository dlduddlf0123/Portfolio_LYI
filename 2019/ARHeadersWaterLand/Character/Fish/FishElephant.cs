using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.ReadOnly;

public class FishElephant : Character
{
    protected override void DoAwake()
    {
        characterState = CharacterState.PATROL;

        Init();
    }

    public override void Init()
    {
        Debug.Log(this.gameObject.name + "Init");
        Status.header = Headers.ELEPHANT;
        Status.moveSpeed = 1.0f;
        Status.findRange = 0.0f;
        Status.maxHp = 3;
        Status.hp = Status.maxHp;
        accel = 1.0f;
        Debug.Log("HP: " + Status.hp);
        isDie = false;
        isHit = false;

        //피부를 오염상태로 초기화
        SetSkinTex(0);
        lastHit = 0;

        //컬리더 활성화
        for (int i = 0; i < 3; i++)
        {
            colls[i].SetActive(true);
        }
    }

    void Start()
    {
        Init();
        int rand = Random.Range(0, gameMgr.list_SpawnPoints.Count);
        spawnPoint = gameMgr.list_SpawnPoints[rand].localPosition;
        this.transform.localPosition = spawnPoint;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        Init();
        int rand = Random.Range(0, gameMgr.list_SpawnPoints.Count);
        spawnPoint = gameMgr.list_SpawnPoints[rand].localPosition;
        this.transform.localPosition = spawnPoint;
    }

    //--------------------------Type별 동작--------------------------
    /// <summary>
    /// 각 Type에 따른 코루틴을 실행시킨다
    /// </summary>
    /// <param name="_type">0:patrol/1:hit/2:flee/3:stay/4:fear/d:clear</param>
    public override void AI_Move(int _type)
    {
        if (gameMgr.gameState == GameState.RESULT)
        {
            StopAllCoroutines();
            return;
        }
        switch (_type)
        {
            case 0: //배회(미 인식)
                StartCoroutine(PatrolMove());
                break;
            case 1: //맞았을 때
                //StartCoroutine(Hit());
                break;
            case 2: //도망갈 때
                StartCoroutine(Flee());
                break;
            case 3: //가만히 있을 때, 정지할 때
                //StartCoroutine(Stay());
                break;
            case 4: //시야 안에 들어왔을 떄
                    // StartCoroutine(Fear());
                break;
            default: //깨끗해졌을 때
                StartCoroutine(Clean());
                break;
        }
    }
    /// <summary>
    /// 애니메이션 변경 시 호출, 현재 State에 따른 애니메이션 적용
    /// </summary>
    protected override void SetAnim()
    {
        switch (base.characterState)
        {
            case CharacterState.PATROL:
                m_Animator.SetBool(Defines.ANIM_BOOL_FLEE, false);
                break;
            case CharacterState.STAY:
                m_Animator.SetBool(Defines.ANIM_BOOL_FLEE, false);
                break;
            case CharacterState.HIT:
                m_Animator.SetTrigger(Defines.ANIM_TRIGGER_HIT);
                m_Animator.SetBool(Defines.ANIM_BOOL_FLEE, false);
                break;
            case CharacterState.FLEE:
                m_Animator.SetBool(Defines.ANIM_BOOL_FLEE, true);
                break;
            case CharacterState.FEAR:
                m_Animator.SetBool(Defines.ANIM_BOOL_FLEE, false);
                break;
            case CharacterState.CLEAN:
                m_Animator.SetTrigger("isClean");
                m_Animator.SetBool(Defines.ANIM_BOOL_FLEE, false);
                break;
        }
    }


}
