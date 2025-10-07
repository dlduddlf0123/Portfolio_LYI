using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Burbird
{

    /// <summary>
    /// º¸½º Å¬¸®¾î½Ã ·ê·¿µîÀå
    /// ·£´ý º¸»ó È¹µæ
    /// </summary>
    public class RouletteObject : NPCInteraction
    {
        [Header("Roulette Object")]
        [SerializeField]
        UIRoulette ui_roulette;

        public override void NPCInit(Transform spawnPos)
        {
            base.NPCInit(spawnPos);
        }

        public override void StartInteraction()
        {
            base.StartInteraction();
            ui_roulette.Init();
            ui_roulette.gameObject.SetActive(true);
        }
    }
}