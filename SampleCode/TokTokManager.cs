using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Oculus.Interaction;

using VRTokTok.Character;
using VRTokTok.Interaction;

namespace VRTokTok.Manager
{

    /// <summary>
    /// 바닥 두드리는 동작 관련 구현
    /// 손과 상호작용 관련된 요소 구현
    /// 캐릭터 관련 상호작용 구현
    /// 네비게이션 관련 요소 구현
    /// Interactable 참조
    /// 
    /// 일반적인 게임의 조작부분(컨트롤러)
    /// 
    /// Don'tDestroyOnLoad
    /// 스테이지 시작 시 해당 스테이지의 땅과 캐릭터 정보를 바로 적용
    /// </summary>
    public class TokTokManager : MonoBehaviour
    {
        GameManager gameMgr;
        PlaySceneManager playMgr;

        [Header("Input")]
        public OVRHand[] arr_hand = new OVRHand[2];
        public PokeInteractor[] arr_pokeHand = new PokeInteractor[2];
        public PokeInteractor[] arr_pokeController = new PokeInteractor[2];
        public Transform[] arr_indexTipSyntheticHand;
        public Transform[] arr_indexTipSyntheticController;

        public TokGround[] tokGround; //바닥 이벤트 체크, 스테이지마다 변경됨


        [Header("Marker")]
        public TokMarker tokMarker; //선택된 바닥 포인트 표시
        public TokMarker selectMarker; //선택된 캐릭터 포인트 표시


        [Header("Particle")]
        public FingerFollower fingerFollower;

        public GameObject particleOrigin_tok;
        public Transform tr_particleEnable;
        public Transform tr_particleDisable;

        List<GameObject> list_particleDisable = new List<GameObject>();


        [Header("Property")]
        public Transform currentIndex;
        public Tok_Movement selectCharacter; //선택된 캐릭터
        public TokGround lastTokGround;

        public bool isHandTracking;
        public bool isLeftHanded = false; //주 손 변경 옵션

        public float doubleTime = 2f; //더블클릭 체크 시간
        public bool isDouble = false; //더블클릭 여부 체크

        Coroutine currentCoroutine = null;

        private void Awake()
        {
            gameMgr = GameManager.Instance;
            playMgr = GameManager.Instance.playMgr;

        }

        public void HandInit()
        {
            isLeftHanded = ES3.Load(Constants.ES3.IS_LEFT_HANDED, false);
            ChangeMainHand(isLeftHanded);

            StartCoroutine(CheckControllerStatus());
        }

        public void TokGroundInit()
        {
            tokGround = playMgr.currentStage.GetComponentsInChildren<TokGround>();
            //GameObject.FindObjectsByType<TokGround>(FindObjectsSortMode.InstanceID);

            GroundsInit(tokGround);
        }

        /// <summary>
        /// 매 스테이지마다 그라운드 할당 시 호출
        /// 그라운드 변경
        /// </summary>
        /// <param name="ground"></param>
        public void GroundInit(TokGround ground)
        {
            ground.OnGroundTok = OnGroundTok;
            ground.OnGroundRelease = OnGroundRelease;
            ground.OnGroundStay = OnGroundStay;
            ground.Init();
        }
        public void GroundsInit(TokGround[] ground)
        {
            tokGround = ground;

            for (int i = 0; i < ground.Length; i++)
            {
                ground[i].OnGroundTok = OnGroundTok;
                ground[i].OnGroundRelease = OnGroundRelease;
                ground[i].OnGroundStay = OnGroundStay;
                ground[i].Init();
            }

        }

        /// <summary>
        /// 7/3/2024-LYI
        /// 손 동작 체크로 활성화 변경
        /// </summary>
        private void Update()
        {
            if (isHandTracking)
            {
                CheckHandGesture();
            }
            else
            {
                CheckControllerGesture();
            }
        }

