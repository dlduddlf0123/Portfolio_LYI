using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movingplatform : Movable
{
    public enum LoopType
    {
        Once,
        PingPong
    }

    public LoopType loopType;
    
    public float duration = 1;
    public AnimationCurve accelCurve;

    public bool activate = false;

    [Range(0, 1)]
    public float previewPosition;
    float time = 0f;
    float position = 0f;
    float direction = 1f;

    Platform m_Platform;
    public new Rigidbody rigidbody;
    public Vector3 start = -Vector3.forward;
    public Vector3 end = Vector3.forward;

    void Awake()
    {
        m_Platform = GetComponentInChildren<Platform>();
        type = MoveType.NONE;
    }

    public void FixedUpdate()
    {
        if (activate)
        {
            time = time + (direction * Time.deltaTime / duration);
            switch (loopType)
            {
                case LoopType.Once:
                    LoopOnce();
                    break;
                case LoopType.PingPong:
                    LoopPingPong();
                    break;
            }
            PerformTransform(position);
        }
    }

    void LoopPingPong()
    {
        position = Mathf.PingPong(time,1f);
    }

    void LoopOnce()
    {
        position = Mathf.Clamp01(time);
        if(position >=1)
        {
            enabled = false;
            direction *= -1;
        }
    }

    public void PerformTransform(float position)
    {
        var curvePosition = accelCurve.Evaluate(position);
        var pos = transform.TransformPoint(Vector3.Lerp(start, end, curvePosition));
        Vector3 deltaPosition = pos - rigidbody.position;
        if (Application.isEditor && !Application.isPlaying)
            rigidbody.transform.position = pos;
        rigidbody.MovePosition(pos);

        if (m_Platform != null)
            m_Platform.MoveCharacterController(deltaPosition);
    }

    public override void Active(bool _active)
    {
        activate = _active;
    }
}
