using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rabyrinth.ReadOnlys;


public class SkillController : MonoBehaviour
{
    private GameManager GameMgr;

    private bool isTouchEnd;
    private bool isRelease;

    private int ChangeSkil_Index;

    private int CurrentIndex;

    public bool isSkillChange { get; set; }

    public Animator[] SkillButton_Anis;
    public Image[] Skill_BaseImage;
    public Image[] Skill_Image;
    public Text[] SkillText;
    public Button[] SkillButton;
    public int[] nActiveSkills;

    private GameObject MagicPoint;
    private ParticleSystem MagicPoint_Effect;

    private Sprite[] SkillImages;

    // Use this for initialization
    void Awake()
    {
        GameMgr = MonoSingleton<GameManager>.Inst;

        MagicPoint = (Instantiate(Resources.Load(Defines.UI_MAGIC_PATH)) as GameObject);
        GameMgr.DontDestroyOnLoadAndAdd(MagicPoint);
        MagicPoint.gameObject.SetActive(false);
        MagicPoint_Effect = MagicPoint.transform.GetChild(0).GetComponent<ParticleSystem>();

        SkillImages = Resources.LoadAll<Sprite>("SkillImage");

        isSkillChange = false;
        isTouchEnd = false;
        isRelease = false;
        CurrentIndex = 0;
        ChangeSkil_Index = -1;
        ResetSkill();
    }

    public void ResetSkill()
    {
        Skill_BaseImage = new Image[4];
        Skill_Image = new Image[4];
        SkillText = new Text[4];
        SkillButton = new Button[4];
        nActiveSkills = new int[4];
        SkillButton_Anis = new Animator[4];

        for (int index = 0; index < 4; index++)
        {
            nActiveSkills[index] = index;
            Skill_BaseImage[index] = transform.GetChild(0).GetChild(index).GetComponent<Image>();
            Skill_Image[index] = transform.GetChild(1).GetChild(index).GetComponent<Image>();
            SkillButton[index] = transform.GetChild(1).GetChild(index).GetComponent<Button>();
            SkillText[index] = transform.GetChild(2).GetChild(index).GetComponent<Text>();
            SkillButton_Anis[index] = SkillButton[index].GetComponent<Animator>();
        }
    }

    public void ActiveAddSkil(int _skill)
    {
        if (isSkillChange)
            return;

        for (int index = 0; index < 4; index++)
            if (nActiveSkills[index] == _skill)
                return;


        for (int index = 0; index < 4; index++)
            if(SkillButton[index].enabled)
                SkillButton_Anis[index].SetBool(Defines.ANI_PARAM_POP, true);

        ChangeSkil_Index = _skill;
        isSkillChange = true;

        GameMgr.Main_UI.popUpController.SkillButtons[ChangeSkil_Index].image.color = Color.gray;
        GameMgr.Main_UI.popUpController.ExitPanel.SetActive(false);
    }

    public void SetSkillSlot(int _index)
    {
        isSkillChange = false;

        GameMgr.Main_UI.popUpController.SkillButtons[nActiveSkills[_index]].image.color = Color.white;

        nActiveSkills[_index] = ChangeSkil_Index;

        Skill_BaseImage[_index].sprite = SkillImages[ChangeSkil_Index];
        SkillButton[_index].image.sprite = SkillImages[ChangeSkil_Index];

        for (int index = 0; index < 4; index++)
            SkillButton_Anis[index].SetBool(Defines.ANI_PARAM_POP, false);
    }


    //////////////////////////////////////////////////터치 스킬//////////////////////////////////

    public IEnumerator FollowMagic(Skill _skill)
    {
        Vector3 newpos = Vector3.zero;
        while (isTouchEnd)
        {
            Vector3 objPos = GameMgr.Main_Cam.WorldToScreenPoint(MagicPoint.transform.position);
#if UNITY_ANDROID && !UNITY_EDITOR
            newpos = GameMgr.Main_Cam.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, objPos.z));
#else
            newpos = GameMgr.Main_Cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, objPos.z));
