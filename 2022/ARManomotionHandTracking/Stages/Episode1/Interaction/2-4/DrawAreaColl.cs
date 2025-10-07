using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawAreaColl : MonoBehaviour
{
    public DrawLightInteraction drawMgr { get; set; }

    public int collNum { get; set; }
    public bool isColl = false;

    private void OnTriggerEnter(Collider other)
    {
        if (drawMgr.gameMgr.statGame != GameStatus.INTERACTION ||
            drawMgr.gameMgr.currentEpisode.currentStage.currentInteraction != 2)
        {
            return;
        }

        if (other.gameObject.layer == 10 &&
            other.gameObject.CompareTag("Index"))
        {
            isColl = true;
            Debug.Log(gameObject.name + isColl);

            switch (collNum)
            {
                case 0: //Start
                    drawMgr.StartCoroutine(drawMgr.DrawLight(other.transform.position));
                    gameObject.SetActive(false);
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10 &&
            other.gameObject.CompareTag("Index"))
        {
            isColl = false;
            Debug.Log(gameObject.name + isColl);
            if (collNum == 2)
            {
                drawMgr.arr_areaColl[0].gameObject.SetActive(true);
            }
        }
    }
}
