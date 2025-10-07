using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EmotionStatus
{
    NONE = 0,
    GOOD,
    BAD,
    SMOKE,
    SLEEP,
    WATER,
    DRY,
}


/// <summary>
/// 대사 불러오기 및 주기적으로 랜덤 대사 출력 및 눈 애니메이션 변경
/// </summary>
public class ARCharacter : MonoBehaviour
{
    GameManager gameMgr;
    public HeaderCanvas headerCanvas;

    Animator m_anim;
    GameObject particle_steam;

    EmotionStatus e_emotion = EmotionStatus.NONE;
      
    List<List<object>> list__headersDialog = new List<List<object>>();

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        m_anim = transform.GetChild(0).GetComponent<Animator>();
        particle_steam = transform.GetChild(2).gameObject;


        list__headersDialog = gameMgr.dialogMgr.ReadDialogDatas(gameObject.name);
    }

    private void Start()
    {
        headerCanvas.list__currentDialog = list__headersDialog;

        StartCoroutine(C_RandomDialog());
    }

    IEnumerator C_RandomDialog()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);

            ChangeEmotion((EmotionStatus)Random.Range(0, 6));
        }
    }

    /// <summary>
    /// 코에서 뿜는 안개 효과
    /// </summary>
    void ToggleSteam(bool _isActive)
    {
        if (_isActive == true)
        {
            particle_steam.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            particle_steam.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        }
        else if(_isActive == false)
        {
            particle_steam.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
            particle_steam.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
        }
    }

    /// <summary>
    /// 캐릭터 감정 표현 변경
    /// </summary>
    public void ChangeEmotion(EmotionStatus _emotion)
    {
        e_emotion = _emotion;

        ToggleSteam(false);

        switch (_emotion)
        {
            case EmotionStatus.NONE:
                m_anim.SetFloat("EyeNum", 0);
                break;
            case EmotionStatus.GOOD:
                m_anim.SetFloat("EyeNum", 1);
                headerCanvas.ShowText(0, 0);
                break;
            case EmotionStatus.BAD:
                m_anim.SetFloat("EyeNum", 2);
                headerCanvas.ShowText(1, 0);
                break;
            case EmotionStatus.SMOKE:
                m_anim.SetFloat("EyeNum", 3);
                headerCanvas.ShowText(2, 0);
                ToggleSteam(true);
                break;
            case EmotionStatus.SLEEP:
                m_anim.SetFloat("EyeNum", 4);
                headerCanvas.ShowText(3, 0);
                break;
            case EmotionStatus.WATER:
                m_anim.SetFloat("EyeNum", 5);
                headerCanvas.ShowText(4, 0);
                break;
            case EmotionStatus.DRY:
                m_anim.SetFloat("EyeNum", 6);
                headerCanvas.ShowText(5, 0);
                break;
            default:
                break;
        }
    }


}
