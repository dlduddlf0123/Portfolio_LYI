using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 돌멩이와 나무가 일정시간이 되면 없어지게 하는 코드
/// </summary>
public class Timer : MonoBehaviour
{
    Spawner spawner;
    public float lifeTime = 5f;
    public float currentTime = 0f;

    private void Awake()
    {
        spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
    }

    void Update()
    {
        if (GetTimer() > lifeTime)
        {
            SetTimer();
            if (this.gameObject.CompareTag("rock"))
            {
                spawner.PushToPool(spawner.list_Rock, gameObject);
                this.gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
            }
            else if (this.gameObject.CompareTag("wood"))
            {
                spawner.PushToPool(spawner.list_Wood, gameObject);
                this.gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
            }
        }
        
    }
    float GetTimer()
    {
        return (currentTime += Time.deltaTime);
    }

    void SetTimer()
    {
        currentTime = 0f;
    }



}
