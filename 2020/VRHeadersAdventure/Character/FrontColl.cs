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
            soundMgr.PlaySfx(this.transform, soundMgr.LoadClip("jump_15"));
            header.PlayEffect(other.transform.position, this.gameObject);
            header.headerCanvas.ShowText(1, 0);
            header.LikeChange(-10);
            header.currentAI = StartCoroutine(header.Touched(this.GetComponent<Collider>()));
        }
    }
}
