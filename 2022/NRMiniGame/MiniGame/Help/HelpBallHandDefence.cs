using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpBallHandDefence : MonoBehaviour
{
    public int handNum = 0;
    float moveSpeed = 8f;


    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(FollowHand());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }


    IEnumerator FollowHand()
    {
        GameManager gameMgr = GameManager.Instance;
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, gameMgr.handCtrlL.NRHandMove.arr_handFollwer[0].transform.position, moveSpeed * Time.deltaTime);

            yield return new WaitForSeconds(0.01f);
        }
    }
}
