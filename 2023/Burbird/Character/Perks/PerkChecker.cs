using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Burbird
{
    public enum PerkGrade
    {
        NONE = 0,
        S,
        A,
        B,
        C,
    }

    /// <summary>
    /// CSV파일에서 읽어올 퍽 정보
    /// </summary>
    public class PerkInfo
    {
        public int num; //퍽 고유 번호 각 스크립트에서 지정
        public string name; //퍽 표시 이름
        public string description; //퍽 설명
        public PerkGrade grade; //퍽 등급
        public object status; //퍽 스탯 수치(각 데이터마다 타입이 다르니 변형해서 쓸것)

        public PerkInfo(List<object> csvData)
        {
            name = csvData[1].ToString();
            string s = csvData[2].ToString();
            description = (s.Contains("{0}")) ? string.Format(s, csvData[4].ToString()) : s;

            switch (csvData[3].ToString())
            {
                case "S":
                    grade = PerkGrade.S;
                    break;
                case "A":
                    grade = PerkGrade.A;
                    break;
                case "B":
                    grade = PerkGrade.B;
                    break;
                case "C":
                    grade = PerkGrade.C;
                    break;
                default:
                    grade = PerkGrade.NONE;
                    break;
            }

            if (csvData.Count > 4)
            {
                string csvStat = csvData[4].ToString();
                if (csvStat.Contains("%"))
                {
                    int length = 1;
                    if (csvStat.Contains("\r"))
                    {
                        length = 2;
                    }
                    double d = System.Convert.ToDouble(csvStat.Substring(0, csvStat.Length - length));
                    d *= 0.01f;
                    status = d;
                }
                else
                {
                    status = csvStat;
                }
            }
            else
            {
                status = null;
            }
        }
    }

    /// <summary>
    /// 퍽 기능들을 체크하고 활성화 여부를 가지고 있는 클래스
    /// 퍽 데이터 로드 역할
    /// 스테이지 매니저를 통해 접근 가능
    /// </summary>
    public class PerkChecker : MonoBehaviour
    {
        StageManager stageMgr;

        public List<List<object>> list__perkInfo = new List<List<object>>();


        //발사 방식 관련(중첩 가능)
        public int perk_multiShot = 0;      //멀티샷 퍽 획득시 1 증가
        public int perk_sideShot = 0;        //사이드샷 획득 시 1 증가
        public int perk_doubleShot = 0;    //더블샷 획득 시 1 증가
        public int perk_backShot = 0;       //백샷 획득 시 1 증가
        public int perk_diagonalShot = 0; //사선샷 획득 시 1 증가

        //투사체 이동 관련 효과
        public bool perk_piercingShot = false;
        public bool perk_bounceShot = false;
        public bool perk_chainShot = false;

        //캐릭터 이동 관련 효과
        public bool perk_fastMove = false;
        public bool perk_highJump = false;
        public bool perk_fly = false;

        //적 사망시 폭발 효과
        public bool perk_enemyBoom = false;
        public bool perk_fireBoom = false;
        public bool perk_poisonBoom = false;
        public bool perk_iceBoom = false;

        //기타 퍽
        public float perk_lifeSteal = 0; //체력 흡수
        public bool perk_berserk = false; //체력이 낮으면 공격력 상승
        public bool perk_fastLearn = false; //경험치 획득량 증가
        public bool perk_extraLife = false; //목숨 +1
        public bool perk_deadEye = false; //적 투사체가 느려진다
        public bool perk_fade = false; //일정시간마다 무적

        //퍽 관련 스텟
        public float perk_deathShot = 0f;
        public float perk_healMultiplier = 1f;
        public float perk_expMultiplier = 1f;
        public float perk_fadeTime = 0f;

        public float perk_playerShotSpeedMultiplier = 1f;
        public float perk_enemyShotSpeedMultiplier = 1f;

        public float perk_enemyBoomPower = 0f;
        public float perk_fireBoomPower = 0f;
        public float perk_poisonBoomPower = 0f;
        public float perk_iceBoomPower = 0f;

        void Awake()
        {
            stageMgr = StageManager.Instance;
            LoadPerkInfo();
        }

        public void LoadPerkInfo()
        {
            list__perkInfo = GameManager.Instance.csvLoader.ReadCSVDatas2("PerkList_ENG");
        }

    }
}