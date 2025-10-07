using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
public class Food : MonoBehaviour
{
    public Transform spawnTr;
    public Throwable throwable;
    public int foodNum;   //음식종류
    public int satiety;      //포만도
    public int taste;        //맛(취향)
    private void Awake()
    {
        throwable = GetComponent<Throwable>();
        spawnTr = transform.parent;
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Mouth"))
        {
            Character header = coll.gameObject.GetComponent<MouthColl>().header;
            header.StartCoroutine(header.EatFood(this));
        }
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(CheckHeaderLikeFood());
        }
    }

    public IEnumerator CheckHeaderLikeFood()
    {
        float rottenTime = 10.0f;
        while (this.gameObject.activeSelf &&
            rottenTime > 0)
        {
            foreach (var header in GameManager.Instance.arr_headers)
            {
                if (header.Status.likeTaste == this.taste)
                {
                    header.Stop();
                    header.currentAI = header.StartCoroutine(header.MoveToTarget(this.transform.position, this.gameObject));
                }
            }
            yield return new WaitForSeconds(1.0f);
            rottenTime += 1;
        }
        gameObject.SetActive(false);
    }
}
