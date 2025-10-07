using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OculusSampleFramework;

public enum HandGesture
{
    NONE,
    FIST,
    FIVE,
    PINCH,
    THUMBSUP,
    //POINT,
    //GUN,
}

/// <summary>
/// 손 관련 데이터 저장
/// 잡기 관련 함수 구현
/// </summary>
public class HandInteract : PlayerHand
{
    public HandGesture lastGesture;
    public HandGesture handGesture;

    public OVRSkeleton skeleton;
    public OVRHand hand;

    public HandInteract otherHand;
   // GameObject attachObject;

    public HandMotion handMotion; //손동작 체크

    //ParticleSystem handEffect;

    public LineRenderer pinchLine;
    public CommandManager commandMgr;

    SkinnedMeshRenderer mSkin;

    [Range(0.0f, 1.0f)]
    public float maxPinchTrethold = 0.7f;
    [Range(0.0f, 1.0f)]
    public float minPinchTrethold = 0.3f;

    public delegate void OnGroundClick();

    public int[] arr_state = new int[4] { 0, 0, 0 ,0 };    //제스쳐 상태
    public float[] arr_fingerStrength = new float[5];   //핀치 체크
    float[] arr_fingerDistance = new float[5];      //손가락 굽힘체크

    public int state = 0; //제스쳐 상태 0: 제스쳐 없음 1:제스쳐 2: 제스쳐 해제 시
    public int doublePinchState = 0; //제스쳐 상태 0: 제스쳐 없음 1:제스쳐 2: 제스쳐 해제 시
    public int pinchState = 0; //제스쳐 상태 0: 제스쳐 없음 1:제스쳐 2: 제스쳐 해제 시

    public float headHeight = 0.1f;

   // bool isClap = false;

    protected override void Awake()
    {
        base.Awake();
        gameMgr = GameManager.Instance;

        hand = GetComponent<OVRHand>();
        skeleton = GetComponent<OVRSkeleton>();

        mSkin = GetComponent<SkinnedMeshRenderer>();
        handColor[0] = mSkin.material.GetColor("_ColorBottom");  //손 머테리얼 변경 시 문제 될 수 있음
        handColor[1] = mSkin.material.GetColor("_ColorTop");
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < skeleton.Capsules.Count; i++)
        {
            skeleton.Capsules[i].CapsuleCollider.isTrigger = true;
        }
        // handEffect = this.transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Item") &&
    //        handGesture == HandGesture.FIST)
    //    {
    //        AttachHand(other.gameObject, this.transform);
    //    }
    //    if (other.CompareTag("Food") &&
    //        handGesture == HandGesture.FIST)
    //    {
    //        other.GetComponent<Food>().attachHand = this;
    //        other.GetComponent<Food>().isAttach = true;
    //        AttachHand(other.gameObject, this.transform);
    //       // gameMgr.selectHeader.AI_Move(4);
    //       // gameMgr.selectHeader.isAction = true;
    //    }
    //    if (other.CompareTag("Player"))
    //    {
    //        ActionClap();
    //    }
    //}

