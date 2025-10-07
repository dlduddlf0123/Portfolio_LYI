using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UI_AnimationTouch : MonoBehaviour, IPointerDownHandler
{
    GameManager gameMgr;
    UI_Animation ui_anim;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        ui_anim = gameMgr.uiMgr.ui_animation;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ui_anim.PlayCurrentAnimation();
    }


}
