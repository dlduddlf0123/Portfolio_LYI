using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rabyrinth.ReadOnlys;
public class PopUpController : MonoBehaviour {

    private GameManager GameMgr;
    /// ///////////////////////////일반팝업////////////////////////////////////////
    
    private Button[] Buttons;
    private GameObject[] Panels;

    public Button[] SkillButtons { get; private set; }
    /// ///////////////////////////인벤토리////////////////////////////////////////////////

    private Button[] IButtons;
    private GameObject[] IPanels;

    public Animator[] PopUpAni;

    public GameObject ExitPanel { get; set; }

    public InputField EventInput;

    public GameObject EventPop;

    /// ///////////////////////////중앙 팝업////////////////////////////////////////////////
    /// 
    private PopUp middlePop;

    public System.Action callBack_GEvent;

    private System.Action PopUpCallBack;
    private void Awake()
    {
        callBack_GEvent = null;

        GameMgr = MonoSingleton<GameManager>.Inst;
        ExitPanel = transform.GetChild(2).gameObject;
        middlePop = new PopUp
        {
            gameObject = transform.GetChild(3).gameObject,
            text = transform.GetChild(3).GetChild(2).GetComponent<Text>(),
        };
        PopUpCallBack = null;
        /// ///////////////////////////일반팝업////////////////////////////////////////
        Buttons = new Button[4];
        Panels = new GameObject[4];
        IButtons = new Button[4];
        IPanels = new GameObject[4];
        for (int i = 0; i < 3; i++)
        {
            Buttons[i] = transform.GetChild(0).GetChild(1).GetChild(i+3).GetComponent<Button>();
            Panels[i] = transform.GetChild(0).GetChild(2).GetChild(i).gameObject;
            
        }

        SkillButtons = new Button[8];
        for (int i = 0; i < 8; i++)
            SkillButtons[i] = transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Button>();

        ButtonReset();

        //////////////////////////////인벤토리////////////////////////////////////////////////

        for (int i = 0; i < 4; i++)
        {
            IButtons[i] = transform.GetChild(1).GetChild(0).GetChild(i + 4).GetComponent<Button>();
            IPanels[i] = transform.GetChild(1).GetChild(2).GetChild(0).GetChild(i).gameObject;
        }
        ResetIButtons();
        PopIButton1();
	}

    public void PopUpMiddle(string _text, System.Action _callBack)
    {
        ExitButton(-1);

        StartCoroutine(PopTextAction(_text));
        middlePop.gameObject.SetActive(true);

        PopUpCallBack = _callBack;
        StartCoroutine(WaitMiddlePop());
    }

    private IEnumerator PopTextAction(string _text)
    {
        middlePop.text.text = "";
        string[] arrStr = _text.Split(',');

        for(int index = 0; index < arrStr.Length; index++)
        {
            middlePop.text.text += arrStr[index];
            yield return new WaitForSeconds(0.1f);
        }

    }

    private IEnumerator WaitMiddlePop()
    {
        yield return new WaitForSeconds(10.0f);
        ClickOkPopUpMiddle();
    }

    public void ClickOkPopUpMiddle()
    {
        middlePop.gameObject.SetActive(false);

        if (PopUpCallBack != null)
            PopUpCallBack();

        PopUpCallBack = null;

        StopAllCoroutines();
    }
    
    public void PopUpReset()
    {
        PopUpAni[0].SetBool(Defines.ANI_PARAM_POP, false);
        PopUpAni[1].SetBool(Defines.ANI_PARAM_POP, false);
        ExitPanel.SetActive(false);
    }


