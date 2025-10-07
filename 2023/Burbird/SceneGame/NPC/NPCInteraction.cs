using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Burbird
{

    /// <summary>
    /// 플레이어와 부딪혔을 때 작동되는 NPC들 동작
    /// </summary>
    public class NPCInteraction : MonoBehaviour
    {
        public bool isActive = false;

        StageStat statHolder;

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                StartInteraction();
            }
        }


        public virtual void NPCInit(Transform spawnPos = null)
        {
            if (spawnPos != null)
            {
                transform.position = spawnPos.position;
            }
            isActive = false;
        }

        public virtual void StartInteraction()
        {
            if (isActive) return;
            isActive = true;

            statHolder = StageManager.Instance.statStage;
            StageManager.Instance.statStage = StageStat.EVENT;

            StageManager.Instance.playerControll.Stop();
        }

        /// <summary>
        /// 상호작용 이후에 호출, 이벤트 상태 종료
        /// </summary>
        public virtual void EndInteraction()
        {
            StageManager.Instance.statStage = statHolder;
            statHolder = StageStat.NONE;
            Time.timeScale = 1f;
        }
    }
}