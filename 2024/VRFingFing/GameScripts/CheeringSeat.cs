using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTokTok.Manager;
using MoreMountains.Feedbacks;
using AroundEffect;

namespace VRTokTok.Character
{
    /// <summary>
    /// 8/24/2023-LYI
    /// 응원석
    /// 선택되지 않은 캐릭터들은 응원석에서 응원을 한다
    /// Idle, Success, Fail에 따라 동작이 바뀐다
    /// 5마리가 같은 타이밍에 동작되지 않도록 동작 전에 약간의 랜덤 딜레이를 부여한다
    /// </summary>
    public class CheeringSeat : MonoBehaviour
    {
        PlaySceneManager playMgr;

        public HeaderSelect headerSelect;

        public Tok_CheeringCharacter[] arr_cheeringCharacter;
        public Tok_CheeringCharacter groundTena;
        public Tok_CheeringCharacter headTena;
        public Transform[] arr_characterPoint;

        public List<Tok_CheeringCharacter> list_activeCheer = new List<Tok_CheeringCharacter>();

        //클리어 시 폭죽 효과
        public List<ParticleSystem> list_fireworks_origin = new();
        public List<GameObject> list_fireworkDisable = new();
        public Transform[] arr_tr_fire;
        public Transform tr_disable;
        public Transform tr_fireLast;

        public ParticleSystem efx_unlock;


        public bool isChearingSeatActive = false;

        private void Awake()
        {
            playMgr = GameManager.Instance.playMgr;
        }

        private void Start()
        {
            CheeringSeatInit();
        }

        public void CheeringSeatInit()
        {
            ActiveCheeringSeat(true);

            ResetCheeringCharacter();

            CheckCharacterLock();
        }


        public void ActiveCheeringSeat(bool isActive)
        {
            isChearingSeatActive = isActive;
            transform.GetChild(0).gameObject.SetActive(isActive);
            transform.GetChild(1).gameObject.SetActive(isActive);
        }


