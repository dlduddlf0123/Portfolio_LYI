using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindColl : MonoBehaviour
{
    public WindBoat windBoat;
    public  WindColl otherColl;

    RayInteractObject m_ray;
    

    public bool isColl = false;
    float collTimer = 2f;

    int collCount = 0;

    Coroutine currentCoroutine = null;

    private void Awake()
    {
        m_ray = GetComponent<RayInteractObject>();
    }
    private void Start()
    {
        m_ray.m_RayEvent.AddListener(() => EnterEvent());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10 &&
            other.gameObject.CompareTag("Player"))
        {
            isColl = true;
            collCount++;
            collTimer = 1f;

            CheckWindGesture();

            if (currentCoroutine == null)
            {
                currentCoroutine = StartCoroutine(CollTimer());
            }
        }
    }

    void EnterEvent()
    {
        if (m_ray.rayOriginTag != "Player")
        {
            return;
        }
        isColl = true;
        collCount++;
        collTimer = 1f;

        CheckWindGesture();

        if (currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(CollTimer());
        }
    }


    IEnumerator CollTimer()
    {
        while (collTimer > 0)
        {
            collTimer -= 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        isColl = false;
        collCount = 0;
        currentCoroutine = null;
    }

    void CheckWindGesture()
    {
        if (collCount > 2)
        {
            windBoat.wind += 0.3f;

            GameManager.Instance.soundMgr.PlaySfx(windBoat.transform, ReadOnly.Defines.SOUND_SFX_WIND);

            if (windBoat.wind > windBoat.MAX_SPEED)
            {
                windBoat.wind = windBoat.MAX_SPEED;
            }

            isColl = false;
            otherColl.isColl = false;
       
        }
    }

}
