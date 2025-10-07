using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

using Pathfinding;
using RogoDigital.Lipsync;

public enum AnimationState
{
    NONE,
    IDLE,
    WALK,
    RUN,
    THINK,
}


public class Character : MonoBehaviour
{
    protected GameManager gameMgr;
    protected SoundManager soundMgr;

    //This gameObject components
    public LipSync m_lipSync { get; set; }

    public Seeker m_seeker { get; set; }
    public RichAI m_richAI { get; set; }

    protected CharacterController m_controller;


    //Child gameObejct components
    public Animator m_animator { get; set; }
    public AudioSource lipSound { get; set; }
    public HeaderCanvas headerCanvas;

    public Foot[] arr_feet;
    public Renderer[] arr_skin;
    protected Material m_sharedMat;
    protected Shader[] arr_shader = new Shader[2];

    public AnimationState statAnim;
    public Coroutine currentAI = null;

    public List<List<List<object>>> list___dialog_kor { get; set; }
    public List<List<List<object>>> list___dialog_eng { get; set; }

    UnityAction _action;

    public Color characterColor;

    public float walkSpeed = 1f;
    public bool isNavMove = false;

    public bool isTouched = false;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        soundMgr = gameMgr.soundMgr;
        m_lipSync = GetComponent<LipSync>();
        m_seeker = GetComponent<Seeker>();
        m_richAI = GetComponent<RichAI>();
        m_controller = GetComponent<CharacterController>();
        m_richAI.enabled = false;
        //m_controller.enabled = false;

        m_animator = transform.GetChild(0).GetComponent<Animator>();
        lipSound = transform.GetChild(0).GetChild(2).GetComponent<AudioSource>();

        arr_feet = GetComponentsInChildren<Foot>();
        arr_skin = GetComponentsInChildren<SkinnedMeshRenderer>();
        arr_shader[0] = Shader.Find("FlatKit/Stylized SurfaceDressY");
        arr_shader[1] = Shader.Find("FlatKit/Stylized SurfaceDressZ");


        list___dialog_kor = new List<List<List<object>>>();
        list___dialog_eng = new List<List<List<object>>>();

        m_richAI.radius *= gameMgr.uiMgr.stageSize;
        m_richAI.height *= gameMgr.uiMgr.stageSize;

