using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터 물건 입에 물기, 뱉기 클래스
/// </summary>
public class MouthColl : MonoBehaviour
{
    public Character header;
    SoundManager soundMgr;

    public Transform trMouth;   //입
    GameObject attachObject;    //입에 물고 있는 오브젝트
    
    private void Awake()
    {
        soundMgr = GameManager.Instance.soundMgr;
        header = this.GetComponentInParent<Character>();
        trMouth = this.transform;
    }

    //아이템을 캐릭터에 귀속
    public void AttachMouth(GameObject _target)
    {
        if (attachObject != null)
        {
            DetachMouth();
        }
        _target.transform.SetParent(trMouth);
        _target.transform.position = trMouth.position;
        _target.GetComponent<Rigidbody>().isKinematic = true;

        attachObject = _target;
    }
    //해제
    public void DetachMouth()
    {
        if (attachObject == null) return;
        attachObject.transform.SetParent(null);
        attachObject.GetComponent<Rigidbody>().isKinematic = false;
        attachObject = null;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Food"))
    //    {
    //        Food food = other.GetComponent<Food>();
    //        //음식먹기
    //        header.StartCoroutine(header.EatFood(food));
    //    }
    //}
}

