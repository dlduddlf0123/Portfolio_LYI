using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터 물건 입에 물기, 뱉기 클래스
/// </summary>
public class MouthColl : MonoBehaviour
{
    Character header;
    SoundManager soundMgr;

    public GameObject attachObject { get; set; }    //입에 물고있는 오브젝트

    // Start is called before the first frame update
    void Awake()
    {
        soundMgr = GameManager.Instance.soundMgr;
        header = this.GetComponentInParent<Character>();

        attachObject = null;
    }

    /// <summary>
    /// 아이템 입에 물기
    /// </summary>
    /// <param name="_go">입에 고정할 오브젝트</param>
    public void AttachMouth(GameObject _go)
    {
        if (attachObject != null)
        {
            DetachMouth();
        }
        _go.GetComponent<Item>().isThrowing = false;
        if (_go.CompareTag("Ball"))
        {
            _go.GetComponent<Ball>().isTalk = false;
        }

        _go.transform.SetParent(this.transform);
        _go.transform.position = this.transform.position;

        _go.GetComponent<Rigidbody>().isKinematic = true;
        attachObject = _go;
    }

    /// <summary>
    /// 아이템 떨어트리기
    /// </summary>
    public void DetachMouth()
    {
        if (attachObject == null) { return; }

        attachObject.transform.SetParent(null);
        attachObject.GetComponent<Rigidbody>().isKinematic = false;
        attachObject = null;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Poop"))
        {
            header.Stop();
            header.currentAI = header.StartCoroutine(header.HateSmell());
            return;
        }
        if (other.CompareTag("Food"))
        {
            Food food = other.GetComponent<Food>();
            //음식먹기
            header.Stop();
            header.currentAI = header.StartCoroutine(header.EatFood(food));
        }
        else if (other.gameObject.layer == 8)
        {
            if (header.isAction == true)
            {
                //아이템 줍는 모션
                //header.Stop();
                //header.SetAnim(1);
                AttachMouth(other.gameObject);
                header.MoveCharacter(GameManager.Instance.mainCam.transform.position, GameManager.Instance.mainCam.gameObject);
            }
        }
    }
}
