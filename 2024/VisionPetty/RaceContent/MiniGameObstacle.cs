using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;


namespace AroundEffect
{

    /// <summary>
    /// 12/4/2024-LYI
    /// 움직이는 장애물
    /// </summary>
    public class MiniGameObstacle : MonoBehaviour
    {

        public Rigidbody m_rigidbody;

        public UnityAction onReset;

        public void ObstacleInit()
        {
            m_rigidbody.velocity = Vector3.zero;

        }


        public void OnReset()
        {
            onReset?.Invoke();
        }


        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag(Constants.TAG.TAG_CHARACTER))
            {
                if (GameManager.Instance.lifeMgr.miniGameMgr.statMiniGame == MiniGameStatus.GAME)
                {
                    GameManager.Instance.lifeMgr.miniGameMgr.MiniGameResult();
                }
            }
            
        }




    }
}