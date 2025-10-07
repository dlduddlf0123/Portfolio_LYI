using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using MoreMountains.Feedbacks;
using MoreMountains.Tools;

namespace AroundEffect
{

    /// <summary>
    /// 9/4/2024-LYI
    /// 손 위에 표시되는 UI
    /// 인벤토리 표시 예시
    /// 실제 작동은 DataManager 홀드 데이터 갱신해서 보여주기
    /// UI 상호작용은 Slot으로 관리
    /// </summary>
    public class UI_OnHand : MonoBehaviour
    {
        GameManager gameMgr;

        [SerializeField] CanvasGroup canvasGroup;

        [Header("Button")]
        [SerializeField] Button[] arr_menuButton;
        [SerializeField] GameObject[] arr_menuInventory;

        //스테이터스 창
        //캐릭터에 따라 바뀐다
        [Header("Status")]
        [SerializeField] RectTransform tr_status;
        [SerializeField] TextMeshProUGUI txt_name;
        [SerializeField] UI_StatusGauge Gauge_like;
        [SerializeField] MMProgressBar MMP_hunger;
        [SerializeField] MMProgressBar MMP_energy;
        [SerializeField] MMProgressBar MMP_emotion;

        [SerializeField] Image img_likebg;
        [SerializeField] Sprite[] arr_barLevelSprite;

        [Header("MMF Player")]
        [SerializeField] MMF_Player mmf_open;
        [SerializeField] MMF_Player mmf_close;

        public CharacterManager handCharacter;



        [Header("Property")]
        public float minDistance = 1.5f;
        public float maxDistance = 2f;
        public bool isUIActive = false;
        public bool isLeft = false;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            gameMgr = GameManager.Instance;

            for (int i = 0; i < arr_menuButton.Length; i++)
            {
                int a = i;
                arr_menuButton[i].onClick.AddListener(() => ClickMenuButton(a));
            }

            MenuDisable();

        }

        /// <summary>
        /// 10/7/2024-LYI
        /// 메뉴 활성화 시
        /// </summary>
        public void MenuEnable()
        {
            for (int i = 0; i < arr_menuButton.Length; i++)
            {
                arr_menuButton[i].gameObject.SetActive(true);
            }
            CheckStatusUIActive();

            isUIActive = true;

            gameMgr.invenMgr.RedrawInventory();
        }

