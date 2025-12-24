using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ReadOnly; //custum namespace
using UnityEngine.UI;
public enum NRUIActive
{
    NONE = 0,
    MAINMENU,
    GAMESELECT,
    SETTING,
    INFO,
}

public class NRUIManager : MonoBehaviour
{
    AudioSource m_audioSource;
    public GameObject ui_title { get; set; }
    public GameObject ui_gameSelect { get; set; }
    NRRayInteract title_btn_start { get; set; }

    NRSelectable[] arr_selectable_gameSelect;

    public NRUIActive currentUI = NRUIActive.NONE;

    public MiniGameType gameselect_gameType = MiniGameType.NONE;


    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.clip = GameManager.Instance.soundMgr.LoadClip(ReadOnly.Defines.SOUND_BGM_TITLE);

        //GameObject Setting
        ui_title = transform.GetChild(0).GetChild(0).gameObject;
        ui_gameSelect = transform.GetChild(1).gameObject;

        //NRSelectable Object(3D Button) Setting
        title_btn_start = ui_title.transform.GetChild(0).GetComponent<NRRayInteract>();

        arr_selectable_gameSelect = ui_gameSelect.GetComponentsInChildren<NRSelectable>();

    }

    private void Start()
    {
        //AddListener
        //arr_selectable_menu[0].action.AddListener(() => UIActive(NRUIActive.GAMESELECT));
        //arr_selectable_menu[1].action.AddListener(() => UIActive(NRUIActive.SETTING));
        //arr_selectable_menu[2].action.AddListener(() => UIActive(NRUIActive.INFO));
        title_btn_start.pointerClick.AddListener(() => UIActive(NRUIActive.GAMESELECT));

        arr_selectable_gameSelect[0].action.AddListener(() => UIActive(NRUIActive.MAINMENU));

        //게임 시작, 맵 생성, 맵 선택 버튼에 할당
        for (int i = 1; i < arr_selectable_gameSelect.Length; i++)
        {
            int a = i-1;
            arr_selectable_gameSelect[i].action.AddListener(() =>
            {
                gameselect_gameType = (MiniGameType)a;
                GameManager.Instance.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_EPISODE_IN);
                GameManager.Instance.miniGameMgr.CreateMiniGame((int)gameselect_gameType);
            });
        }

        UIInit();
    }

    public void UIInit()
    {
        UIActive(NRUIActive.MAINMENU);
    }

    public void UIDisable()
    {
        ui_title.SetActive(false);
        ui_gameSelect.SetActive(false);
    }

    public void UIActive(NRUIActive activeUI)
    {
        currentUI = activeUI;

        GameManager.Instance.soundMgr.ChangeBGMAudioSource(m_audioSource);
        UIDisable();

        switch (activeUI)
        {
            case NRUIActive.MAINMENU:
                ui_title.SetActive(true);
                break;
            case NRUIActive.GAMESELECT:
                ui_gameSelect.SetActive(true);
                break;
        }
    }

}
