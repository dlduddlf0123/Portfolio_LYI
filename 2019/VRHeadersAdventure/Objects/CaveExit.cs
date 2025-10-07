using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveExit : MonoBehaviour
{
    SoundManager soundMgr;
    public CaveExit otherExit;
    public Transform exitPos;

    public int camNum = 0;
    public float coolTime = 1.0f;//연속으로 동굴 들어가는 것 방지
    public float pitch = 0.2f;
    public bool changeCam = false;
    public bool isEnter = false;

    private void Awake()
    {
        soundMgr = GameManager.Instance.soundMgr;
        exitPos = transform.GetChild(0);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Header") &&
            isEnter == false)
        {
            otherExit.StartCoroutine(otherExit.CaveCoolTime());
            soundMgr.PlaySfx(transform, soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_CAVE), Random.Range(1 - pitch, 1 + pitch));

            if (changeCam)
            {
                StartCoroutine(GameManager.Instance.stageMgr.ChangeCameraPosition(camNum));
            }
            coll.transform.position = otherExit.exitPos.position;
            coll.transform.rotation = otherExit.exitPos.rotation;
        }
    }

    IEnumerator CaveCoolTime()
    {
        isEnter = true;
        yield return new WaitForSeconds(coolTime);
        isEnter = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green * 0.75f;
        Gizmos.DrawSphere(exitPos.position, 0.5f);
        Gizmos.DrawRay(exitPos.position, exitPos.forward * 2);
    }
}
