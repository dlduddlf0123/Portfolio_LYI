using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AroundEffect
{
    //캐릭터 번호
    public enum HeaderType
    {
        NONE = 0,
        KANTO = 1,
        ZINO = 2,
        OODADA = 3,
        COCO = 4,
        DOINK = 5,
        TENA = 6,
    }

    public enum StatusLevel
    {
        VERY_BAD = 0,
        BAD,
        NORMAL,
        GOOD,
        VERY_GOOD,
    } 


    public enum StatusType
    {
        NONE = 0,
        LIKE,
        HUNGER,
        ENERGY,
        EMOTION,
        SICK,
        INJURE,
        CLEAN,
        POOP,
    }

    /// <summary>
    /// 240805 LYI
    /// Character status management
    /// m_status.HP, etc..
    /// </summary>
    public class CharacterStatus : MonoBehaviour
    {
        public CharacterManager charMgr;
        public CharacterStatus_Race Race;

        public HeaderType typeHeader;


        //0~100 수치, 저장 데이터
        public float likeMeter { get; set; }
        public float hungerMeter { get; set; }
        public float energyMeter { get; set; }
        public float emotionMeter { get; set; }
        public float likeMeterMax { get; set; }
        public float hungerMeterMax { get; set; }
        public float energyMeterMax { get; set; }
        public float emotionMeterMax { get; set; }

        //Status level
        public StatusLevel level_like { get; set; }
        public StatusLevel level_hunger { get; set; }
        public StatusLevel level_energy { get; set; }
        public StatusLevel level_emotion { get; set; }
        public StatusLevel level_clean { get; set; }


        //상태이상 관련
        public float cleanMeter { get; set; }
        public float cleanMeterMax { get; set; }
        public float sicknessPercent { get; set; } //질병 확률, 확률에 따라 질병에 걸린다 / 배고픔, 감정상태에 따라 영향?
        public float injurePercent { get; set; } //부상 확률, 확률에 따라 부상에 걸린다 / 배고픔, 에너지가 낮을 때?

        public float poopMeter { get; set; } //변의. 밥먹을때마다 차오름. 가득차면 똥싼다
        public float poopMeterMax { get; set; }

        //질병 + 부상 가능???
        //사망?
        public bool isSick = false;
        public bool isInjured = false;


        //11/4/2024-LYI
        //잠 자는 중인가?
        public bool isSleep = false;


        public bool isPromised = false;


        //호감도 관련
        //캐릭터마다 고정된 수치
        //음식
        public FoodType likeFood { get; set; }    //좋아하는 음식 종류
        public FoodType hateFood { get; set; }    //싫어하는 음식 종류
        public int likeTaste { get; set; }      //좋아하는 맛
        public int hateTaste { get; set; }      //싫어하는 맛

        //행동 취향
        public TouchCollider_HandType likehandType { get; set; } //좋아하는 손 형태
        public TouchCollider_HandType hatehandType { get; set; } //싫어하는 손 형태

        public TouchCollider_Direction likeTouchDirection { get; set; } //좋아하는 부위
        public TouchCollider_Direction hateTouchDirection { get; set; } //싫어하는 부위


        //내부 변수
        //캐릭터 대사리스트의 리스트
        //it will be change with emotes
        public List<List<List<object>>> list___dialog_kor { get; set; }
        public List<List<List<object>>> list___dialog_eng { get; set; }


        CharacterStatus()
        {
            likeMeter = 0f;
            hungerMeter = 50f;
            energyMeter = 50f;
            emotionMeter = 50f;

           // likeMeterMax = 100f
            hungerMeterMax = 100f;
            energyMeterMax = 100f;
            emotionMeterMax = 100f;
            poopMeterMax = 100f;

          //  level_like = StatusLevel.VERY_BAD;
            LikeLevelChange(StatusLevel.VERY_BAD);
            level_hunger = StatusLevel.NORMAL;
            level_energy = StatusLevel.NORMAL;
            level_emotion = StatusLevel.NORMAL;
            level_clean = StatusLevel.NORMAL;

            cleanMeter = 50f;
            cleanMeterMax = 100f;
            sicknessPercent = 0f;
            injurePercent = 0f;
            isSick = false;
            isInjured = false;
        }

        /// <summary>
        /// 9/6/2024-LYI
        /// 캐릭터 스탯 초기화
        /// Initialize status. with json?
        /// </summary>
        public virtual void Init()
        {
            //Race.Init();

            LoadStatus();
            
            //likeMeter = 0f;
            //hungerMeter = 50f;
            //energyMeter = 50f; 
            //emotionMeter = 50f;

            //level_like = StatLevel.VERY_BAD;
            //level_hunger = StatLevel.NORMAL;
            //level_energy = StatLevel.NORMAL;
            //level_emotion = StatLevel.NORMAL;

            //sicknessPercent = 0f;
            //injurePercent = 0f;
            //isSick = false;
            //isInjured = false;
        }

        public void SaveStatus()
        {
            ES3.Save<CharacterStatus>(typeHeader.ToString() + "Status", this);
        }

        public void LoadStatus()
        {
            CopyStatus(ES3.Load<CharacterStatus>(typeHeader.ToString() + "Status", new CharacterStatus()));
        }

        public void CopyStatus(CharacterStatus copyStat)
        {
            likeMeter = copyStat.likeMeter;
            hungerMeter = copyStat.hungerMeter;
            energyMeter = copyStat.energyMeter;
            emotionMeter = copyStat.emotionMeter;
            cleanMeter = copyStat.cleanMeter;

            LikeLevelChange(copyStat.level_like);
            level_hunger = copyStat.level_hunger;
            level_energy = copyStat.level_energy;
            level_emotion = copyStat.level_emotion;
            level_clean = copyStat.level_clean;


            sicknessPercent = copyStat.sicknessPercent;
            injurePercent = copyStat.injurePercent;
            isSick = copyStat.isSick;
            isInjured = copyStat.isInjured;

            isSleep = copyStat.isSleep;
        }

        /// <summary>
        /// 9/6/2024-LYI
        /// 외부에서 호출? or 내부에서 호출
        /// 해당 스테이터스 값 변화
        /// </summary>
        /// <param name="statusType"></param>
        /// <param name="value"></param>
        public virtual void ChangeStatusAdd(StatusType statusType, float value)
        {
            switch (statusType)
            {
                case StatusType.LIKE:
                    if (likeMeter + value >= likeMeterMax)
                    {
                        if (level_like != StatusLevel.VERY_GOOD)
                        {
                            //호감도 레벨 업
                            float remainValue = (likeMeterMax - (likeMeter + value)) * -1;
                            likeMeter = 0;

                            LikeLevelChange(level_like + 1);
                            likeMeter += remainValue;

                            OnLikeLevelUp(level_like, likeMeter);
                        }
                        else
                        {
                            likeMeter = likeMeterMax;
                            charMgr.UI.RefreshUI();
                            if (charMgr.Gesture.isOnHand)
                                GameManager.Instance.lifeMgr.lifeUIMgr.StatUIRefresh();
                        }
                    }
                    else if (likeMeter + value < 0)
                    {
                        if (level_like != StatusLevel.VERY_BAD)
                        {
                            LikeLevelChange(level_like - 1);

                            //호감도 레벨 다운
                            float remainValue = (likeMeter + value) * -1;
                            likeMeter = remainValue;

                            OnLikeLevelDown(level_like, likeMeter);
                        }
                        else
                        {
                            likeMeter = 0;
                            charMgr.UI.RefreshUI();
                            if (charMgr.Gesture.isOnHand)
                                GameManager.Instance.lifeMgr.lifeUIMgr.StatUIRefresh();
                        }
                    }
                    else
                    {
                        likeMeter += value;
                        charMgr.UI.RefreshUI();
                        if (charMgr.Gesture.isOnHand)
                            GameManager.Instance.lifeMgr.lifeUIMgr.StatUIRefresh();
                    }

                    break;
                case StatusType.HUNGER:
                    hungerMeter += value;
                    if (hungerMeter > hungerMeterMax)
                        hungerMeter = hungerMeterMax;
                    if (hungerMeter < 0)
                        hungerMeter = 0;
                    break;
                case StatusType.ENERGY:
                    energyMeter += value;
                    if (energyMeter > energyMeterMax)
                        energyMeter = energyMeterMax;
                    if (energyMeter < 0)
                        energyMeter = 0;
                    break;
                case StatusType.EMOTION:
                    emotionMeter += value;
                    if (emotionMeter > emotionMeterMax)
                        emotionMeter = emotionMeterMax;
                    if (emotionMeter < 0)
                        emotionMeter = 0;
                    break;
                case StatusType.SICK:
                    sicknessPercent += value;
                    if (sicknessPercent > 100f)
                        sicknessPercent = 100f;
                    if (sicknessPercent < 0)
                        sicknessPercent = 0;
                    break;
                case StatusType.INJURE:
                    injurePercent += value;
                    if (injurePercent > 100f)
                        injurePercent = 100f;
                    if (injurePercent < 0)
                        injurePercent = 0;
                    break;
                case StatusType.CLEAN:
                    cleanMeter += value;
                    if (cleanMeter > cleanMeterMax)
                        cleanMeter = cleanMeterMax;
                    if (cleanMeter < 0)
                        cleanMeter = 0;
                    break;
                case StatusType.POOP:
                   poopMeter += value;
                    if (poopMeter > poopMeterMax)
                        poopMeter = poopMeterMax;
                    if (poopMeter < 0)
                        poopMeter = 0;
                    break;
                default:
                    break;
            }

            CheckStatLevel(statusType);
        }
        public virtual void ChangeStatusFixed(StatusType statusType, float value)
        {
            switch (statusType)
            {
                case StatusType.LIKE:
                    likeMeter = value;
                    break;
                case StatusType.HUNGER:
                    hungerMeter = value;
                    break;
                case StatusType.ENERGY:
                    energyMeter = value;
                    break;
                case StatusType.EMOTION:
                    emotionMeter = value;
                    break;
                case StatusType.SICK:
                    sicknessPercent = value;
                    break;
                case StatusType.INJURE:
                    injurePercent = value;
                    break;
                case StatusType.CLEAN:
                    cleanMeter = value;
                    break;
                case StatusType.POOP:
                    poopMeter = value;
                    break;
                default:
                    break;
            }

            CheckStatLevel(statusType);
        }


        #region Like Stat 관련

        /// <summary>
        /// 10/30/2024-LYI
        /// 수치로 인해 호감도 단계가 바뀌면 호출
        /// 색깔 변경, 올라가는거 보여주고
        /// 이벤트? 파티클? 보여주기
        /// </summary>
        /// <param name="level"></param>
        void OnLikeLevelUp(StatusLevel level, float remainValue)
        {
            charMgr.UI.statGaugeUI.GaugeLevelUp(level, remainValue, likeMeterMax);

            if (charMgr.Gesture.isOnHand)
                GameManager.Instance.lifeMgr.lifeUIMgr.StatUILevelUp(level, remainValue, likeMeterMax);
        }
        void OnLikeLevelDown(StatusLevel level, float remainValue)
        {
            charMgr.UI.statGaugeUI.GaugeLevelDown(level, remainValue, likeMeterMax);

            if (charMgr.Gesture.isOnHand)
                GameManager.Instance.lifeMgr.lifeUIMgr.StatUILevelDown(level, remainValue, likeMeterMax);
        }

        void LikeLevelChange(StatusLevel changeLevel)
        {
            level_like = changeLevel;

            switch (changeLevel)
            {
                case StatusLevel.VERY_BAD:
                    likeMeterMax = 30f;
                    break;
                case StatusLevel.BAD:
                    likeMeterMax = 50f;
                    break;
                case StatusLevel.NORMAL:
                    likeMeterMax = 100f;
                    break;
                case StatusLevel.GOOD:
                    likeMeterMax = 150f;
                    break;
                case StatusLevel.VERY_GOOD:
                    likeMeterMax = 200f;
                    break;
            }
        }
        #endregion


        #region Status Level Check
        public void CheckStatLevel(StatusType type)
        {
            switch (type)
            {
                case StatusType.HUNGER:
                    CheckHunger();
                    break;
                case StatusType.ENERGY:
                    CheckEnergy();
                    break;
                case StatusType.EMOTION:
                    CheckEmotion();
                    break;
                case StatusType.SICK:
                    CheckSickness();
                    break;
                case StatusType.INJURE:
                    CheckInjure();
                    break;
                case StatusType.CLEAN:
                    CheckCleanliness();
                    break;
                case StatusType.POOP:
                    CheckPoop();
                    break;
            }
        }

        public void CheckAllStatLevel()
        {
            CheckHunger();
            CheckEnergy();
            CheckEmotion();
            CheckSickness();
            CheckInjure();
            CheckCleanliness();
            CheckPoop();
        }


        /// <summary>
        /// 11/4/2024-LYI
        /// 배고픔 상태 변경
        /// 상태에 따른 눈동자 변경
        /// </summary>
        public virtual void CheckHunger()
        {
            if (hungerMeter < 20)
            {
                level_hunger = StatusLevel.VERY_BAD;
            }
            else if (hungerMeter < 40)
            {
                level_hunger = StatusLevel.BAD;
            }
            else if (hungerMeter < 70)
            {
                level_hunger = StatusLevel.NORMAL;
            }
            else if (hungerMeter < 80)
            {
                level_hunger = StatusLevel.GOOD;
            }
            else if (hungerMeter <= 100)
            {
                level_hunger = StatusLevel.VERY_GOOD;
            }

            Debug.Log(typeHeader.ToString() + "Hunger Level: " + level_hunger.ToString());

            switch (level_hunger)
            {
                case StatusLevel.VERY_BAD:
                    charMgr.Animation.EyeShapeChange(EyeShapeType.DEATH);
                    break;
                case StatusLevel.BAD:
                    charMgr.Animation.EyeShapeChange(EyeShapeType.TORNADO);
                    break;
                case StatusLevel.NORMAL:
                case StatusLevel.GOOD:
                case StatusLevel.VERY_GOOD:
                    charMgr.Animation.EyeShapeReset();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 11/4/2024-LYI
        /// 에너지 상태 변경
        /// 상태에 따른 눈꺼풀 변경
        /// </summary>
        public virtual void CheckEnergy()
        {
            if (energyMeter < 5)
            {
                level_energy= StatusLevel.VERY_BAD;

                charMgr.AI.OnGoingSleep();//잠자러 가기
            }
            else if (energyMeter < 30)
            {
                level_energy = StatusLevel.BAD;
            }
            else if (energyMeter < 50)
            {
                level_energy = StatusLevel.NORMAL;
            }
            else if (energyMeter  < 70)
            {
                level_energy = StatusLevel.GOOD;
            }
            else if (energyMeter <= 100)
            {
                level_energy = StatusLevel.VERY_GOOD;
            }

            Debug.Log(typeHeader.ToString() + "Energy Level: " + level_energy.ToString());

            //잠자면 눈꺼풀 닫기
            if (isSleep)
            {
                charMgr.Animation.EyeLidChange(EyeLidShape.CLOSE);
                return;
            }

            //아닌 경우 상태에 따라 변경
            switch (level_energy)
            {
                case StatusLevel.VERY_BAD:
                case StatusLevel.BAD:
                    charMgr.Animation.EyeLidChange(EyeLidShape.HALF);
                    break;
                case StatusLevel.NORMAL:
                case StatusLevel.GOOD:
                case StatusLevel.VERY_GOOD:
                    charMgr.Animation.EyeLidChange(EyeLidShape.OPEN);
                    break;
                default:
                    charMgr.Animation.EyeLidChange(EyeLidShape.OPEN);
                    break;
            }
        }

        /// <summary>
        /// 11/4/2024-LYI
        /// 감정 상태 변경
        /// 상태에 따른 ??? 가중치? 배율? 변경
        /// </summary>
        public virtual void CheckEmotion()
        {
            if (emotionMeter < 10)
            {
                level_emotion = StatusLevel.VERY_BAD;
            }
            else if (emotionMeter < 30)
            {
                level_emotion = StatusLevel.BAD;
            }
            else if (emotionMeter < 50)
            {
                level_emotion = StatusLevel.NORMAL;
            }
            else if (emotionMeter < 70)
            {
                level_emotion = StatusLevel.GOOD;
            }
            else if (emotionMeter <= 100)
            {
                level_emotion = StatusLevel.VERY_GOOD;
            }

            Debug.Log(typeHeader.ToString() + "Emotion Level: " + level_emotion.ToString());
        }

        /// <summary>
        /// 11/26/2024-LYI
        /// 밥, 똥, 만짐, 잠에서 굴림 체크
        /// </summary>
        public virtual void CheckSickness()
        {
            //0~99. 100각 다이스 굴림
            float sickDice = Random.Range(0, 100);

            if (sickDice > 99 - sicknessPercent)
            {
                isSick = true;
            }

            if (isSick)
            {
                //질병 효과 시작
            }
            else
            {
                //질병효과 제거

            }
        }

        /// <summary>
        /// 11/26/2024-LYI
        /// 이동, 명령, 행동 관련 시 굴림 체크
        /// </summary>
        public virtual void CheckInjure()
        {
            //0~99. 100각 다이스 굴림
            float injureDice = Random.Range(0, 100);

            if (injureDice > 99 - injurePercent)
            {
                isInjured = true;
            }

            if (isInjured)
            {
                //질병 효과 시작
            }
            else
            {
                //질병효과 제거

            }
        }

        /// <summary>
        /// 11/4/2024-LYI
        /// 청결 상태 변경
        /// 상태 변경에 따른 더러움 효과 추가
        /// </summary>
        public virtual void CheckCleanliness()
        {
            if (cleanMeter < 10)
            {
                level_clean = StatusLevel.VERY_BAD;
                ChangeStatusFixed(StatusType.SICK, 10f);
            }
            else if (cleanMeter < 30)
            {
                level_clean = StatusLevel.BAD;
                ChangeStatusFixed(StatusType.SICK, 5f);
            }
            else if (cleanMeter < 50)
            {
                level_clean = StatusLevel.NORMAL;
                ChangeStatusFixed(StatusType.SICK, 1f);
            }
            else if (cleanMeter < 70)
            {
                level_clean = StatusLevel.GOOD;
                ChangeStatusFixed(StatusType.SICK, 0f);
            }
            else if (cleanMeter <= 100)
            {
                level_clean = StatusLevel.VERY_GOOD;
                ChangeStatusFixed(StatusType.SICK, 0f);
            }

            Debug.Log(typeHeader.ToString() + "Clean Level: " + level_clean.ToString());

            switch (level_clean)
            {
                case StatusLevel.VERY_BAD:
                case StatusLevel.BAD:
                    charMgr.Particle.PlayParticleLoop(ParticleLoopType.DIRTY);
                    break;
                case StatusLevel.NORMAL:
                case StatusLevel.GOOD:
                default:
                    charMgr.Particle.StopParticleLoop(ParticleLoopType.DIRTY);
                    break;
                case StatusLevel.VERY_GOOD:
                    //반짝반짝 효과
                    charMgr.Particle.StopParticleLoop(ParticleLoopType.DIRTY);
                    break;
            }
        }


        /// <summary>
        /// 11/26/2024-LYI
        /// 게이지 체크 이후 똥싸기
        /// </summary>
        public virtual void CheckPoop()
        {
            if (poopMeter >= poopMeterMax)
            {
                charMgr.Stop();
                charMgr.AI.AIMove(AIState.POOP);
                OnPoop();
            }
        }
        
        #endregion 


        #region Reaction 리액션이라 하지만 상태에 따른 스탯 변화량 정리

        void TouchReaction(StatusLevel level)
        {
            switch (level)
            {
                case StatusLevel.VERY_BAD:
                    ChangeStatusAdd(StatusType.LIKE, -50);
                    ChangeStatusAdd(StatusType.HUNGER, -10);
                    ChangeStatusAdd(StatusType.ENERGY, -30);
                    ChangeStatusAdd(StatusType.EMOTION, -20);
                    ChangeStatusAdd(StatusType.CLEAN, -10);
                    break;
                case StatusLevel.BAD:
                    ChangeStatusAdd(StatusType.LIKE, -30);
                    ChangeStatusAdd(StatusType.HUNGER, -10);
                    ChangeStatusAdd(StatusType.ENERGY, -20);
                    ChangeStatusAdd(StatusType.EMOTION, -5);
                    ChangeStatusAdd(StatusType.CLEAN, -5);
                    break;
                case StatusLevel.NORMAL:
                    ChangeStatusAdd(StatusType.LIKE, 5);
                    ChangeStatusAdd(StatusType.HUNGER, -5);
                    ChangeStatusAdd(StatusType.ENERGY, -10);
                    ChangeStatusAdd(StatusType.EMOTION, 5);
                    ChangeStatusAdd(StatusType.CLEAN, -3);
                    break;
                case StatusLevel.GOOD:
                    ChangeStatusAdd(StatusType.LIKE, 10);
                    ChangeStatusAdd(StatusType.HUNGER, -10);
                    ChangeStatusAdd(StatusType.ENERGY, -5);
                    ChangeStatusAdd(StatusType.EMOTION, 10);
                    ChangeStatusAdd(StatusType.CLEAN, -3);
                    break;
                case StatusLevel.VERY_GOOD:
                    ChangeStatusAdd(StatusType.LIKE, 15);
                    ChangeStatusAdd(StatusType.HUNGER, -10);
                    ChangeStatusAdd(StatusType.ENERGY, -5);
                    ChangeStatusAdd(StatusType.EMOTION, 10);
                    ChangeStatusAdd(StatusType.CLEAN, -3);
                    break;
            }
        }

        void FoodReaction(StatusLevel level)
        {
            switch (level)
            {
                case StatusLevel.VERY_BAD:
                    ChangeStatusAdd(StatusType.LIKE, -5);
                    ChangeStatusAdd(StatusType.HUNGER, 30);
                   // ChangeStatusAdd(StatusType.ENERGY, 10);
                    ChangeStatusAdd(StatusType.EMOTION, -10);
                    ChangeStatusAdd(StatusType.CLEAN, -20);
                    break;
                case StatusLevel.NORMAL:
                    ChangeStatusAdd(StatusType.LIKE, 10);
                    ChangeStatusAdd(StatusType.HUNGER, 50);
                    //ChangeStatusAdd(StatusType.ENERGY, 20);
                    ChangeStatusAdd(StatusType.EMOTION, 10);
                    ChangeStatusAdd(StatusType.CLEAN, -10);
                    break;
                case StatusLevel.VERY_GOOD:
                    ChangeStatusAdd(StatusType.LIKE, 20);
                    ChangeStatusAdd(StatusType.HUNGER, 70);
                    //ChangeStatusAdd(StatusType.ENERGY, 30);
                    ChangeStatusAdd(StatusType.EMOTION, 30);
                    ChangeStatusAdd(StatusType.CLEAN, -5);
                    break;
            }
            ChangeStatusAdd(StatusType.POOP, 20);
        }

        #endregion

        #region On AI Active.. AI등에서 호출 시 스탯 변화 관련.. 행동 반응 함수들

        /// <summary>
        /// 11/26/2024-LYI
        /// 이동 시 스탯 변화
        /// </summary>
        public void OnMoveWalk()
        {
            ChangeStatusAdd(StatusType.HUNGER, -1);
            ChangeStatusAdd(StatusType.ENERGY, -1);
            // ChangeStatusAdd(StatusType.EMOTION, 30);
            ChangeStatusAdd(StatusType.CLEAN, -1);
            ChangeStatusAdd(StatusType.POOP, 1);
        }
        public void OnMoveRun()
        {
            ChangeStatusAdd(StatusType.HUNGER, -3);
            ChangeStatusAdd(StatusType.ENERGY, -3);
            // ChangeStatusAdd(StatusType.EMOTION, 30);
            ChangeStatusAdd(StatusType.CLEAN, -3);
            ChangeStatusAdd(StatusType.POOP, 3);
        }

        /// <summary>
        /// 9/6/2024-LYI
        /// 손과 터치 상호작용 시 호출
        /// 손 상태, 부위에 따라 반응
        /// </summary>
        /// <param name="handType"></param>
        /// <param name="direction"></param>
        public void OnTouch(TouchCollider_HandType handType, TouchCollider_Direction direction)
        {
            Debug.Log("Touched:" + handType.ToString() + "  -  Direction:" + direction.ToString());
            if (handType == likehandType)
            {
                TouchReaction(StatusLevel.GOOD);
            }
            else if (handType == hatehandType)
            {
                TouchReaction(StatusLevel.BAD);
            }
            else
            {
                switch (handType)
                {
                    case TouchCollider_HandType.NORMAL:
                        if (direction == likeTouchDirection)
                        {
                            TouchReaction(StatusLevel.GOOD);
                        }
                        else if (direction == hateTouchDirection)
                        {
                            TouchReaction(StatusLevel.BAD);
                        }
                        else
                        {
                            TouchReaction(StatusLevel.NORMAL);
                        }
                        break;
                    case TouchCollider_HandType.POINT:
                        if (direction == likeTouchDirection)
                        {
                            TouchReaction(StatusLevel.GOOD);
                        }
                        else if (direction == hateTouchDirection)
                        {
                            TouchReaction(StatusLevel.BAD);
                        }
                        else
                        {
                            TouchReaction(StatusLevel.NORMAL);
                        }
                        break;
                    case TouchCollider_HandType.FIST:
                        if (direction == hateTouchDirection)
                        {
                            TouchReaction(StatusLevel.VERY_BAD);
                        }
                        else
                        {
                            TouchReaction(StatusLevel.BAD);
                        }
                        break;
                    default:
                        break;
                }
            }

        }


        /// <summary>
        /// 11/26/2024-LYI
        /// 스펀지로 닦을 때 스탯 변화
        /// </summary>
        public void OnBath()
        {
            ChangeStatusAdd(StatusType.LIKE, 5);
            ChangeStatusAdd(StatusType.HUNGER, -5);
            ChangeStatusAdd(StatusType.ENERGY, -5);
            ChangeStatusAdd(StatusType.EMOTION, 5);
            ChangeStatusAdd(StatusType.CLEAN, 50);
        }

        public void OnFoodEat(Food food)
        {
            if (food.foodType == likeFood)
            {
                //선호
                FoodReaction(StatusLevel.VERY_GOOD);

            }
            else if (food.foodType == hateFood)
            {
                //불호
                FoodReaction(StatusLevel.VERY_BAD);
            }
            else
            {
                //기타
                FoodReaction(StatusLevel.NORMAL);
            }

        }


        /// <summary>
        /// 11/26/2024-LYI
        /// 똥 쌀 때 스탯 변화
        /// </summary>
        public void OnPoop()
        {
            ChangeStatusAdd(StatusType.EMOTION, 10);
            ChangeStatusAdd(StatusType.CLEAN, -20);
            ChangeStatusFixed(StatusType.POOP, 0);
        }

        /// <summary>
        /// 11/26/2024-LYI
        /// 아파서 싫어한다? 고쳐줘서 좋아한다?
        /// 좋지만 기운이 빠진다?
        /// </summary>
        public void OnSyringe()
        {
            ChangeStatusAdd(StatusType.LIKE, 5);
            ChangeStatusAdd(StatusType.ENERGY, -10);
            ChangeStatusAdd(StatusType.EMOTION, -10);

            //질병 상태 해제
            ChangeStatusFixed(StatusType.SICK, 0);
            isSick = false;
            CheckSickness();
        }

        /// <summary>
        /// 12/2/2024-LYI
        /// 중지 올릴 때 스텟 적용
        /// </summary>
        public void OnMiddleUp()
        {
            ChangeStatusAdd(StatusType.LIKE, -50);
            ChangeStatusAdd(StatusType.EMOTION, -10);


        }


        Coroutine sleepCoroutine;
        /// <summary>
        /// 12/3/2024-LYI
        /// 잠자기 시작, 에너지 회복
        /// </summary>
        public void StartSleep()
        {
            if (isSleep)
            {
                return;
            }

            if (sleepCoroutine !=null)
            {
                StopCoroutine(sleepCoroutine);
            }
            isSleep = true;
            sleepCoroutine = StartCoroutine(SleepGetEnergy());
        }


        public void StopSleep()
        {
            if (!isSleep)
            {
                return;
            }
            if (sleepCoroutine != null)
            {
                StopCoroutine(sleepCoroutine);
            }
            isSleep = false;
        }

        //초당 회복
        IEnumerator SleepGetEnergy()
        {
            while (isSleep && energyMeter < energyMeterMax)
            {
                yield return new WaitForSeconds(1f);
                ChangeStatusAdd(StatusType.ENERGY, 10f);
            }
            charMgr.AI.EndSleep();
        }

        /// <summary>
        /// 12/3/2024-LYI
        /// 약속 시 효과
        /// </summary>
        public void OnPrimise()
        {
            isPromised = true;
            ChangeStatusAdd(StatusType.LIKE, 30f);
            ChangeStatusFixed(StatusType.EMOTION, emotionMeterMax);
        }

        #endregion


    }
}