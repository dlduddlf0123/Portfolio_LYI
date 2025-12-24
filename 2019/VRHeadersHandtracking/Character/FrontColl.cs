using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontColl : MonoBehaviour
{
    public Character header;
    SoundManager soundMgr;

    // Start is called before the first frame update
    void Awake()
    {
        soundMgr = GameManager.Instance.soundMgr;
        header = this.GetComponentInParent<Character>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            header.Stop();
            header.SetAnim(2);
            soundMgr.PlaySfx(this.transform.position, soundMgr.LoadClip("Sounds/SFX/jump_15"));
            GameManager.Instance.PlayEffect(this.transform.position, GameManager.Instance.particles[1]);
            header.LikeChange(-10);
            header.headerCanvas.ShowText(1, 0);
            header.StartCoroutine(header.BodyTouched());
        }
    }
}
