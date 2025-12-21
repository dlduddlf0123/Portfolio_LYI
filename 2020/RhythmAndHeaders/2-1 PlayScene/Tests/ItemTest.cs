using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTest : MonoBehaviour
{
    PlayManager ingameMgr;
    private void Awake()
    {
        ingameMgr = PlayManager.Instance;
    }
    private void OnEnable()
    {
        StartCoroutine(Timer());
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Note"))
        {
            Note note;
            note = other.GetComponent<Note>();
            ingameMgr.PopNote();
            //note.Judge(note.checkPosition.position);
            Instantiate(ingameMgr.particles[5], other.gameObject.transform.position, Quaternion.identity);
            other.gameObject.SetActive(false);
        }
        
    }
    public IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }
}
