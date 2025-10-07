using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace Burbird
{

    public class UIWorldSelect : MonoBehaviour
    {
        GameManager gameMgr;

        public WorldSelectImage[] arr_selectImg = null;
        public GameObject prefabSelectImgOrigin;

        Button btn_back;


        private void Awake()
        {
            gameMgr = GameManager.Instance;

            btn_back = transform.GetChild(1).GetComponent<Button>();
        }
        void Start()
        {
            btn_back.onClick.AddListener(ButtonBack);
        }

        private void OnEnable()
        {
            CreateSelectImage();
        }

        void ButtonBack()
        {
            gameMgr.uiMgr.SetUIActive(UIWindow.WORLD, false);
        }


        public void CreateSelectImage()
        {
            if (arr_selectImg.Length > 0)
            {
                return;
            }
            Transform contents = transform.GetChild(0).GetChild(0).GetChild(0);

            arr_selectImg = new WorldSelectImage[gameMgr.dataMgr.list_stageData.Count];
            for (int i = 0; i < arr_selectImg.Length; i++)
            {
                GameObject selectImg = Instantiate(prefabSelectImgOrigin);
                selectImg.transform.SetParent(contents);
                selectImg.GetComponent<WorldSelectImage>().SetWorldSelectImage(gameMgr.dataMgr.list_stageData[i]);
                arr_selectImg[i] = selectImg.GetComponent<WorldSelectImage>();
            }
        }
    }
}