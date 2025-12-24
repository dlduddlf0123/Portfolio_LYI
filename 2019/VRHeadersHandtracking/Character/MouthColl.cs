using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (other.CompareTag("Food"))
        {
            Food food = other.GetComponent<Food>();
            //음식먹기
            header.StartCoroutine(header.EatFood(food));
        }
        if (other.CompareTag("Item"))
        {
            if (header.isAction == true)
            {
                //아이템 줍는 모션
                //header.Stop();
                //header.SetAnim(1);
                AttachMouth(other.gameObject);
                header.Stop();
                header.AI_Move(3);
            }
        }
    }
}