        /// <summary>
        /// 10/30/2023-LYI
        /// 모든 캐릭터 응원석으로 이동
        /// </summary>
        public void ResetCheeringCharacter()
        {
            for (int i = 0; i < arr_cheeringCharacter.Length; i++)
            {
                if (arr_cheeringCharacter[i].gameObject.activeSelf == false)
                {
                    arr_cheeringCharacter[i].gameObject.SetActive(true);
                    arr_cheeringCharacter[i].CharacterAppear();
                }
                else
                {
                    arr_cheeringCharacter[i].gameObject.SetActive(true);
                }
            }

            groundTena.gameObject.SetActive(false);
            if (headTena.gameObject.activeSelf == false)
            {
                headTena.gameObject.SetActive(true);
                if (playMgr.selectCharacterType != HeaderType.OODADA)
                {
                    headTena.CharacterAppear();
                }
            }
            else
            {
                headTena.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Int형 전달 오버로드
        /// </summary>
        /// <param name="headerNum"></param>
        public void SetCheeringCharacter(int[] headerNum)
        {
            HeaderType[] type = new HeaderType[headerNum.Length];

            for (int i = 0; i < type.Length; i++)
            {
                type[i] = (HeaderType)headerNum[i];
            }

            SetCheeringCharacter(type);
        }

        /// <summary>
        /// 8/30/2023-LYI
        /// 스테이지에서 사용될 캐릭터가 결정되었을 때 호출
        /// 전체 응원단에서 스테이지에서 사용된 캐릭터를 제외한 상태로 대기
        /// </summary>
        /// <param name="activeHeaders"></param>
        public void SetCheeringCharacter(HeaderType[] activeHeaders)
        {
            if (!GameManager.Instance.playMgr.cheeringSeat.isChearingSeatActive)
            {
                return;
            }
            groundTena.gameObject.SetActive(false);
            headTena.gameObject.SetActive(true);

            for (int i = 0; i < arr_cheeringCharacter.Length; i++)
            {
                for (int header = 0; header < activeHeaders.Length; header++)
                {
                    if (activeHeaders[header] == HeaderType.TENA)
                    {
                        headTena.gameObject.SetActive(false);
                        //headTena.CharacterDisappear();
                    }
                    if (activeHeaders[header] == HeaderType.OODADA)
                    {
                        headTena.gameObject.SetActive(false);
                        // headTena.CharacterDisappear();
                        groundTena.gameObject.SetActive(true);
                        //groundTena.CharacterAppear();
                    }

                    if (arr_cheeringCharacter[i].typeHeader == activeHeaders[header])
                    {
                        if (arr_cheeringCharacter[i].gameObject.activeSelf)
                        {
                            arr_cheeringCharacter[i].CharacterDisappear();
                        }

                        continue;
                    }
                    arr_cheeringCharacter[i].gameObject.SetActive(true);
                }
            }

            list_activeCheer.Clear();
            //Check active cheering characters
            for (int i = 0; i < arr_cheeringCharacter.Length; i++)
            {
                if (arr_cheeringCharacter[i].gameObject.activeSelf)
                {
                    list_activeCheer.Add(arr_cheeringCharacter[i]);
                }
            }
            if (playMgr.selectCharacterType == HeaderType.OODADA)
            {
                list_activeCheer.Add(groundTena);
            }
            if (playMgr.selectCharacterType != HeaderType.OODADA &&
                playMgr.selectCharacterType != HeaderType.TENA)
            {
                list_activeCheer.Add(headTena);
            }

        }


        /// <summary>
        /// 7/15/2024-LYI
        /// 캐릭터 잠김 상태 확인
        /// </summary>
        public void CheckCharacterLock()
        {
            Debug.Log("CheeringSeat: CheckCharacterLock()");
            int checkStage = 9000 + headTena.lockStageNum;
            bool isUnlock = ES3.Load<bool>(checkStage.ToString(), false);

            headTena.SetLock(!isUnlock);
            groundTena.SetLock(!isUnlock);

            for (int i = 0; i < arr_cheeringCharacter.Length; i++)
            {
                checkStage = 9000 + arr_cheeringCharacter[i].lockStageNum;
                isUnlock = ES3.Load<bool>(checkStage.ToString(), false);

                //칸토 제외
                if (arr_cheeringCharacter[i].lockStageNum == 0)
                {
                    isUnlock = true;
                }

                arr_cheeringCharacter[i].SetLock(!isUnlock);
            }
        }


        /// <summary>
        /// 7/17/2024-LYI
        /// 캐릭터 언락 여부 체크
        /// </summary>
        /// <param name="stageNum"></param>
        /// <returns></returns>
        public bool IsCharacterUnlock(int stageNum)
        {
            int mixednum;
            DataManager dataMgr = GameManager.Instance.dataMgr;
            if (dataMgr.dic_StageToMixed.ContainsKey(stageNum))
            {
                mixednum = GameManager.Instance.dataMgr.dic_StageToMixed[stageNum];
            }
            else
            {
                return false;
            }

            if (mixednum == 9000 + groundTena.lockStageNum)
            {
                unlockHeader = headTena;
                return true;
            }

            for (int i = 0; i < arr_cheeringCharacter.Length; i++)
            {
                if (mixednum == 9000 + arr_cheeringCharacter[i].lockStageNum)
                {
                    unlockHeader = arr_cheeringCharacter[i];
                    return true;
                }
            }

            unlockHeader = null;
            return false;
        }

        Tok_CheeringCharacter unlockHeader;
        /// <summary>
        /// 7/17/2024-LYI
        /// 캐릭터 언락 진행
        /// </summary>
        public void OnCharacterUnlock()
        {
            if (unlockHeader == null)
            {
                return;
            }

            StartCoroutine(UnlockSequence());
        }


        /// <summary>
        /// 7/16/2024-LYI
        /// 해금 시 효과
        /// </summary>
        /// <returns></returns>
        IEnumerator UnlockSequence()
        {
           // headerSelect.objConfirm.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
            efx_unlock.transform.position = unlockHeader.transform.position + Vector3.up * 0.04f;
            efx_unlock.Play();
            yield return new WaitForSeconds(1.5f);
            //잠금 효과 해제
            CheckCharacterLock();
            //해당 캐릭 점프 애니
            unlockHeader.PlayCheeringAnim(2);
            efx_unlock.Stop();
            //해당 캐릭터에 폭죽 효과
            PlayFirework(unlockHeader.transform);

            yield return new WaitForSeconds(2f);

            StartCoroutine(PlayFirework(5));

            //headerSelect.objConfirm.gameObject.SetActive(true);
        }

        


        /// <summary>
        /// 7/17/2024-LYI
        /// 랜덤 색상 폭죽 발사
        /// </summary>
        /// <param name="tr"></param>
        void PlayFirework(Transform tr)
        {
           int random = Random.Range(0, list_fireworks_origin.Count);

            GameObject go = GameManager.Instance.objPoolingMgr.CreateObject(list_fireworkDisable, list_fireworks_origin[random].gameObject, tr.position, tr);
            
            go.transform.localScale = Vector3.one * 0.1f;
            go.GetComponent<ParticleSystem>().Play();

            GameManager.Instance.soundMgr.PlaySfxRandomPitch(tr.position, Constants.Sound.SFX_CHEER_FIREWORK);

            StartCoroutine(GameManager.Instance.LateFunc(() =>
            GameManager.Instance.objPoolingMgr.ObjectInit(list_fireworkDisable, go, tr_disable),
            2f));
        }

        /// <summary>
        /// 7/18/2024-LYI
        /// 폭죽 터트리는 효과
        /// </summary>
        /// <param name="fireCount"></param>
        /// <returns></returns>
        IEnumerator PlayFirework(int fireCount)
        {
            Queue<int> que_randomNum = new Queue<int>(ShuffleList(fireCount));


            for (int i = 0; i < fireCount; i++)
            {
                int random = que_randomNum.Dequeue();
                PlayFirework(arr_tr_fire[random]);

                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(0.5f);
            PlayFirework(tr_fireLast);

        }

        List<int> ShuffleList(int Count)
        {
            List<int> dice = new List<int>();

            for (int i = 0; i < Count; i++)
            {
                dice.Add(i);
            }

            //Fisher-Yates 알고리즘
            //역순 for문
            for (int i = dice.Count -1; i >0; i--)
            {
                //최대 값 랜덤으로 시작
                int randomIndex = Random.Range(0, i + 1);
                int temp = dice[i]; //리스트의 현재 값 일시 저장
                dice[i] = dice[randomIndex]; //리스트 마지막에 랜덤값 삽입
                dice[randomIndex] = temp; //랜덤 위치에 마지막 값 삽입(지금 랜덤으로 된 값은 빠짐)
            }

            return dice;
        }



        /// <summary>
        /// 7/16/2024-LYI
        /// 스테이지 시작 시 호출
        /// </summary>
        public void OnStart()
        {
            for (int i = 0; i < list_activeCheer.Count; i++)
            {
                list_activeCheer[i].PlayCheeringAnim(0);
            }
        }

        /// <summary>
        /// 7/16/2024-LYI
        /// 스테이지 실패 시 호출
        /// </summary>
        public void OnFail()
        {
            for (int i = 0; i < list_activeCheer.Count; i++)
            {
                list_activeCheer[i].PlayCheeringAnim(1);
            }
        }

        /// <summary>
        /// 7/16/2024-LYI
        /// 클리어 시 호출
        /// </summary>
        public void OnClear()
        {
            CheckCharacterLock();

            StartCoroutine(PlayFirework(3));

            for (int i = 0; i < list_activeCheer.Count; i++)
            {
                list_activeCheer[i].PlayCheeringAnim(2);
            }
        }

    }
}