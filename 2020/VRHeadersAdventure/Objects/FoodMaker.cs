using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 지정된 물체를 지정된 곳에 생성해 주는 클래스
/// +오브젝트풀링 내장
/// </summary>
public class FoodMaker : MonoBehaviour
{
    public GameObject food;
    public Transform makePos;

    public List<GameObject> list_Food = new List<GameObject>();

    float coolTime = 1f;

    float t = 0.0f;
    private void Update()
    {
        if (this.transform.childCount == 0)
        {
            t += Time.deltaTime;
            if (t > coolTime)
            {
                t = 0;
                MakeFood();
            }
        }
    }

    public void MakeFood()
    {
        //if (list_Food.Count > 0)
        //{
        //    list_Food[0].SetActive(true);
        //    list_Food[0].transform.position = makePos.position;
        //    list_Food[0].transform.SetParent(this.transform);
        //    list_Food[0].GetComponent<Rigidbody>().isKinematic = true;
        //}
        //else
        //{
            GameObject go = Instantiate(food, makePos);
            go.transform.position = makePos.position;
            go.GetComponent<Food>().throwable.onPickUp.AddListener(()=>
            {
                go.GetComponent<Rigidbody>().isKinematic = false;
                MakeFood();
            });
            list_Food.Add(go);
        
    }

}