    public override void Update()
    {
        base.Update();

        if (hand.IsTracked)
        {   //손이 추적 중 일때
            CheckGesture();
            PinchRaycast();
            ActionPinch();
            ActionFist();
            ActionFive();

            ActionMenu();
            //ActionDoublePinch();
            //ActionPoint(); 
        }
        else
        {
            //손이 보이지 않을 때 변경
            handGesture = HandGesture.NONE;
            for (int i = 0; i < 5; i++)
            {
                arr_fingerStrength[i] = 0;
            }
            for (int i = 0; i < arr_state.Length; i++)
            {
                arr_state[i] = 0;
            }
            if (pinchLine.gameObject.activeSelf)
            {
                pinchLine.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 각 손가락의 핀치값을 가져와 제스쳐 상태 변경
    /// </summary>
    public void CheckGesture()
    {
        arr_fingerStrength[0] = hand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb);
        arr_fingerStrength[1] = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        arr_fingerStrength[2] = hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle);
        arr_fingerStrength[3] = hand.GetFingerPinchStrength(OVRHand.HandFinger.Ring);
        arr_fingerStrength[4] = hand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky);

        int fistCount = 0;
        int fiveCount = 0;

        float maxDist = 0.08f;
        float minDist = 0.052f;

        //각 손가락의 본 거리 계산(구부림 여부)
        for (int i = 1; i < 6; i++)
        {
            arr_fingerDistance[i - 1] = Vector3.Distance(skeleton.Bones[i * 3].Transform.position, skeleton.Bones[i + 18].Transform.position);

            //Debug.Log("Thumb:" + Vector3.Distance(skeleton.Bones[3].Transform.position, skeleton.Bones[19].Transform.position));
            //Debug.Log("Index:" + Vector3.Distance(skeleton.Bones[6].Transform.position, skeleton.Bones[20].Transform.position));
            //Debug.Log("Middle:" + Vector3.Distance(skeleton.Bones[9].Transform.position, skeleton.Bones[21].Transform.position));
            //Debug.Log("Ring:" + Vector3.Distance(skeleton.Bones[12].Transform.position, skeleton.Bones[22].Transform.position));
            //Debug.Log("Pinky:" + Vector3.Distance(skeleton.Bones[15].Transform.position, skeleton.Bones[23].Transform.position));

            if (arr_fingerDistance[i - 1] < minDist)
            {
                fistCount++;
            }
            else if (arr_fingerDistance[i - 1] > maxDist)
            {
                fiveCount++;
            }
        }

        //Debug.Log("FistCount: " + fistCount);
        //Debug.Log("FiveCount: " + fiveCount);
        
         if (fistCount >= 4 &&
            arr_fingerStrength[0] == 0)
        {
            handGesture = HandGesture.THUMBSUP;
        }
        else if(fistCount >= 4)
        {
            handGesture = HandGesture.FIST;
            lastGesture = handGesture;
            commandMgr.StopMotionCommand();
        }
        else if (fiveCount >= 4)
        {
            handGesture = HandGesture.FIVE;
            lastGesture = handGesture;
        }
        //else if (arr_fingerStrength[1] < minPinchTrethold &&
        //    fistCount == 3)
        //{
        //    handGesture = HandGesture.POINT;
        //}
        else if (arr_fingerStrength[1] > maxPinchTrethold)
        {
            handGesture = HandGesture.PINCH;
            lastGesture = handGesture;
        }
        //else if (arr_fingerStrength[1] < minPinchTrethold)
        //{
        //    handGesture = HandGesture.POINT;
        //}
        else
        {
            handGesture = HandGesture.NONE;
            //if (attachObject != null)
            //{
            //    StartCoroutine(ActionDetachHand());
            //}
        }
    }


    /// <summary>
    /// 레이 렌더링
    /// </summary>
    public void PinchRaycast()
    {
        // gameMgr.selectHeader.OrderAction(0);
        Vector3 rayPos = (skeleton.Bones[8].Transform.position + (skeleton.Bones[5].Transform.position - skeleton.Bones[8].Transform.position) * 0.5f) - transform.parent.parent.position;  //엄지와 검지 2번째 관절 기준 가운데위치
        Vector3 bodyPos = (gameMgr.mainCam.transform.position - Vector3.up * headHeight) - transform.parent.parent.position;
        Vector3[] arr_rayPos = new Vector3[2] { rayPos, (rayPos - bodyPos) * 200f };

        pinchLine.SetPositions(arr_rayPos);

        if (handGesture == HandGesture.PINCH ||
            handGesture == HandGesture.NONE)
        {
            pinchLine.gameObject.SetActive(true);
        }
        else
        {
            pinchLine.gameObject.SetActive(false);

        }
    }

