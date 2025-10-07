using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AroundEffect
{
    public class MiniGameReset : MonoBehaviour
    {
        GameManager gameMgr;

        private void Awake()
        {
            gameMgr = GameManager.Instance;
        }

        private void OnTriggerEnter(Collider coll)
        {
            if (gameMgr.statGame == GameStatus.MINIGAME)
            {
                if (coll.gameObject.CompareTag(Constants.TAG.TAG_OBSTACLE))
                {
                    MiniGameObstacle obstacle = coll.gameObject.GetComponentInParent<MiniGameObstacle>();
                    obstacle.ObstacleInit();
                    gameMgr.lifeMgr.miniGameMgr.ObjectReset(obstacle.gameObject);
                }
            }
        }

    }
}