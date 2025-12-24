using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WatchCamera : MonoBehaviour
{
    GameManager gameMgr;
    Character chara;

    Transform startTransform;

    void Awake()
    {
        gameMgr = GameManager.Instance;
        startTransform = transform;
    }

    // Use this for initialization
    void OnEnable()
    {
        chara = this.transform.parent.GetComponent<Character>();
        StartCoroutine(FixedPosition());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MainCamera")
            && gameMgr.ai_level >= 1
            && chara.isClean == false
            && chara.statAnim != AnimState.RUN
            && chara.statAnim != AnimState.HIT)
        {
            chara.Stop();
            chara.AI_Move(1);
        }
    }

    public void GetHeader()
    {
        switch (chara.Status.header)
        {
            case Headers.NONE:
                break;
            case Headers.GIRRAFE:
                chara = chara.GetComponent<CharGirrafe>();
                break;
            case Headers.ZEBRA:
                chara = chara.GetComponent<CharZebra>();
                break;
            case Headers.PIG:
                chara = chara.GetComponent<CharPig>();
                break;
            case Headers.RHINO:
                chara = chara.GetComponent<CharRhino>();
                break;
            case Headers.ELEPHANT:
                chara = chara.GetComponent<CharElephant>();
                break;
            case Headers.MOUSE:
                chara = chara.GetComponent<CharMouse>();
                break;
            default:
                chara = chara.GetComponent<Enemy_Cloud>();
                break;
        }
    }

    private IEnumerator FixedPosition()
    {
        while (true)
        {
            transform.localPosition = new Vector3(0,2,0);
            transform.localRotation = new Quaternion(0,0,0,0);
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
