using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackColl : MonoBehaviour
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
            PlayerHand hand = other.GetComponent<PlayerHand>();

            soundMgr.PlaySfx(this.transform.position, soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_CLICK));
            GameManager.Instance.PlayEffect(this.transform.position, GameManager.Instance.particles[1]);

            if (hand.isDirty)
            {
                header.Stop();
                header.currentAI = header.StartCoroutine(header.HateSmell());
                return;
            }
            else if (hand.isHit)
            {
                header.headerCanvas.ShowText(4, Random.Range(0, 2));
                header.AI_Move(1);
                return;
            }
            else
            {
                header.Stop();
                switch (header.statLike)
                {
                    case LikeState.HATE:
                        header.currentAI = header.StartCoroutine(header.BackWalk());
                        break;
                    case LikeState.NORMAL:
                        header.PlayTriggerAnimation(0);
                        break;
                    case LikeState.FRIEND:
                        header.PlayTriggerAnimation(0);
                        break;
                }

                header.LikeChange(5);
                header.headerCanvas.ShowText(3, Random.Range(0, 2));
            }

            header.StartCoroutine(header.AfterTouch(3));
        }

        if (other.CompareTag("Brush"))
        {
            if (!other.GetComponent<OVRGrabbable>().isGrabbed)
            {
                return;
            }

            Debug.Log("BackBrush!");

            if (other.GetComponent<Brush>().brushCount > 2)
            {
                header.AI_Move(6);
                other.GetComponent<Brush>().brushCount = 0;
            }
            else
            {
                other.GetComponent<Brush>().brushCount++;
                header.Stop();
                header.PlayTriggerAnimation(0);
                header.headerCanvas.ShowText(9, Random.Range(0, 2));
            }


            soundMgr.PlaySfx(this.transform.position, soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_BRUSH));
            GameManager.Instance.PlayEffect(this.transform.position, GameManager.Instance.particles[1]);
            header.StartCoroutine(header.AfterTouch(3));
        }
    }
}