        DoAwake();
    }

    protected virtual void DoAwake() { }

    public virtual void PlayTriggerAnimation(int _num)
    {
        m_animator.SetBool("isMove", false);
        m_animator.SetBool("isThink", false);
        m_animator.SetFloat("TriggerNum", _num);
        m_animator.SetTrigger("isTrigger");
        Debug.Log(gameObject.name + "TraggerNum: " + _num);
    }

    public virtual void PlayJustTrigger(int _num)
    {
        float _defaultTriggerNum = m_animator.GetFloat("TriggerNum");
        m_animator.SetFloat("TriggerNum", _num);
        m_animator.SetTrigger("isTrigger");
        Debug.Log(gameObject.name + "TriggerNum: " + _num);
        StopAllCoroutines();
        StartCoroutine(gameMgr.LateFunc(() => m_animator.SetFloat("TriggerNum", _defaultTriggerNum), m_animator.GetCurrentAnimatorClipInfo(0).Length));
    }

    public virtual void SetAnim(int _num)
    {
        if (!gameObject.activeSelf) { return; }

        m_animator.SetBool("isMove", false);
        m_animator.SetBool("IdleMove", false);
        m_animator.SetBool("isThink", false);
        switch (_num)
        {
            case 0:
                //IDLE
                break;
            case 1:
                //WALK
                m_animator.SetBool("isMove", true);
                m_animator.SetFloat("Speed", 0);
                break;
            case 2:
                //RUN
                m_animator.SetBool("isMove", true);
                m_animator.SetFloat("Speed", 2);
                break;
            case 3:
                m_animator.SetBool("isThink", true);
                break;
            default:
                break;
        }
    }
    public virtual void SetAnim(AnimationState _anim)
    {
        if (!gameObject.activeSelf) { return; }
        statAnim = _anim;

        m_animator.SetBool("isMove", false);
        m_animator.SetBool("IdleMove", false);
        m_animator.SetBool("isThink", false);
        switch (_anim)
        {
            case AnimationState.NONE:
                break;
            case AnimationState.IDLE:
                break;
            case AnimationState.WALK:
                m_animator.SetBool("isMove", true);
                m_animator.SetFloat("Speed", 0);
                break;
            case AnimationState.RUN:
                m_animator.SetBool("isMove", true);
                m_animator.SetFloat("Speed", 2);
                break;
            case AnimationState.THINK:
                m_animator.SetBool("isThink", true);
                break;
            default:
                break;
        }
    }

    public virtual void SetEyeAnim(int _num)
    {
        if (_num == 0)
        {
            m_animator.SetBool("isEye", false);
            return;
        }

        m_animator.SetBool("isEye", true);
        switch (_num)
        {
            case 1:
                m_animator.SetFloat("EyeNum", _num);
                break;
            default:
                break;
        }
    }

    public virtual void ChangeIdleAnimation(int _num)
    {
        m_animator.SetBool("isMove", false);
        m_animator.SetBool("IdleMove", true);
        m_animator.SetFloat("TriggerNum", _num);
    }

    public void MoveCharacter(Vector3 _endPoint, float _maxSpeed, UnityAction _action = null)
    {
        if (currentAI != null)
        {
            StopCoroutine(currentAI);
        }
        currentAI = StartCoroutine(WalkMove(_endPoint, _maxSpeed, _action));

        Debug.Log(gameObject.name + " Move To " + _endPoint);
    }
    public void MultipleMoveCharacter(List<Vector3> _endPoint, float _maxSpeed, UnityAction _action = null, List<ParticleSystem> _arr_p = null)
    {
        if (currentAI != null)
        {
            StopCoroutine(currentAI);
        }
        currentAI = StartCoroutine(WalkMultipleMove(_endPoint, _maxSpeed, _action, _arr_p));

        Debug.Log(gameObject.name + " Move To " + _endPoint);
    }
    public void StopMove()
    {
        if (currentAI != null)
        {
            StopCoroutine(currentAI);
        }
        m_richAI.enabled = false;
      //  m_controller.enabled = false;
    }


    /// <summary>
    /// AI: 00
    /// 걸으면서 돌아다니기
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator WalkMove(Vector3 _endPoint, float _maxSpeed, UnityAction _action = null)
    {
        Vector3 target = _endPoint;

        m_richAI.maxSpeed = _maxSpeed;

        m_richAI.enabled = true;
       // m_controller.enabled = true;

        isNavMove = true;
        var path = m_seeker.StartPath(transform.position, target);

        yield return StartCoroutine(path.WaitForPath());

        float _t = 0;
        while (!m_richAI.reachedDestination &&
            _t < 5)
        {
            _t += 0.02f;
            m_richAI.destination = target;
           // gameMgr.currentEpisode.currentStage.ApathMgr.
            yield return new WaitForSeconds(0.02f);
        }

        m_richAI.enabled = false;
    //    m_controller.enabled = false;

        if (_action != null)
        {
            _action.Invoke();
        }
    }
    public virtual IEnumerator WalkMultipleMove(List<Vector3> _arr_point, float _maxSpeed, UnityAction _action = null, List<ParticleSystem> _arr_p = null)
    {
        List<Vector3> target = _arr_point;

        m_richAI.maxSpeed = _maxSpeed;
        m_richAI.slowdownTime = 0;

        m_richAI.enabled = true;
        // m_controller.enabled = true;

        isNavMove = true;

        for (int point = 0; point < target.Count; point++)
        {
            //var path = ABPath.FakePath(target);
             var path = m_seeker.StartPath(transform.position, target[point]);

            yield return StartCoroutine(path.WaitForPath());
            //m_richAI.SetPath(path);

            float skipTime = 0f;
            while (!m_richAI.reachedDestination && skipTime < 2f)
            {
                m_richAI.destination = target[point];
                //transform.position = new Vector3(transform.position.x,0,transform.position.z);
                //transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
                skipTime += 0.02f;

                yield return new WaitForSeconds(0.02f);
            }

            if (_arr_p != null)
            {
                _arr_p[point].Stop();
            }
        }

        m_richAI.enabled = false;
        //    m_controller.enabled = false;

        Debug.Log(gameObject.name + ": Navigation End");
        if (_action != null)
        {
            _action.Invoke();
        }
    }

    public void CheckSuccess(bool _isSuccess, string _text)
    {
        if (_isSuccess)
        {
            Success(_text);
        }
        else
        {
            Failure(_text);
        }
    }

    /// <summary>
    /// 성공 시 캐릭터 동작, 효과
    /// </summary>
    /// <param name="_text">대사</param>
    public void Success(string _text = null)
    {
        PlayJustTrigger(0);
        if (headerCanvas == null)
        {
            return;
        }
        headerCanvas.gameObject.SetActive(true);
        headerCanvas.ShowText("That's Right!");
    }

    /// <summary>
    /// 실패 시 캐릭터 동작, 효과 
    /// </summary>
    /// <param name="_text">대사</param>
    public void Failure(string _text = null)
    {
        PlayJustTrigger(2);
        if (headerCanvas == null)
        {
            return;
        }
        headerCanvas.gameObject.SetActive(true);
        headerCanvas.ShowText("No..");

    }

    Quaternion lastRot;
    public void TurnLook(Transform _target)
    {
        lastRot = transform.rotation;
        currentAI = StartCoroutine(Turn(_target));
    }
    public void TurnBack()
    {
        if (currentAI != null)
        {
            StopCoroutine(currentAI);
        }

        currentAI = StartCoroutine(Turn(null, true));
    }
    public void ChildTurnLook(Transform _target)
    {
        lastRot = transform.GetChild(0).rotation;
        StartCoroutine(ChildTurn(_target));
    }
  
    public void ChildTurnBack()
    {
        StartCoroutine(ChildTurn(null, true));
    }

    protected IEnumerator Turn(Transform _target = null, bool isback = false)
    {
        float t = 0;
        if (isback)
        {
            while (t < 1)
            {
                t += Time.deltaTime * 5;
                transform.rotation = Quaternion.Lerp(transform.rotation, lastRot, t);
                transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
                yield return new WaitForSeconds(0.01f);
            }

        }
        else
        {
            while (t < 1 || !isback)
            {
                t += Time.deltaTime * 5;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_target.position - transform.position), t);
                transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
    protected IEnumerator ChildTurn(Transform _target = null, bool isback = false)
    {
        float t = 0;
        if (isback)
        {
            while (t < 1)
            {
                t += Time.deltaTime * 5;
                transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).rotation, lastRot, t);
                transform.GetChild(0).rotation = new Quaternion(0, transform.GetChild(0).rotation.y, 0, transform.GetChild(0).rotation.w);
                yield return new WaitForSeconds(0.01f);
            }

        }
        else
        {
            while (t < 1)
            {
                t += Time.deltaTime * 5;
                transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).rotation, Quaternion.LookRotation(_target.position - transform.GetChild(0).position), t);
                transform.GetChild(0).rotation = new Quaternion(0, transform.GetChild(0).rotation.y, 0, transform.GetChild(0).rotation.w);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    public float changeSpeed = 1f;
    public IEnumerator ChangeColor(bool isZ = false)
    {
        float fill = 1;

        if (isZ)
            m_sharedMat.shader = arr_shader[1];
        else
            m_sharedMat.shader = arr_shader[0];

        //m_material.SetFloat("_BandHeight", 0.1f);
        while (fill > -1)
        {
            fill  -= Time.deltaTime * changeSpeed;

            m_sharedMat.SetFloat("_ChangeHeight", fill);
            yield return new WaitForSeconds(0.01f);
        }
       // m_sharedMat.mainTexture = m_sharedMat.GetTexture("_BlankTex");
//        m_sharedMat.SetTexture("_BlankTex", gameMgr.b_stagePrefab.LoadAsset<Texture>(""));
    }

    protected IEnumerator Touch()
    {
        yield return new WaitForSeconds(2f);

        isTouched = false;
        m_animator.applyRootMotion = false;
        gameMgr.currentEpisode.currentStage.m_director.Play();

    }

    public void StartBlink(UnityAction _action = null)
    {
        StartCoroutine(HitEffect(_action));
    }
    public void StartBlink()
    {
        StartCoroutine(HitEffect(null));
    }

    protected IEnumerator HitEffect(UnityAction _action = null)
    {
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < arr_skin.Length; i++)
            {
                arr_skin[i].enabled = false;
            }
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < arr_skin.Length; i++)
            {
                arr_skin[i].enabled = true;
            }
            yield return new WaitForSeconds(0.1f);
        }

        if (_action != null)
        {
            _action.Invoke();
        }
    }


}
