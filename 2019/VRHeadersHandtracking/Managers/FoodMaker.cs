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

    List<GameObject> list_Food;

    private void Awake()
    {
        list_Food = new List<GameObject>();
    }

    private void Update()
    {
        if (this.transform.childCount == 0)
        {
            MakeFood();
        }
    }

    public void MakeFood()
    {
        GameObject go = Instantiate(food, makePos);
        go.transform.position = makePos.position;
    }

}
