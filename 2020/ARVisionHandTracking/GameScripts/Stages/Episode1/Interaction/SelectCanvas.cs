using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCanvas : ARObjectSelect
{
    public Text txt_title;
    public string titleString;
    public float waitTime = 2f;
    public int correctNum = 5;

    private void Start()
    {
        //txt_title.text = titleString; //지정된 텍스트로 변환

        for (int i = 0; i < arr_arSelectables.Length; i++)
        {
            arr_arSelectables[i].action.AddListener(()=>CheckSuccess());
        }
    }

    private void OnEnable()
    {
        transform.position = new Vector3(gameMgr.currentEpisode.currentStage.header.transform.position.x, transform.position.y, gameMgr.currentEpisode.currentStage.header.transform.position.z);
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    //버튼동작
    /// <summary>
    /// 클릭한 오브젝트가 정답이 맞는지 체크
    /// </summary>
    public void CheckSuccess()
    {
        if (IsEqual(arr_arSelectables[correctNum]))
        {
            gameMgr.currentEpisode.currentStage.header.Success("맞았어!");
            gameMgr.currentEpisode.currentStage.header.StartCoroutine(gameMgr.LateFunc(() =>  EndInteraction(), waitTime));
        }
        else
        {
            gameMgr.currentEpisode.currentStage.header.Failure("아니야!");
            gameMgr.currentEpisode.currentStage.header.StartCoroutine(gameMgr.LateFunc(() => this.gameObject.SetActive(true), waitTime));
        }
    }

    public override void StartInteraction()
    {
        base.StartInteraction();
        gameMgr.currentEpisode.currentStage.header.SetAnim(3);
    }

    public override void EndInteraction()
    {
        base.EndInteraction();
        gameMgr.currentEpisode.currentStage.header.SetAnim(0);
        gameObject.SetActive(false);
    }
}
