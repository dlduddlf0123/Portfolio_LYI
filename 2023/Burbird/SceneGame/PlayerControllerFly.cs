using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using BoingKit;

namespace Burbird
{
    public class PlayerControllerFly : PlayerController2D
    {
        StageManager stageMgr;

        //속도 관련
        public float fireTime = 0.3f;
        public float fireTic = 0f;

        public Enemy fireTarget = null;

        private void Awake()
        {
            stageMgr = StageManager.Instance;

            m_rigidbody = GetComponent<Rigidbody2D>();
            m_animator = transform.GetChild(0).GetComponent<Animator>();
            m_spriteAnim = transform.GetChild(0).GetChild(1).GetComponent<Animator>();


            Init();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            Move();
            MobileJoystickInput();

            //AutoFire();
        }


        #region Player Actions
        public override void Move()
        {
            Vector2 _velocity = m_rigidbody.velocity;

            // Apply acceleration directly as we'll want to clamp
            // prior to assigning back to the body.
            _velocity += input_move * moveSpeed * Time.fixedDeltaTime;

            // Clamp horizontal speed.
            _velocity.x = Mathf.Clamp(_velocity.x, -maxSpeed, maxSpeed);
            _velocity.y = Mathf.Clamp(_velocity.y, -maxSpeed, maxSpeed);

            if (isSprint)
            {
                maxVelocity = 5;
            }
            if (!isSprint)
            {
                maxVelocity = 2;
            }

            if (_velocity.x > maxVelocity)
            {
                _velocity.x = maxVelocity;
            }
            if (_velocity.x < -maxVelocity)
            {
                _velocity.x = -maxVelocity;
            }
            if (_velocity.y > maxVelocity)
            {
                _velocity.y = maxVelocity;
            }
            if (_velocity.y < -maxVelocity)
            {
                _velocity.y = -maxVelocity;
            }

            //if (Mathf.Abs(_velocity.x) >= maxVelocity && !isSprint)
            //{
            //    isSprint = true;
            //}

            // Assign back to the body.
            //m_rigidbody.velocity = _velocity;
            transform.position += (Vector3)_velocity;
            m_animator.SetFloat("MoveSpeed", Mathf.Abs(_velocity.SqrMagnitude()));
            m_animator.SetFloat("fHeight", m_rigidbody.velocity.y);
        }

        //IEnumerator coJumpHeight()
        //{
        //    Vector3 _startVec = transform.position;
        //    float _height = 0f;
        //    while (!isGround)
        //    {
        //        _height = transform.position.y - _startVec.y;
        //        m_animator.SetFloat("fHeight", _height);

        //        yield return new WaitForSeconds(0.02f);
        //    }
        //}

        /// <summary>
        /// 시간 체크
        /// 쿨타임마다 발사 준비
        /// 일정 거리 안쪽에 적들이 들어올 경우 발사
        /// </summary>
        //public void AutoFire()
        //{
        //    if (fireTic < fireTime)
        //    {
        //        fireTic += Time.deltaTime;
        //    }
        //    else if(fireTic > fireTime)
        //    {
        //        CheckEnemy();
        //    }

        //}

        //void CheckEnemy()
        //{
        //    float currentDist = 0.0f;
        //    float targetDist = 100.0f;

        //    for (int i = 0;  i < stageMgr.enemySpawner.list_active.Count; i++)
        //    {
        //        currentDist = Vector3.Distance(transform.position, stageMgr.enemySpawner.list_active[i].transform.position);
        //        if (fireTarget != null)
        //        {
        //            targetDist = Vector3.Distance(transform.position, fireTarget.transform.position);
        //        }

        //        if (currentDist < targetDist)
        //        {
        //            fireTarget = stageMgr.enemySpawner.list_active[i];
        //        }
        //    }

        //    if (fireTarget != null)
        //    {
        //        Fire(Vector3.Normalize(fireTarget.centerTr.position - centerTr.position), minFirePower);
        //        fireTic = 0;
        //    }
        //}


        //public override void Fire(Vector2 _fireVec, float _firePower)
        //{
        //    if (_firePower < minFirePower)
        //    {
        //        return;
        //    }
        //    if (currentFeatherCount <= 0)
        //    {
        //        //가시 없음 모션
        //        return;
        //    }
        //    if (_firePower > maxFirePower)
        //    {
        //        _firePower = maxFirePower;
        //    }
        //    Feather _feather;
        //    if (list_feather.Count == 0)
        //    {
        //        //발사 위치에 가시 생성(오브젝트 풀링)
        //        int rand = Random.Range(0, arr_featherOrigin.Length);
        //        _feather = Instantiate(arr_featherOrigin[rand], featherPool).GetComponent<Feather>();
        //        _feather.transform.position = centerTr.position;

        //        //float angle = Mathf.Atan2(_fireVec.y, _fireVec.x) * Mathf.Rad2Deg;
        //        //_feather.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //        _feather.player = this;
        //    }
        //    else
        //    {
        //        _feather = list_feather[0];
        //        list_feather.RemoveAt(0);

        //        _feather.transform.position = centerTr.position;

        //        //float angle = Mathf.Atan2(_fireVec.y, _fireVec.x) * Mathf.Rad2Deg;
        //        //_feather.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //        _feather.gameObject.SetActive(true);
        //    }

        //    //가시 발사
        //    _feather.StartCoroutine(_feather.FeatherMove(_fireVec, _firePower));
        //    //_feather.GetComponent<Rigidbody2D>().AddForce(_fireVec * _firePower * _feather.featherSpeed);

        //    //현재 가시 카운트 감소
        //    //currentFeatherCount--;
        //    //Debug.Log("Fire Feather: " + currentFeatherCount);

        //    //가시 개수에 따른 외형 변경
        //    //ChangeFeatherState();
        //}
        #endregion


        #region Mobile Input Actions
        /// <summary>
        /// 조이스틱 입력값
        /// </summary>
        public override void MobileJoystickInput()
        {
            if (InputSystem.devices[0].IsPressed())
            {
                return;
            }

            input_move = variableJoystick.Direction;

            if (Mathf.Abs(input_move.x) > 0.5f)
            {
                isSprint = true;
            }
            else
            {
                isSprint = false;
            }

            if (input_move != Vector2.zero)
            {
                if (input_move.x > 0)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                    isLeft = false;
                }
                else if (input_move.x < 0)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                    isLeft = true;
                }
                m_animator.SetBool("isMove", true);
            }
            else
            {
                m_animator.SetBool("isMove", false);
            }

            //if (variableJoystick.Vertical > 0.6f)
            //{
            //    if (!isJoystickY)
            //    {
            //        Jump();
            //    }
            //    isJoystickY = true;

            //}
            //else
            //{
            //    isJoystickY = false;
            //}
        }

        public void MoveUp()
        {
            input_move = Vector2.up;
            m_animator.SetBool("isMove", true);
            //Debug.Log("Move:" + input_move);
        }
        public void MoveDown()
        {
            input_move = Vector2.down;
            m_animator.SetBool("isMove", true);
            //Debug.Log("Move:" + input_move);
        }
        #endregion



    }
}