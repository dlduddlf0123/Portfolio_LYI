using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class UIAngel : UIPerk
    {
        public Angel angel;
        public List<Perk> list_randStat = new List<Perk>();
        public Perk healPerk;

        private void Awake()
        {
            angel = transform.parent.GetComponent<Angel>();
        }
        public override void PerkCanvasActive()
        {
            if (StageManager.Instance.currentRoom.GetComponent<RestRoom>().isAngelUsed)
            {
                return;
            }
            arr_selectPerk = new Perk[2];
            arr_selectPerk[0] = list_randStat[Random.Range(0, list_randStat.Count)];
            arr_selectPerk[1] = healPerk;

            for (int i = 0; i < transform.GetChild(1).childCount; i++)
            {
                Destroy(transform.GetChild(1).GetChild(i).gameObject);
            }

            Perk perk = Instantiate(arr_selectPerk[0].gameObject, transform.GetChild(1)).GetComponent<Perk>();
            perk.action_click = () => PerkCanvasClose(perk);

            Perk perk2 = Instantiate(arr_selectPerk[1].gameObject, transform.GetChild(1)).GetComponent<Perk>();
            perk2.action_click = () => PerkCanvasClose(perk2);


            gameObject.SetActive(true);
            Time.timeScale = 0f;
        }

        protected override void PerkCanvasClose(Perk selectedPerk)
        {
            base.PerkCanvasClose(selectedPerk);

            StageManager.Instance.currentRoom.GetComponent<RestRoom>().isAngelUsed = true;
           
            //angel 비활성화
            angel.gameObject.SetActive(false);
            angel.EndInteraction();

        }
        public override void PerkCanvasChange(Perk[] arr_perk)
        {
            arr_selectPerk = arr_perk;
            for (int i = 0; i < 2; i++)
            {
                arr_selectPerk[i] = arr_perk[i];
            }
        }
    }
}