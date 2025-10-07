using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ep0_ScreenInteraction : ARObjectSelect
{
    RectTransform img_textBG;
    Animator m_animator;
    Text txt_title;
    public string[] titleString;
    public float waitTime = 2f;
    public int[] correctNum = new int[]{ 5, 2, 3};

    int questionNum = 0;


    private void Start()
    {
        e_handIcon = HandIcon.SCREEN;
        m_animator = GetComponent<Animator>();

        for (int i = 0; i < arr_arSelectables.Length; i++)
        {
            ARSelectableObject _obj = arr_arSelectables[i];
            arr_arSelectables[i].action.AddListener(() =>
            {
                _obj.SetTriggerAnimation(4);
                CheckSuccess();
            });
        }
    }

    private void Update()
    {
        transform.position = new Vector3(gameMgr.currentEpisode.currentStage.arr_header[0].transform.position.x, transform.position.y, gameMgr.currentEpisode.currentStage.arr_header[0].transform.position.z);

        transform.rotation = Quaternion.LookRotation(transform.position - gameMgr.arMainCamera.transform.position);
    }

    public override IEnumerator DisableObject()
    {
        gameMgr.uiMgr.worldCanvas.StopTimer();

        StopGuideParticle();
        lastSelect.action.Invoke();
        bool _isCorrect = (lastSelect == arr_arSelectables[correctNum[questionNum]]);
        if (_isCorrect)
        {
            yield return new WaitForSeconds(1f);
            for (int index = 0; index < arr_arSelectables.Length; index++)
            {
                arr_arSelectables[index].SetTriggerAnimation(3);
                arr_arSelectables[index].isSelected = false;
                arr_arSelectables[index].isTimer = false;
            }
            Animator titleAnim = gameMgr.uiMgr.ui_game.game_txt_selectTitle.transform.parent.GetComponent<Animator>();
            titleAnim.ResetTrigger("tDestroy");
            titleAnim.SetTrigger("tDestroy");
            m_animator.ResetTrigger("tDestroy");
            m_animator.SetTrigger("tDestroy");
        }
        isDisable = true;
        yield return new WaitForSeconds(1f);
        isDisable = false;

        lastSelect = null;

        if (questionNum > 2 )
        {
            gameObject.SetActive(!_isCorrect);
        }
    }

    //버튼동작
    /// <summary>
    /// 클릭한 오브젝트가 정답이 맞는지 체크
    /// </summary>
    public void CheckSuccess()
    {
        if (IsEqual(arr_arSelectables[correctNum[questionNum]]))
        {
            gameMgr.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_SUCCESS);
            gameMgr.PlayParticleEffect(arr_arSelectables[correctNum[questionNum]].transform.GetChild(0).position - arr_arSelectables[correctNum[questionNum]].transform.forward * 0.3f, ReadOnly.Defines.PREFAB_EFFECT_TOUCH);
            gameMgr.currentEpisode.currentStage.arr_header[0].Success();
            gameMgr.currentEpisode.currentStage.arr_header[0].StartCoroutine(gameMgr.LateFunc(() => NextQuestion(), 2));
        }
        else
        {
            gameMgr.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_FAILURE);
            gameMgr.currentEpisode.currentStage.arr_header[0].Failure();
            gameMgr.currentEpisode.currentStage.arr_header[0].StartCoroutine(gameMgr.LateFunc(() => this.gameObject.SetActive(true), waitTime));
        }
    }

    void NextQuestion()
    {
        questionNum++;

        if (questionNum > 2)
        {
            EndInteraction();
            return;
        }
        else
        {
            txt_title.text = titleString[questionNum];
            img_textBG.sizeDelta = new Vector2(txt_title.text.Length * 50f, img_textBG.sizeDelta.y);

            for (int index = 0; index < arr_arSelectables.Length; index++)
            {
                arr_arSelectables[index].SetTriggerAnimation(0);
            }
            Animator titleAnim = gameMgr.uiMgr.ui_game.game_txt_selectTitle.transform.parent.GetComponent<Animator>();
            titleAnim.ResetTrigger("tSpawn");
            titleAnim.SetTrigger("tSpawn");
            m_animator.ResetTrigger("tSpawn");
            m_animator.SetTrigger("tSpawn");

        }
    }

    public override void StartInteraction()
    {
        e_handIcon = HandIcon.SCREEN;
        base.StartInteraction();
        stageMgr.StopAllCoroutines();

        gameMgr.currentEpisode.currentStage.arr_header[0].SetAnim(3);

        txt_title = gameMgr.uiMgr.ui_game.game_txt_selectTitle;
        img_textBG = txt_title.transform.parent.GetComponent<RectTransform>();

        img_textBG.gameObject.SetActive(true);
        txt_title.text = titleString[0]; 
        img_textBG.sizeDelta = new Vector2(txt_title.text.Length * 50f, img_textBG.sizeDelta.y);

    }

    public override void EndInteraction()
    {
        base.EndInteraction();
        gameMgr.currentEpisode.currentStage.arr_header[0].SetAnim(0);

        img_textBG.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}

