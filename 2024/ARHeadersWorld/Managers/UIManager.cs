using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


/// <summary>
/// 12/18/2023-LYI
/// UI 상태 관리 및 UI 창 변환 관리 클래스
/// </summary>
public class UIManager : MonoBehaviour
{
    GameManager gameMgr;

    public UI_CharacterSelect ui_characterSelect;
    public UI_CharacterTransform ui_characterTransform;
    public UI_DetailTransform ui_detailTransform;
    public UI_Animation ui_animation;
    public UI_ModeChange ui_modeChange;

    public Sprite sprite_blue;
    public Sprite sprite_yellow;
    public Sprite sprite_red;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
    }


    public void ChangeUI(GameMode stat)
    {
        ui_characterSelect.gameObject.SetActive(false);
        ui_characterTransform.gameObject.SetActive(false);
        ui_detailTransform.gameObject.SetActive(false);
        ui_animation.gameObject.SetActive(false);
        ui_animation.ButtonReset();


        switch (stat)
        {
            case GameMode.NONE:
                break;
            case GameMode.CHARACTER_SELECT:
                ui_characterSelect.gameObject.SetActive(true);
                break;
            case GameMode.CHARACTER_TRANSFORM:
                ui_characterTransform.gameObject.SetActive(true);
                ui_characterTransform.CharacterTransformUIInit();
                break;
            case GameMode.DETAIL_TRANSFORM:
                ui_detailTransform.gameObject.SetActive(true);
                break;
            case GameMode.ANIMATION:
                if (gameMgr.spawnARCharacter != null)
                {
                    ui_animation.gameObject.SetActive(true);
                    ui_animation.AnimationUIInit();
                }
                break;
            default:
                break;
        }

        ui_modeChange.ButtonChangeMode(stat);
    }
}
