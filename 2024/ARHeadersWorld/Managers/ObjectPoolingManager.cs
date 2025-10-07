using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;


/// <summary>
/// 전체 스크립트에서 오브젝트 풀링 관련 구현을 담당한다
/// 범용성있게 제작, 다양한 기능을 넣을 수 있도록 할 것
/// </summary>
public class ObjectPoolingManager : MonoBehaviour
{
    #region GameObject Management

    /// <summary>
    /// 4/13/2023-LYI
    /// List형태로 오브젝트 검사
    /// 리스트에 이미 오브젝트가 있는지 검사 추가(적 미사일 때문)
    /// 있으면 같은 타입 오브젝트로 생성
    /// EnemySpawner 참조
    /// </summary>
    /// <param name="list"></param>
    /// <param name="originGo"></param>
    /// <param name="pos"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject CreateObject(List<GameObject> list, GameObject originGo, Vector3 pos, Transform parent)
    {
        GameObject go = null;


        list.RemoveAll(item => item == null);

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].name == originGo.name + "(Clone)" &&
            list[i].activeSelf == false)
            {
                go = list[i];
                list.Remove(list[i]);
                break;
            }
        }

        if (go == null)
        {
            go = Instantiate(originGo);
        }

        //if (list.Count > 0 &&
        //    list[0].activeSelf == false)
        //{
        //    go = list[0];
        //    list.RemoveAt(0);
        //}
        //else
        //{
        //    go = Instantiate(originGo);
        //}

        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.gameObject.SetActive(true);

        return go;
    }
    public GameObject CreateObject(Queue<GameObject> queue, GameObject originGo, Vector3 pos, Transform parent)
    {
        GameObject go;

        //4/13/2023-LYI
        //이미 활성화 된 오브젝트를 가져오지 않도록 수정
        if (queue.Count > 0 &&
            queue.Peek().activeSelf == false)
        {
            go = queue.Dequeue();
        }
        else
        {
            go = Instantiate(originGo);
        }

        go.transform.position = pos;
        go.transform.SetParent(parent);
        go.gameObject.SetActive(true);

        return go;
    }


    /// <summary>
    /// 해당 리스트에 그 오브젝트를 다시 배치, 비활성화
    /// </summary>
    /// <param name="list"></param>
    /// <param name="go"></param>
    public void ObjectInit(List<GameObject> list, GameObject go, Transform parent)
    {
        go.transform.SetParent(parent);
        list.Add(go);
        go.SetActive(false);
    }
    public void ObjectInit(Queue<GameObject> queue, GameObject go, Transform parent)
    {
        go.transform.SetParent(parent);
        queue.Enqueue(go);
        go.SetActive(false);
    }

    /// <summary>
    /// 잠시 후에 오브젝트 초기화 진행
    /// </summary>
    /// <param name="list"></param>
    /// <param name="go"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator LateInit(List<GameObject> list, GameObject go, float time, Transform parent)
    {
        yield return new WaitForSeconds(time);
        ObjectInit(list, go, parent);
    }
    public IEnumerator LateInit(Queue<GameObject> queue, GameObject go, float time, Transform parent)
    {
        yield return new WaitForSeconds(time);
        ObjectInit(queue, go, parent);
    }
    #endregion

    #region General Type
    public void PlayParticle(List<GameObject> list, GameObject origin, Vector3 pos, Transform active, Transform disable, UnityAction action = null)
    {
        GameObject go = CreateObject(list, origin, pos, active);


        if (action != null)
        {
            StartCoroutine(ParticleAction(go.GetComponent<ParticleSystem>(), action));
        }

        StartCoroutine(LateInit(list, go, 2f, disable));
    }

    public IEnumerator ParticleAction(ParticleSystem particle, UnityAction action)
    {
        yield return new WaitForSeconds(particle.main.duration);

        action.Invoke();
    }

    #endregion


}