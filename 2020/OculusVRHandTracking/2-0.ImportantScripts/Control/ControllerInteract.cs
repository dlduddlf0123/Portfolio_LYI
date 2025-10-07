using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInteract : PlayerHand
{
    public LineRenderer raycastLine;

    protected override void Awake()
    {
        base.Awake();
        gameMgr = GameManager.Instance;

    }


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (m_controller == OVRInput.Controller.LTouch)
        {
            LeftControlerInput();
        }
        if (m_controller == OVRInput.Controller.RTouch)
        {
            RightControlerInput();
        }
    }

    /// <summary>
    /// 왼손 컨트롤
    /// </summary>
    public void LeftControlerInput()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            RaycastLine();
        }
        else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
        {
            RaycastAction();
        }
        else
        {
            if (raycastLine.gameObject.activeSelf)
            {
                raycastLine.gameObject.SetActive(false);
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
        {
            GrabBegin();
        }
        else if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger))
        {
            GrabEnd();
        }

        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            gameMgr.menuUI.gameObject.SetActive(!gameMgr.menuUI.gameObject.activeSelf);
        }
    }

    /// <summary>
    /// 오른손 컨트롤
    /// </summary>
    public void RightControlerInput()
    {

        if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
        {
            RaycastLine();
        }
        else if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
        {
            RaycastAction();
        }
        else
        {
            if (raycastLine.gameObject.activeSelf)
            {
                raycastLine.gameObject.SetActive(false);
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
        {
            GrabBegin();
        }
        else if (OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
        {
            GrabEnd();
        }
    }

    /// <summary>
    /// 레이 렌더링
    /// </summary>
    public void RaycastLine()
    {
        // gameMgr.selectHeader.OrderAction(0);
        Vector3 rayPos = transform.position+ transform.forward * 2f;
        Vector3 bodyPos = transform.position;

        Vector3[] arr_rayPos = new Vector3[2] { bodyPos, (rayPos - bodyPos) * 200f };

        RaycastHit hit;
        if (Physics.Raycast(bodyPos, (rayPos - bodyPos) * 200f, out hit))
        {
            //캐릭터 가리킬 시
            if (hit.transform.CompareTag("Header"))
            {
                Character _header = hit.transform.GetComponent<Character>();
                arr_rayPos = new Vector3[2] { bodyPos, hit.point};
            }

            //땅 가리킬 시
            if (hit.transform.CompareTag("Ground"))
            {
                arr_rayPos = new Vector3[2] { bodyPos, hit.point };
            }

            //버튼 가리킬 시
            if (hit.transform.CompareTag("Button"))
            {
                RayInteractObject obj = hit.transform.gameObject.GetComponent<RayInteractObject>();
                if (!obj.isActive)
                {
                    arr_rayPos = new Vector3[2] { bodyPos, hit.point };
                    //하이라이트
                }
            }
        }


        raycastLine.SetPositions(arr_rayPos);
        raycastLine.gameObject.SetActive(true);
    }

    void RaycastAction()
    {
        //레이 발사
        Vector3 rayPos = transform.position+ transform.forward * 2f;
        Vector3 bodyPos = transform.position;

        RaycastHit hit;
        if (Physics.Raycast(bodyPos, (rayPos - bodyPos) * 200f, out hit))
        {
            //캐릭터 선택
            if (hit.transform.CompareTag("Header"))
            {
                // Debug.Log(hit.transform.gameObject.name);
                //gameMgr.selectHeader = hit.transform.GetComponent<Character>();

                //gameMgr.selectPointer.transform.position = gameMgr.selectHeader.transform.position + Vector3.up * 1.3f;
                //gameMgr.selectPointer.transform.SetParent(gameMgr.selectHeader.transform);
                //gameMgr.selectPointer.SetActive(true);

                Character _header = hit.transform.GetComponent<Character>();
                _header.headerCanvas.ToggleCircleUI();

                gameMgr.PlayEffect(hit.point, gameMgr.particles[1]);
                gameMgr.soundMgr.PlaySfx(transform, gameMgr.soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_CLICK));
            }

            //땅 찍을시 이동
            if (hit.transform.CompareTag("Ground") &&
                gameMgr.statGame == GameState.MINIGAME &&
                 gameMgr.currentPlay.GetComponent<StageManager>().currentMiniGame == MiniGameType.BASKET)
            {
                gameMgr.currentPlay.GetComponent<StageManager>().arr_miniGame[1].GetComponent<MiniGameBasket>().header.MoveCharacter(hit.point, hit.transform.gameObject);
                //gameMgr.selectHeader.MoveCharacter(hit.point, hit.transform.gameObject);

                gameMgr.PlayEffect(hit.point, gameMgr.particles[1]);
                gameMgr.soundMgr.PlaySfx(transform, gameMgr.soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_CLICK));
            }

            //UI 상호작용
            if (hit.transform.CompareTag("Button"))
            {
                RayInteractObject obj = hit.transform.gameObject.GetComponent<RayInteractObject>();
                if (!obj.isActive)
                {
                    obj.Active();

                    // hit.transform.gameObject.GetComponent<UICube>().ToggleCharacter();
                    gameMgr.PlayEffect(hit.point, gameMgr.particles[1]);
                    gameMgr.soundMgr.PlaySfx(transform, gameMgr.soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_CLICK));
                }
            }
        }

    }

}
