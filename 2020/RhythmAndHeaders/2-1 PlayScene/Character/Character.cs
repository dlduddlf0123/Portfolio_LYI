using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Status 
{
    public int maxHP = 100;
    public int hp = 0;
    public float speed;
    public string name;
}


public class Character : MonoBehaviour
{
    public PlayManager inGameMgr { get; set; }

    public Status status = new Status();

    public Rigidbody mRigidbody;
    public Animator mAnimator;
    public int attackMotion;

    public SkinnedMeshRenderer[] mRenderer;
    public GameObject dustEffect;

    public bool isGround = true;
    public bool isAttack = false;
    public bool isUp = false;
    public bool isPositioned;
    public float attackCooltime = 1f;
    protected float coolTime = 0;

    public float sineAngle;

    private Touch touchInput;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        inGameMgr = PlayManager.Instance;

        mRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();

        mAnimator.SetBool("isMove", true);
    }
    protected virtual void Start()
    {
        Init();
    }

    public void Init()
    {
        status.hp = status.maxHP;
        inGameMgr.uiMgr.SetHPGauge(status.hp);
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            isGround = true;

        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
 /*       //입력 관련 동작
        if (coolTime > 0)
        {
            coolTime -= Time.deltaTime;
        }
        else// if (!EventSystem.current.IsPointerOverGameObject())
        {
            TouchMove1();
        }*/
        //돌아오는 동작 update
        if (isGround == false && inGameMgr.isLongNote == false)
        {
            isAttack = false;
            dustEffect.SetActive(false);
            sineAngle = 0;
            transform.Translate(Vector3.down * 0.3f);
        }
        else if (isGround == true && inGameMgr.isLongNote == false)
        {
            sineAngle = 0;
            dustEffect.SetActive(true);
            if (inGameMgr.isFever == true)
            {
                isAttack = false;
                transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(-5,0,-3), Time.deltaTime*7);
            }
            else if (inGameMgr.isFever == false)
            {
                isAttack = false;
                transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(-1.5f,0,1), Time.deltaTime*7);
            }
        }
        if (inGameMgr.isLongNote == true)
        {
            transform.Translate(0, Mathf.Sin(sineAngle) * -0.1f, 0);
            sineAngle += 0.1f;
        }



        /*      if (inGameMgr.isLongNote == false)
              {
                  if (inGameMgr.isFever == true)
                  {
                      if (isGround == true)
                      {
                          isAttack = false;
                          transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 0, 5), Time.deltaTime * 5);
                      }
                      else
                      {
                          isAttack = false;
                          sineAngle = 0;
                          transform.Translate(Vector3.down * 0.3f);
                      }

                  }
                  else if(inGameMgr.isFever ==false)
                  {
                      isAttack = false;
                      transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 0, 0), Time.deltaTime * 5);
                      if(isGround==false)
                      {
                          isAttack = false;
                          sineAngle = 0;
                          transform.Translate(Vector3.down * 0.3f);
                      }
                  }

              }*/
        if(Input.GetKeyDown(KeyCode.Space))
        {
            inGameMgr.doubleNoteHit.SetActive(true);
        }
    }
    /// <summary>
    /// 인수가 true면 땅공격, false면 공중공격
    /// </summary>
    /// <param name="groundAttack"></param>
    /// <returns></returns>
    public void AttackAnim()
    {
        if (isAttack)
        {
            return;
        }
        mAnimator.SetInteger("AttackTriggers", attackMotion);
        StartCoroutine(AttackMove());
    }
    protected IEnumerator AttackMove()
    {
        isAttack = true;
        coolTime = attackCooltime;
        mAnimator.SetTrigger("isTrigger");
        yield return new WaitForSeconds(0.1f);
    }

    public void Hit()
    {
        StopAllCoroutines();
        StartCoroutine(HitEffect());

        inGameMgr.combo = 0;
        inGameMgr.uiMgr.SetComboText(0);

        status.hp -= 20;
        if (status.hp < 0)
        {
            status.hp = 0;
        }
        if (status.hp == 0)
        {
            inGameMgr.uiMgr.ShowResult();
        }
        inGameMgr.uiMgr.SetHPGauge(status.hp);
        mAnimator.SetTrigger("isHit");
    }

    protected IEnumerator HitEffect()
    {
        gameObject.tag = "Invincible";
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < mRenderer.Length; i++)
            {
                mRenderer[i].enabled = false;
            }
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < mRenderer.Length; i++)
            {
                mRenderer[i].enabled = true;
            }
            yield return new WaitForSeconds(0.1f);
        }
        gameObject.tag = "Player";
    }

    //public void TouchMove()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        if (Input.mousePosition.x < gameMgr.uiMgr.canvaswidth / 2)
    //        {
    //            if (gameMgr.check.isLeft == true && gameMgr.check.isUp == false)
    //            {
    //                transform.localPosition = new Vector3(0, 0, -2.6f);
    //            }
    //            else if (gameMgr.check.isLeft == false && gameMgr.check.isUp == false)
    //            {
    //                transform.localPosition = new Vector3(0, 0, 10f); //5.3f 기존값

    //            }
    //            Attack();
    //        }
    //        else if (Input.mousePosition.x > gameMgr.uiMgr.canvaswidth / 2)
    //        {
    //            if (gameMgr.check.isLeft == true && gameMgr.check.isUp == true)
    //            {
    //                transform.localPosition = new Vector3(0, 6.8f, -2.6f);
    //            }
    //            else if (gameMgr.check.isLeft == false && gameMgr.check.isUp == true)
    //            {
    //                transform.localPosition = new Vector3(0, 6.8f, 10f);

    //            }
    //            Attack();
    //        }
    //    }
    //}
    public void TouchMove1()
    {

            if (/*Input.mousePosition.x < inGameMgr.uiMgr.canvaswidth / 2 ||*/ inGameMgr.inputMgr[0].stateInput == StateInput.CLICK)
            {
                if (inGameMgr.check.isLeft == true && inGameMgr.check.isUp == false)
                {
                    AttackAnim();
                    transform.position = inGameMgr.guidePoint[2].position - new Vector3(1, 0, 0);
                }
                else if (inGameMgr.check.isLeft == true && inGameMgr.check.isUp == true)
                {
                    isGround = false;
                    AttackAnim();
                    transform.position = inGameMgr.guidePoint[0].position - new Vector3(1, 0, 0);

                }

            }
            else if (/*Input.mousePosition.x > inGameMgr.uiMgr.canvaswidth / 2 || */inGameMgr.inputMgr[1].stateInput == StateInput.CLICK)
            {
                if (inGameMgr.check.isLeft == false && inGameMgr.check.isUp == false)
                {
                    AttackAnim();
                    transform.position = inGameMgr.guidePoint[3].position - new Vector3(1, 0, 0);
                }
                else if (inGameMgr.check.isLeft == false && inGameMgr.check.isUp == true)
                {
                    isGround = false;
                    AttackAnim();
                    transform.position = inGameMgr.guidePoint[1].position - new Vector3(1, 0, 0);
                }

            }
        
    }
    //public void TouchMove2()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        if (Input.mousePosition.y < gameMgr.uiMgr.canvasheight / 2)
    //        {
    //            if (gameMgr.check.isLeft == true && gameMgr.check.isUp == false)
    //            {
    //                transform.localPosition = new Vector3(0, 0, -2.6f);
    //            }
    //            else if (gameMgr.check.isLeft == false && gameMgr.check.isUp == false)
    //            {
    //                transform.localPosition = new Vector3(0, 0, 10f); //5.3f 기존값

    //            }
    //            Attack();
    //        }
    //        else if (Input.mousePosition.y > gameMgr.uiMgr.canvasheight / 2)
    //        {
    //            if (gameMgr.check.isLeft == true && gameMgr.check.isUp == true)
    //            {
    //                transform.localPosition = new Vector3(0, 6.8f, -2.6f);
    //            }
    //            else if (gameMgr.check.isLeft == false && gameMgr.check.isUp == true)
    //            {
    //                transform.localPosition = new Vector3(0, 6.8f, 10f);

    //            }
    //            Attack();
    //        }
    //    }
    //}

}