    protected override void GrabBegin()
    {
        float closestMagSq = float.MaxValue;
        OVRGrabbable closestGrabbable = null;
        Collider closestGrabbableCollider = null;

        // Iterate grab candidates and find the closest grabbable candidate
        foreach (OVRGrabbable grabbable in m_grabCandidates.Keys)
        {
            bool canGrab = !(grabbable.isGrabbed && !grabbable.allowOffhandGrab);
            if (!canGrab)
            {
                continue;
            }

            for (int j = 0; j < grabbable.grabPoints.Length; ++j)
            {
                Collider grabbableCollider = grabbable.grabPoints[j];
                // Store the closest grabbable
                Vector3 closestPointOnBounds = grabbableCollider.ClosestPointOnBounds(m_gripTransform.position);
                float grabbableMagSq = (m_gripTransform.position - closestPointOnBounds).sqrMagnitude;
                if (grabbableMagSq < closestMagSq)
                {
                    closestMagSq = grabbableMagSq;
                    closestGrabbable = grabbable;
                    closestGrabbableCollider = grabbableCollider;
                }
            }
        }

        // Disable grab volumes to prevent overlaps
        GrabVolumeEnable(false);

        if (closestGrabbable != null)
        {
            if (closestGrabbable.isGrabbed)
            {
                closestGrabbable.grabbedBy.OffhandGrabbed(closestGrabbable);
            }

            m_grabbedObj = closestGrabbable;
            m_grabbedObj.GrabBegin(this, closestGrabbableCollider);

            m_lastPos = transform.position;
            m_lastRot = transform.rotation;

            gameMgr.PlayEffect(grabbedObject.grabbedTransform, gameMgr.particles[1], ReadOnly.Defines.SOUND_SFX_CLICK);

            // Set up offsets for grabbed object desired position relative to hand.
            if (m_grabbedObj.snapPosition)
            {
                m_grabbedObjectPosOff = m_gripTransform.localPosition;
                if (m_grabbedObj.snapOffset)
                {
                    Vector3 snapOffset = m_grabbedObj.snapOffset.position;
                    if (m_controller == OVRInput.Controller.LTouch) snapOffset.x = -snapOffset.x;
                    m_grabbedObjectPosOff += snapOffset;
                }
            }
            else
            {
                Vector3 relPos = m_grabbedObj.transform.position - transform.position;
                relPos = Quaternion.Inverse(transform.rotation) * relPos;
                m_grabbedObjectPosOff = relPos;
            }

            if (m_grabbedObj.snapOrientation)
            {
                m_grabbedObjectRotOff = m_gripTransform.localRotation;
                if (m_grabbedObj.snapOffset)
                {
                    m_grabbedObjectRotOff = m_grabbedObj.snapOffset.rotation * m_grabbedObjectRotOff;
                }
            }
            else
            {
                Quaternion relOri = Quaternion.Inverse(transform.rotation) * m_grabbedObj.transform.rotation;
                m_grabbedObjectRotOff = relOri;
            }

            // Note: force teleport on grab, to avoid high-speed travel to dest which hits a lot of other objects at high
            // speed and sends them flying. The grabbed object may still teleport inside of other objects, but fixing that
            // is beyond the scope of this demo.
            MoveGrabbedObject(m_lastPos, m_lastRot, true);
            SetPlayerIgnoreCollision(m_grabbedObj.gameObject, true);
            if (m_parentHeldObject)
            {
                m_grabbedObj.transform.parent = transform;
            }
        }
    }

    protected override void GrabEnd()
    {
        // base.GrabEnd();

        if (m_grabbedObj)
        {
            Vector3 linearVelocity = (transform.position - m_lastPos) / Time.fixedDeltaTime;
            Vector3 angularVelocity = (transform.eulerAngles - m_lastRot.eulerAngles) / Time.fixedDeltaTime;

            GrabbableRelease(linearVelocity, angularVelocity);
        }

        GrabVolumeEnable(true);
        //if (m_grabbedObj != null)
        // m_grabbedObj.GetComponent<Rigidbody>().AddForce(handMotion.GetComponent<Rigidbody>().velocity);

    }

    #region -------------------------Gesture Actions(Private)-------------------------
    /// <summary>
    /// state 0
    /// </summary>
    void ActionFist()
    {
        if (handGesture == HandGesture.FIST)
        {
            if (arr_state[0] == 0)
            {
                arr_state[0] = 1;
                GrabBegin();
            }
        }
        else
        {
            if (arr_state[0] != 0)
            {
                arr_state[0] = 0;
            }
        }
    }

    void ActionFive()
    {
        if (handGesture == HandGesture.FIVE)
        {
            if (arr_state[1] == 0)
            {
                arr_state[1] = 1;
                GrabEnd();
            }
        }
        else
        {
            if (arr_state[1] != 0)
            {
                arr_state[1] = 0;
            }
        }
    }
    
