using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXEvents : MonoBehaviour
{
    public AudioSource m_audio;
    public AudioClip sfx_walk;
    public AudioClip sfx_run;
    public AudioClip sfx_jump;

    private void Awake()
    {
        m_audio = GetComponent<AudioSource>();
    }

    public void Active(int _a)
    {
        gameObject.SetActive(true);
    }

    public void SFX_Walk()
    {
        m_audio.PlayOneShot(sfx_walk);
    }

    public void SFX_Run()
    {
        m_audio.PlayOneShot(sfx_run);
    }
    public void SFX_Jump()
    {
        m_audio.PlayOneShot(sfx_jump);
    }
}
