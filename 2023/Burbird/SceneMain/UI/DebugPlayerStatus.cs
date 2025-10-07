using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Burbird
{
    public class DebugPlayerStatus : MonoBehaviour
    {
        public Player player;

        Text[] arr_text;

        private void Awake()
        {
            arr_text = transform.GetComponentsInChildren<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            arr_text[1].text = "ATK Damage: " + player.playerStatus.ATKDamage;
            arr_text[2].text = "ATK Speed: " + player.playerStatus.ATKSpeed;
            arr_text[3].text = "Max HP: " + player.playerStatus.maxHp;

            arr_text[4].text = "Avoid Chance: " + player.playerStatus.avoidChance;
            arr_text[5].text = "Crit Chance: " + player.playerStatus.critChance;
            arr_text[6].text = "Avoid Chance: " + player.playerStatus.critDamage;
        }
    }
}