using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Project.ReadOnly;

enum EmotionType
{
    NORMAL = 0,
    ANGRY,
    QUESTION,
    SAD,
    ANNOYING,
    SWEAT,
    SPIT,
    SHOUT,
    DIZZY,
    SHOT,
    JOY,
    RUN,
    LAUGH,
    HIT,
}

public class DialogManager : MonoBehaviour, IPointerDownHandler
{
    GameManager gameMgr;
    CSVparser parse = new CSVparser();

    //파싱 데이터 리스트들
    List<object> list_talker;   //말하는 사람
    List<object> list_dialog;   //대사
    List<object> list_chara;    //캐릭터 이미지
    List<object> list_emote;    //감정표현
    List<object> list_font;     //폰트 사이즈

    //파일 위치관리
    public Transform spriteUI;
    Transform backGround;
    Transform spriteMask;
    RectTransform trText;
    Transform trImage;
    Transform trEmotion;

    EmotionType[] emotionTypes; //감정상태

    //텍스트 (1: 왼쪽, 2: 오른쪽, 3: 테나)
    Image[] textBoxes; //텍스트 박스 이미지
    Text[] logTexts;  //텍스트

    //캐릭터 스프라이트 (1: 왼쪽, 2: 오른쪽, 3: 테나)
    //SpriteRenderer[] charImgs;//캐릭터 이미지
    List<GameObject> charToons;
    Animator[] charAnims; //캐릭터 애니메이터

    //이모티콘(감정표현)  (1: 왼쪽, 2: 오른쪽, 3: 테나)
    SpriteRenderer[] emoteImgs; //이미지
    Animator[] emoteAnims; //감정표현 효과
    Transform[][] emotePositions; //0=눈 / 1=정면 / 2=위 / 3=귀

    Image[] eyeImages; //눈 모양들

    Button btn_skip; //대화 스킵 버튼

    int currentIndex; //현재 대화 번호 (열 번호)
    int lastTalker;

    // Use this for initialization
    void Awake()
    {
        gameMgr = GameManager.Instance;

        list_talker = new List<object>();
        list_dialog = new List<object>();
        list_chara = new List<object>();
        list_emote = new List<object>();
        list_font = new List<object>();

        //카메라에 붙은 스프라이트
        backGround = spriteUI.GetChild(0).GetComponent<Transform>();
        spriteMask = spriteUI.GetChild(1).GetComponent<Transform>();
        trImage = spriteMask.GetChild(0).GetComponent<Transform>();
        trEmotion = spriteMask.GetChild(1).GetComponent<Transform>();

        //UI 이미지
        trText = transform.GetChild(0).GetComponent<RectTransform>();
        btn_skip = transform.GetChild(1).GetComponent<Button>();
        btn_skip.onClick.AddListener(() => { EndDialog(); gameMgr.soundMgr.PlaySfx(btn_skip.transform.position, gameMgr.soundMgr.sfx_bt); });


        emotionTypes = new EmotionType[3];
        //charImgs = new SpriteRenderer[3];
        charToons = new List<GameObject>();
        charAnims = new Animator[3];
        textBoxes = new Image[3];
        logTexts = new Text[3];
        emoteImgs = new SpriteRenderer[3];
        emoteAnims = new Animator[3];

        eyeImages = new Image[10];

        emotePositions = new Transform[3][];

        for (int i = 0; i < trImage.childCount; i++)
        {
            charToons.Add(trImage.GetChild(i).gameObject);
        }

        for (int i = 0; i < 3; i++)
        {
            emotePositions[i] = new Transform[3];
            emotionTypes[i] = EmotionType.NORMAL;

            //charImgs[i] = trImage.GetChild(i).GetComponent<SpriteRenderer>();
            //charAnims[i] = charImgs[i].GetComponent<Animator>();

            textBoxes[i] = trText.GetChild(i).GetComponent<Image>();
            logTexts[i] = textBoxes[i].transform.GetChild(0).GetComponent<Text>();

            //emoteImgs[i] = trEmotion.GetChild(i).GetComponent<SpriteRenderer>();
            //emoteAnims[i] = emoteImgs[i].GetComponent<Animator>();
            //for (int j = 0; j < 4; j++)
            //{
            //    emotePositions[i][j] = charImgs[i].transform.GetChild(j).GetComponent<Transform>();
            //}

        }


        currentIndex = 0;
        lastTalker = 0;

        Init();
    }


