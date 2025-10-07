using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTokTok.Manager;
using VRTokTok.Character;

namespace VRTokTok.Interaction.Shooting
{

    /// <summary>
    /// 10/11/2023-LYI
    /// Ç³¼± Áß·Â Á¦¾î ÇÔ¼ö
    /// </summary>
    public class Tok_FloatingBalloon : Tok_Interact
    {
        PlaySceneManager playMgr;

        public List<Balloon> list_balloon = new List<Balloon>();
        public Transform tr_hanger;
        public Vector3[] arr_balloonStartPos;
        public Vector3 boxStartPos;

        public GameObject[] arr_hangingHeaders;
        public GameObject[] arr_moveHeaders;
        public GameObject hangingHeaderPool;
        public GameObject moveHeaderPool;
        public GameObject hangingHeader = null;
        public Tok_Movement moveHeader = null;

        public bool isActing = false;

        public bool isUp = true;
        public float moveGap = 1f;
        public float changeTime = 2f;
        public float changeSmooth = 1f;
        float t = 0;

        bool isFirst = true;


        public override void InteractInit()
        {
            if (isFirst)
            {
                playMgr = GameManager.Instance.playMgr;

                arr_balloonStartPos = new Vector3[list_balloon.Count];
                for (int i = 0; i < arr_balloonStartPos.Length; i++)
                {
                    arr_balloonStartPos[i] = list_balloon[i].transform.position;
                }

                boxStartPos = tr_hanger.position;

                isFirst = false;
            }

            base.InteractInit();

            if (hangingHeaderPool != null)
            {
                hangingHeaderPool.SetActive(true);
                if (playMgr.selectCharacterType == HeaderType.KANTO)
                {
                    ActiveCharacter(arr_hangingHeaders, HeaderType.ZINO);
                    hangingHeader = arr_hangingHeaders[(int)HeaderType.ZINO - 1];
                }
                else
                {
                    ActiveCharacter(arr_hangingHeaders, HeaderType.KANTO);
                    hangingHeader = arr_hangingHeaders[(int)HeaderType.KANTO - 1];
                }

                if (moveHeader != null)
                {
                    moveHeader.transform.SetParent(moveHeaderPool.transform);
                }
                moveHeaderPool.transform.SetParent(tr_hanger);
                moveHeaderPool.SetActive(false);

                for (int i = 0; i < arr_balloonStartPos.Length; i++)
                {
                    list_balloon[i].BalloonInit();
                    list_balloon[i].transform.position = arr_balloonStartPos[i];
                }
            }
            
            tr_hanger.GetComponent<Rigidbody>().velocity = Vector3.zero;
            tr_hanger.position = boxStartPos;
            isInteractable = true;
        }


        void ActiveCharacter(GameObject[] arr_go, HeaderType type)
        {
            for (int i = 0; i < arr_go.Length; i++)
            {
                arr_go[i].gameObject.SetActive(false);
            }

            arr_go[(int)type - 1].SetActive(true);
        }


        void OnBalloonPop()
        {
            if (hangingHeaderPool != null)
            {
                if (!hangingHeader.activeSelf)
                {
                    return;
                }

                hangingHeaderPool.SetActive(false);
                if (playMgr.selectCharacterType == HeaderType.KANTO)
                {
                    ActiveCharacter(arr_moveHeaders, HeaderType.ZINO);
                    moveHeader = arr_moveHeaders[(int)HeaderType.ZINO - 1].GetComponent<Tok_Movement>();
                }
                else
                {
                    ActiveCharacter(arr_moveHeaders, HeaderType.KANTO);
                    moveHeader = arr_moveHeaders[(int)HeaderType.KANTO - 1].GetComponent<Tok_Movement>();
                }
                moveHeaderPool.SetActive(true);
                if (moveHeader != null)
                {
                    moveHeader.transform.parent = null;
                    moveHeader.Init();
                    playMgr.list_activeCharacter.Add(moveHeader);
                }
            }

            isInteractable = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (isInteractable)
            {
                if (list_balloon.Count > 0)
                {
                    int activeCount = 0;
                    for (int i = 0; i < list_balloon.Count; i++)
                    {
                        if (list_balloon[i].gameObject.activeSelf)
                        {
                            activeCount++;
                            isActing = true;
                        }
                    }

                    if (!isActing)
                    {
                        return;
                    }

                    //¸ðµç Ç³¼± ÅÍÁü
                    if (activeCount == 0)
                    {
                        OnBalloonPop();
                    }


                    t += Time.deltaTime;

                    if (t > changeTime)
                    {
                        isUp = !isUp;
                        t = 0;
                    }

                    for (int i = 0; i < list_balloon.Count; i++)
                    {
                        list_balloon[i].floatingPower = -1 * Physics.gravity.y / activeCount;

                        if (isUp)
                        {
                            list_balloon[i].movePower = Mathf.Lerp(list_balloon[i].movePower, moveGap, changeSmooth * Time.deltaTime);
                        }
                        else
                        {
                            list_balloon[i].movePower = Mathf.Lerp(list_balloon[i].movePower, -moveGap, changeSmooth * Time.deltaTime);
                        }
                    }
                }
            }
        }


    }
}
