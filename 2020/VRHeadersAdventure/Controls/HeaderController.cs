using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HeaderControl
/// </summary>
public class HeaderController : MonoBehaviour
{
    //PlatformerControl
    //public CharacterController mCharCtrl;
    Character header;
    Rigidbody mRigidbody;

    public Vector3 input;
    private float currentSpeed = 0;
    public float walkSpeed = 2;
    public float runSpeed = 4;
    public float jumpPower = 50f;
    public float jumpingPower = 0.3f;
    public float dragPower = 3f;

    public float specialDashPower = 3f;//지노라 특수

    public bool isGround = true;
    public bool isMove = false;

    [SerializeField]
    private float m_MovingTurnSpeed = 360;
    [SerializeField]
    private float m_StationaryTurnSpeed = 180;
    private float turnAmount;
    private float forwardAmount;

    float jumpSpeed;
    float jumpTimer;
    float airReduceTime = 0f;

    private void Awake()
    {
        header = GetComponent<Character>();
        mRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!header.isPlatfomer)
        {
            header.mAnimator.SetFloat("Speed", header.mNavAgent.speed);
        }
        else
        {
            jumpTimer -= Time.deltaTime;
        }
        if (!isGround && !isMove)
        {
            airReduceTime += Time.deltaTime * 0.1f;
            mRigidbody.velocity = Vector3.Lerp(mRigidbody.velocity, new Vector3(0, mRigidbody.velocity.y, 0),airReduceTime);
        }
        else
        {
            airReduceTime = 0;
        }
    }

    #region PlatfomerMoves
    public void Move(Vector3 _move, bool _run)
    {
        if (_run &&
            !header.isDie)
        {
            input = _move;
            currentSpeed = runSpeed;
            Debug.Log(mRigidbody.velocity.sqrMagnitude);
            if (isGround)
            {
                mRigidbody.velocity = new Vector3(
                    input.normalized.x * currentSpeed,
                    mRigidbody.velocity.y,
                    input.normalized.z * currentSpeed);
            }
            else
            {
                currentSpeed = 0.1f;
                if (mRigidbody.velocity.sqrMagnitude < runSpeed * 8f)
                {
                    mRigidbody.velocity += new Vector3(
                        input.normalized.x * currentSpeed,
                        0,
                        input.normalized.z * currentSpeed);
                }

            }
            
            if (_move.magnitude > 1f)
                _move.Normalize();

            _move = transform.InverseTransformDirection(_move);
            turnAmount = Mathf.Atan2(_move.x, _move.z);
            forwardAmount = _move.z;

            ApplyExtraTurnRotation();
            isMove = true;
        }
        else
        {
            isMove = false;
        }

        header.mAnimator.SetFloat("Speed", mRigidbody.velocity.sqrMagnitude);
    }


    private void FixRotation()
    {
        Vector3 eulers = transform.eulerAngles;
        eulers.x = 0;
        eulers.z = 0;
        Quaternion targetRotation = Quaternion.Euler(eulers);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * (isGround ? 20 : 3));
    }

    private void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, forwardAmount);
        transform.Rotate(0, turnAmount * turnSpeed * 8 * Time.deltaTime, 0);
    }

    public void Jump()
    {
        isGround = false;
        jumpTimer = 0.3f;
        header.mAnimator.SetBool("isGround", isGround);
        header.mAnimator.SetTrigger("isJump");

        float _pitch = 0.3f;
        GameManager.Instance.soundMgr.PlaySfx(header.transform, header.sfx_jump, Random.Range(1 - _pitch, 1 + _pitch));
        jumpSpeed = jumpPower;
        Vector3 v  = mRigidbody.velocity;
        v.y = jumpSpeed;

        mRigidbody.drag = 0;
        mRigidbody.AddForce(v);
    }
    public void Jumping()
    {
        mRigidbody.AddForce(Vector3.up * jumpingPower);
    }
    #endregion

    public IEnumerator Dash()
    {
        float s = runSpeed;
        runSpeed *= specialDashPower;
        yield return new WaitForSeconds(0.2f);
        runSpeed = s;
    }

    private void OnTriggerStay(Collider coll)
    {
        if (jumpTimer < 0)
        {
            isGround = true;
            mRigidbody.drag = dragPower;
            header.mAnimator.SetBool("isGround", isGround);
        }
        else
        {
            jumpTimer -= Time.deltaTime;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        isGround = false;
        header.mAnimator.SetBool("isGround", isGround);

    }
}
