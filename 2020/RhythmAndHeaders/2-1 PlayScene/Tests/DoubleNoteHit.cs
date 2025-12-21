using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleNoteHit : MonoBehaviour
{
    PlayManager ingameMgr;
    private void Awake()
    {
        ingameMgr = PlayManager.Instance;
    }
    private void OnEnable()
    {
        StartCoroutine(Timer());
        ingameMgr.player.attackMotion = 2;
        ingameMgr.player.AttackAnim();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Note"))
        {
            Note note;
            note = other.GetComponent<Note>();
            note.Judge(StateInput.DOUBLEHIT);
        }
    }
    public IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }
}