#endif
            newpos = new Vector3(newpos.x, 0.1f, newpos.z);
            MagicPoint.transform.position = newpos;
            yield return null;
        }

        if (!isRelease)
        {
            GameMgr.skillManager.UseSkill(newpos, _skill);
            StartCoroutine(ActiveSkill(_skill.cooltime, CurrentIndex));
        }
    }

    private IEnumerator CheckInsideRect(int _index)
    {
        isRelease = true;

        Vector2 newPos;
        Rect rect = Skill_BaseImage[_index].rectTransform.rect;
        Vector2 localPos = Skill_BaseImage[_index].transform.localPosition;
        Vector2 image_pos = new Vector2(localPos.x - rect.width * 0.5f, localPos.y - rect.height * 0.5f);
#if UNITY_ANDROID && !UNITY_EDITOR
        newPos = new Vector2(Input.GetTouch(0).position.x - Screen.width * 0.5f, Input.GetTouch(0).position.y - Screen.height * 0.5f);
#else
        newPos = new Vector2(Input.mousePosition.x - Screen.width * 0.5f, Input.mousePosition.y - Screen.height * 0.5f);
#endif

        while (newPos.x > image_pos.x && newPos.x < image_pos.x + rect.width &&
                newPos.y > image_pos.y && newPos.y < image_pos.y + rect.height)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                newPos = new Vector2(Input.GetTouch(0).position.x - Screen.width * 0.5f, Input.GetTouch(0).position.y - Screen.height * 0.5f);
#else
            newPos = new Vector2(Input.mousePosition.x - Screen.width * 0.5f, Input.mousePosition.y - Screen.height * 0.5f);
#endif
            yield return null;
        }
        isRelease = false;
    }

    public void ReleaseMagic()
    {
        MagicPoint.gameObject.SetActive(false);
        isRelease = true;
        isTouchEnd = false;
    }

    /// ///////////////////////////////////////////////////////스킬리셋////////////////////////////////////////////
    public void TouchEnd(BaseEventData _bed)
    {
        isTouchEnd = false;
        MagicPoint.gameObject.SetActive(false);
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void ResetSkillCoolTime()
    {
        StopAllCoroutines();

        for (int index = 0; index < 4; index++)
        {
            Skill_Image[index].fillAmount = 1.0f;
            SkillText[index].gameObject.SetActive(false);
            SkillButton[index].enabled = true;
        }
    }
    public void AcitveSkillButton(int _index)
    {
        if (isSkillChange)
        {
            SetSkillSlot(_index);
            GameMgr.Main_UI.popUpController.ExitPanel.SetActive(true);
            return;
        }

        if (isTouchEnd)
            return;

        CurrentIndex = _index;

        if (GameMgr.PlayData.GameData.lSkillData[nActiveSkills[_index]].type == SkillType.FlameSword ||
            GameMgr.PlayData.GameData.lSkillData[nActiveSkills[_index]].type == SkillType.PlassmaSword ||
            GameMgr.PlayData.GameData.lSkillData[nActiveSkills[_index]].type == SkillType.Barrier)
        {
            if (nActiveSkills[_index] == (int)SkillType.PlassmaSword ||
                nActiveSkills[_index] == (int)SkillType.FlameSword)
            {
                for (int index = 0; index < 4; index++)
                    if (nActiveSkills[index] == (int)SkillType.PlassmaSword ||
                        nActiveSkills[index] == (int)SkillType.FlameSword)
                        StartCoroutine(ActiveSkill(GameMgr.PlayData.GameData.lSkillData[nActiveSkills[index]].cooltime, index));
            }
            else
            {
                StartCoroutine(ActiveSkill(GameMgr.PlayData.GameData.lSkillData[nActiveSkills[_index]].cooltime, _index));
            }
            GameMgr.skillManager.UseSkill(Vector3.zero, GameMgr.PlayData.GameData.lSkillData[nActiveSkills[_index]]);
            return;
        }

        isTouchEnd = true;
        MagicPoint.transform.localScale = new Vector3(
            GameMgr.PlayData.GameData.lSkillData[nActiveSkills[_index]].range * 0.4f,
            GameMgr.PlayData.GameData.lSkillData[nActiveSkills[_index]].range * 0.4f,
            GameMgr.PlayData.GameData.lSkillData[nActiveSkills[_index]].range * 0.4f);
        MagicPoint_Effect.startSize = GameMgr.PlayData.GameData.lSkillData[nActiveSkills[_index]].range * 1.5f;
        MagicPoint.gameObject.SetActive(true);
        StartCoroutine(CheckInsideRect(_index));
        StartCoroutine(FollowMagic(GameMgr.PlayData.GameData.lSkillData[nActiveSkills[_index]]));
    }

    public IEnumerator ActiveSkill(float _coolTime, int _index)
    {
        float time = 0.0f;
        SkillButton[_index].enabled = false;
        while (time < _coolTime)
        {
            float CoolAmount = (time / _coolTime);
            Skill_Image[_index].fillAmount = CoolAmount;
            SkillText[_index].gameObject.SetActive(true);
            SkillText[_index].text = (_coolTime - time).ToString("0");

            time += Time.deltaTime;
            yield return null;

        }
        Skill_Image[_index].fillAmount = 1.0f;
        SkillText[_index].text = "0";
        SkillText[_index].gameObject.SetActive(false);
        SkillButton[_index].enabled = true;
    }

}



//public class SkillController : MonoBehaviour {
//    private GameManager GameMgr;

//    private bool isTouchEnd;
//    private bool isRelease;
//    private bool isSkillControll;

//    private int CurrentIndex;

//    public Image[] Skill_BaseImage;
//    public Image[] Skill_Image;
//    public Text[] SkillText;
//    public Button[] SkillButton;
//    public int[] nActiveSkills;

//    // Use this for initialization
//    void Awake ()
//    {
//        isTouchEnd = false;
//        isRelease = false;
//        isSkillControll = false;
//        CurrentIndex = 0;
//        GameMgr = MonoSingleton<GameManager>.Inst;
//        ResetSkill();
//    }
	
