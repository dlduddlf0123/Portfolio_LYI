using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingFruit : MonoBehaviour
{
    public MiniGameBasket basket;

    Rigidbody m_rigidbody;
    public float gravity = 0;

    public float randomGravityMin = -9.7f;
    public float randomGravityMax = -9f;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        gravity = Random.Range(-9.7f, -9);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Init()
    {
        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.isKinematic = false;
        GetComponent<Collider>().enabled = true;
        basket.q_apple.Enqueue(this.gameObject);

        gameObject.SetActive(false);
        gravity = Random.Range(-8, 0);
    }


    IEnumerator Timer(float _time)
    {
        yield return new WaitForSeconds(_time);

        Init();
    }

    private void FixedUpdate()
    {
        m_rigidbody.AddForce(Vector3.down * gravity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(Timer(3f));

        }
        if (collision.gameObject.CompareTag("Basket"))
        {
            //스코어 획득, 바구니에 쌓이고 리지드바디 비활성화
            //gameObject.tag = "Basket";
            transform.SetParent(collision.gameObject.transform);
            basket.AddApple(gameObject);

            basket.GetScore(100);
        }
    }
}
