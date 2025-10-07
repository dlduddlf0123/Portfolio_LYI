using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPoint : MonoBehaviour
{
    SoundManager soundMgr;
    private void Awake()
    {
        soundMgr = GameManager.Instance.soundMgr;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Header"))
        {
            Character header = collision.gameObject.GetComponent<Character>();
            if (!header.isDie)
            {
                header.StartCoroutine(header.Respawn());
                soundMgr.PlaySfx(this.transform, soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_DIE));
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red * 0.75f;
        Gizmos.DrawCube(this.transform.position, GetComponent<BoxCollider>().size);
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
