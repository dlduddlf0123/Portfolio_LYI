using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveHandTracking;


public enum HandState
{
    NORMAL,
    ITEM,
}

public class HandInteract : MonoBehaviour
{
    GameManager gameMgr;
    HandRenderer hand;
    public HandInteract otherHand;

    GameObject attachObject;

    ParticleSystem handEffect;
    public bool isLeft;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        hand = this.GetComponent<HandRenderer>();
        handEffect = this.transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        isLeft = hand.isLeft;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            AttachHand(other.gameObject, this.transform);
        }
        if (other.CompareTag("Food"))
        {
            other.GetComponent<Food>().attachHand = this;
            other.GetComponent<Food>().isAttach = true;
            AttachHand(other.gameObject, this.transform);
            gameMgr.selectHeader.AI_Move(4);
            gameMgr.selectHeader.isAction = true;
        }
    }

    /// <summary>
    /// 아이템 입에 물기
    /// </summary>
    /// <param name="_go">입에 고정할 오브젝트</param>
    public void AttachHand(GameObject _go, Transform _tr)
    {
        if (attachObject != null)
        {
            StartCoroutine(ActionDetachHand());
        }

        _go.transform.SetParent(_tr);
        _go.transform.position = _tr.position;

        _go.GetComponent<Rigidbody>().isKinematic = true;
        attachObject = _go;
        hand.isShowHand = false;
        hand.colliderObject.GetComponent<Collider>().enabled = false;

        gameMgr.soundMgr.PlaySfx(this.transform.position, gameMgr.soundMgr.LoadClip("Sounds/SFX/jump_15"));
    }

    /// <summary>
    /// 아이템 떨어트리기
    /// </summary>
    public IEnumerator ActionDetachHand()
    {
        if (attachObject != null)
        {
            gameMgr.HandEffect(this);
            attachObject.transform.SetParent(null);
            attachObject.GetComponent<Rigidbody>().isKinematic = false;
            attachObject = null;
            hand.isShowHand = true;
            yield return new WaitForSeconds(0.2f);
            hand.colliderObject.GetComponent<Collider>().enabled = true;
        }
    }
    public void DetachHand()
    {
        if (attachObject != null)
        {
            //gameMgr.HandEffect(this);
            attachObject.transform.SetParent(null);
            attachObject.GetComponent<Rigidbody>().isKinematic = false;
            attachObject = null;
            hand.isShowHand = true;
            hand.colliderObject.GetComponent<Collider>().enabled = true;
        }
    }




}
