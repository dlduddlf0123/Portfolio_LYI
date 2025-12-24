using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CharacterMove : MonoBehaviour
{
    public Spawner spawner;

    public GameObject[] lifeGauge = new GameObject[3];
    GameObject[] blinkBody = new GameObject[7];

    Animator mAnimator;

    private int hp = 2;
    private int laneState = 1;    ////// 0: 왼쪽라인  1 : 가운데라인  2 : 오른쪽라인
    private bool isMove = false;
    bool isGround = true;////// 움직일떄와 점프중일때 구분(점프중에 방향키 눌러서 위치 바뀌는것 방지)

    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();
        BlinkBodyInit();
    }

    //배열 초기화  //
    #region
    void BlinkBodyInit()
    {
        blinkBody[0] = transform.GetChild(1).GetChild(0).gameObject;
        blinkBody[1] = transform.GetChild(1).GetChild(3).gameObject;
        blinkBody[2] = transform.GetChild(1).GetChild(4).gameObject;
        blinkBody[3] = transform.GetChild(1).GetChild(5).gameObject;
        blinkBody[4] = transform.GetChild(1).GetChild(9).gameObject;
        blinkBody[5] = transform.GetChild(1).GetChild(10).gameObject;
        blinkBody[6] = transform.GetChild(1).GetChild(11).gameObject;
    }
    #endregion
    private void OnCollisionEnter(Collision collision)   ////캐릭터가 장애물과 충돌시 장애물 사라짐
    {
        if (collision.gameObject.CompareTag("rock"))
        {
            spawner.PushToPool(spawner.list_Rock, collision.gameObject);
            GetDamage();
        }
        else if (collision.gameObject.CompareTag("wood"))
        {
            spawner.PushToPool(spawner.list_Wood, collision.gameObject);
            GetDamage();
        }
    }

    void GetDamage()
    {
        spawner.soundMgr.PlaySfx(spawner.transform.position, spawner.soundMgr.sfx_hit);
        lifeGauge[hp].SetActive(false);
        hp--;
        if (hp == -1)
        {
            spawner.GameChange(GameState.GAMEOVER);
        }
        StartCoroutine(DoBlink(0.05f));
    }

    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////// 캐릭터 피격시 깜빡이는 효과 구현
    /// </summary>
    IEnumerator DoBlink(float seconds)
    {
        for (int ia = 0; ia < 3; ia++)
        {
            for (int i = 0; i < blinkBody.Length; i++)
            {
                blinkBody[i].SetActive(false);
            }
            yield return new WaitForSeconds(seconds);

            for (int i = 0; i < blinkBody.Length; i++)
            {
                blinkBody[i].SetActive(true);
            }
            yield return new WaitForSeconds(seconds);
        }
    }

    /// <summary>
    /// /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////동작관렴함수
    /// </summary>
    #region 동작관련함수

    //Like 따봉
    public void ActionLeftLike(int state)
    {
        StartCoroutine(LeftMove());
    }
    public void ActionRightLike(int state)
    {
        StartCoroutine(RightMove());
    }
    public void ActionLeftJump(int state)
    {
        StartCoroutine(Jump(0));
    }
    public void ActionRightJump(int state)
    {
        StartCoroutine(Jump(1));
    }

    IEnumerator LeftMove()
    {
        if (laneState != 0 && isMove == false)
        {
            spawner.HandEffect(0);
            isMove = true;
            mAnimator.SetTrigger("isRun");
            for (int i = 0; i < 60; i++)
            {
                transform.Translate(-0.05f, 0, 0);
                yield return new WaitForSeconds(0.01f);
            }
            isMove = false;

            if (laneState > 0)
                laneState--;
        }
    }
    IEnumerator RightMove()
    {
        if (laneState != 2 && isMove == false)
        {
            spawner.HandEffect(1);
            isMove = true;
            mAnimator.SetTrigger("isRun");
            for (int i = 0; i < 60; i++)
            {
                transform.Translate(0.05f, 0, 0);
                yield return new WaitForSeconds(0.01f);
            }
            isMove = false;

            if (laneState < 2)
                laneState++;
        }
    }
    IEnumerator Jump(int _hand)
    {
        if (isGround == true)
        {
            spawner.HandEffect(_hand);
            gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 6.2f, ForceMode.Impulse);
            mAnimator.SetTrigger("isJump");
            isGround = false;
            yield return new WaitForSeconds(1.35f);
            isGround = true;
        }
    }
    #endregion
    // ///////////////////////////////////////////////////////////////////////////////////////////////////////////

    //public void Move()
    //{

    //    if (Input.GetKeyDown(KeyCode.LeftArrow) && laneState != 0 && isMove == false)
    //    {

    //        //GetComponent<Rigidbody>().constraints = Default;
    //        StartCoroutine(LeftMove());

    //        if (laneState > 0)
    //            laneState--;

    //    }
    //    if (Input.GetKeyDown(KeyCode.RightArrow) && laneState != 2 && isMove == false)
    //    {

    //        //GetComponent<Rigidbody>().constraints = Default;
    //        StartCoroutine("RightMove");

    //        if (laneState < 2)
    //            laneState++;
    //    }
    //    //yield return new WaitForSeconds(1.0f);

    //}

}
