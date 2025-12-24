using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NRSelectable: Selectable
{
    NRObjectSelect currentSelector;
    Animator m_animator;

    //public Text txt_name;
    //public TextMesh ptxt_name;
    public TextMeshPro ptxt_name = null;

    //하위오브젝트 컬리더 전달 
    public NRRayInteract rayEvent;

    public bool isSelected = false;
    public bool isTimer = false;
    public bool isLock = false;

    int lastAnim = 0;

    private void Awake()
    {
        if (GetComponent<Animator>())
        {
            m_animator = GetComponent<Animator>();
        }
        if (GetComponent<NRRayInteract>())
        {
            rayEvent = GetComponent<NRRayInteract>();
        }
        else if (transform.childCount >= 1)
        {
            rayEvent = transform.GetChild(1).GetComponent<NRRayInteract>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentSelector = transform.parent.GetComponent<NRObjectSelect>();

        if (ptxt_name != null)
        {
            ptxt_name.text = gameObject.name;
        }

        //RayCast 이벤트가 있을 경우
        if (rayEvent != null)
        {
            rayEvent.m_RayEvent.AddListener(() => currentSelector.SelectObject(this));
            rayEvent.pointerClick.AddListener(() => PointerClickEvent());
            rayEvent.pointerHover.AddListener(() => PointerHoverEvent());
            rayEvent.pointerExit.AddListener(() => PointerExitEvent());
        }
        else
        {
            action.AddListener(() =>
             currentSelector.SelectObject(this));
        }

    }

    private void Update()
    {
        if (ptxt_name != null)
        {
            ptxt_name.transform.rotation = Quaternion.LookRotation(ptxt_name.transform.position - GameManager.Instance.mainCamera.transform.position);
        }
    }

    private void OnDisable()
    {
        lastAnim = 0;
    }

    /// <summary>
    /// 손과 직접 충돌체크   
    /// </summary><param name="coll"></param>
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.layer == 10)
        {
            PointerClickEvent();
        }
    }
    private void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject.layer == 10)
        {
            PointerHoverEvent();
        }
    }
    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.layer == 10)
        {
            PointerExitEvent();
        }
    }

    public void PointerClickEvent()
    {
        if (isLock)
            return;

        if (isSelected)
        {
            currentSelector.SelectObject(this);
        }
    }
    public void PointerHoverEvent()
    {
        if (isLock)
            return;

        if (!isSelected)
        {
            isSelected = true;
            currentSelector.SelectObject(this);
        }
    }
    public void PointerExitEvent()
    {
        if (isLock)
            return;

        if (isSelected)
        {
            currentSelector.DeSelectObject();
        }
    }


    /// <summary>
    /// SetAnimation of SelectableObject
    /// </summary>
    /// <param name="_animationNum">0:Spawn 1:Select 2:Deselect</param>
    public void SetTriggerAnimation(int _animationNum)
    {

        if (_animationNum != 4 &&
            _animationNum == lastAnim ||
            m_animator == null)
        {
            return;
        }

        lastAnim = _animationNum;

        switch (_animationNum)
        {
            case 0:
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
            case 4:
                m_animator.ResetTrigger("tClick");
                m_animator.SetTrigger("tClick");
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
