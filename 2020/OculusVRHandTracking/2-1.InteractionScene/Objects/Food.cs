using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FoodType
{
    NONE,
    FRUIT,
    GRASS,
    MEAT,
    EVERYTHING,
}

public class Food : Item
{
    public FoodType typeFood;  //음식종류
    public int satiety;      //포만도
    public int taste;        //맛(취향)

    public bool isDirty = false;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerHand>().isDirty && !isDirty)
            {
                isDirty = true;
                typeFood = FoodType.MEAT;
                GetComponentInChildren<MeshRenderer>().material.color = new Color(0.3f,0.15f,0.05f,1f);
                GameManager.Instance.HandEffect(collision.GetComponent<PlayerHand>());
            }
            if (isAttach)
            {
                FoodDetach();
                StartCoroutine(Wait());
            }
        }
        if (collision.gameObject.CompareTag("MainCamera"))
        {
            GameManager.Instance.soundMgr.PlaySfx(this.transform, ReadOnly.Defines.SOUND_SFX_EAT);
            transform.parent = null;
            FoodDetach();
            gameObject.SetActive(false);
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isThrowing)
            {
                StartCoroutine(Destroy(30));
                stageMgr.interactHeader.MoveCharacter(transform.position, gameObject);
            }
            else
            {
                StartCoroutine(Destroy(10));
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            StopAllCoroutines();
        }
    }

    IEnumerator Destroy(int _time)
    {
        yield return new WaitForSeconds(_time);
        Destroy(this.gameObject);
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
           // attachHand.DetachHand();
            attachHand = null;
            isAttach = false;
        }
    }
}