//    public void ResetSkill()
//    {
//        Skill_BaseImage = new Image[4];
//        Skill_Image = new Image[4];
//        SkillText = new Text[4];
//        SkillButton = new Button[4];
//        nActiveSkills = new int[4];

//        for (int index = 0; index < 4; index++)
//        {
//            nActiveSkills[index] = index;
//            Skill_BaseImage[index] = transform.GetChild(0).GetChild(index).GetComponent<Image>();
//            Skill_Image[index] = transform.GetChild(1).GetChild(index).GetComponent<Image>();
//            SkillButton[index] = transform.GetChild(1).GetChild(index).GetComponent<Button>();
//            SkillText[index] = transform.GetChild(2).GetChild(index).GetComponent<Text>();
//        }
//        nActiveSkills[3] = 5;
//    }


//    //////////////////////////////////////////////////터치 스킬//////////////////////////////////

//    public IEnumerator FollowMagic(Skill _skill)
//    {
//        Vector3 newpos = Vector3.zero;
//        while (! isTouchEnd)
//        {
//            Vector3 objPos = GameMgr.Main_Cam.WorldToScreenPoint(Magic.transform.position);
//#if UNITY_ANDROID && !UNITY_EDITOR
//            newpos = GameMgr.Main_Cam.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, objPos.z));
//#else
//            newpos = GameMgr.Main_Cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, objPos.z));
//#endif
//            newpos = new Vector3(newpos.x, 0.1f, newpos.z);
//            Magic.transform.position = newpos;
//            yield return null;
//        }

//        if (!isRelease)
//        {
//            GameMgr.skillManager.UseSkill(newpos, _skill);
//            StartCoroutine(ActiveSkill(_skill.cooltime));
//        }
//    }
//    public void ReleaseMagic()
//    {
//        Magic.gameObject.SetActive(false);
//        isRelease = true;
//        isTouchEnd = true;
//    }

//    /// ///////////////////////////////////////////////////////스킬리셋////////////////////////////////////////////
//    public void TouchEnd(BaseEventData _bed)
//    {
//        isTouchEnd = true;
//        Magic.gameObject.SetActive(false);
//    }
//    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

//    private IEnumerator AcitveSkillPosition(int _index)
//    {
//        isSkillControll = true;
//        isTouchEnd = false;
//        Vector2 newPos;
//        Rect rect = Skill_BaseImage[_index].rectTransform.rect;
//        Vector2 localPos = Skill_BaseImage[_index].transform.localPosition;
//        Vector2 image_pos = new Vector2(localPos.x - rect.width * 0.5f, localPos.y - rect.height * 0.5f);
//#if UNITY_ANDROID && !UNITY_EDITOR
//            newPos = new Vector2(Input.GetTouch(0).position.x - Screen.width * 0.5f, Input.GetTouch(0).position.y - Screen.height * 0.5f);
//#else
//        newPos = new Vector2(Input.mousePosition.x - Screen.width * 0.5f, Input.mousePosition.y - Screen.height * 0.5f);
//#endif

//        while (((newPos.x > image_pos.x && newPos.x < image_pos.x + rect.width &&
//                newPos.y > image_pos.y && newPos.y < image_pos.y + rect.height)) && ! isTouchEnd)
//        {
//            Debug.Log(newPos);
//#if UNITY_ANDROID && !UNITY_EDITOR
//            newPos = new Vector2(Input.GetTouch(0).position.x - Screen.width * 0.5f, Input.GetTouch(0).position.y - Screen.height * 0.5f);
//#else
//            newPos = new Vector2(Input.mousePosition.x - Screen.width * 0.5f, Input.mousePosition.y - Screen.height * 0.5f);
//#endif
//            yield return null;
//        }

//        CurrentIndex = _index;
//        isSkillControll = false;
//        isRelease = false;
//        Magic.gameObject.SetActive(true);
//        if (! isTouchEnd)
//            StartCoroutine(FollowMagic(GameMgr.PlayerData.GameData.lSkillData[nActiveSkills[_index]]));
//    }

//    public void AcitveSkillButton(int _index)
//    {
//        if (isSkillControll)
//            return;

//        StartCoroutine(AcitveSkillPosition(_index));
//    }

//    public IEnumerator ActiveSkill(float _coolTime)
//    {
//        int index = CurrentIndex;
//        float time = 0.0f;
//        SkillButton[index].enabled = false;
//        while (time < _coolTime)
//        {
//            float CoolAmount = (time / _coolTime);
//            Skill_Image[index].fillAmount = CoolAmount;
//            SkillText[index].gameObject.SetActive(true);
//            SkillText[index].text = (_coolTime - time).ToString("0");
            
//            time += Time.deltaTime;
//            yield return null;
            
//        }
//        Skill_Image[index].fillAmount = 1.0f;
//        SkillText[index].text = "0";
//        SkillText[index].gameObject.SetActive(false);
//        SkillButton[index].enabled = true;
//    }
    
//}
