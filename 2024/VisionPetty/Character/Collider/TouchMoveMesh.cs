using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchMoveMesh : MonoBehaviour
{
    public Transform bone_front;
    Vector3 bone_front_origin;
    public Transform bone_back;

    public Transform desireTr; //Vector3 계산용 Transform, 적용할 때에는 localPosition 사용
    public Transform contactTransform;

    public float pettingDistance = 0.1f; // 쓰다듬기 효과의 강도
    public float pettingLerpPercent = 10.0f; // 메시가 따라오는 속도

    public bool isPetting = false; // 쓰다듬기 효과 활성화 여부

    void Start()
    {
        bone_front_origin = bone_front.position;
    }


    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Hand"))
        {
            if (isPetting)
            {
                return;
            }
            isPetting = true; // 쓰다듬기 효과 활성화

            StopAllCoroutines();
            Debug.Log("Petting Start");
            contactTransform = coll.gameObject.transform;
            StartCoroutine(PettingEffect()); // 쓰다듬기 효과 코루틴 시작
        }

    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag("Hand"))
        {
            if (!isPetting)
            {
                return;
            }
            isPetting = false; // 쓰다듬기 효과 비활성화

            contactTransform = null;
            StopAllCoroutines();
            Debug.Log("Petting End");
            StartCoroutine(ResetMesh()); // 메시를 원래 상태로 되돌리는 코루틴 시작
        }
    }

    IEnumerator PettingEffect()
    {
        Transform parentTransform = bone_front.parent;
        bone_front_origin = bone_front.localPosition;
        Vector3 startPos , handPos;

        while (isPetting)
        {
            startPos = parentTransform.TransformPoint(bone_front_origin);
            handPos = FlattenY(contactTransform.position, startPos.y);

            Vector3 normalVec = (handPos - startPos).normalized;
            float distance = Vector3.Distance(handPos, startPos);

            if (distance > pettingDistance)
            {
                distance = pettingDistance;
            }

            Vector3 result = startPos + (normalVec * distance);
            bone_front.position = Vector3.Lerp(startPos, result, pettingLerpPercent);

            Debug.Log("Distance: " + distance);

            yield return null;
        }
    }

   Vector3 FlattenY(Vector3 pos, float y)
    {
        return new Vector3(pos.x, y, pos.z);
    }

    IEnumerator ResetMesh()
    {
        desireTr.SetParent(this.transform);

        float distance = Vector3.Distance(bone_front.localPosition, bone_front_origin);
        while (distance > 0.0003f)
        {
            bone_front.localPosition = Vector3.Lerp(bone_front.localPosition, bone_front_origin, pettingLerpPercent);
            distance = Vector3.Distance(bone_front.localPosition, bone_front_origin);
            yield return null;
        }

        bone_front.localPosition = bone_front_origin;
    }
}