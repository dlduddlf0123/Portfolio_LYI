using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum HeaderType
{
    NONE = 0,
    KANTO,
    ZINO,
    OODADA,
    COCO,
    DOINK,
    TENA,
}

/// <summary>
/// 12/18/2023-LYI
/// 캐릭터 포지션, 애니메이션 통제 스크립트
/// </summary>
public class WorldCharacterController : MonoBehaviour
{
    public HeaderType typeHeader;
    public Animator m_animator;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
