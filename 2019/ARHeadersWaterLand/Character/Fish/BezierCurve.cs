using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour {

    GameManager gameMgr;
    Transform currentTarget;

    public float moveSpeed;
    public bool isMoving;
    public bool isFlee;

	void Awake () {
        gameMgr = GameManager.Instance;
        Init();
    }
    void Init()
    {
        StopAllCoroutines();
        //moveSpeed = Random.Range(0.7f, 1.4f); //각 캐릭터마다 다른 속도
        moveSpeed = 1f;
        isFlee = false;
        isMoving = false;
        this.transform.localPosition = Vector3.zero;
    }

    void Start()
    {
        Init();
    }
    private void OnDisable()
    {
        Init();
    }

    /// <summary>
    /// 베지에 커브를 활용한 이동
    /// </summary>
    /// <param name="_isFlee">도망갈 시 더 빠르게</param>
    /// <returns></returns>
	public IEnumerator CurveMove()
    {
       // Debug.Log(this.gameObject.name + " AI: Patrol");

        //랜덤 변수들
        int dir = Random.Range(0, 2); //1/2
        int randLayer = Random.Range(0, gameMgr.list_FleePoints.Count); //레이어 1/3
        int randChild = Random.Range(0, gameMgr.list_FleePoints[randLayer].childCount); //포인트 1/8

        Transform target = gameMgr.list_FleePoints[randLayer].GetChild(randChild);
        //Debug.Log(gameMgr.list_FleePoints[randLayer] +" "+ target);

        //현재 타겟과 같은 값일경우 다시 찾기
        if (target == currentTarget)
        {
            randChild = Random.Range(0, gameMgr.list_FleePoints[randLayer].childCount);
            target = gameMgr.list_FleePoints[randLayer].GetChild(randChild);
        }
        currentTarget = target;

        //Position 변수들
        Vector3 start = transform.position; //시작 위치 저장
        Vector3 p1 = start + (currentTarget.position - start) + Vector3.forward;   //커브 기준점 1
        Vector3 p2 = start + (currentTarget.position - start) + Vector3.back;       //커브 기준점 2

        //50%확률로 커브방향을 반대로
        if (dir == 0)
        {
            Vector3 temp = p1;
            p1 = p2;
            p2 = temp;
        }

        float t = 0.0f; //0 = 시작위치, 1 = 타겟 위치
        // moveSpeed = Random.Range(0.7f, 1.4f); //각 캐릭터마다 다른 속도
        moveSpeed = 1f;
        if (isFlee == true)  { moveSpeed *= 4; }//도망 시 빠르게
        isMoving = true;

        while (t < 1
            && isMoving == true)
        {
            t += 0.01f * moveSpeed;
            transform.position = BezierCurve4(start, p1, p2, currentTarget.position, t);
            yield return new WaitForSeconds(0.1f);  //실제로 보이지 않기에 시간을 적당히 준다
        }
        isMoving = false;
        t = 0.0f;
        //Debug.Log(this.gameObject.name + " AI: Patrol End");
    }

    public IEnumerator EnemyMove()
    {
        //Debug.Log(this.gameObject.name + " AI: Patrol");

        //랜덤 변수들
        int dir = Random.Range(0, 2); //1/2
        int randHead = Random.Range(0, gameMgr.list_Headers.Count); //대가리들

        Transform target = gameMgr.list_Headers[0].transform;
        print(target);
        
        currentTarget = target;

        //Position 변수들
        Vector3 start = transform.position; //시작 위치 저장
        Vector3 p1 = start + (currentTarget.position - start) + Vector3.forward;   //커브 기준점 1
        Vector3 p2 = start + (currentTarget.position - start) + Vector3.back;       //커브 기준점 2

        //50%확률로 커브방향을 반대로
        if (dir == 0)
        {
            Vector3 temp = p1;
            p1 = p2;
            p2 = temp;
        }

        float t = 0.0f; //0 = 시작위치, 1 = 타겟 위치
        isMoving = true;

        while (t < 1
            && isMoving == true)
        {
            t += 0.01f * moveSpeed;
            transform.position = BezierCurve4(start, p1, p2, currentTarget.position, t);
            yield return new WaitForSeconds(0.1f);  //실제로 보이지 않기에 시간을 적당히 준다
        }
        isMoving = false;
        t = 0.0f;
        //Debug.Log(this.gameObject.name + " AI: Patrol End");
    }

    /// <summary>
    /// 점 4개를 사용하는 베지에 커브
    /// </summary>
    /// <param name="p0">Start Point</param>
    /// <param name="p1">Curve Point 1</param>
    /// <param name="p2">Curve Point 2</param>
    /// <param name="p3">End Point</param>
    /// <param name="t">time 0~1</param>
    /// <returns></returns>
    Vector3 BezierCurve4(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float u2 = u * u;
        float u3 = u2 * u;
        float t2 = t * t;
        float t3 = t2 * t;

        Vector3 result =
            p0 * u3 +
            p1 * 3 * t * u2 +
            p2 * 3 * t2 * u +
            p3 * t3;

        return result;
    }
}
