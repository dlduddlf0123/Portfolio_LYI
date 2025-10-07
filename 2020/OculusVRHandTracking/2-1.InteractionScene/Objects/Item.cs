using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    protected StageManager stageMgr;
    public HandInteract attachHand = null;

    protected Rigidbody mRigidbody;

    protected Vector3 startPos;
    protected Quaternion startRot;

    public bool isAttach = false;
    public bool isThrowing = false;
    bool isRespawn = false;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        stageMgr = GameManager.Instance.currentPlay.GetComponent<StageManager>();
        mRigidbody = GetComponent<Rigidbody>();

        startPos = transform.position;
        startRot = transform.rotation;
    }

    protected  virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isThrowing)
            {
                stageMgr.interactHeader.MoveCharacter(transform.position, gameObject);
            }
            else
            {
                StartCoroutine(Reset(30));
            }
        }
    }

    protected virtual IEnumerator Reset(float _time)
    {
        if (isRespawn)
        {
            yield break;
        }
        isRespawn = true;
        yield return new WaitForSeconds(5);
        isRespawn = false;
        isThrowing = false;

        mRigidbody.isKinematic = true;
        mRigidbody.isKinematic = false;

        transform.position = startPos;
        transform.rotation = startRot;
    }
}
