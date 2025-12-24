using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public int foodNum;   //음식종류
    public int satiety;      //포만도
    public int taste;        //맛(취향)

    public HandInteract attachHand = null;
    public bool isAttach = false;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (isAttach)
            {
                FoodDetach();
                StartCoroutine(Wait());
            }
        }
        if (collision.gameObject.CompareTag("MainCamera"))
        {
            GameManager.Instance.soundMgr.PlaySfx(this.transform.position, GameManager.Instance.soundMgr.LoadClip("Sounds/SFX/eat_01"));
            FoodDetach();
            gameObject.SetActive(false);
        }
    }

    IEnumerator Wait()
    {
        this.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(1f);
        this.GetComponent<Collider>().enabled = true;
    }


    public void FoodDetach()
    {
        if (isAttach)
        {
            attachHand.DetachHand();
            attachHand = null;
            isAttach = false;
        }
    }
}
