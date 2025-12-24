using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
public class HelpBallPrefab : MonoBehaviour
{
    public UnityAction onDestroy;
    public UnityAction onDamage;
    bool isHand = false;
    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Header"))
        {
            //대가리에게 데미지, 점수 다운
            onDamage.Invoke();
        }
        else if (coll.gameObject.CompareTag("Player"))
        {
            if (!isHand)
            {
                isHand = true;
            }
            //점수 상승, 사라지고 풀로 복귀
            //GameManager.Instance.miniGameMgr.currentMiniGame.GetScore(100);

            GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1,1.1f), Random.Range(-1, 1.1f), Random.Range(-1, 1.1f)) * 100f);
            StartCoroutine(GameManager.Instance.LateFunc(()=>
            {
                isHand = false;
                onDestroy.Invoke();
            }));
        }
        else if (coll.gameObject.CompareTag("Terrain"))
        {
            onDestroy.Invoke();
        }
    }
}
