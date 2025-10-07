using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRTokTok.Interaction.Bridge
{
    public class Bridge_Glass : MonoBehaviour
    {
        public GameObject glass_safe;
        public GameObject glass_break;

        public Material mat_glass;
        public Material mat_correct;

        public AudioSource audio_crash;

        public bool isSafe = false;


        // Start is called before the first frame update
        void Start()
        {

        }

        public void GlassInit()
        {
            glass_break.GetComponent<ShatterableGlass>().Init();
        }

        public void GlassSelect()
        {
            glass_safe.GetComponent<Renderer>().material = mat_correct;
            glass_break.GetComponent<Renderer>().material = mat_correct;
        }
        public void GlassDeselect()
        {
            glass_safe.GetComponent<Renderer>().material = mat_glass;
            glass_break.GetComponent<Renderer>().material = mat_glass;
        }

        public void SetGlassSafe(bool isSafe)
        {
            this.isSafe = isSafe;

            if (isSafe)
            {
                glass_break.SetActive(false);
                glass_safe.SetActive(true);
            }
            else
            {
                glass_safe.SetActive(false);
                glass_break.SetActive(true);
            }
        }


        public void OnGlassCrash()
        {
            GameManager.Instance.soundMgr.PlaySfx(transform.position, audio_crash.clip);
        }

    }
}