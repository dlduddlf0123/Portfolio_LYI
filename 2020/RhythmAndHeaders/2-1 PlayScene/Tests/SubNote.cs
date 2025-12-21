using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubNote : MonoBehaviour
{
    PlayManager ingameMgr;
    public bool isLastnote = false;
    // Start is called before the first frame update
    private void Awake()
    {
        ingameMgr = PlayManager.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pass")&&isLastnote==true)
        {
            ingameMgr.Pass();
            this.gameObject.transform.parent.gameObject.SetActive(false);
            ingameMgr.PopNote();
        }
        if(other.gameObject.CompareTag("Player"))
        {
            if(isLastnote==true)
            {
                ingameMgr.player.mAnimator.SetBool("LongNoteMove", false);
                ingameMgr.isLongNote = false;
            }
            this.gameObject.SetActive(false);
        }
    }
}
