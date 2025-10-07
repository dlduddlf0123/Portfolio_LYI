using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCharacter : MonoBehaviour
{
    Animator anim_cube;
    Animator anim_header;

    private void Awake()
    {
        anim_cube = GetComponent<Animator>();
        anim_header = transform.GetChild(0).GetComponent<Animator>();        
    }

    private void Start()
    {

    }

    public void PlayAnim(int animNum)
    {
        anim_header.SetFloat("TriggerNum", animNum);
        anim_header.SetTrigger("isTrigger");
    }

    public void RandomAnim()
    {
        anim_header.SetFloat("TriggerNum", Random.Range(0,9));
        anim_header.SetTrigger("isTrigger");
    }

    public void ImageFound()
    {
       // anim_cube.SetTrigger("tDetect");
        anim_header.SetFloat("TriggerNum", 3);
        anim_header.SetTrigger("isTrigger");
    }

    public void ImageLost()
    {
        //anim_cube.SetTrigger("tLost");
        anim_header.ResetTrigger("isTrigger");
    }

}
