using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 손으로 빛의 길을 그려서 칸토와 지노를 만나게 해준다.
/// 칸토에 손이 닿으면 시작
/// 특정 영역에서 벗어나지 않고 이동하면 일정 거리 마다 포인트 등록
/// 지노에 도달하면 끝
/// 전체 포인트 중 포인트 하나 선택, 두 캐릭터가 순서대로 네비게이션 이동
/// 도착 포인트 몇 전에 점프
/// 점프해서 부딪힌 후 타임라인 진행
/// -4.32,0.87
/// 4.65,-2.92
/// </summary>
public class DrawLightInteraction : InteractionManager
{
    List<Vector3> list_movePoint = new List<Vector3>();

    public Transform[] arr_fixedPos;

    public DrawAreaColl[] arr_areaColl;

    public LineRenderer drawingLine;

    public AudioClip sfx_light;

    bool kantoEnd = false;
    bool zinoEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < arr_areaColl.Length; i++)
        {
            arr_areaColl[i].drawMgr = this;
            arr_areaColl[i].collNum = i;
            arr_areaColl[i].gameObject.SetActive(false);
        }
        list_guidePosition.Add(arr_areaColl[0].transform.position + Vector3.up * 2 * gameMgr.uiMgr.stageSize);
        list_guidePosition.Add(arr_areaColl[1].transform.position + Vector3.up * 2 * gameMgr.uiMgr.stageSize);
    }

    // 길 그리기
    public IEnumerator DrawLight(Vector3 _start)
    {
        Debug.Log("Start Drawing");

        list_guideParticle[0].Stop();
        list_guideParticle[1].Play();

        //선 위치 초기화
        Vector3 currentPos = arr_areaColl[0].transform.position;
        Vector3 handPos = gameMgr.handCtrl.manoHandMove.finger_index.transform.position;


        //라인 렌더러 활성화, 위치 설정
        drawingLine.gameObject.SetActive(true);
        drawingLine.SetPosition(0,
            new Vector3(arr_areaColl[0].transform.position.x,
          stageMgr.transform.position.y + 2 * gameMgr.uiMgr.stageSize,
           arr_areaColl[0].transform.position.z));


        gameMgr.handCtrl.manoHandMove.arr_handFollwer[1].ToggleHandEffect(true);

        //Update
        while (arr_areaColl[2].isColl
            && !arr_areaColl[1].isColl)
        {
            handPos = gameMgr.handCtrl.manoHandMove.finger_index.transform.position;

            //그림 그리기, 선 생성
            //거리 비교 시 Y값 배제, 통일해서 계산
            if (Vector3.Distance(
                new Vector3(handPos.x,
                2,
               handPos.z),
                new Vector3(currentPos.x, 2, currentPos.z)) > 0.2f)
            {
                currentPos = handPos;
                //리스트에 지면 기준 X,Z 위치 저장
                list_movePoint.Add(new Vector3(currentPos.x, stageMgr.transform.position.y, currentPos.z));

                //LineRenderer의 Setposition은 LocalPosition이므로 높이 2로 설정
                drawingLine.SetPosition(drawingLine.positionCount - 1, new Vector3(currentPos.x, stageMgr.transform.position.y + 2 * gameMgr.uiMgr.stageSize, currentPos.z));

                if (list_guideParticle.Count < drawingLine.positionCount + 2)
                {
                    ParticleSystem _particle;
                    _particle = Instantiate(gameMgr.b_stagePrefab.LoadAsset<GameObject>("InteractionEffect")).GetComponent<ParticleSystem>();

                    _particle.transform.parent = episodeMgr.particlePool;
                    _particle.transform.localScale *= gameMgr.uiMgr.stageSize;
                    _particle.transform.position = new Vector3(currentPos.x, stageMgr.transform.position.y + 2 * gameMgr.uiMgr.stageSize, currentPos.z);

                    _particle.Play();
                    list_guideParticle.Add(_particle);
                }
                else
                {
                    list_guideParticle[drawingLine.positionCount +1].transform.position = new Vector3(currentPos.x, stageMgr.transform.position.y + 2 * gameMgr.uiMgr.stageSize, currentPos.z);
                    list_guideParticle[drawingLine.positionCount + 1].Play();
                }

                drawingLine.positionCount++;
            }

            drawingLine.SetPosition(drawingLine.positionCount - 1,
                new Vector3(handPos.x,
                stageMgr.transform.position.y + 2 * gameMgr.uiMgr.stageSize,
                handPos.z));

            yield return new WaitForSeconds(0.01f);
        }

        if (arr_areaColl[1].isColl)
        {
            //그림 그리기 끝
            gameMgr.handCtrl.manoHandMove.arr_handFollwer[1].ToggleHandEffect(false);

            gameMgr.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_SUCCESS);

            list_guidePosition = list_movePoint;
            drawingLine.SetPosition(drawingLine.positionCount - 1, new Vector3(arr_areaColl[1].transform.localPosition.x, stageMgr.transform.position.y + 2 * gameMgr.uiMgr.stageSize, arr_areaColl[1].transform.position.z));

            PlayGuideParticle();
            for (int index = 0; index < list_guideParticle.Count; index++)
            {
                list_guideParticle[index].transform.SetParent(transform.GetChild(0));
                list_guideParticle[index].transform.localPosition = new Vector3(list_guideParticle[index].transform.localPosition.x, 0, list_guideParticle[index].transform.localPosition.z);
            }
            MoveToTarget();
        }
        else
        {
            gameMgr.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_FAILURE);
            Init();
        }
    }

    void Init()
    {
        Debug.Log(gameObject.name + "Init");
        list_movePoint = new List<Vector3>();

        arr_areaColl[0].gameObject.SetActive(true);

        gameMgr.handCtrl.manoHandMove.arr_handFollwer[1].ToggleHandEffect(false);

        drawingLine.positionCount = 2;
        drawingLine.SetPosition(1, arr_areaColl[0].transform.position);
        drawingLine.gameObject.SetActive(false);

        StartCoroutine(RemoveGuideParticle());
        list_guidePosition = new List<Vector3>();

    }



    public void MoveToTarget()
    {
        drawingLine.gameObject.SetActive(false);
        Debug.Log(gameObject.name + "MoveToTarget");

        List<Vector3> kantoMove, zinoMove;
        kantoMove = new List<Vector3>();
        zinoMove = new List<Vector3>();

        List<ParticleSystem> kantoP, zinoP;
        kantoP = new List<ParticleSystem>();
        zinoP = new List<ParticleSystem>();
        
        for (int i = 0; i < list_movePoint.Count; i++)
        {
            if (i < list_movePoint.Count / 2 + 1)
            {
                kantoMove.Add(list_movePoint[i]);
                kantoP.Add(list_guideParticle[i]);
            }
            else
            {
                zinoMove.Add(list_movePoint[i]);
                zinoP.Add(list_guideParticle[i]);
            }
        }
        zinoMove.RemoveAt(0);
        zinoMove.Reverse();
        zinoP.RemoveAt(0);
        zinoP.Reverse();

        if (list_guideParticle.Count > list_movePoint.Count)
        {
            for (int i = list_movePoint.Count; i < list_guideParticle.Count; i++)
            {
                list_guideParticle[i].Stop();
            }
        }

        Character[] arr_header = gameMgr.currentEpisode.currentStage.arr_header;

        arr_header[0].SetAnim(2);
        arr_header[0].MultipleMoveCharacter(kantoMove, gameMgr.uiMgr.stageSize, () =>
        {
            arr_header[0].transform.LookAt(arr_header[1].transform);
            arr_header[0].SetAnim(0);
            kantoEnd = true;
            CheckMoveEnd();
        },kantoP);
        arr_header[1].SetAnim(2);
        arr_header[1].MultipleMoveCharacter(zinoMove, gameMgr.uiMgr.stageSize, () =>
        {
            arr_header[1].transform.LookAt(arr_header[0].transform);
            arr_header[1].SetAnim(0);
            zinoEnd = true;
            CheckMoveEnd();
        }, zinoP);

        StopAllCoroutines();
    }

    //두 캐릭터 모두 목표지점까지 도착했는지 체크
    void CheckMoveEnd()
    {
        if (zinoEnd && kantoEnd)
        {
            Character[] arr_header = gameMgr.currentEpisode.currentStage.arr_header;
            arr_header[1].transform.LookAt(arr_header[0].transform);
            arr_header[0].transform.LookAt(arr_header[1].transform);

            EndInteraction();
        }
    }

    //타임라인 트리거 호출함수
    public void JumpAction()
    {
        StartCoroutine(cJumpAction());
    }
    //타임라인에서 점프해서 부딪히는 장면까지 강제 이동해서 위치 맞추기
    IEnumerator cJumpAction()
    {
        Character[] arr_header = gameMgr.currentEpisode.currentStage.arr_header;
        float t = 0;
        float anim = 4f;

        while (Vector3.Distance(arr_header[0].transform.position, arr_header[1].transform.position) > (1.8f * gameMgr.uiMgr.stageSize))
        {
            t += 0.008f / anim * 2 * Time.deltaTime;
            arr_header[0].transform.position = Vector3.Lerp(arr_header[0].transform.position, arr_header[0].transform.position + (arr_header[1].transform.position - arr_header[0].transform.position) / 2, t);
            arr_header[1].transform.position = Vector3.Lerp(arr_header[1].transform.position, arr_header[1].transform.position + (arr_header[0].transform.position - arr_header[1].transform.position) / 2, t);

            yield return new WaitForSeconds(0.01f);
        }
        gameObject.SetActive(false);
    }

    //대기중 대사 출력
    public override IEnumerator DialogWaitTime()
    {
        gameMgr.currentEpisode.currentStage.arr_header[0].headerCanvas.gameObject.SetActive(true);
        while (gameMgr.statGame == GameStatus.INTERACTION &&
            arr_LoopDialog.Length > 0 &&
            !arr_areaColl[1].isColl)
        {
            //  int rand = Random.Range(0, arr_LoopDialog.Length);
            for (int i = 0; i < arr_LoopDialog.Length; i++)
            {
                yield return new WaitForSeconds(5f);
                if (arr_areaColl[2].isColl)
                {
                    gameMgr.currentEpisode.currentStage.arr_header[0].headerCanvas.ShowText(arr_LoopDialog[1], 5);
                }
                else
                {
                    gameMgr.currentEpisode.currentStage.arr_header[0].headerCanvas.ShowText(arr_LoopDialog[0], 5);
                }
                yield return new WaitForSeconds(5f);
            }
        }
    }

    IEnumerator RemoveGuideParticle()
    {
        if (list_guideParticle.Count == 0)
        {
            yield break;
        }
        for (int i = list_guideParticle.Count-1; i >1; i--)
        {
            if (list_guideParticle[i] != null &&
                list_guideParticle[i].isPlaying)
            {
                list_guideParticle[i].Stop();
                
                gameMgr.soundMgr.PlaySfx(list_guideParticle[i].transform.position, sfx_light,Random.Range(0.7f,1.3f));
                yield return new WaitForSeconds(0.05f);
            }
        }
        list_guideParticle[0].Play();
        list_guideParticle[1].Stop();
        list_guideParticle[0].transform.position = arr_areaColl[0].transform.position + Vector3.up * 2 * gameMgr.uiMgr.stageSize;
        list_guideParticle[1].transform.position = arr_areaColl[1].transform.position + Vector3.up * 2 * gameMgr.uiMgr.stageSize;
    }


    public override void StartInteraction()
    {
        base.StartInteraction();
        stageMgr.StopAllCoroutines();

        gameMgr.currentEpisode.currentStage.arr_header[0].transform.position =
            new Vector3(arr_areaColl[0].transform.position.x,
            gameMgr.currentEpisode.currentStage.arr_header[0].transform.position.y,
            arr_areaColl[0].transform.position.z);

        gameMgr.currentEpisode.currentStage.arr_header[1].transform.position =
            new Vector3(arr_areaColl[1].transform.position.x,
            gameMgr.currentEpisode.currentStage.arr_header[1].transform.position.y,
            arr_areaColl[1].transform.position.z);

        StartCoroutine(stageMgr.SmoothRotReset(0, Quaternion.Euler(0, 90, 0), true));
        StartCoroutine(stageMgr.SmoothRotReset(1, Quaternion.Euler(0, -90, 0), true));
        //gameMgr.currentEpisode.currentStage.arr_header[0].transform.localRotation = Quaternion.Euler(0, 90, 0);
        //gameMgr.currentEpisode.currentStage.arr_header[1].transform.localRotation = Quaternion.Euler(0, -90, 0);

        for (int i = 0; i < arr_areaColl.Length; i++)
        {
            arr_areaColl[i].gameObject.SetActive(true);
        }

        //Vector3 _linePos = gameMgr.currentEpisode.gameObject.transform.position / gameMgr.uiMgr.stageSize;
        //drawingLine.transform.localPosition = new Vector3(_linePos.x, 0, _linePos.z);
        //drawingLine.transform.rotation = new Quaternion(drawingLine.transform.rotation.x, drawingLine.transform.rotation.y - gameMgr.currentEpisode.transform.rotation.y, drawingLine.transform.rotation.z, drawingLine.transform.rotation.w);

        PlayGuideParticle();
        gameMgr.uiMgr.ui_game.ChangeHandIcon(HandIcon.INDEX);

        list_guideParticle[1].Stop();
    }

    public override void EndInteraction()
    {
        base.EndInteraction();

        StopAllCoroutines();
        StopGuideParticle();
    }


}
