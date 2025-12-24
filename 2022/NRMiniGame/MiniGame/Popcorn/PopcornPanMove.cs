using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopcornPanMove : MonoBehaviour
{
    Rigidbody m_rigidbody;

    Vector3 targetRot;
    Vector3 lastRot;

    public float rotPower = 20f; //min 5 ~ max 20
    public float panSpeed = 1f; //min 0.5f ~ max 2
    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        StartCoroutine(PanMovePattern1());
    }


    /// <summary>
    /// 팬 움직임1 동그랗게 굴리기
    /// </summary>
    /// <returns></returns>
    IEnumerator PanMovePattern1()
    {
        int pointCount = 0; //max 4
        float t = 0;

        lastRot = Vector3.zero;
        targetRot = ChangeTargetRotation(pointCount);

        while (true)
        {
            t += 0.01f * panSpeed;
            m_rigidbody.rotation = Quaternion.Euler(Vector3.Lerp(lastRot, targetRot, t));

            if (t >= 1)
            {
                t = 0;

                pointCount++;
                if (pointCount > 3)
                {
                    pointCount = 0;
                }
                lastRot = targetRot;
                targetRot = ChangeTargetRotation(pointCount);
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    Vector3 ChangeTargetRotation(int targetNum)
    {
        switch (targetNum)
        {
            case 0:
                return new Vector3(-rotPower, 0, -rotPower);
            case 1:
                return new Vector3(-rotPower, 0, rotPower);
            case 2:
                return new Vector3(rotPower, 0, rotPower);
            case 3:
                return new Vector3(rotPower, 0, -rotPower);
            default:
                return Vector3.zero;
        }
    }
}