    /// ///////////////////////////일반팝업////////////////////////////////////////
    public void ButtonReset()
    {  
   //     for (int i = 0; i < 3; i++)
			//{
   //             Buttons[i].gameObject.SetActive(false);
   //             Panels[i].gameObject.SetActive(false);
			//}
    }
    public void PopButton(int _index)
    {
        GameMgr.Main_UI.GstarButton.SetActive(false);
        GameMgr.Main_UI.GstarButton_BG.SetActive(false);

        ButtonReset();
        Panels[_index].gameObject.SetActive(true);
        PopUpAni[_index].SetBool(Defines.ANI_PARAM_POP, true);
        ExitPanel.SetActive(true);

        for (int index = 0; index < SkillButtons.Length; index++)
            SkillButtons[index].image.color = Color.white;

        for (int index = 0; index < 4; index++)
            SkillButtons[GameMgr.Main_UI.SkillCtrl.nActiveSkills[index]].image.color = Color.gray;
    }

    public void ExitButton(int _index)
    {
        if (!GameMgr.isEvent)
        {
            GameMgr.Main_UI.GstarButton.SetActive(true);
            GameMgr.Main_UI.GstarButton_BG.SetActive(true);
        }

        GameMgr.Main_UI.SkillCtrl.isSkillChange = false;

        for (int index = 0; index < 4; index++)
            GameMgr.Main_UI.SkillCtrl.SkillButton_Anis[index].SetBool(Defines.ANI_PARAM_POP, false);

        if (_index != -1)
            PopUpAni[_index].SetBool(Defines.ANI_PARAM_POP, false);
        else
            for(int index = 0; index < 2; index++)
                if(PopUpAni[index].GetBool(Defines.ANI_PARAM_POP))
                    PopUpAni[index].SetBool(Defines.ANI_PARAM_POP, false);

        ExitPanel.SetActive(false);

        if(callBack_GEvent!=null)
        {
            callBack_GEvent();
            callBack_GEvent = null;
        }
    }

    //////////////////////////////인벤토리////////////////////////////////////////////////
    public void ResetIButtons()
    {
         for (int i = 0; i < 4; i++)
			{
               IButtons[i].gameObject.SetActive(false);
               IPanels[i].gameObject.SetActive(false);
			}
    }
    public void PopIButton1()
    {
        ResetIButtons();
        IButtons[0].gameObject.SetActive(true);
        IPanels[0].gameObject.SetActive(true);
    }
    public void PopIButton2()
    {
        ResetIButtons();
        IButtons[1].gameObject.SetActive(true);
        IPanels[1].gameObject.SetActive(true);
    }
    public void PopIButton3()
    {
        ResetIButtons();
        IButtons[2].gameObject.SetActive(true);
        IPanels[2].gameObject.SetActive(true);
    }
    public void PopIButton4()
    {
        ResetIButtons();
        IButtons[3].gameObject.SetActive(true);
        IPanels[3].gameObject.SetActive(true);
    }

    public void PopUpEvent(System.Action _callBack)
    {
        EventPop.gameObject.SetActive(true);

        PopUpCallBack = _callBack;
    }

    public void PopUpEventEnter()
    {
        if (EventInput.text == "")
            return;

        EventPop.gameObject.SetActive(false);

        GameMgr.EventName = EventInput.text;

        GameMgr.Main_UI.GstarButton.SetActive(false);
        GameMgr.Main_UI.GstarButton_BG.SetActive(false);

        PopUpCallBack();
        PopUpCallBack = null;
    }

    public void EventButton()
    {
        System.Action<bool> action= (_bool) => {
            if (_bool)
            {
                System.Action action1 = () => { GameMgr.spawnManager.GStarEvent(); };
                GameMgr.AWS_Mgr.LoadRankingData(action1);
            }
            else
            {
                System.Action action2 = () => { GameMgr.GameRestart(); };
                GameMgr.Main_UI.popUpController.PopUpMiddle("인터넷 연결 오류 입니다.,\n게임을 재시작합니다.,\n스태프에게 문의해주세요", action2);
            }
        };
        GameMgr.AWS_Mgr.SaveRankingData("NAME", 0.0f, 1, action);
    }

    public void EventPopExit()
    {
        EventPop.gameObject.SetActive(false);
        PopUpCallBack = null;
    }
}

public class PopUp
{
    public GameObject gameObject;
    public Text text;
    public Animator aniCtrl;
}