    /// <summary>
    /// 초기화(비활성화)
    /// </summary>
    void Init()
    {
        for (int i = 0; i < trImage.childCount; i++)
        {
            charToons[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < 3; i++)
        {
            //charImgs[i].gameObject.SetActive(false);
            textBoxes[i].gameObject.SetActive(false);
            //emoteImgs[i].gameObject.SetActive(false);
        }
        //charAnims[0].runtimeAnimatorController = Resources.Load(Defines.ANIMATOR_SLIDERIGHT) as RuntimeAnimatorController;
        //charAnims[1].runtimeAnimatorController = Resources.Load(Defines.ANIMATOR_SLIDELEFT) as RuntimeAnimatorController;
        //charAnims[2].runtimeAnimatorController = Resources.Load(Defines.RESOURCE_ANIMATOR_SLIDELEFT) as RuntimeAnimatorController;

        spriteUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// 레벨을 넣으면 레벨에 해당되는 값을 리턴
    /// </summary>
    public List<object> ReadDialogData(int _level)
    {
        Table table;
        switch (gameMgr.language)
        {
            case 0:
                table = parse.ParsingCSV(Defines.CSV_DIALOG_DATA_KOR);
                break;
            default:
                table = parse.ParsingCSV(Defines.CSV_DIALOG_DATA_ENG);
                break;
        }
        List<object> _data = table.Row[_level].Col;
        return _data;
    }

    public List<List<object>> ReadDialogDatas(string _path)
    {
        Table table = parse.ParsingCSV(_path);
        List<List<object>> _datas = new List<List<object>>();
        for (int i = 0; i < table.Row.Count; i++)
        {
            List<object> _data = table.Row[i].Col;
            _datas.Add(_data);
        }
        return _datas;
    }

    public List<object> ReadDialogData(string _path, int _level)
    {
        Table table = parse.ParsingCSV(_path);
        List<object> _data = table.Row[_level].Col;
        return _data;
    }

    /// <summary>
    /// GameManager에서 호출, 대화 시작
    /// </summary>
    /// <param name="_number">몇번째 대화인지</param>
    public void StartDialog(int _number)
    {
        Init();
        int index = (_number - 1) * 5;
        list_talker = ReadDialogData(index);
        list_dialog = ReadDialogData(index + 1);
        list_chara = ReadDialogData(index + 2);
        list_emote = ReadDialogData(index + 3);
        list_font = ReadDialogData(index + 4);

        currentIndex = 1;
        gameMgr.statGame = GameState.DIALOG;
        spriteUI.gameObject.SetActive(true);

        gameMgr.soundMgr.PlayBgm(gameMgr.soundMgr.bgm_dialog);
        gameMgr.stage.SetActive(false);

        NextDialog();
    }

    /// <summary>
    /// 대화 진행하기
    /// </summary>
    void NextDialog()
    {
        int list_t = Convert.ToInt32(list_talker[currentIndex]);
        int _talker = list_t - 1;
        int _chara = Convert.ToInt32(list_chara[currentIndex]);
        int _emote = Convert.ToInt32(list_emote[currentIndex]);

        if (list_t == 0 || list_t > 3)
        {
            EndDialog();
        }
        else
        {
            //Debug.log(_talker+" "+(EmotionType)_emote);
            emotionTypes[_talker] = (EmotionType)_emote;

            SetText(logTexts[_talker], currentIndex);
            //SetCharImage(charImgs[_talker], charAnims[_talker], _chara);
            SetCharActive(_talker, _chara);
            SetEmotion(_talker);

            //SetFade
            //for (int i = 0; i < 3; i++)
            //{
            //    charImgs[i].transform.GetChild(0).gameObject.SetActive(true);
            //    textBoxes[i].transform.GetChild(1).gameObject.SetActive(true);
            //}
            //charImgs[_talker].transform.GetChild(0).gameObject.SetActive(false);
            //textBoxes[_talker].transform.GetChild(1).gameObject.SetActive(false);
        }

        lastTalker = _chara;
    }

    //string 잘라서 줄 바꾸기
    void SetText(Text _txt, int _index)
    {
        string t = list_dialog[_index].ToString();
        string[] _t = t.Split('@');
        SetFontSize(_txt, Convert.ToInt32(list_font[_index])); //폰트 사이즈 변경
        _txt.text = null;
        for (int i = 0; i < _t.Length; i++)
        {
            _txt.text += _t[i];
            if (i < _t.Length - 1)
            {
                _txt.text += '\n';
            }
        }
    }

    /// <summary>
    /// 폰트 사이즈 변경하기
    /// </summary>
    /// <param name="_txt">변경할 텍스트</param>
    /// <param name="_size">1:작게 2:크게 default:보통</param>
    void SetFontSize(Text _txt, int _size)
    {
        switch (_size)
        {
            case 1:
                _txt.fontSize = 30;//작게
                break;
            case 2:
                _txt.fontSize = 80;//크게
                //진동효과
                break;
            default:
                _txt.fontSize = 50; //기본
                break;
        }

    }

    /// <summary>
    /// 캐릭터 이미지 및 애니메이션 교체
    /// </summary>
    /// <param name="_charImg">교체될 이미지(char1 or 2)</param>
    /// <param name="_charnum">교체할 이미지(0~9) </param>
    void SetCharActive(int _talker, int _charnum)
    {
        switch (_talker)
        {
            case 0:
                if (lastTalker != _talker)
                {
                    charToons[8].SetActive(false);
                    charToons[9].SetActive(false);
                }
                charToons[_charnum].SetActive(true);
                break;
            case 1:
                if (lastTalker != _talker)
                {
                    for (int i = 1; i <= 6; i++)
                    {
                        charToons[i].SetActive(false);
                    }
                }
                charToons[_charnum].SetActive(true);
                break;
            case 2:
                charToons[0].SetActive(true);
                break;
        }

    }

    //이미지 스프라이트 변경
    void SpriteChange(SpriteRenderer _img, string _path)
    {
        _img.gameObject.SetActive(true);
        Sprite a = gameMgr.b_sprites.LoadAsset<Sprite>(_path);
        if (_img.sprite == a) { return; }

        _img.gameObject.SetActive(false);
        _img.sprite = a;
        _img.gameObject.SetActive(true);
    }


    /// <summary>
    /// CharType에 따른 동작들 변경
    /// 1.캐릭터 애니메이션 변경
    /// 2.텍스트 박스 이미지 변경
    /// 3.이모티콘 변경
    /// 4.이모티콘 애니메이션 변경
    /// </summary>
    /// <param name="_type">char1 or char2</param>
    /// <param name="_num">type num</param>
    void SetEmotion(int _talker)
    {
        for (int i = 0; i < 3; i++)
        {
            textBoxes[i].gameObject.SetActive(false);
            // emoteImgs[i].gameObject.SetActive(false);
        }
        textBoxes[_talker].gameObject.SetActive(true);
        //emoteImgs[_talker].gameObject.SetActive(true);

        /* switch (emotionTypes[_talker])
         {
             case EmotionType.NORMAL:
                 emoteImgs[_talker].gameObject.SetActive(false);
                 break;
             case EmotionType.ANGRY:
                // textBoxes[_talker].sprite = Resources.Load<Sprite>(Defines.SPRITE_TEXTBOX_ANGRY) as Sprite;
                 emoteImgs[_talker].sprite = Resources.Load<Sprite>(Defines.SPRITE_EMOTE_ANGRY) as Sprite;
                 emoteImgs[_talker].transform.position = emotePositions[_talker][3].position;
                 emoteAnims[_talker].SetInteger("CharType", 1); //emotion Type 1
                 charAnims[_talker].SetInteger("CharType", (int)emotionTypes[_talker]);
                 break;
             case EmotionType.QUESTION:
                 break;
             case EmotionType.SAD:
                 break;
             case EmotionType.ANNOYING:
                 break;
             case EmotionType.SWEAT:
                 break;
             case EmotionType.SPIT:
                 break;
             case EmotionType.SHOUT:
                 break;
             case EmotionType.DIZZY:
                 break;
             case EmotionType.SHOT:
                 break;
             case EmotionType.JOY:
                 break;
             case EmotionType.RUN:
                 break;
             case EmotionType.LAUGH:
                 break;
             case EmotionType.HIT:
                 break;
             default:
                 break;
         }*/
    }

    /// <summary>
    /// 텍스트박스의 모양 결정,
    /// 껐다 켜서 팝업 애니메이션 재생
    /// </summary>
    /// <param name="_box">변경할 이미지</param>
    /// <param name="_type">현재 상태</param>
    void SetTextBox(Image _box, Image _emt, EmotionType _type)
    {
        _box.gameObject.SetActive(false);
        _emt.gameObject.SetActive(false);
        switch (_type)
        {
            case EmotionType.NORMAL:
                _box.sprite = gameMgr.b_sprites.LoadAsset<Sprite>(Defines.SPRITE_TEXTBOX_NORMAL);
                //_emt.sprite = Resources.Load<Sprite>(Defines.RESOURCE_SPRITE_TEXTBOX_NORMAL);
                // _emt.GetComponent<Animator>().SetInteger("CharType", 0);
                break;
            case EmotionType.ANGRY:
                _box.sprite = gameMgr.b_sprites.LoadAsset<Sprite>(Defines.SPRITE_TEXTBOX_ANGRY);
                //_emt.sprite = Resources.Load<Sprite>(Defines.RESOURCE_SPRITE_TEXTBOX_ANGRY);
                _emt.gameObject.SetActive(true);
                break;
            case EmotionType.QUESTION:
                _box.sprite = gameMgr.b_sprites.LoadAsset<Sprite>(Defines.SPRITE_TEXTBOX_HAPPY);
                //_emt.sprite = Resources.Load<Sprite>(Defines.RESOURCE_SPRITE_TEXTBOX_HAPPY);
                _emt.gameObject.SetActive(true);
                break;
            case EmotionType.SAD:
                _box.sprite = gameMgr.b_sprites.LoadAsset<Sprite>(Defines.SPRITE_TEXTBOX_SAD);
                // _emt.sprite = Resources.Load<Sprite>(Defines.RESOURCE_SPRITE_TEXTBOX_SAD);
                _emt.gameObject.SetActive(true);
                break;
        }
        _box.gameObject.SetActive(true);
    }

    /// <summary>
    /// 화면 터지 시 동작
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        currentIndex++;
        gameMgr.soundMgr.PlaySfx(this.transform.position, gameMgr.soundMgr.sfx_bt);
        NextDialog();
    }

    /// <summary>
    /// 대화 모드 종료 및 진행
    /// </summary>
    public void EndDialog()
    {
        //dialogUI.gameObject.SetActive(false);
        Init();
        this.gameObject.SetActive(false);
        gameMgr.soundMgr.PlayBgm(null);
        gameMgr.stage.SetActive(true);
        gameMgr.uiMgr.ShowGoal();
    }
}
