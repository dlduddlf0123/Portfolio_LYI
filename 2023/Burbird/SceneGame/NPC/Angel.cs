using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    /// <summary>
    /// 5스테이지 마다 등장
    /// 체력 회복 혹은 약간의 기본 스텟 강화
    /// </summary>
    public class Angel : NPCInteraction
    {
        private UIAngel canvas_angel;

        private void Awake()
        {
            canvas_angel = transform.GetChild(1).GetComponent<UIAngel>();
        }

        private void OnDisable()
        {
            EndInteraction();
        }

        public override void NPCInit(Transform spawnPos)
        {
            canvas_angel = transform.GetChild(1).GetComponent<UIAngel>();
            base.NPCInit(spawnPos);
        }

        public override void StartInteraction()
        {
            base.StartInteraction();
            canvas_angel.gameObject.SetActive(true);

            canvas_angel.PerkCanvasActive();
        }


    }
}