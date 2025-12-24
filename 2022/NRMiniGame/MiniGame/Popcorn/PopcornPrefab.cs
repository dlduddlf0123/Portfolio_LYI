using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

/// <summary>
/// 팝콘, 손(불)과 닿으면 가열됨
/// 시간 뒤엔 이펙트와 함께 팝콘으로 변화
/// </summary>
public class PopcornPrefab : MonoBehaviour
{
    public UnityAction onPop;
    public UnityAction onDestroy;

    Material m_mat;

    public bool isHand = false; //손과 닿았을 때
    bool isPop = false; //팝콘 터졌는지

    float fireTime = 0f;
    float maxTime = 0.5f;

    float fireSpeed = 0.3f;

    //Popcorn
    public GameObject popCorn;
    public GameObject corn;

    Vector3 popForce = new Vector3(0.1f, 1f, 0.1f);
    private void Awake()
    {
        m_mat = corn.GetComponent<Renderer>().material;
        CornInit();
    }

    public void CornInit()
    {
        isHand = false;
        isPop = false;
        fireTime = 0f;
        m_mat.color = Color.white;

        corn.SetActive(true);
        popCorn.SetActive(false);
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Hit"))
        {
            onDestroy.Invoke();
        }
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            if (!isHand)
            {
                isHand = true;
            }
        }
    }
    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            isHand = false;
        }
    }

    private void Update()
    {
        if (isHand)
        {
            InFire();
        }
        else
        {
            OutFire();
        }
    }

    /// <summary>
    /// 불 안에 있을 때
    /// </summary>
    void InFire()
    {
        if (fireTime < maxTime)
        {
            fireTime += fireSpeed * Time.deltaTime;
            m_mat.color = new Color(1, 1- fireTime, 1- fireTime);
        }
        else
        {
            fireTime = maxTime;
            if (isPop)
            {
                return;
            }
            StartCoroutine(CornPop());
        }
    }

    /// <summary>
    /// 불 밖에 있을 때
    /// </summary>
    void OutFire()
    {
        if (fireTime > 0)
        {
            fireTime -= fireSpeed/2 * Time.deltaTime;
            m_mat.color = new Color(1, 1- fireTime, 1- fireTime);
        }
        else
        {
            fireTime = 0;
        }
    }

    public IEnumerator CornPop()
    {
        isPop = true;

        if (corn != null)
            corn.SetActive(false);

        GetComponent<AudioSource>().Play();
        GetComponent<AudioSource>().volume *= UnityEngine.Random.value;
        GetComponent<AudioSource>().pitch = .25f * UnityEngine.Random.value + .75f;

        GameManager.Instance.PlayParticleEffect(transform.position, GameManager.Instance.transform.GetChild(0).GetChild(2).gameObject);

        popCorn.SetActive(true);

        GetComponent<Rigidbody>().AddForce(
            UnityEngine.Random.Range(-popForce.x, popForce.x),
            UnityEngine.Random.Range(0.0f, popForce.y),
            UnityEngine.Random.Range(-popForce.z, popForce.z));

        yield return new WaitForSeconds(1f);

        onPop.Invoke();
    }
}
