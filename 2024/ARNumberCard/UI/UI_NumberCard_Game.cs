using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;


namespace AroundEffect
{

    public class UI_NumberCard_Game : MonoBehaviour
    {
        GameManager gameMgr;
        public Button btn_language;
        public TextMeshProUGUI txt_language;


        private void Awake()
        {
            gameMgr = GameManager.Instance;
        }

        private void Start()
        {
            btn_language.onClick.AddListener(LanguageChangeButton);
        }

        public void Init()
        {
            ChangeLanguageText();
        }

        public void LanguageChangeButton()
        {
            if (gameMgr.gameLanguage == Language.KOREAN)
            {
                gameMgr.gameLanguage = Language.ENGLISH;
            }
            else if (gameMgr.gameLanguage == Language.ENGLISH)
            {
                gameMgr.gameLanguage = Language.KOREAN;
            }

            ChangeLanguageText();
            ES3.Save<Language>(Constants.ES3.GAME_LANGUAGE, gameMgr.gameLanguage);
        }

        public void ChangeLanguageText()
        {
            if (gameMgr.gameLanguage == Language.KOREAN)
            {
                txt_language.text = "Korean";
            }
            if (gameMgr.gameLanguage == Language.ENGLISH)
            {
                txt_language.text = "English";
            }
        }

    }
}