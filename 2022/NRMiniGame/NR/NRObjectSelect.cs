using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NRObjectSelect : MonoBehaviour
{
    GameManager gameMgr;
    public NRSelectable[] arr_nrSelectables;
    public NRSelectable lastSelect;

    public UnityAction onSelectEnd;
    //public float spacing = 0.0f;
    //public bool isLayout = false;
    protected bool isDisable = false;

    void Awake()
    {
        gameMgr = GameManager.Instance;
        arr_nrSelectables = GetComponentsInChildren<NRSelectable>();

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
        for (int index = 0; index < arr_nrSelectables.Length; index++)
        {
            if (arr_nrSelectables[index].isSelected)
            {
                arr_nrSelectables[index].SetTriggerAnimation(2);
                arr_nrSelectables[index].isSelected = false;
            }
            arr_nrSelectables[index].isTimer = false;
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
    public void SelectObject(NRSelectable _selectObj)
    {
        if (lastSelect != null &&
            lastSelect == _selectObj)
        {
            StartCoroutine(DisableObject());
            return;
        }

        for (int index = 0; index < arr_nrSelectables.Length; index++)
        {
            if (arr_nrSelectables[index] != _selectObj &&
                arr_nrSelectables[index].isSelected)
            {
                arr_nrSelectables[index].SetTriggerAnimation(2);
                arr_nrSelectables[index].isSelected = false;
                arr_nrSelectables[index].isTimer = false;
            }
            else if (arr_nrSelectables[index] == _selectObj)
            {
                _selectObj.SetTriggerAnimation(1);
                _selectObj.isSelected = true;
            }
        }
        Debug.Log(_selectObj.name + "Selected");
        if (lastSelect == _selectObj)
        {
            StartCoroutine(DisableObject());
        }
        else
        {
            lastSelect = _selectObj;
        }
    }

    /// <summary>
    /// 전체 오브젝트들 사라지는 효과 보여주기
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator DisableObject()
    {
        for (int index = 0; index < arr_nrSelectables.Length; index++)
        {
            arr_nrSelectables[index].SetTriggerAnimation(3);
            arr_nrSelectables[index].isSelected = false;
            arr_nrSelectables[index].isTimer = false;
        }
        isDisable = true;
        yield return new WaitForSeconds(0.5f);
        isDisable = false;

        lastSelect.action.Invoke();
        lastSelect = null;
        gameObject.SetActive(false);
    }

    public bool IsEqual(ARSelectableObject _correct)
    {
        return _correct == lastSelect;
    }

}
