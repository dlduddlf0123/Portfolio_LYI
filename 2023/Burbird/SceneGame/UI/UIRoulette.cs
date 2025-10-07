using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Burbird
{
    public class UIRoulette : MonoBehaviour
    {
        [SerializeField]
        private RouletteObject rouletteObject;
        [SerializeField]
        private RouletteWheel rouletteWheel;
        [SerializeField]
        private Button btn_start;
        [SerializeField]
        private Button btn_close;

        void Start()
        {
            Init();
            btn_start.onClick.AddListener(RouletteStartButton);
            btn_close.onClick.AddListener(RouletteCloseButton);
        }

        public void Init()
        {
            btn_start.gameObject.SetActive(true);
            btn_close.gameObject.SetActive(false);
            rouletteWheel.SetWheelItems();
        }

        /// <summary>
        /// ·ê·¿ µ¹¸®±â ½ÃÀÛ
        /// </summary>
        public void RouletteStartButton()
        {
            rouletteWheel.SpinWheel();
            btn_start.gameObject.SetActive(false);
            rouletteWheel.onSpinEnd = () => btn_close.gameObject.SetActive(true);
        }

        /// <summary>
        /// ·ê·¿ Ã¢ Á¾·á, º¸»ó ¼ö·É
        /// </summary>
        public void RouletteCloseButton()
        {
            rouletteWheel.GetRouletteReward();
            this.gameObject.SetActive(false);
            rouletteObject.gameObject.SetActive(false);
            rouletteObject.EndInteraction();
        }

    }
}