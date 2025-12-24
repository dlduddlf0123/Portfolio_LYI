using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// HandColl과 상호작용 가능한 오브젝트 들
/// 손과 오래 닿아 있으면 미리 설정된 이벤트를 작동시킨다.
/// /// </summary>
public class Selectable : MonoBehaviour
{
    public UnityEvent action;
}
