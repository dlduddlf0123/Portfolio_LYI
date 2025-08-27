using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderMiddle : MonoBehaviour {

    Character fish;

    // Use this for initialization
    void Start () {
        fish = this.transform.GetComponentInParent<Character>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("ball")) { return; }
        print("MiddleHited!");

        //파티클 효과 재생

        if (fish.mSkin != null)
        {
            //머테리얼 변경
            switch (fish.Status.hp)
            {
                case 3:
                    fish.SetSkinTex(2);
                    break;
                case 2:
                    switch (fish.lastHit)
                    {
                        case 0:
                            fish.SetSkinTex(6);
                            break;
                        case 1:
                            fish.SetSkinTex(6);
                            break;
                        case 2:
                            fish.SetSkinTex(4);
                            break;
                        default:
                            break;
                    }
                    break;
                case 1:
                    fish.SetSkinTex(7);
                    break;
                default:
                    break;
            }
        }
        fish.lastHit = 1;
        this.gameObject.SetActive(false);

        fish.Hit(other);
    }
}
