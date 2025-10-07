using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

/// <summary>
/// 매직큐브 물음표 인식 시 작동될 구구단 오브젝트
/// 랜덤 구구단 생성
/// 랜덤 캐릭터 활성화, 랜덤 애니메이션 표출
/// </summary>
public class GugudanQuestion : GugudanObject
{
    CubeCharacter[] arr_cubeHeadersa = new CubeCharacter[6];

    private void Awake()
    {
        gugudanMgr = GetComponentInParent<GugudanManager>();

        if (txt_gugudan == null)
            txt_gugudan = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        if (txt_result == null)
            txt_result = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();

        anim_gugudan = txt_gugudan.GetComponent<Animator>();
        anim_result = txt_result.GetComponent<Animator>();

        for (int i = 0; i < arr_cubeHeadersa.Length; i++)
        {
            arr_cubeHeadersa[i] = transform.GetChild(i + 1).GetComponent<CubeCharacter>();
            arr_cubeHeadersa[i].gameObject.SetActive(false);
        }
    }

    //private void OnEnable()
    //{
    //    OnGugudanStart();
    //}

    //private void OnDisable()
    //{
    //    OnGugudanEnd();
    //}

    void ActiveHeader(int headerNum)
    {
        for (int i = 0; i < arr_cubeHeadersa.Length; i++)
        {
            arr_cubeHeadersa[i].gameObject.SetActive(false);
        }
        arr_cubeHeadersa[headerNum].gameObject.SetActive(true);
    }

    public void SetRandomGugudan()
    {
        firstNum = Random.Range(1,10);
        secondNum = Random.Range(1, 10);
        resultNum = firstNum * secondNum;

        txt_gugudan.text = firstNum + " x " + secondNum + " = ";
        txt_result.text = resultNum.ToString();

        gameObject.name = firstNum + " x " + secondNum + " = " + resultNum;
    }

    /// <summary>
    /// 질문 먼저 보여주기, 뒤에 정답 올라옴
    /// </summary>
    /// <returns></returns>
    IEnumerator UpAnimAct()
    {
        txt_result.gameObject.SetActive(false);
        anim_gugudan.SetTrigger("tUp");
        gugudanMgr.ReadQuestionText();

        yield return new WaitForSeconds(2f);

        txt_result.gameObject.SetActive(true);
        anim_result.SetTrigger("tUp");
        gugudanMgr.ReadResultText();
    }

    public override void OnGugudanStart()
    {
        gugudanMgr.currentGugudan = this;

        SetRandomGugudan();

        int random = Random.Range(0, 6);
        cubeHeader = arr_cubeHeadersa[random];
        ActiveHeader(random);

        cubeHeader.RandomAnim();

        StartCoroutine(UpAnimAct());

    }


    public override void OnGugudanEnd()
    {

    }
}
