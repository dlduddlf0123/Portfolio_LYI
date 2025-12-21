using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningTire : MonoBehaviour
{
    Gyroscope m_gyro;

    public TireRunInteract runMgr;
    Collider m_coll;

    public Animator m_anim;

    public int maxHP = 3;
    public int hp = 3;

    public float moveSpeed = 1f;

    public float deadLine = 3f;
    public float deadRot = 30f;

    Vector3 gyroStandard;

    private void Awake()
    {
        m_coll = GetComponent<Collider>();
        m_gyro = Input.gyro;
        m_gyro.enabled = true;
    }

    private void Start()
    {
        m_anim.SetBool("isRoll", true);
    }

    public void ResetGyro()
    {
        gyroStandard = m_gyro.rotationRateUnbiased;

        Debug.Log("GyroReset:" + gyroStandard);
    }

    public void Update()
    {
        runMgr.uiCanvas.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = "Gyro: " + m_gyro.rotationRateUnbiased;

        if (m_gyro.rotationRateUnbiased.z < gyroStandard.z - 0.1f)
        {
            MoveLeft();
        }
        else if ( m_gyro.rotationRateUnbiased.z > gyroStandard.z + 0.1f)
        {
            MoveRight();
        }

        if (Input.GetAxis("Horizontal") > 0)
        {
            MoveLeft();
        }
        else if(Input.GetAxis("Horizontal") < 0)
        {
            MoveRight();
        }
        else
        {
            Stay();
        }
    }

    public void MoveRight()
    {
        if (transform.localPosition.x < deadLine)
        {
            transform.localPosition += Vector3.right * moveSpeed * Time.deltaTime;
        }
        if (transform.localRotation.eulerAngles.y > 180 - deadRot)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 180 - deadRot, 0), Time.deltaTime * 2);
        }
    }

    public void MoveLeft()
    {
        if (transform.localPosition.x > -deadLine)
        {
            transform.localPosition -= Vector3.right * moveSpeed * Time.deltaTime;
        }
       
        if (transform.localRotation.eulerAngles.y < 180 + deadRot)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 180 + deadRot, 0), Time.deltaTime * 2);
        }

    }

    void Stay()
    {
        if (transform.localRotation.y != 180)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 180, 0), Time.deltaTime * 2);
        }
    }


    public void GetHit()
    {
        Debug.Log("!!Hit!!");
        hp -= 1;
        runMgr.SetHeartUI(hp);
        Debug.Log("HP: " + hp);

        if (hp <= 0)
        {
            runMgr.GameOver();
            return;
        }
        //체력감소 이펙트, 깜빡임 효과, UI


        StartCoroutine(HitEffect());

    }

    //깜빡이기
    protected IEnumerator HitEffect()
    {
        m_coll.enabled = false;

        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < 2; i++)
            {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < 2; i++)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(1).gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(0.1f);
        }

        m_coll.enabled = true;
    }

}
