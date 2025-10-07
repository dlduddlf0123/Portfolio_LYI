using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;

namespace Burbird
{
    public enum ItemType
    {
        NONE,
        GOLD,
        DIAMOND,
        PERK,
    }

    /// <summary>
    /// 룰렛 돌려서 결과 콜백
    /// </summary>
    public class RouletteWheel : MonoBehaviour
    {
        StageManager stageMgr;

        RouletteItem[] arr_item;
        public Sprite[] arr_sprite = new Sprite[2];
        public Perk_Heal healPerk;
        public Perk[] arr_statPerk = new Perk[3];

        public RouletteItem resultItem;

        int sections = 6;
        public float spinNum = 8f;

        private float sectionAngle;
        private int currentSection;
        public UnityAction onSpinEnd;
        Coroutine currentCoroutine = null;
        private void Awake()
        {
            stageMgr = StageManager.Instance;

        }

        public void WheelInit()
        {
            transform.eulerAngles = Vector3.zero; 
            currentSection = -1;
            resultItem = null;
        }

        /// <summary>
        /// 룰렛 활성화 시 각 룰렛에 아이템 세팅
        /// 힐 하나
        /// 스탯 하나
        /// 랜덤 퍽 하나
        /// 골드 소, 중, 대
        /// </summary>
        public void SetWheelItems()
        {
            //  arr_item = new RouletteItem[sections];
            WheelInit();
            arr_item = transform.GetComponentsInChildren<RouletteItem>();

            sections = arr_item.Length;
            sectionAngle = 360f / sections;
            currentSection = -1;

            arr_item[0].SetItem(ItemType.PERK, 1, null, healPerk);
            arr_item[1].SetItem(ItemType.GOLD, 100, arr_sprite[0]);
            arr_item[2].SetItem(ItemType.PERK, 1, null, arr_statPerk[Random.Range(0,arr_statPerk.Length)]);
            arr_item[3].SetItem(ItemType.GOLD, 1000, arr_sprite[0]);
            arr_item[4].SetItem(ItemType.PERK, 1, null, StageManager.Instance.PickRandomPerk());
            arr_item[5].SetItem(ItemType.GOLD, 10000, arr_sprite[0]);

        }

        /// <summary>
        /// 결과 결정 후 해당 방향으로 로테이션 애니메이션 진행
        /// </summary>
        public void SpinWheel()
        {
            int targetSection = Random.Range(0, sections); //1/6 랜덤 결과값
            float targetAngle = 30f + targetSection * sectionAngle; //결과값 기준 앵글 결정

            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
           currentCoroutine =  StartCoroutine(RotateWheel(targetAngle, targetSection));
        }

        /// <summary>
        /// 4바퀴 돈 뒤 서서히 느려지면서 선택된 객체로 정지
        /// </summary>
        /// <param name="targetAngle"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private IEnumerator RotateWheel(float targetAngle, int target)
        {
            float startAngle = 0;
            float remainingAngle = Mathf.Abs(targetAngle - startAngle);
            float currentRot = 1;
            int spinCheck = 0;
            
            WaitForSeconds wait = new WaitForSeconds(0.01f);
            float t = 0;
            float rotateSpeed = 20;

            float circleAngle = 360 * spinNum;
            //최종 전에 몇바퀴 돌기
            while (spinCheck < spinNum)
            {
                if (transform.rotation.eulerAngles.z < currentRot - spinCheck * 360)
                {
                    spinCheck++;
                    rotateSpeed-=2;
                }
                currentRot = transform.rotation.eulerAngles.z + spinCheck * 360;

                t = (currentRot + rotateSpeed) / circleAngle;
                float angle = Mathf.Lerp(startAngle, circleAngle, t);
                transform.rotation = Quaternion.Euler(0f, 0f, startAngle + angle);

                yield return wait;
            }

            startAngle = 0;
            t = 0;

            //최종 결정
            while (remainingAngle  > 0.1f)
            {
                if (rotateSpeed > 0.5f)
                {
                    rotateSpeed = 10 *(remainingAngle/targetAngle);
                }

                currentRot = transform.rotation.eulerAngles.z;

                t =(currentRot + rotateSpeed) / targetAngle;
                float angle = Mathf.Lerp(startAngle, targetAngle, t);
                transform.rotation = Quaternion.Euler(0f, 0f, startAngle + angle);

                remainingAngle = targetAngle - currentRot;

                yield return wait;
            }

            transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);

            currentSection = target;
            resultItem = arr_item[currentSection];

            onSpinEnd.Invoke();
        }

        /// <summary>
        /// 룰렛 보상 수령
        /// </summary>
        public void GetRouletteReward()
        {
            if (resultItem == null)
            {
                StaticManager.UI.MessageUI.PopupMessage("There is No Item");
                Debug.LogError("There is No Item");
                return;
            }

            Debug.Log("Get Roulette Reward:" + resultItem.itemType.ToString());

            switch (resultItem.itemType)
            {
                case ItemType.GOLD:
                    stageMgr.coinSpawner.SpawnCoins(stageMgr.playerControll.centerTr.position, resultItem.value);
                    stageMgr.coinSpawner.AbsorbCoin(true);

                    if (resultItem.value < 110)
                    {
                        stageMgr.playerControll.player.GetRewardExp(1);
                    }
                    else if (resultItem.value < 1100)
                    {
                        stageMgr.playerControll.player.GetRewardExp(2);
                    }
                    else if(resultItem.value < 11000)
                    {
                        stageMgr.playerControll.player.GetRewardExp(3);
                    }
                    else
                    {
                        stageMgr.playerControll.player.GetRewardExp(4);
                    }
                    break;
                case ItemType.DIAMOND:
                    GameManager.Instance.dataMgr.GetDiamond(resultItem.value);
                    break;
                case ItemType.PERK:
                    resultItem.perk.PerkClick();
                    break;
                default:
                    break;
            }
        }
    }
}