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
        gap *= gameMgr.uiMgr.stageSize;
        speed *= gameMgr.uiMgr.stageSize;

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

        if (lastSelect != null)
        {
            lastSelect.action.Invoke();
        }
        lastSelect = null;
        // gameObject.SetActive(false);
    }

    IEnumerator GrassMove(Transform _target)
    {
        Vector3 startPos = _target.position;
        float dir = 1f;
        int count = 0;
        int maxCount = Random.Range(7, 14);

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
            gameMgr.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_SUCCESS);
            gameMgr.PlayParticleEffect(arr_arSelectables[correctNum].transform.GetChild(0).position + Vector3.forward, ReadOnly.Defines.PREFAB_EFFECT_TOUCH);

            gameMgr.currentEpisode.currentStage.arr_header[0].Success("That's Right!");
            gameMgr.currentEpisode.currentStage.arr_header[1].transform.position = new Vector3(arr_arSelectables[correctNum].transform.position.x, stageMgr.arr_header[1].transform.position.y, arr_arSelectables[correctNum].transform.position.z);
            gameMgr.currentEpisode.currentStage.arr_header[1].gameObject.SetActive(true);
            gameMgr.currentEpisode.currentStage.arr_header[0].StartCoroutine(gameMgr.LateFunc(() => EndInteraction(), waitTime));
        }
        else
        {
            gameMgr.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_FAILURE);
            gameMgr.currentEpisode.currentStage.arr_header[0].Failure("No..");
            gameMgr.currentEpisode.currentStage.arr_header[0].StartCoroutine(gameMgr.LateFunc(() => this.gameObject.SetActive(true), waitTime));
        }
    }

    public override void StartInteraction()
    {
        base.StartInteraction();
        gameMgr.currentEpisode.currentStage.arr_header[0].SetAnim(3);

        for (int i = 0; i < arr_arSelectables.Length; i++)
        {
            arr_arSelectables[i].GetComponent<Collider>().enabled = true;
            arr_arSelectables[i].action.AddListener(() => CheckSuccess());
            list_guidePosition.Add(arr_arSelectables[i].transform.position + Vector3.up * 2 * gameMgr.uiMgr.stageSize);
            StartCoroutine(GrassMove(arr_arSelectables[i].transform));
        }
        PlayGuideParticle();
        gameMgr.uiMgr.ui_game.ChangeHandIcon(HandIcon.INDEX);
    }

    public override void EndInteraction()
    {
        StopAllCoroutines();

        stageMgr.arr_header[1].transform.position = new Vector3(arr_arSelectables[correctNum].transform.position.x, stageMgr.arr_header[1].transform.position.y, arr_arSelectables[correctNum].transform.position.z);
        stageMgr.arr_header[1].gameObject.SetActive(true);

        for (int i = 0; i < arr_arSelectables.Length; i++)
        {
            arr_arSelectables[i].GetComponent<Collider>().enabled = false;
        }
        StopGuideParticle();
        base.EndInteraction();
        gameMgr.currentEpisode.currentStage.arr_header[0].SetAnim(0);
    }
}