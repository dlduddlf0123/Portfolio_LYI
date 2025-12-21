using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSelect : ARObjectSelect
{
    public string titleString;
    public float waitTime = 1f;
    public int correctNum = 4;

    public float gap = 0.05f;
    public float speed = 5f;

    private void Start()
    {
        //txt_title.text = titleString; //지정된 텍스트로 변환

        for (int i = 0; i < arr_arSelectables.Length; i++)
        {
            arr_arSelectables[i].action.AddListener(() => CheckSuccess());
            list_guidePosition.Add(arr_arSelectables[i].transform.position);
        }

    }

    public override IEnumerator DisableObject()
    {
        gameMgr.uiMgr.worldCanvas.StopTimer();

        lastSelect.SetTriggerAnimation(3);
        lastSelect.isSelected = false;
        lastSelect.isTimer = false;

        isDisable = true;
        yield return new WaitForSeconds(0.5f);
        isDisable = false;

        lastSelect.action.Invoke();
        lastSelect = null;
       // gameObject.SetActive(false);
    }

    IEnumerator GrassMove(Transform _target)
    {
        Vector3 startPos = _target.position;
        float dir = 1f;
        int count = 0;
        int maxCount = Random.Range(7,14);

        while (true)
        {
            if (count < maxCount)
            {
                if (_target.position.x < startPos.x - gap)
                {
                    dir = 1;
                    count++;
                }
                else if (_target.position.x > startPos.x + gap)
                {
                    dir = -1;
                    count++;
                }

                _target.position += Vector3.right * dir * speed * Time.deltaTime;
            }
            else 
            {
                maxCount = Random.Range(7, 14);
                count = 0;
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(0.0167f);
        }
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
            gameMgr.currentEpisode.currentStage.header.StartCoroutine(gameMgr.LateFunc(() => EndInteraction(), waitTime));
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

        for (int i = 0; i < arr_arSelectables.Length; i++)
        {
            arr_arSelectables[i].StartCoroutine(GrassMove(arr_arSelectables[i].transform));
        }
            PlayGuideParticle();
    }

    public override void EndInteraction()
    {
        gameMgr.currentEpisode.currentStage.arr_header[1].gameObject.SetActive(true);
        base.EndInteraction();
        gameMgr.currentEpisode.currentStage.header.SetAnim(0);
        gameObject.SetActive(false);
    }
}
