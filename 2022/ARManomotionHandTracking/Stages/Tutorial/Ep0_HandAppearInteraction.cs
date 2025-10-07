using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ep0_HandAppearInteraction : InteractionManager
{
    public CanvasGroup grp_handGuide;
    Collider m_collider;

    bool isActive = false;
    // Start is called before the first frame update
    void Start()
    {
        m_collider = GetComponent<Collider>();
        list_guidePosition.Add(stageMgr.arr_header[0].transform.position + Vector3.up);
        grp_handGuide.alpha = 0;

        m_collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
           gameMgr.uiMgr.worldCanvas.StartTimer(stageMgr.arr_header[0].transform.position + Vector3.up, 1f,
                () => EndInteraction());

            gameMgr.handCtrl.handFollower.ToggleHandEffect(true);
            StopGuideParticle();

            if (!isActive)
            {
                StartCoroutine(AlphaLerp(false));
                isActive = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameMgr.uiMgr.worldCanvas.StopTimer();

            gameMgr.handCtrl.handFollower.ToggleHandEffect(false);
            PlayGuideParticle();

            gameMgr.uiMgr.ui_game.ChangeHandIcon(HandIcon.FRONT);
        }
    }

    IEnumerator AlphaLerp(bool _isAppear)
    {
        float _t = 0;

        if (_isAppear)
        {
            while (_t < 1)
            {
                _t += 0.03f;

                grp_handGuide.alpha = _t;
                yield return new WaitForSeconds(0.01f);
            }

        }
        else
        {
            _t = 1;
            while (_t > 0)
            {
                _t -= 0.03f;

                grp_handGuide.alpha = _t;
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    public override void StartInteraction()
    {
        base.StartInteraction();

        PlayGuideParticle();
        StartCoroutine(AlphaLerp(true));
        m_collider.enabled = true;
    }

    public override void EndInteraction()
    {
        StopGuideParticle();
        gameMgr.uiMgr.worldCanvas.StopTimer();
        m_collider.enabled = false;

        base.EndInteraction();
    }
}