        /// <summary>
        /// 10/7/2024-LYI
        /// 메뉴 비활성화 시
        /// </summary>
        public void MenuDisable()
        {
            for (int i = 0; i < arr_menuButton.Length; i++)
            {
                arr_menuButton[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < arr_menuInventory.Length; i++)
            {
                arr_menuInventory[i].gameObject.SetActive(false);
            }
            DisableStatusUI();

            isUIActive = false;
        }

        private void Update()
        {
            if (isUIActive)
            {
                UpdateUIAlpha();
                UpdateUITransform();
            }
        }

        void UpdateUIAlpha()
        {
            float distance = Vector3.Distance(gameMgr.MRMgr.polySpatialInput.Device_headPos, transform.position);
            float alpha;


            if (distance < minDistance)
            {
                alpha = 1;
            }
            else
            {
                alpha = Mathf.Clamp01(1 - ((distance - minDistance) / (maxDistance - minDistance)));
            }

            canvasGroup.alpha = alpha;

        }

        /// <summary>
        /// 9/9/2024-LYI
        /// UI를 위한 연산이 필요해서 따로 제작
        /// </summary>
        void UpdateUITransform()
        {
            Quaternion look;
            if (isLeft)
            {
                transform.position = Vector3.Lerp(transform.position,
                    gameMgr.MRMgr.polySpatialInput.handInputL.tr_characterAnchor.position + Vector3.up * 0.1f, 0.8f);

               // Vector3 direction = transform.position - gameMgr.polySpatialInput.handInputL.tr_handWrist.position;
                Vector3 direction = transform.position - gameMgr.MRMgr.polySpatialInput.Device_headPos;
                look = Quaternion.LookRotation(direction);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position,
                    gameMgr.MRMgr.polySpatialInput.handInputR.tr_characterAnchor.position + Vector3.up * 0.1f, 0.8f);

               // Vector3 direction = transform.position - gameMgr.polySpatialInput.handInputR.tr_handWrist.position;
                Vector3 direction = transform.position - gameMgr.MRMgr.polySpatialInput.Device_headPos;
                look = Quaternion.LookRotation(direction);
            }

            bool rotationFix_X = true;
            bool rotationFix_Y = false;
            bool rotationFix_Z = true;
            if (rotationFix_X || rotationFix_Y || rotationFix_Z)
            {
                Quaternion q = Quaternion.Lerp(transform.rotation, look, 0.8f);
                float x = rotationFix_X ? 0 : q.x;
                float y = rotationFix_Y ? 0 : q.y;
                float z = rotationFix_Z ? 0 : q.z;
                transform.rotation = new Quaternion(x, y, z, q.w);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, look, 0.8f);
            }
        }


        /// <summary>
        /// 9/4/2024-LYI
        /// 각 버튼 클릭 시 동작
        /// </summary>
        /// <param name="num"></param>
        public void ClickMenuButton(int num)
        {
            arr_menuInventory[num].SetActive(!arr_menuInventory[num].activeSelf);
            OnMenuActive(num);
        }


        /// <summary>
        /// 9/4/2024-LYI
        /// 열릴 때 각 인벤토리 정보 받아와서 갱신?
        /// </summary>
        /// <param name="num"></param>
        public void OnMenuActive(int num)
        {
            gameMgr.invenMgr.RedrawInventory();
        }


        /// <summary>
        /// 10/7/2024-LYI
        /// 메뉴 활성화 시 호출
        /// 스탯창이 활성화 될 때 조건 체크 후 활성화
        /// </summary>
        public void CheckStatusUIActive()
        {
            XRHandGestureInput input = isLeft ?
                gameMgr.MRMgr.polySpatialInput.handInputL:
                gameMgr.MRMgr.polySpatialInput.handInputR;

            if (input.handCharacter != null)
            {
                handCharacter = input.handCharacter;
            }
            else
            {
                handCharacter = null;
            }

            if (handCharacter != null)
            {
                EnableStatusUI();
            }
        }


        /// <summary>
        /// 10/4/2024-LYI
        /// Status 창 활성화
        /// </summary>
        /// <param name="character"></param>
        public void EnableStatusUI()
        {
            tr_status.gameObject.SetActive(true);

            txt_name.text = handCharacter.Status.typeHeader.ToString();

            SliderInit();
        }

        /// <summary>
        /// 10/7/2024-LYI
        /// UI 비활성화 시 처리
        /// </summary>
        public void DisableStatusUI()
        {
            tr_status.gameObject.SetActive(false);
        }


        /// <summary>
        /// 10/4/2024-LYI
        /// 슬라이더들 초기화
        /// </summary>
        void SliderInit()
        {
            if (handCharacter  == null ||
                isUIActive == false)
            {
                return;
            }
            
            Gauge_like.SliderInit(handCharacter.Status.level_like, handCharacter.Status.likeMeter, handCharacter.Status.likeMeterMax);
            MMP_hunger.SetBar01(handCharacter.Status.hungerMeter/ handCharacter.Status.hungerMeterMax);
            MMP_energy.SetBar01(handCharacter.Status.energyMeter/ handCharacter.Status.energyMeterMax);
            MMP_emotion.SetBar01(handCharacter.Status.emotionMeter/ handCharacter.Status.emotionMeterMax);
        }

        /// <summary>
        /// 10/7/2024-LYI
        /// 터치, 먹이 등 수치 변화 시 호출
        /// 현재 슬라이더 변화 보여주기
        /// </summary>
        public void SliderRefresh()
        {
            if (handCharacter == null ||
                isUIActive == false)
            {
                return;
            }

            Debug.Log("Stat Slider Refresh()");

            Gauge_like.SliderRefresh(handCharacter.Status.level_like, handCharacter.Status.likeMeter, handCharacter.Status.likeMeterMax);
            MMP_hunger.UpdateBar01(handCharacter.Status.hungerMeter / handCharacter.Status.hungerMeterMax);
            MMP_energy.UpdateBar01(handCharacter.Status.energyMeter / handCharacter.Status.energyMeterMax);
            MMP_emotion.UpdateBar01(handCharacter.Status.emotionMeter / handCharacter.Status.emotionMeterMax);
        }

        public void SliderLevelUp(StatusLevel level, float remainGauge, float statMax)
        {
            Gauge_like.GaugeLevelUp(level, remainGauge, statMax);
        }
        public void SliderLevelDown(StatusLevel level, float remainGauge, float statMax)
        {
            Gauge_like.GaugeLevelDown(level, remainGauge, statMax);
        }

    }
}