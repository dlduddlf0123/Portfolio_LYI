using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AroundEffect
{

    public enum ParticleShotType
    {
        NONE = -1,
        HEART = 0,
        SELECT,
    }
    public enum ParticleLoopType
    {
        NONE = -1,
        DIRTY= 0,
    }

    /// <summary>
    /// 11/5/2024-LYI
    /// 캐릭터 파티클 관리 클래스
    /// </summary>
    public class CharacterParticle : MonoBehaviour
    {
        GameManager gameMgr;

        public CharacterManager charMgr;

        //array와 enum 연동
        public ParticleSystem[] arr_shotParticle;
        public ParticleSystem[] arr_loopParticle;

        private void Awake()
        {

        }

        public void Init()
        {

        }

        public void Stop()
        {
            for (int i = 0; i < arr_shotParticle.Length; i++)
            {
                arr_shotParticle[i].Stop();
            }

            for (int i = 0; i < arr_loopParticle.Length; i++)
            {
                arr_loopParticle[i].Stop();
            }

            Debug.Log(charMgr.Status.typeHeader.ToString() + "- Particle All Stop()");

        }


        public void PlayParticleOneShot(ParticleShotType type)
        {
            if (arr_shotParticle[(int)type] == null)
            {
                Debug.Log(charMgr.Status.typeHeader.ToString() + "- Particle Missing : " + type.ToString());
            }

            arr_shotParticle[(int)type].gameObject.SetActive(true);
            arr_shotParticle[(int)type].Play();
            Debug.Log(charMgr.Status.typeHeader.ToString() + "- Particle Play : " + type.ToString());

        }
        public void PlayParticleLoop(ParticleLoopType type)
        {
            if (arr_loopParticle[(int)type] == null)
            {
                Debug.Log(charMgr.Status.typeHeader.ToString() + "- Particle Missing : " + type.ToString());
            }
            arr_loopParticle[(int)type].gameObject.SetActive(true);
            arr_loopParticle[(int)type].Play();
            Debug.Log(charMgr.Status.typeHeader.ToString() + "- Particle Play : " + type.ToString());

        }

        public void StopParticleLoop(ParticleLoopType type)
        {
            if (arr_loopParticle[(int)type] == null)
            {
                Debug.Log(charMgr.Status.typeHeader.ToString() + "- Particle Missing : " + type.ToString());
            }
            arr_loopParticle[(int)type].Stop();
            Debug.Log(charMgr.Status.typeHeader.ToString() + "- Particle Stop : " + type.ToString());

        }


    }
}