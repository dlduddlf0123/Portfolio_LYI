using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Timeline;
using UnityEngine.Playables;

public class HipsongController : MonoBehaviour
{
     PlayableDirector[] arr_director;

    private void Awake()
    {
        arr_director = GetComponentsInChildren<PlayableDirector>();
    }

    public void OnHipsongStart()
    {
        for (int i = 0; i < arr_director.Length; i++)
        {
            arr_director[i].Play();
        }
    }
    public void OnHipsongEnd()
    {
        for (int i = 0; i < arr_director.Length; i++)
        {
            arr_director[i].Stop();
        }
    }

}
