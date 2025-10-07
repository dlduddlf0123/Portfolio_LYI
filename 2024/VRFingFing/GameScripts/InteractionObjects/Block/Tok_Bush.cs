using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 6/21/2024-LYI
/// 부시 장식 랜덤
/// </summary>
public class Tok_Bush : MonoBehaviour
{
    public Transform topAnchor;
    public List<GameObject> list_bodyObjects = new();
    public List<GameObject> list_topObjects = new();


    public float headActivePercent = 20f;
    public float bodyRandomPercent = 30f;

    bool isBodyChanged = false;

    //활성화 시 한 번만
    private void Start()
    {
        RandomBody();
        RandomHead();
    }

    /// <summary>
    /// 6/21/2024-LYI
    /// 몸통 비주얼 랜덤
    /// </summary>
    public void RandomBody()
    {
        if (list_bodyObjects.Count == 0)
        {
            return;
        }

        float random = Random.Range(0.0f, 100.0f);

        //확률 설정
        if (random <= bodyRandomPercent)
        {
            isBodyChanged = true;

            int bodyRandom = Random.Range(0, list_bodyObjects.Count);
            GameObject go = null;
            for (int i = 0; i < list_bodyObjects.Count; i++)
            {
                list_bodyObjects[i].SetActive(false);
                if (i == bodyRandom)
                {
                    go = list_bodyObjects[i];
                }
            }

            if (go != null)
            {
                go.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 6/21/2024-LYI
    /// 랜덤 장식 결정 함수
    /// </summary>
    public void RandomHead()
    {
        if (isBodyChanged ||
            list_topObjects.Count == 0)
        {
            return;
        }

        float random = Random.Range(0.0f, 100.0f);

        topAnchor.gameObject.SetActive(false);
        //확률 설정
        if (random <= headActivePercent)
        {
            topAnchor.gameObject.SetActive(true);

            int topRandom = Random.Range(0, list_topObjects.Count);

            GameObject go = null;
            for (int i = 0; i < list_topObjects.Count; i++)
            {
                list_topObjects[i].SetActive(false);
                if (i == topRandom)
                {
                    go = list_topObjects[i];
                }
            }

            if (go != null)
            {
                go.SetActive(true);
                go.transform.localRotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
            }

        }
    }



}