    void ActionPinch()
    {
        if (handGesture == HandGesture.PINCH)
        {
            if (arr_state[2] == 0)
            {
                arr_state[2] = 1;

                //모션 커맨드 작동 시작
                //if (gameMgr.statGame == GameState.INTERACTION)
                //{
                //    commandMgr.StartMotionCommand(this);
                //}

                //레이 발사
                Vector3 rayPos = skeleton.Bones[8].Transform.position + (skeleton.Bones[5].Transform.position - skeleton.Bones[8].Transform.position) * 0.5f;  //엄지와 검지 2번째 관절 기준 가운데위치
                Vector3 bodyPos = gameMgr.mainCam.transform.position - Vector3.up * headHeight;
                Vector3[] arr_rayPos = new Vector3[2] { rayPos, (rayPos - bodyPos) * 100f };

                RaycastHit hit;
                if (Physics.Raycast(rayPos, (rayPos - bodyPos) * 200f, out hit))
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
                        gameMgr.soundMgr.PlaySfx(hand.transform, gameMgr.soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_CLICK));
                        arr_state[2] = 2;
                    }

                    //땅 찍을시 이동
                    if (hit.transform.CompareTag("Ground") &&
                        gameMgr.statGame == GameState.MINIGAME &&
                         gameMgr.currentPlay.GetComponent<StageManager>().currentMiniGame == MiniGameType.BASKET)
                    {
                        gameMgr.currentPlay.GetComponent<StageManager>().arr_miniGame[1].GetComponent<MiniGameBasket>().header.MoveCharacter(hit.point, hit.transform.gameObject);
                        //gameMgr.selectHeader.MoveCharacter(hit.point, hit.transform.gameObject);

                        gameMgr.PlayEffect(hit.point, gameMgr.particles[1]);
                        gameMgr.soundMgr.PlaySfx(hand.transform, gameMgr.soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_CLICK));
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
                            gameMgr.soundMgr.PlaySfx(hand.transform, gameMgr.soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_CLICK));
                        }
                        arr_state[2] = 2;
                    }
                }

            }
        }
        else
        {
            //초기화
            arr_state[2] = 0;
        }
    }

    void ActionMenu()
    {
        if (lastGesture == HandGesture.FIST && otherHand.lastGesture == HandGesture.FIST &&
            handGesture == HandGesture.THUMBSUP && otherHand.handGesture == HandGesture.THUMBSUP)
        {
            if (arr_state[3] == 0 &&
                !gameMgr.menuUI.gameObject.activeSelf)
            {
                arr_state[3] = 1;
                gameMgr.menuUI.gameObject.SetActive(true);
            }
        }
        else
        {
            if (arr_state[3] != 0)
            {
                arr_state[3] = 0;
            }
        }
    }

    ///// <summary>
    ///// state 
    ///// </summary>
    //public void ActionDoublePinch()
    //{
    //    if (handGesture == HandGesture.PINCH &&
    //        otherHand.handGesture == HandGesture.PINCH)
    //    {
    //        if (arr_state[0] == 0)
    //        {
    //            arr_state[0] = 1;
    //            gameMgr.ActionToggleUI(arr_state[0], otherHand.arr_state[0]);
    //        }
    //    }
    //    else
    //    {
    //        arr_state[0] = 0;
    //    }
    //}

    ////OK
    //public void ActionToggleUI(int state, int state2)
    //{
    //    if (state == 1 &&
    //        state2 == 1)
    //    {
    //        soundMgr.PlaySfx(this.transform, ReadOnly.Defines.SOUND_SFX_CLICK);
    //        gameMgr.PlayEffect(uiMgr.transform.position, particles[0]);
    //        uiMgr.transform.GetChild(0).gameObject.SetActive(!uiMgr.transform.GetChild(0).gameObject.activeSelf);
    //        uiMgr.transform.position = new Vector3(hand[0].transform.position.x - 0.2f, hand[0].transform.position.y + 0.15f, hand[0].transform.position.z);
    //        state = 2;
    //        state2 = 2;
    //    }
    //}
    //public void ActionPoint()
    //{
    //    if (handGesture == HandGesture.POINT)
    //    {
    //        // gameMgr.selectHeader.OrderAction(0);
    //        RaycastHit hit;
    //        if (Physics.Raycast(skeleton.Bones[8].Transform.position, (skeleton.Bones[8].Transform.position - skeleton.Bones[6].Transform.position) * 100f, out hit))
    //        {
    //            Debug.DrawRay(skeleton.Bones[8].Transform.position, (skeleton.Bones[8].Transform.position - skeleton.Bones[6].Transform.position) * 100f, Color.red, 0.1f);
    //            if (hit.transform.CompareTag("Header"))
    //            {
    //                Debug.Log(hit.transform.gameObject.name);
    //                gameMgr.selectHeader = hit.transform.GetComponent<Character>();
    //                gameMgr.HandEffect(this);
    //            }
    //        }
    //    }
    //    else
    //    {
    //        arr_state[2] = 0;
    //    }
    //}
    #endregion

    public override void ChangeHandColor(Color _top, Color _bottom)
    {
        mSkin.material.SetColor("_ColorTop", _top);
        mSkin.material.SetColor("_ColorBottom", _bottom);
    }

    #region -------------------------Old Codes-------------------------
    //IEnumerator ClapTimer()
    //{
    //    if (isClap || otherHand.isClap) { yield break; }

    //    isClap = true;
    //    yield return new WaitForSeconds(0.5f);
    //    isClap = false;
    //}

    //public void ActionClap()
    //{
    //    if (isClap || otherHand.isClap) { return; }
    //    StartCoroutine(ClapTimer());

    //    soundMgr.PlaySfx(transform, ReadOnly.Defines.SOUND_SFX_CLAP, Random.Range(0.8f, 1.2f));
    //    gameMgr.PlayEffect(transform.position, gameMgr.particles[1]);

    //    //if (gameMgr.selectHeader.statAnim != AnimState.CALL)
    //    //{
    //    //    gameMgr.selectHeader.Stop();
    //    //    gameMgr.selectHeader.AI_Move(4);
    //    //}
    //}

    ///// <summary>
    ///// 아이템 입에 물기
    ///// </summary>
    ///// <param name="_go">입에 고정할 오브젝트</param>
    //public void AttachHand(GameObject _go, Transform _tr)
    //{
    //    if (attachObject != null)
    //    {
    //        StartCoroutine(ActionDetachHand());
    //    }

    //    _go.transform.SetParent(_tr);
    //    _go.transform.position = _tr.position;

    //    _go.GetComponent<Rigidbody>().isKinematic = true;
    //    attachObject = _go;
    //    // hand.isShowHand = false;
    //    //  hand.transform.GetChild(0).gameObject.SetActive(false);

    //    gameMgr.soundMgr.PlaySfx(this.transform, gameMgr.soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_CLICK));
    //    gameMgr.HandEffect(this);
    //}

    ///// <summary>
    ///// 아이템 떨어트리기
    ///// </summary>
    //public IEnumerator ActionDetachHand()
    //{
    //    if (attachObject != null)
    //    {
    //        gameMgr.HandEffect(this);
    //        attachObject.transform.SetParent(null);

    //        Rigidbody rb = attachObject.GetComponent<Rigidbody>();
    //        rb.isKinematic = false;

    //        //rb.velocity = (transform.position - lastPos) / Time.fixedDeltaTime*0.2f;
    //        //rb.angularVelocity = (transform.eulerAngles - lastRot.eulerAngles) / Time.fixedDeltaTime*0.2f;

    //        attachObject = null;
    //        //    hand.isShowHand = true;
    //        yield return new WaitForSeconds(0.2f);
    //        //    hand.transform.GetChild(0).gameObject.SetActive(true);
    //    }
    //}

    //public void DetachHand()
    //{
    //    if (attachObject != null)
    //    {
    //        //gameMgr.HandEffect(this);
    //        attachObject.transform.SetParent(null);
    //        Rigidbody rb = attachObject.GetComponent<Rigidbody>();
    //        rb.isKinematic = false;

    //        rb.velocity = Vector3.zero;
    //        rb.angularVelocity = Vector3.zero;

    //        attachObject = null;
    //        //    hand.isShowHand = true;
    //        //    hand.transform.GetChild(0).gameObject.SetActive(true);
    //    }
    //}
    #endregion

}
