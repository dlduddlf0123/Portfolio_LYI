using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class PlayerAnimationEvent : MonoBehaviour
    {
        StageManager stageMgr;

        public UnityEngine.Audio.AudioMixerGroup mixerGroup;
        public AudioClip sfx_walk;
        public AudioClip sfx_run;

        private void Awake()
        {
            stageMgr = StageManager.Instance;
        }
        public void PlayWalkSound()
        {
            stageMgr.soundMgr.PlaySfx(transform.position, sfx_walk, Random.Range(0.7f, 1.4f), 1, mixerGroup);
        }
        public void PlayRunSound()
        {
            stageMgr.soundMgr.PlaySfx(transform.position, sfx_run, Random.Range(0.7f, 1.4f), 1, mixerGroup);
        }
    }
}