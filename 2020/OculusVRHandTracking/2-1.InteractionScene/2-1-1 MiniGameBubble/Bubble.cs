using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public MiniGameBubble bubbleMgr;

    public int destination;
    private bool isChecked;

    public Vector3 startPos;

    public ParticleSystem Effect;
    

    private void OnEnable()
    {
        BubbleInit();
    }
    private void OnDisable()
    {
        transform.position = startPos;
        isChecked = false;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, bubbleMgr.array[destination].position, Time.deltaTime*3);
        if(isChecked ==true)
        {
            if(Effect.isPlaying == false)
            {
                bubbleMgr.ReturnObject(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Check")) //벽에 닿았을때(놓쳤을때)
        {          
            Effect.Play();
            isChecked = true;
            bubbleMgr.combo = 0;
        }
        else if(other.gameObject.CompareTag("Player")) //손으로 맞췄을때
        {
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            bubbleMgr.GetScore(100);
            bubbleMgr.GetScore(10 * bubbleMgr.combo);
            bubbleMgr.combo++;
            Effect.Play();
            isChecked = true;
            GameManager.Instance.soundMgr.PlaySfx(transform, ReadOnly.Defines.SOUND_SFX_CLAP);
        }
    }

    public void BubbleInit()  //버블상태 초기화
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = true;
        transform.position = startPos;
    }
}
