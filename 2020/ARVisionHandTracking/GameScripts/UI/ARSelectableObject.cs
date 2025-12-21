using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ARSelectableObject : Selectable
{
    ARObjectSelect currentSelector;
    Animator m_animator;
    public GameObject stagePrefab;

    //public Text txt_name;
    //public TextMesh ptxt_name;
    public TextMeshPro ptxt_name = null;

    //하위오브젝트 컬리더 전달 
    public RayInteractObject rayEvent;

    public bool isSelected = false;
    public bool isTimer = false;

    int lastAnim = 0;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        if (transform.childCount == 1)
        {
            rayEvent = transform.GetChild(0).GetComponent<RayInteractObject>();
        }
        if (GetComponent<RayInteractObject>())
        {
            rayEvent = GetComponent<RayInteractObject>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (ptxt_name != null)
        {
            ptxt_name.text = gameObject.name;
        }
        if (rayEvent != null)
        {
            rayEvent.m_RayEvent.AddListener(() => currentSelector.SelectObject(this));
            rayEvent.collsionEnter.AddListener(() => CollsionEnterEvent());
            rayEvent.collsionStay.AddListener(() => CollsionStayEvent());
            rayEvent.collsionExit.AddListener(() => CollsionExitEvent());
        }
        else
        {
            action.AddListener(() =>
             currentSelector.SelectObject(this));
        }
        currentSelector = transform.parent.GetComponent<ARObjectSelect>();
    }

    private void Update()
    {
        if (ptxt_name != null)
        {
            ptxt_name.transform.rotation = Quaternion.LookRotation(ptxt_name.transform.position - Camera.main.transform.position);
        }
    }

    private void OnDisable()
    {
        lastAnim = 0;
    }

    /// <summary>
    /// 손과 충돌체크   
    /// </summary><param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            CollsionEnterEvent();
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            CollsionStayEvent();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            CollsionExitEvent();
        }
    }

    public void CollsionEnterEvent()
    {
        if (!isSelected)
        {
            currentSelector.SelectObject(this);
        }
    }
    public void CollsionStayEvent()
    {
        if (isSelected &&
            !isTimer)
        {
            isTimer = true;
            GameManager.Instance.uiMgr.worldCanvas.StartTimer(transform.position, GameManager.Instance.waitTime, () => currentSelector.SelectObject(this));
        }
    }
    public void CollsionExitEvent()
    {
        if (isTimer)
        {
            GameManager.Instance.uiMgr.worldCanvas.StopTimer();
            currentSelector.DeSelectObject();
        }
    }


    /// <summary>
    /// SetAnimation of SelectableObject
    /// </summary>
    /// <param name="_animationNum">0:Spawn 1:Select 2:Deselect</param>
    public void SetTriggerAnimation(int _animationNum)
    {
        if (_animationNum == lastAnim)
        {
            return;
        }

        lastAnim = _animationNum;

        switch (_animationNum)
        {
            case 0:
                m_animator.ResetTrigger("tSpawn");
                m_animator.SetTrigger("tSpawn");
                break;
            case 1:
                m_animator.ResetTrigger("tOnSelect");
                m_animator.SetTrigger("tOnSelect");
                break;
            case 2:
                m_animator.ResetTrigger("tDeSelect");
                m_animator.SetTrigger("tDeSelect");
                break;
            case 3:
                m_animator.ResetTrigger("tDestroy");
                m_animator.SetTrigger("tDestroy");
                break;
            default:
                break;
        }

    }

    public void ActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
