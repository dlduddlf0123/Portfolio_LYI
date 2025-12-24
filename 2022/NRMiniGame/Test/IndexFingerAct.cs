using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexFingerAct : MonoBehaviour
{
    [SerializeField]
    NRHandMove nrHand;
    HandFollower indexFollower;

    [SerializeField]
    GameObject particle_click;

    Vector3 lastClickPos = Vector3.zero;

    int groundClickCount = 0;
    bool isGrab = false;

    private void Awake()
    {
        indexFollower = GetComponent<HandFollower>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Header") &&
            !nrHand.isPinch)
        {
            other.GetComponent<Character>().StopMove();
            other.GetComponent<Character>().PlayTriggerAnimation(1);
            other.GetComponent<TestKanto>().isRun = false;
        }
        if (other.gameObject.CompareTag("Header") &&
         nrHand.isPinch &&
         !isGrab)
        {
            isGrab = true;
            other.GetComponent<Character>().StopMove();
            other.GetComponent<Character>().PlayTriggerAnimation(2);
            StartCoroutine(PinchHeader(other.gameObject));
        }
        if (other.gameObject.CompareTag("Terrain"))
        {

            if (Vector3.Distance(lastClickPos, transform.position) < 0.3f)
            {
                groundClickCount++;
            }
            else
            {
                groundClickCount = 0;
            }


            if (groundClickCount > 2)
            {
                MoveToClickPoint(transform.position);
                groundClickCount = 0;
            }

            lastClickPos = transform.position;
        }
    }

    public void MoveToClickPoint(Vector3 _pos)
    {
        GameManager.Instance.PlayParticleEffect(transform.position, particle_click);
        GameManager.Instance.currentEpisode.currentStage.arr_header[0].GetComponent<TestKanto>().RunToPoint(_pos);
    }


    IEnumerator PinchHeader(GameObject _go)
    {
        float t = 0;
        Vector3 startPos = _go.transform.position;

        while (t < 0.5f &&
            nrHand.isPinch &&
            Vector3.Distance(startPos, _go.transform.position) < 2f)
        {
            _go.transform.position  = Vector3.Lerp(_go.transform.position, transform.position, 10 * Time.deltaTime);

            t += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }

        Vector3 randVec = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        _go.GetComponent<Character>().MoveCharacter(_go.transform.position + randVec, 2);

        isGrab = false;
    }
}
