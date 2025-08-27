using UnityEngine;
using System.Collections;
using Rabyrinth.ReadOnlys;

public class ErekiBall : MonoBehaviour
{

    private float speed = 500;

    public int damage { get; set; }

    private Rigidbody rig;

    private GameManager GameMgr;

    void Awake()
    {
        GameMgr = MonoSingleton<GameManager>.Inst;
        rig = GetComponent<Rigidbody>();
    }

    public void Use(Vector3 pos, Vector3 target, int _damage)
    {
        damage = _damage;
        StopAllCoroutines();
        transform.position = pos;
        gameObject.SetActive(true);

        transform.LookAt(target);
        transform.Rotate(Vector3.up, 180.0f);
        rig.velocity = Vector3.zero;
        rig.AddForce((target - transform.position).normalized * speed);

        StartCoroutine(WaitBoom());
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag(Defines.TAG_PLAYER))
        {
            GameMgr.Player.TakeDamage(damage, Rabyrinth.ReadOnlys.HitEffect.Elect);
            //StartCoroutine(SetParticle(transform.position, 0.6f));
            gameObject.SetActive(false);
        }
    }

    private IEnumerator WaitBoom()
    {
        yield return new WaitForSeconds(3.0f);
        rig.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}