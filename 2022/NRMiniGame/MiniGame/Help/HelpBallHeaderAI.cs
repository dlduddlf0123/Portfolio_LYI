using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpBallHeaderAI : Character
{
    public Transform[] arr_movePoint;

    public void StartMove()
    {
        
    }

    public IEnumerator RandomMove()
    {
        while (gameMgr.statGame == GameStatus.GAMEPLAY)
        {
            MoveCharacter(arr_movePoint[Random.Range(0, arr_movePoint.Length)].position, 3f);

            yield return new WaitForSeconds(1f);
        }
    }
}
