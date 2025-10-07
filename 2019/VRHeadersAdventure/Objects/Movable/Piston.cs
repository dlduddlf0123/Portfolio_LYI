using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : Movable
{
    Character header = null;

    Vector3 start;
    public Vector3 end;

    public float moveSpeed = 1.0f;
    public float waitTime = 0.5f;
    public float currentPos = 0.0f;

    public bool isOnce = false;
    public bool isActive = false;

    float time = 0.0f;
    bool isFront = true;
    bool isWait = false;

    // Start is called before the first frame update
    void Start()
    {
        start = this.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isActive)
        {
            if (currentPos < 1 &&
                !isWait &&
                isFront)
            {
                transform.position = Vector3.Lerp(start, start+end, currentPos);
                currentPos += Time.deltaTime * moveSpeed;

            }else  if(currentPos >= 1)
            {
                if (isFront)
                {
                    isWait = true;
                }
                isFront = false;
                currentPos = 1;
            }
            
            if (currentPos > 0 &&
                !isWait &&
                !isFront &&
                !isOnce)
            {
                transform.position = Vector3.Lerp(start, start + end, currentPos);
                currentPos -= Time.deltaTime * moveSpeed;
            }
            else if (currentPos <=0 )
            {
                if (!isFront)
                {
                    isWait = true;
                }
                isFront = true;
                currentPos = 0;
            }

            if (isWait &&
                time <= waitTime)
            {
                time += Time.deltaTime;
            }
            else
            {
                time = 0.0f;
                isWait = false;
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Header"))
        {
            switch (type)
            {
                case MoveType.TRAP:
                    collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 1000f + Vector3.up * 1000f);
                    break;
                default:
                    break;
            }
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Header"))
        {
            switch (type)
            {
                case MoveType.PLATFORM:
                    header = collision.GetComponent<Character>();
                    collision.transform.SetParent(this.transform, true);
                    break;
                default:
                    break;
            }
        } 
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Header"))
        {
            switch (type)
            {
                case MoveType.PLATFORM:
                    header = null;
                    collision.transform.SetParent(GameManager.Instance.stageMgr.transform.GetChild(0));
                    collision.transform.localScale = collision.GetComponent<Character>().currentScale;
                    break;
            }
        }
    }

    public override void Active(bool _active)
    {
        if (isActive != _active)
        {
            isActive = _active;
            StartCoroutine(LateSound(_active, 0.1f));
        }
    }

    //버튼 누르는 소리 이후에 소리가 나도록 함
    IEnumerator LateSound(bool _active, float _time)
    {
        yield return new WaitForSeconds(_time);
        if (_active)
        {
            soundMgr.PlaySfx(transform, soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_MOVE), Random.Range(1 - 0.2f, 1 + 0.2f));
        }
        else
        {
            soundMgr.PlaySfx(transform, soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_STOP), Random.Range(1 - 0.2f, 1 + 0.2f));
        }
    }
}
