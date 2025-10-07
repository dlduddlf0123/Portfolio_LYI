using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

/// <summary>
/// 구구단 인식 시 생성될 각 오브젝트
/// 현재 구구단 번호에 따라 텍스트 파일 결정 및 오디오 재생
/// 헤더스 위치를 비워놓고 캐릭터 소환 및 애니메이션 재생
/// </summary>
public class GugudanObject : MonoBehaviour
{
    public  GugudanManager gugudanMgr;

    public TextMeshProUGUI txt_gugudan;
    public TextMeshProUGUI txt_result;

    protected Animator anim_gugudan;
    protected Animator anim_result;

    public CubeCharacter cubeHeader;
    CubeCharacter[] arr_cubeHeaders = new CubeCharacter[6];

    public bool isPlay = false;


    public int firstNum = 5;
    public int secondNum = 1;
    public int resultNum = 30;

    public Coroutine currentCoroutine = null;

    private void Awake()
    {
        gugudanMgr = GetComponentInParent<GugudanManager>();

        if (txt_gugudan == null)
            txt_gugudan = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        if (txt_result == null)
            txt_result = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();

        anim_gugudan = txt_gugudan.GetComponent<Animator>();
        anim_result = txt_result.GetComponent<Animator>();

        cubeHeader = transform.GetChild(1).GetComponent<CubeCharacter>();

        //for (int i = 0; i < arr_cubeHeaders.Length; i++)
        //{
        //    arr_cubeHeaders[i] = transform.GetChild(i + 1).GetComponent<CubeCharacter>();
        //    arr_cubeHeaders[i].gameObject.SetActive(false);
        //}
    }
    //private void Start()
    //{

    //    SetGugudan(firstNum, secondNum);

    //    OnGugudanStart();
    //}

    public void SetGugudan(int num1 = 1, int num2 = 1)
    {
        firstNum = num1 + 1;
        secondNum = num2 + 1;
        resultNum = firstNum * secondNum;

        txt_gugudan.text = firstNum + " x " + secondNum + " = ";
        txt_result.text = resultNum.ToString();

        gameObject.name = firstNum + " x " + secondNum + " = " + resultNum;
    }
    public void SetGugudan()
    {
        resultNum = firstNum * secondNum;

        txt_gugudan.text = firstNum + " x " + secondNum + " = ";
        txt_result.text = resultNum.ToString();

        gameObject.name = firstNum + " x " + secondNum + " = " + resultNum;
    }

    void UpAnimation()
    {
        anim_gugudan.SetTrigger("tUp");
        anim_result.SetTrigger("tUp");
    }

    void ActiveHeader(int headerNum)
    {
        for (int i = 0; i < arr_cubeHeaders.Length; i++)
        {
            arr_cubeHeaders[i].gameObject.SetActive(false);
        }

        cubeHeader = arr_cubeHeaders[headerNum];
        arr_cubeHeaders[headerNum].gameObject.SetActive(true);
    }

    public void NextGugudan()
    {
        if (secondNum < 9)
        {
            secondNum++;
        }
        else
        {
            return;
        }

        resultNum = firstNum * secondNum;

        txt_gugudan.text = firstNum + " x " + secondNum + " = ";
        txt_result.text = resultNum.ToString();

        gameObject.name = firstNum + " x " + secondNum + " = " + resultNum;

        //int headerNum = System.Convert.ToInt32(gugudanMgr.list__gugudanHeaders[firstNum][secondNum]);
        //ActiveHeader(headerNum);
        cubeHeader.PlayAnim(secondNum);

        UpAnimation();
        gugudanMgr.ReadText();
    }


    /// <summary>
    /// 큐브 인식 시 구구단 캐릭터 등장
    /// 숫자 등장, 숫자 읽기
    /// </summary>
    public virtual void OnGugudanStart()
    {
        if (isPlay)
        {
            return;
        }
        isPlay = true;

        gugudanMgr.currentGugudan = this;

        SetGugudan();
        //int headerNum = System.Convert.ToInt32(gugudanMgr.list__gugudanHeaders[firstNum + 1][secondNum + 1]);
        //ActiveHeader(headerNum);
        cubeHeader.PlayAnim(secondNum);

        UpAnimation();
        gugudanMgr.ReadText();
    }


    public virtual void OnGugudanEnd()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        secondNum = 1;
        isPlay = false;
    }


}
