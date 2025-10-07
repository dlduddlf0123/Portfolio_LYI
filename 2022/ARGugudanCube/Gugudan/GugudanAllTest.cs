using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

/// <summary>
/// 구구단 인식 시 생성될 각 오브젝트
/// 현재 구구단 번호에 따라 텍스트 파일 결정 및 오디오 재생
/// 헤더스 위치를 비워놓고 캐릭터 소환 및 애니메이션 재생
/// </summary>
public class GugudanAllTest : GugudanObject
{
    private void Awake()
    {
        gugudanMgr = GetComponentInParent<GugudanManager>();

        cubeHeader = transform.GetChild(0).GetComponent<CubeCharacter>();

        txt_gugudan = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        txt_result = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();

        anim_gugudan = txt_gugudan.GetComponent<Animator>();
        anim_result = txt_result.GetComponent<Animator>();

    }

    //private void Start()
    //{
    //    SetGugudan(firstNum, secondNum);

    //    OnGugudanStart();
    //}

    public new void SetGugudan(int num1 = 1, int num2 = 1)
    {
        firstNum = num1 + 1;
        secondNum = num2 + 1;
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

    public void NextGugudana()
    {
        if (secondNum < 9)
        {
            secondNum++;
        }
        else
        {
            secondNum = 1;
            firstNum++;
        }

        resultNum = firstNum * secondNum;

        txt_gugudan.text = firstNum + " x " + secondNum + " = ";
        txt_result.text = resultNum.ToString();

        gameObject.name = firstNum + " x " + secondNum + " = " + resultNum;

        cubeHeader.RandomAnim();
        UpAnimation();
        gugudanMgr.ReadText();
    }

    /// <summary>
    /// 큐브 인식 시 구구단 캐릭터 등장
    /// 숫자 등장, 숫자 읽기
    /// </summary>
    public new void OnGugudanStart()
    {
        gugudanMgr.currentGugudan = this;
        cubeHeader.ImageFound();

        UpAnimation();

        gugudanMgr.ReadText();

    }


    public new void OnGugudanEnd()
    {

    }


}
