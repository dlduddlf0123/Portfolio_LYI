using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.ReadOnly;

public class Enemy : Character
{
    protected override void DoAwake()
    {
        characterState = CharacterState.PATROL;

    }

    public override void Init()
    {
        Debug.Log(this.gameObject.name + "Init");
        Status.header = Headers.ENEMY;
        Status.moveSpeed = 0.5f;
        Status.findRange = 0.0f;
        Status.maxHp = 1;
        Status.hp = Status.maxHp;
        accel = 1.0f;
        Debug.Log("HP: " + Status.hp);
        isDie = false;
        isHit = false;
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

    //충돌 반응
    private void OnTriggerEnter(Collider other)
    {
        //캐릭터와 충돌 시 데미지 리셋
        if (other.gameObject.CompareTag("Header"))
        {
            Character chara = other.GetComponentInParent<Character>();
            gameMgr.uiMgr.GaugeMinus(chara.Status.maxHp - chara.Status.hp);
            chara.Init();
        }

        //총알에 맞으면 터짐
        if (other.CompareTag("ball"))
        {
            Hit(other);
        }
    }

    public override IEnumerator PatrolMove()
    {
        Debug.Log(this.gameObject.name + " AI: Patrol");

        characterState = CharacterState.PATROL;
        //SetAnim();

        int randHead = Random.Range(0, gameMgr.limit_headers); //대가리들
        Transform target = gameMgr.list_Headers[randHead].transform;
        float t = 0.0f;
        while (isHit == false
            && isDie == false
            && t < 5.0f)
        {
            t += 0.01f;
            transform.position = Vector3.Lerp(transform.position, target.transform.position, Status.moveSpeed * accel * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), Status.moveSpeed * 4 * accel * Time.deltaTime);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
            yield return new WaitForSeconds(0.0167f);
        }
        Debug.Log(this.gameObject.name + " AI: Patrol End");
        yield return new WaitForSeconds(0.1f);
        AI_Move(0);
    }
    

    public override void Hit(Collider other)
    {
        if (isDie == true || gameMgr.gameState != GameState.PLAYING) { return; }
        isHit = true;
        gameMgr.current_enemies--;
        this.gameObject.SetActive(false);
        StopAllCoroutines();
        Init();
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


}
