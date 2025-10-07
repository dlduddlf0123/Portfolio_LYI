using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleaner : MonoBehaviour
{
    float t = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHand hand = other.GetComponent<PlayerHand>();
            if (hand.isDirty)
            {
                if(t <= 1f)
                {
                    t += Time.deltaTime*0.3f;
                    hand.ChangeHandColor(
                        Color.Lerp(hand.handColor[3], hand.handColor[1], t),
                        Color.Lerp(hand.handColor[2], hand.handColor[0], t));
                    GetComponent<AudioSource>().volume = 1;
                }
                else
                {
                    t = 0;
                    hand.isDirty = false;
                    hand.ChangeHandColor(hand.handColor[1], hand.handColor[0]);
                    GameManager.Instance.HandEffect(hand, 1, ReadOnly.Defines.SOUND_SFX_GAUGEUP);
                    GetComponent<AudioSource>().volume = 0;
                }
            }
        }
    }


}
