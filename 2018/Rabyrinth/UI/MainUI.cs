using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rabyrinth.ReadOnlys;
public class MainUI : MonoBehaviour
{
    private GameManager GameMgr;

    /////////////////차일드//////////////////////////////
    private GameObject TopBar;
    private GameObject LoadingPanel;
    private GameObject KpmBar;
    private Slider SpBar;
    private Image SpBar_Color;
    private GameObject MainMenu;
    private GameObject DamagePool;
    private GameObject HpBarPool;
    private GameObject CharDamagePool;
    private GameObject RewordTextPool;
    private GameObject PopUp;
    private GameObject Inventory;
    private GameObject TouchExitPop;
    private GameObject ImportantPop;
    public HP_Bar PlayerHpBar;
    public HP_Bar TargetHpBar;
    private Slider Kpm_Blue;
    private Slider Kpm_Red;
    private RectTransform Kpm_Icon;
    //private List<DamageText> lDamageText;
    public Queue<DamageText> qDamageText;
    public Queue<DamageText> qCharDamageText;
    public Queue<DamageText> qRewordText;
    //public Queue<HP_Bar> qHp_Bar;
    public GameObject PlayerHead;

    public PopUpController popUpController { get; private set; }
    public SkillController SkillCtrl { get; private set; }
    public Text Log;

    public Text Level;
    public Text Crystal;
    public Text KPM;
    public Text Floor;
    public Text RewardTime;
    public Text Cur_KPM;

    public Text Log2;

    public GameObject GstarButton;
    public GameObject GstarButton_BG;

    public int getCrystal { get; set; }
    /// ///////////////////////////////////////////////

    ////////////////스핀버튼///////////////////
    public bool spinonoff = false;
    public bool spin2off = false;