        public void CheckHandGesture()
        {
            if (isLeftHanded)
            {
                if (IsIndexGesture(arr_hand[0]))
                {
                    arr_pokeHand[0].gameObject.SetActive(true);
                    fingerFollower.gameObject.SetActive(true);
                }
                else
                {
                    arr_pokeHand[0].gameObject.SetActive(false);
                    fingerFollower.gameObject.SetActive(false);
                }
            }
            else
            {
                if (IsIndexGesture(arr_hand[1]))
                {
                    arr_pokeHand[1].gameObject.SetActive(true);
                    fingerFollower.gameObject.SetActive(true);
                }
                else
                {
                    arr_pokeHand[1].gameObject.SetActive(false);
                    fingerFollower.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 7/5/2024-LYI
        /// 컨트롤러 버튼 체크
        /// </summary>
        public void CheckControllerGesture()
        {
            if (isLeftHanded)
            {
                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
                {
                    arr_pokeController[0].gameObject.SetActive(false);
                    fingerFollower.gameObject.SetActive(false);
                }
                else
                {
                    arr_pokeController[0].gameObject.SetActive(true);
                    fingerFollower.gameObject.SetActive(true);
                }
            }
            else
            {
                if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
                {
                    arr_pokeController[1].gameObject.SetActive(false);
                    fingerFollower.gameObject.SetActive(false);
                }
                else
                {
                    arr_pokeController[1].gameObject.SetActive(true);
                    fingerFollower.gameObject.SetActive(true);
                }
            }
        }



        bool IsIndexGesture(OVRHand hand)
        {
            if (hand == null) return false;

            //if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
            //{
            //    return true;
            //}
            //if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            //{
            //    return true;
            //}

            // 손이 추적 중인지 확인
            if (hand.IsTracked)
            {
                // 검지 손가락이 펴져 있는지 확인
              //  bool isIndexPinching = hand.GetFingerIsPinching(OVRHand.HandFinger.Index);

                // 나머지 손가락이 펴져 있지 않은지 확인
                //bool isThumbPinching = hand.GetFingerIsPinching(OVRHand.HandFinger.Thumb);
                //bool isMiddlePinching = hand.GetFingerIsPinching(OVRHand.HandFinger.Middle);
                //bool isRingPinching = hand.GetFingerIsPinching(OVRHand.HandFinger.Ring);
                //bool isPinkyPinching = hand.GetFingerIsPinching(OVRHand.HandFinger.Pinky);
                OVRSkeleton skeleton = hand.GetComponent<OVRSkeleton>();
               float fingerDistance= Vector3.Distance(skeleton.Bones[6].Transform.position, 
                   skeleton.Bones[20].Transform.position);
                if (fingerDistance > 0.07f)
                {
                    //Debug.Log($"{hand.name} is performing the index-only gesture.");
                    // 검지 손가락만 펴고 있는 동작을 감지했을 때의 처리
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }




        #region OnGround Events
        /// <summary>
        /// 7/17/2023-LYI
        /// 동작 내용 각 TokGround로 이동
        /// </summary>
        public void OnGroundTok()
        {

        }


        /// <summary>
        /// 바닥애 손 대고 있을 때 동작
        /// </summary>
        public void OnGroundStay()
        {

        }

        /// <summary>
        /// 바닥에서 손이 떨어졌을 때 동작
        /// </summary>
        public void OnGroundRelease()
        {

        }
        #endregion


        /// <summary>
        /// 10/16/2023-LYI
        /// 캐릭터 선택 시 동작
        /// 선택된 캐릭터 변경 및 해당 캐릭터 머리 위에 선택 표시 호출
        /// </summary>
        /// <param name="character"></param>
        public void SelectCharacter(Tok_Movement character)
        {
            selectCharacter = character;
            //selectCharacter.OnSelect();


            selectMarker.gameObject.SetActive(true);
            selectMarker.transform.SetParent(selectCharacter.transform);
            selectMarker.transform.localPosition = Vector3.up * 0.1f;
           // selectMarker.OnTok();
        }



        /// <summary>
        /// Tok Marker 활성화
        /// </summary>
        /// <param name="pos"></param>
        public void MarkerActive(Vector3 pos)
        {
            tokMarker.transform.position = pos;

            tokMarker.gameObject.SetActive(true);
            tokMarker.OnTok();

            if (tokMarker.isColled == false)
            {
                tokMarker.clickedInteraction = null;
            }
            if (tokMarker.clickedInteraction == null)
            {
                tokMarker.clickedInteraction = tokMarker.GetClosestInteract();
            }


            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }

            currentCoroutine = StartCoroutine(TokTimer());
        }


     
        /// <summary>
        /// Tok Marker 비활성화
        /// 
        /// 10/16/2023-LYI
        /// 비활성화 시 위치 초기화 추가, Parent구조로 바뀌어 Vector3 lastTokPos 변수 제거
        /// </summary>
        public void MarkerDisable()
        {
            tokMarker.OffTok();
            tokMarker.gameObject.SetActive(false);
        }

        /// <summary>
        /// 손이 떨어졌을 때 터치 해제 타이머 시작
        /// </summary>
        /// <returns></returns>
        IEnumerator TokTimer()
        {
            yield return new WaitForSeconds(doubleTime);
            MarkerDisable();
            isDouble = false;
        }


        /// <summary>
        /// 8/22/2023-LYI
        /// Tok 입력 관련 처리
        /// Tok 위치, Tok된 오브젝트 정보 전달
        /// </summary>
        /// <param name="pos"></param>
        public void Tok(Vector3 pos, GameObject go, TokGround tokGround = null)
        {
            if (gameMgr.playMgr.statPlay != PlayStatus.PLAY)
            {
                return;
            }
            //판정 순서 변경 톡톡 -> 톡
            tokMarker.transform.SetParent(go.transform);
            MarkerActive(pos);
            // isDouble = true;

            float pitch = 0.2f;
            float randomPitch = Random.Range(1 - pitch, 1 + pitch);
            int random = Random.Range(0, 2);
            if (random == 0)
                gameMgr.soundMgr.PlaySfx(pos, Constants.Sound.SFX_PLAY_TOK, randomPitch);
            else
                gameMgr.soundMgr.PlaySfx(pos, Constants.Sound.SFX_PLAY_TOK_2, randomPitch);


            lastTokGround = tokGround;

            //3/6/2024-LYI
            //톡 판정 고정
            TokTok(pos, go);

            #region oldCode
            //클릭 시 근처에 이미 클릭한  것이 가까이 있으면 현재 선택된 대가리들에게 이동명령
            //float tokDistance = Vector3.Distance(tokMarker.transform.position, pos);
            //float d = Mathf.Infinity;

            //Tok_Movement closestCharacter = null;

            ////활성화 캐릭터 거리체크, 충분히 가까우면 선택 변경 입력
            ////일정 거리 캐릭터가 둘 이상이면 가까운 것 선택? (탑쌓기 등 상황)
            //for (int i = 0; i < playMgr.list_activeCharacter.Count; i++)
            //{
            //    float distance = Vector3.Distance(pos, playMgr.list_activeCharacter[i].transform.position);

            //    if (distance < tokMarker.selectDistance)
            //    {
            //        if (distance < d)
            //        {
            //            d = distance;
            //            closestCharacter = playMgr.list_activeCharacter[i];
            //        }
            //    }
            //}


            ////캐릭터 선택 판정인 경우
            //if (closestCharacter != null)
            //{
            //    if (selectCharacter != null &
            //        selectCharacter.m_character.typeHeader == closestCharacter.m_character.typeHeader &&
            //        isDouble)
            //    {
            //        //이미 선택된 캐릭터를 선택한 경우
            //        selectCharacter.OnDoubleSelect();
            //        isDouble = false;
            //    }
            //    else
            //    {
            //        SelectCharacter(closestCharacter);
            //        isDouble = true;
            //    }
            //}
            //else
            //{
            //    //캐릭터 선택 판정이 아닌 경우
            //    if (tokDistance < 0.1f &&
            //        isDouble)
            //    {
            //        // Debug.Log("Tok Distance: " + tokDistance);
            //        TokTok(pos, go);
            //        isDouble = false;
            //    }
            //    else
            //    {
            //        selectCharacter.OnTok(pos, go);
            //        isDouble = true;
            //    }

            //    //바닥 클릭 시 네비게이션에서 현재 위치 저장
            //    //바닥 타일에 마커 남기기
            //    //tokMarker.transform.SetParent(go.transform);
            //    //MarkerActive(pos);
            //}
            #endregion
        }

        /// <summary>
        /// 8/22/2023-LYI
        /// 톡톡 입력 실행
        /// </summary>
        /// <param name="pos"></param>
        public void TokTok(Vector3 pos, GameObject go)
        {
            //currentHeader.Move(pos);
            if (selectCharacter == null)
            {
                return;
            }
            selectCharacter.OnTokTok(pos, go);

            playMgr.currentStage.TokCountPlus();
        }


        /// <summary>
        /// 10/23/2023-LYI
        /// 입력 시 사용될 주 손 변경
        /// </summary>
        /// <param name="isLeft">왼손잡이 인가?</param>
        public void ChangeMainHand(bool isLeft)
        {
            isLeftHanded = isLeft;
            ES3.Save(Constants.ES3.IS_LEFT_HANDED, isLeftHanded);

            arr_pokeHand[0].gameObject.SetActive(false);
            arr_pokeHand[1].gameObject.SetActive(false);
            arr_pokeController[0].gameObject.SetActive(false);
            arr_pokeController[1].gameObject.SetActive(false);


            int handNum = isLeftHanded ? 0 : 1;
            arr_pokeHand[handNum].gameObject.SetActive(true);
            arr_pokeController[handNum].gameObject.SetActive(true);

            CurrentPokeCheck();

            //if (arr_hand[handNum].GetComponent<OVRSkeleton>().Bones.Count <1)
            //{
            //    StartCoroutine(WaitforBone());
            //}
            //else
            //{
            //    CheckFingerFollow();
            //}
        }

        public void CheckFingerFollow()
        {
            fingerFollower.gameObject.SetActive(true);
            fingerFollower.tr_followTarget = currentIndex;
            fingerFollower.PlayParticle();

            //int handNum = isLeftHanded ? 0 : 1;
            //if (gameMgr.isTutorial)
            //{
            //    fingerFollower.gameObject.SetActive(true);
            //   // fingerFollower.tr_followTarget = arr_hand[handNum].GetComponent<OVRSkeleton>().Bones[20].Transform;
            //    fingerFollower.tr_followTarget = currentIndex;
            //    fingerFollower.PlayParticle();
            //}
            //else
            //{
            //    fingerFollower.gameObject.SetActive(false);
            //    fingerFollower.tr_followTarget = null;
            //    fingerFollower.StopParticle();
            //}
        }

        IEnumerator WaitforBone()
        {
            int handNum = isLeftHanded ? 0 : 1;
            OVRSkeleton skeleton = arr_hand[handNum].GetComponent<OVRSkeleton>();

            while (skeleton.Bones.Count < 1)
            {
                yield return null;
            }
            CheckFingerFollow();
        }

        /// <summary>
        /// 6/25/2024-LYI
        /// 현재 인터렉션 중인 손가락 체크
        /// </summary>
        public void CurrentPokeCheck()
        {
            int handNum = isLeftHanded ? 0 : 1;

            if (isHandTracking)
            {
                currentIndex = arr_indexTipSyntheticHand[handNum];
            }
            else
            {
                currentIndex = arr_indexTipSyntheticController[handNum];
            }
            CheckFingerFollow();
        }

        private IEnumerator CheckControllerStatus()
        {
            OVRInput.Controller controller;
            while (true)
            {
                controller = OVRInput.GetConnectedControllers();

                if (controller != OVRInput.Controller.None)
                {
                    if (controller == OVRInput.Controller.Hands ||
                        controller == OVRInput.Controller.LHand ||
                        controller == OVRInput.Controller.RHand)
                    {
                        if (!isHandTracking)
                        {
                            isHandTracking = true;
                            CurrentPokeCheck();
                        }
                    }
                    else
                    {                        
                        if (isHandTracking)
                        {
                            isHandTracking = false;
                            CurrentPokeCheck();
                        }
                    }
                }

                // 1초마다 상태를 확인
                yield return new WaitForSeconds(1f);
            }
        }

        /// <summary>
        /// 3/6/2024-LYI
        /// ObjectPoolling
        /// TokMarker에서 호출
        /// 터치 시 파티클 호출
        /// </summary>
        public void PlayTokParticle(Vector3 position)
        {
            GameObject go = gameMgr.objPoolingMgr.CreateObject(list_particleDisable, particleOrigin_tok, position, tr_particleEnable);
            ParticleSystem p = go.GetComponentInParent<ParticleSystem>();

            PlayParticle(p);

            StartCoroutine(ResetTokParticle(p));
        }

        IEnumerator ResetTokParticle(ParticleSystem p)
        {
            yield return new WaitForSeconds(p.main.duration+1f); //유예시간 1초 추가

            gameMgr.objPoolingMgr.ObjectInit(list_particleDisable, p.gameObject, tr_particleDisable);
        }

        public void PlayParticle(ParticleSystem p)
        {
            ParticleSystem[] arr_p = p.GetComponentsInChildren<ParticleSystem>();

            for (int i = 0; i < arr_p.Length; i++)
            {
                arr_p[i].Play();
            }
        }
    }
}