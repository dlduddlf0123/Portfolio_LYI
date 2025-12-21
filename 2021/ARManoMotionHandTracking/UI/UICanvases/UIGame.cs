using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HandIcon
{
    NONE = 0,
    FRONT = 1,
    PINCH = 2,
    INDEX = 3,
    SCREEN = 4,
    BACK = 5,
}
public class UIGame : MonoBehaviour
{
    public HandIcon iconHand;

    public Button game_btn_skip;
    public Button game_btn_pause;
    public Text game_txt_dialog;
    public CanvasGroup game_grp_frame;

    public Text debug_txt_scene;
    public Text debug_txt_timeline;
    public Text debug_txt_interaction;

    public Text game_txt_selectTitle;

    public Image[] arr_handIcon;

    void Start()
    {

        //game_btn_back = ui_game.GetChild(1).GetComponent<Button>();
        //game_btn_skip = ui_game.GetChild(2).GetComponent<Button>();
        //game_btn_pause = ui_game.GetChild(3).GetComponent<Button>();
        //game_txt_dialog = ui_game.GetChild(4).GetComponent<Text>();
        //game_grp_frame = ui_game.GetChild(5).GetComponent<CanvasGroup>();

        //debug_txt_scene = ui_game.GetChild(6).GetComponent<Text>();
        //debug_txt_timeline = ui_game.GetChild(7).GetComponent<Text>();
        //debug_txt_interaction = ui_game.GetChild(8).GetComponent<Text>();


        //game_btn_back.onClick.AddListener(() => { UIPauseButtonExit(); });
        //game_btn_skip.onClick.AddListener(() =>
        //{
        //    if (gameMgr.statGame == GameStatus.CUTSCENE)
        //    {
        //        gameMgr.currentEpisode.currentStage.EndCutScene();
        //    }
        //    else if (gameMgr.statGame == GameStatus.INTERACTION)
        //    {
        //        gameMgr.currentEpisode.currentStage.EndInteraction();
        //    }
        //});
        //game_btn_pause.onClick.AddListener(() => { UIGameButtonPause(); });

        game_btn_skip.gameObject.SetActive(false);
        game_txt_dialog.gameObject.SetActive(false);
        game_txt_selectTitle.transform.parent.gameObject.SetActive(false);
        ChangeHandIcon(HandIcon.NONE);
    }

    public void ChangeHandIcon(HandIcon _hand)
    {
        iconHand = _hand;

        for (int i = 0; i < arr_handIcon.Length; i++)
        {
            arr_handIcon[i].gameObject.SetActive(false);
        }


        if (_hand == HandIcon.NONE)
        {
            return;
        }
        arr_handIcon[(int)_hand -1].gameObject.SetActive(true);
    }
}
