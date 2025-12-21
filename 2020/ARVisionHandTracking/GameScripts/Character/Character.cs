using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Pathfinding;

public class Character : MonoBehaviour
{
    protected GameManager gameMgr;
    protected SoundManager soundMgr;

    public HeaderCanvas headerCanvas;

    public Foot[] arr_feet;
    public Renderer[] arr_skin;
    protected Material m_sharedMat;
    protected Shader[] arr_shader = new Shader[2];

    //Animation
    public Animator m_animator { get; set; }

    //Navigation
    public Seeker m_seeker { get; set; }
    public RichAI m_richAI { get; set; }
    protected CharacterController m_controller;

    public Coroutine currentAI = null;

    public List<List<List<object>>> list___dialog_kor { get; set; }
    public List<List<List<object>>> list___dialog_eng { get; set; }

    UnityAction _action;

    public float walkSpeed = 1f;
    public bool isNavMove = false;

    public bool isTouched = false;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        soundMgr = gameMgr.soundMgr;

        arr_feet = GetComponentsInChildren<Foot>();
        arr_skin = GetComponentsInChildren<Renderer>();
        arr_shader[0] = Shader.Find("FlatKit/Stylized SurfaceDressY");
        arr_shader[1] = Shader.Find("FlatKit/Stylized SurfaceDressZ");

        m_animator = transform.GetChild(0).GetComponent<Animator>();

        m_seeker = GetComponent<Seeker>();
        m_richAI = GetComponent<RichAI>();
        m_controller = GetComponent<CharacterController>();
        m_richAI.enabled = false;
        //m_controller.enabled = false;

        list___dialog_kor = new List<List<List<object>>>();
        list___dialog_eng = new List<List<List<object>>>();

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

    public virtual void SetAnim(int _num)
    {
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
    public void StopMove()
    {
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

        while (!m_richAI.reachedDestination)
        {
            m_richAI.destination = target;
            yield return new WaitForSeconds(0.02f);
        }

        m_richAI.enabled = false;
    //    m_controller.enabled = false;

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
    public void Success(string _text)
    {
        PlayTriggerAnimation(0);
        if (headerCanvas == null)
        {
            return;
        }
        headerCanvas.gameObject.SetActive(true);
        headerCanvas.ShowText(_text);
    }

    /// <summary>
    /// 실패 시 캐릭터 동작, 효과 
    /// </summary>
    /// <param name="_text">대사</param>
    public void Failure(string _text)
    {
        PlayTriggerAnimation(2);
        if (headerCanvas == null)
        {
            return;
        }
        headerCanvas.gameObject.SetActive(true);
        headerCanvas.ShowText(_text);

    }

    Quaternion lastRot;
    public void TurnLook(Transform _target)
    {
        lastRot = transform.rotation;
        StartCoroutine(Turn(_target));
    }
    public void TurnBack()
    {
        StartCoroutine(Turn(null, true));
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
            while (t < 1)
            {
                t += Time.deltaTime * 5;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_target.position - transform.position), t);
                transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
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