    private Color Green = new Color(Color.green.r, Color.green.g, Color.green.b, 0.6f);
    private Color Red = new Color(Color.red.r, Color.red.g, Color.red.b, 0.6f);
    private Color Cyan = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.6f);

    ///////////////////////////////////////////
    private Button BossButton;
    private Button FloorButton;
    private Animator FloorAni;

    private void Awake()
    {
        GameMgr = MonoSingleton<GameManager>.Inst;
        TopBar = transform.GetChild((int)UI_GetChild.TopBar).gameObject;
        LoadingPanel = transform.GetChild(transform.childCount - 1).gameObject;
        SetLoading(true);
        KpmBar = transform.GetChild((int)UI_GetChild.KpmBar).gameObject;
        SpBar = transform.GetChild((int)UI_GetChild.SpBar).GetComponent<Slider>();
        SpBar_Color = SpBar.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        SkillCtrl = transform.GetChild((int)UI_GetChild.SkillButton).GetComponent<SkillController>();
        MainMenu = transform.GetChild((int)UI_GetChild.MainMenu).gameObject;
        PopUp = transform.GetChild((int)UI_GetChild.PopupMenu).GetChild(0).gameObject;
        popUpController = PopUp.transform.parent.GetComponent<PopUpController>();

        Inventory = transform.GetChild((int)UI_GetChild.PopupMenu).GetChild(1).gameObject;
        TouchExitPop = transform.GetChild((int)UI_GetChild.PopupMenu).GetChild(2).gameObject;
        ImportantPop = transform.GetChild((int)UI_GetChild.PopupMenu).GetChild(3).gameObject;
        DamagePool = transform.GetChild((int)UI_GetChild.DamagePool).gameObject;
        HpBarPool = transform.GetChild((int)UI_GetChild.HpBarPool).gameObject;
        CharDamagePool = transform.GetChild((int)UI_GetChild.CharDamagePool).gameObject;
        RewordTextPool = transform.GetChild((int)UI_GetChild.RewordTextPool).gameObject;

        FloorButton = TopBar.transform.GetChild(0).GetChild(0).GetComponent<Button>();
        BossButton = TopBar.transform.GetChild(0).GetChild(1).GetComponent<Button>();
        FloorAni = FloorButton.transform.GetChild(0).GetComponent<Animator>();

        PlayerHpBar = transform.GetChild((int)UI_GetChild.PlayerHpBar).GetComponent<HP_Bar>();
        TargetHpBar = transform.GetChild((int)UI_GetChild.Target_HpBar).GetComponent<HP_Bar>();
        //lDamageText = new List<DamageText>(DamagePool.GetComponentsInChildren<DamageText>(true));
        qDamageText = new Queue<DamageText>(DamagePool.GetComponentsInChildren<DamageText>(true));
        qCharDamageText = new Queue<DamageText>(CharDamagePool.GetComponentsInChildren<DamageText>(true));
        qRewordText = new Queue<DamageText>(RewordTextPool.GetComponentsInChildren<DamageText>(true));
        //qHp_Bar = new Queue<HP_Bar>(HpBarPool.GetComponentsInChildren<HP_Bar>(true));
        MainMenu.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => Spinbutton());
        // MainMenu.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => ResetSpin());
        Kpm_Blue = transform.GetChild((int)UI_GetChild.KpmBar).GetChild(0).GetComponent<Slider>();
        Kpm_Red = transform.GetChild((int)UI_GetChild.KpmBar).GetChild(1).GetComponent<Slider>();
        Kpm_Icon = transform.GetChild((int)UI_GetChild.KpmBar).GetChild(3).GetComponent<RectTransform>();

        getCrystal = 0;

        //UI 텍스트 세팅
        StartCoroutine(SetUIText());

    }
    public IEnumerator SetUIText(bool _isEvent = false)
    {
        while (GameMgr.PlayData == null)
            yield return null;

        while (GameMgr.PlayData.PlayerData == null)
            yield return null;

        if (!_isEvent)
        {
            Level.text = "LV. " + GameMgr.PlayData.PlayerData.MaxFloor.ToString();
            Crystal.text = GameMgr.PlayData.PlayerData.Gold.ToString();
            string str = GameMgr.PlayData.PlayerData.KPM.ToString();
            string newStr = " ";
            for(int index = 0; index < str.Length; index++)
                newStr += str.Substring(index, index) + " ";

            KPM.text = str;
            Floor.text = GameMgr.PlayData.PlayerData.CurrentFloor.ToString() + "<size=30>층</size>";
            //RewardTime.text = GameMgr.PlayData.PlayerData.MaxFloor.ToString();
        }
        else
        {
            Level.text = "LV. 100";
            Crystal.text = "0";
            KPM.text = "<size=30>" + GameMgr.PlayData.lRankingData[0].Name + "</size>";
            Floor.text = "<size=35>Event</size>";
            //RewardTime.text = GameMgr.PlayData.PlayerData.MaxFloor.ToString();
        }
    }

    public void SetLoading(bool _bool)
    {
        if (LoadingPanel != null)
            LoadingPanel.SetActive(_bool);
    }

    ////////////////////////////////MainMenu//////////////////////////////////////////////////////////////
    public void Spinbutton() { StartCoroutine(Flip()); }
    //public void ResetSpin() { StartCoroutine("BackFlip"); }
    IEnumerator Flip()
    {
        if (spinonoff == false)
        {
            spinonoff = true;

            // 기본 로테이션이 180인 상태에서, z값이 0보다 클 경우 회전
            while (MainMenu.transform.rotation.z > 0)
            {
                //버튼을 회전시킴
                MainMenu.transform.Rotate(new Vector3(0, 0, -180 * Time.deltaTime * 3));
                yield return null;
            }
            //버튼이 맞지 않을 경우를 대비해 로테이션값 초기화
            MainMenu.transform.rotation = Quaternion.Euler(0, 0, 0);
            yield return new WaitForSeconds(1.5f);
            if (spinonoff == true && spin2off == false)
                StartCoroutine(BackFlip());
            
        }
    }
    IEnumerator BackFlip()
    {
        if (spin2off == false)
        {
            spin2off = true;
            //회전하는 버튼이 180도 보다 작을동안 실행
            while (MainMenu.transform.rotation.z < 1 && MainMenu.transform.rotation.w > 0)
            {
                MainMenu.transform.Rotate(new Vector3(0, 0, 180 * Time.deltaTime * 2));
                yield return null;
            }
            MainMenu.transform.rotation = Quaternion.Euler(0, 0, 180);
            yield return new WaitForSeconds(0.2f);
            spinonoff = false;
            spin2off = false;
        }
    }





    //////////////////////////////////////////////////////////////////////////////////////////////////


    /////////////////////////////////////////데미지 텍스트//////////////////////////////////////////////

    public void SetDamageText(DamageText_Type _Type , Vector3 targetpos, int damage, bool isCritical = false)
    {
        Vector3 newPos = Camera.main.WorldToScreenPoint(targetpos);


        if (_Type == DamageText_Type.Player && qCharDamageText.Count > 0)
        {
            newPos = new Vector3((newPos.x - Screen.width * 0.5f), (newPos.y - Screen.height * 0.5f) + 150);
            qCharDamageText.Dequeue().Init(qCharDamageText,DamageText_Type.Player ,  newPos, damage.ToString(), isCritical);
        }
        else if (_Type == DamageText_Type.Reword && qRewordText.Count > 0)
        {
            getCrystal += damage;
            Crystal.text = GameMgr.PlayData.PlayerData.Gold.ToString();
            newPos = new Vector3((newPos.x - Screen.width * 0.5f), (newPos.y - Screen.height * 0.5f));
            qRewordText.Dequeue().SetRewordText(qRewordText, newPos, damage.ToString());
        }
        else if (qDamageText.Count > 0)
        {
            newPos = new Vector3((newPos.x - Screen.width * 0.5f), (newPos.y - Screen.height * 0.5f) + 300);
            qDamageText.Dequeue().Init(qDamageText, DamageText_Type.NPC, newPos, damage.ToString(), isCritical);
        }


    }
    /////////////////////////////////////////////체력게이지////////////////////////////////////////////////////////
    public void UpdatePlayerHpBar()
    {
        if (GameMgr.Player.MainTarget == null)
            return;

        Vector3 newPos = Camera.main.WorldToScreenPoint(GameMgr.Player.transform.localPosition);
        newPos = new Vector3((newPos.x - Screen.width * 0.5f), (newPos.y - Screen.height * 0.5f) + 130);
        PlayerHpBar.transform.localPosition = newPos;
    }

    public void UpdateNPC_HpBarPosition()
    {
        if (GameMgr.Player.MainTarget == null)
            return;

        Vector3 newPos = Camera.main.WorldToScreenPoint(GameMgr.Player.MainTarget.transform.localPosition);
        newPos = new Vector3((newPos.x - Screen.width * 0.5f), (newPos.y - Screen.height * 0.5f) + 130);
        TargetHpBar.transform.localPosition = newPos;
    }

    public void UpdateNPC_HpBarValue()
    {
        if (TargetHpBar.Hp_Bar == null)
            return;

        float EnemyHP = (float)GameMgr.Player.MainTarget.Status.HP / (float)GameMgr.Player.MainTarget.Status.MaxHP;
        TargetHpBar.Hp_Bar.value = EnemyHP;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////팝업창 관리////////////////////////////////////////////////
   public void ImportantPopup()
    {
        TouchExit();
        ImportantPop.gameObject.SetActive(true);
    }
    public void ExitImportantPop()
   {
       ImportantPop.gameObject.SetActive(false);
   }

    //중요한 팝업이 뜰때 현재 있는 팝업창 다 닫음
    public void TouchExit()
    {
        ExitPop();
        ExitInv();
    }

    public void PushPopUp()
    {
        PopUp.gameObject.SetActive(true);
        TouchExitPop.gameObject.SetActive(true);
    }
    public IEnumerator PopUpActive()
    {
        PopUp.gameObject.SetActive(true);
        TouchExitPop.gameObject.SetActive(true);
        PopUp.gameObject.transform.localPosition= new Vector3(-1440, 0, 0);
        while(PopUp.gameObject.transform.localPosition.x <= -470)    
        {
            yield return null;
            PopUp.gameObject.transform.localPosition = new Vector3(PopUp.gameObject.transform.localPosition.x + 145, 0, 0);
            GameMgr.Main_Cam.rect = new Rect(GameMgr.Main_Cam.rect.x + 0.075f,0f,1f,1f);
        }
        GameMgr.Main_Cam.rect = new Rect(0.5f, 0f, 1f, 1f);
        PopUp.gameObject.transform.localPosition = new Vector3(-480, 0, 0);
    }
    public void ExitPop()
    {
        if (TouchExitPop.activeSelf == true)
        {
            TouchExitPop.gameObject.SetActive(false);
            StartCoroutine(ExitPopUp());    
        }
        
    }
    public IEnumerator ExitPopUp()
    {
        while (PopUp.gameObject.transform.localPosition.x >= -1440)
        {
            yield return null;
            PopUp.gameObject.transform.localPosition = new Vector3(PopUp.gameObject.transform.localPosition.x - 145, 0, 0);
            GameMgr.Main_Cam.rect = new Rect(GameMgr.Main_Cam.rect.x - 0.075f, 0f, 1f, 1f);
        }
        GameMgr.Main_Cam.rect = new Rect(0f, 0f, 1f, 1f);
        PopUp.gameObject.SetActive(false);
    }
    public void PushInventory()
    {
        Inventory.gameObject.SetActive(true);
        TouchExitPop.gameObject.SetActive(true);
    }
    public void PopUpInventory()
    {
        Inventory.gameObject.SetActive(true);
        TouchExitPop.gameObject.SetActive(true);
        //Inventory.gameObject.transform.localPosition = new Vector3(-1440, 0, 0);
        //while (Inventory.gameObject.transform.localPosition.x <= -470)
        //{
        //    yield return null;
        //    Inventory.gameObject.transform.localPosition = new Vector3(Inventory.gameObject.transform.localPosition.x + 145, 0, 0);
        //    GameMgr.Main_Cam.rect = new Rect(GameMgr.Main_Cam.rect.x + 0.075f, 0f, 1f, 1f);
        //}
        //GameMgr.Main_Cam.rect = new Rect(0.5f, 0f, 1f, 1f);
        //Inventory.gameObject.transform.localPosition = new Vector3(-480, 0, 0);
    }
    public void ExitInv()
    {
        if (TouchExitPop.gameObject==true)
        {
            TouchExitPop.gameObject.SetActive(false);
            StartCoroutine(ExitInventory());    
        }
        
    }
    public IEnumerator ExitInventory()
    {
        while (Inventory.gameObject.transform.localPosition.x >= -1440)
        {
            yield return null;
            Inventory.gameObject.transform.localPosition = new Vector3(Inventory.gameObject.transform.localPosition.x - 145, 0, 0);
            GameMgr.Main_Cam.rect = new Rect(GameMgr.Main_Cam.rect.x - 0.075f, 0f, 1f, 1f);
        }
        GameMgr.Main_Cam.rect = new Rect(0f, 0f, 1f, 1f);
        Inventory.gameObject.SetActive(false);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////KPM Bar/////////////////////////////////////////////////////
    public void KpmSet(int _MaxKpm, int _CurKpm)
    {
        float _value = 0.0f;

        if (_CurKpm == 0)
            return;

        if (_CurKpm < _MaxKpm)
        {
            _value = 1.0f - (float)_CurKpm / (float)_MaxKpm;
            Kpm_Red.value = _value;
            Kpm_Blue.value = 0.0f;
            Kpm_Icon.localPosition = new Vector2(-(_value * 212.0f), 40.0f);
        }
        else
        {
            _value = 1.0f - (float)_MaxKpm / (float)_CurKpm;
            Kpm_Blue.value = _value;
            Kpm_Red.value = 0.0f;
            Kpm_Icon.localPosition = new Vector2(_value * 212.0f, 40.0f);
        }
    }

    public void KpmInit()
    {
        Kpm_Red.value = 0.0f;
        Kpm_Blue.value = 0.0f;
    }

    public void SpBarSet(int _SP)
    {
        float Sp = _SP * 0.1f;
        SpBar.value = Sp;
        if (Sp < 0.5f)
            SpBar_Color.color = Red;
        else if (Sp < 1.0f)
            SpBar_Color.color = Green;
        else
            SpBar_Color.color = Cyan;
    }



    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void Reset_UI()
    {
        SkillCtrl.ReleaseMagic();

        if (popUpController != null)
            popUpController.PopUpReset();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void SetBossButton(bool _bool)
    {
        if (!_bool)
            BossButton.image.color = new Color(0.33f, 0.33f, 0.33f);
        else
            BossButton.image.color = Color.white;

        BossButton.enabled = _bool;

        FloorAni.SetBool("isPop", _bool);

        //if (PlayerPrefs.GetInt("BossButton") != 0 && _bool)
        //    BossButton.gameObject.SetActive(_bool);
        BossButton.gameObject.SetActive(_bool);
        if (!_bool)
            PlayerPrefs.SetInt("BossButton", 0);
    }

    public void ClickFloorButton()
    {
        if (GameMgr.spawnManager.NPC_Boss.gameObject.activeSelf)
            return;

        if(BossButton.gameObject.activeSelf)
            BossButton.gameObject.SetActive(false);
        else
            BossButton.gameObject.SetActive(true);
    }

    public void ClickBossButton()
    {
        if (GameMgr.isEvent)
            return;

        BossButton.gameObject.SetActive(false);

        FloorAni.SetBool("isPop", false);

        GameMgr.spawnManager.BossSpawn();
    }

}
