using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;


public class TriggerButton : Movable
{
    public enum ButtonType
    {
        INOUT,
        STAYIN, //press button forever
        SPRING, //off when hand off
    }

    public Movable movable;
    BoxCollider boxColl;
    Material mMaterial;
    public ButtonType btnType;

    public delegate void ButtonCallback();
    ButtonCallback callBack;

    public bool isPress = false;    //버튼이 눌려져 있는가 여부
    public bool wait = false;
    public float depth = 0.03f;      //버튼 크기에 따른 눌려지는 깊이 설정
    public float coolTime =0.5f;

    protected override void DoAwake()
    {
        mMaterial = gameObject.GetComponent<MeshRenderer>().material;
        boxColl = GetComponent<BoxCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isPress &&
            !wait)
        {
            StartCoroutine(ButtonCount());
            transform.Translate(Vector3.down * depth);
            isPress = true;  //버튼 눌려진 상태
            OnButtonPress();
            soundMgr.PlaySfx(transform, soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_BUTTON), Random.Range(1 - 0.2f, 1 + 0.2f));

            if (collision.gameObject.GetComponent<HandCollider>())
            {
                collision.gameObject.GetComponent<HandCollider>().hand.hand.TriggerHapticPulse(1000);
            }
        }
        else if (isPress &&
            btnType == ButtonType.INOUT &&
            !wait)
        {
            StartCoroutine(ButtonCount());
            transform.Translate(Vector3.up * depth);
            isPress = false;  //버튼 떼어진 상태
            OnButtonPress();
            soundMgr.PlaySfx(transform, soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_BUTTON2), Random.Range(1 - 0.2f, 1 + 0.2f));

            if (collision.gameObject.GetComponent<HandCollider>())
            {
                collision.gameObject.GetComponent<HandCollider>().hand.hand.TriggerHapticPulse(1000);
            }
        }
    }

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (isPress &&
    //        btnType == ButtonType.SPRING)
    //    {
    //        transform.Translate(Vector3.up * depth);
    //        isPress = false;  //버튼 떼어진 상태
    //    }
    //}

    IEnumerator ButtonCount()
    {
        wait = true;
        yield return new WaitForSeconds(coolTime);
        wait =false;

        if (isPress &&
            btnType == ButtonType.SPRING)
        {
            transform.Translate(Vector3.up * depth);
            isPress = false;  //버튼 떼어진 상태
        }
    }

    /// <summary>
    /// 버튼이 눌릴때 할것
    /// </summary>
    public void OnButtonPress()
    {
        if (movable == null)
        {
            callBack?.Invoke();
            return;
        }
        switch (movable.type)
        {
            case MoveType.DOOR:
                Door door = movable.GetComponent<Door>();

                if (door.isLock)
                {
                    door.SetLock(false);
                }
                break;
            default:
                movable.Active(isPress);    //할당 된 오브젝트의 Active 함수 호출
                break;
        }
        if (isPress)
        {
            movable.Plus();
        }
        else
        {
            movable.Minus();
        }
    }


    public void SetButtonCallBack(ButtonCallback _action)
    {
        callBack = _action;
    }

    /// <summary>
    /// 버튼이 활성화 될 때 기능
    /// </summary>
    /// <param name="_active"></param>
    public override void Active(bool _active)
    {
        gameObject.SetActive(_active);
        soundMgr.PlaySfx(transform, soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_SOLVE), Random.Range(1 - 0.2f, 1 + 0.2f));
    }

}
