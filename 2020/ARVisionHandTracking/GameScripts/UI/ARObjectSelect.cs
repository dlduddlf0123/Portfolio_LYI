using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ARObjectSelect : InteractionManager
{
    public ARSelectableObject[] arr_arSelectables;
    public ARSelectableObject lastSelect;

    //public float spacing = 0.0f;
    //public bool isLayout = false;
    protected bool isDisable = false;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        arr_arSelectables = GetComponentsInChildren<ARSelectableObject>();
        //spacing = transform.localScale.x * 2f;
        //for (int index = 0; index < transform.childCount; index++)
        //{
        //    list_guidePosition.Add(transform.GetChild(index).GetChild(0).position);
        //}
    }

    //private void Update()
    //{
    //    if (isLayout)
    //    {
    //        for (int index = 0; index < arr_arSelectables.Length; index++)
    //        {
    //            arr_arSelectables[index].gameObject.transform.position = Vector3.right * index * spacing;
    //        }

    //    }

    //}

    public void DeSelectObject()
    {
        if (isDisable)
        {
            return;
        }
        for (int index = 0; index < arr_arSelectables.Length; index++)
        {
            if (arr_arSelectables[index].isSelected)
            {
                arr_arSelectables[index].SetTriggerAnimation(2);
                arr_arSelectables[index].isSelected = false;
            }
            arr_arSelectables[index].isTimer = false;
        }
        lastSelect = null;
    }

    /// <summary>
    /// 오브젝트 선택 시 효과
    /// </summary>
    /// <param name="_selectNum"></param>
    //public void SelectObject(int _selectNum)
    //{
    //    if (lastSelect != null &&
    //        lastSelect == arr_arSelectables[_selectNum])
    //    {
    //        StartCoroutine(DisableObject());
    //        return;
    //    }

    //    for (int index = 0; index < arr_arSelectables.Length; index++)
    //    {
    //        if (index != _selectNum &&
    //            arr_arSelectables[index].isSelected)
    //        {
    //            arr_arSelectables[index].SetTriggerAnimation(2);
    //            arr_arSelectables[index].isSelected = false;
    //            arr_arSelectables[index].isTimer = false;
    //            if (list_guideParticle.Count != 0)
    //                list_guideParticle[index].Play();
    //        }
    //    }

    //    arr_arSelectables[_selectNum].SetTriggerAnimation(1);
    //    arr_arSelectables[_selectNum].isSelected = true;
    //    if (list_guideParticle.Count != 0)
    //        list_guideParticle[_selectNum].Stop();

    //    lastSelect = arr_arSelectables[_selectNum];
    //}
    public void SelectObject(ARSelectableObject _selectObj)
    {
        //if (lastSelect != null &&
        //    lastSelect == _selectObj)
        //{
        //    StartCoroutine(DisableObject());
        //    return;
        //}

        //for (int index = 0; index < arr_arSelectables.Length; index++)
        //{
        //    if (arr_arSelectables[index] != _selectObj &&
        //        arr_arSelectables[index].isSelected)
        //    {
        //        arr_arSelectables[index].SetTriggerAnimation(2);
        //        arr_arSelectables[index].isSelected = false;
        //        arr_arSelectables[index].isTimer = false;
        //        if (list_guideParticle.Count != 0)
        //            list_guideParticle[index].Play();
        //    }
        //    else if (arr_arSelectables[index] == _selectObj)
        //    {
        //        if (list_guideParticle.Count != 0)
        //            list_guideParticle[index].Stop();
        //        _selectObj.SetTriggerAnimation(1);
        //        _selectObj.isSelected = true;
        //    }
        //}
      

        lastSelect = _selectObj;
        StartCoroutine(DisableObject());
    }

    /// <summary>
    /// 전체 오브젝트들 사라지는 효과 보여주기
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator DisableObject()
    {
        gameMgr.uiMgr.worldCanvas.StopTimer();
        for (int index = 0; index < arr_arSelectables.Length; index++)
        {
            arr_arSelectables[index].SetTriggerAnimation(3);
            arr_arSelectables[index].isSelected = false;
            arr_arSelectables[index].isTimer= false;
        }
        isDisable = true;
        yield return new WaitForSeconds(0.5f);
        isDisable = false;

        StopGuideParticle();
        lastSelect.action.Invoke();
        lastSelect = null;
        gameObject.SetActive(false);
    }

    public bool IsEqual(ARSelectableObject _correct)
    {
        return _correct == lastSelect;
    }



    /// <summary>
    /// 스테이지 선택 완료 시 해당 위치에 스테이지 생성 후 시작
    /// 버튼으로 동작
    /// </summary>
    public void SetStage(int _stageNum)
    {
        GameObject stage = Instantiate(lastSelect.stagePrefab);
       
        stage.transform.position = gameMgr.planeGenerator.placedPos;
        stage.transform.rotation = gameMgr.planeGenerator.placedRot;
        stage.transform.localScale *= gameMgr.uiMgr.stageSize;
        lastSelect = null;

        gameMgr.uiMgr.SetUIActive(UIWindow.GAME, false);

        gameMgr.currentEpisode = stage.GetComponent<EpisodeManager>();
        gameMgr.currentEpisode.ActiveStage(_stageNum);
    }


    public override void StartInteraction()
    {
        base.StartInteraction();
        PlayGuideParticle();
    }
    public override void EndInteraction()
    {
        base.EndInteraction();
    }
}
