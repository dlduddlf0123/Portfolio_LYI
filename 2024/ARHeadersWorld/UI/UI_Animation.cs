using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// 12/19/2023-LYI
/// 애니메이션 선택 창
/// 선택된 애니메이션이 적용되고 화면 터치시 재생할 수 있게 된다
/// </summary>
public class UI_Animation : MonoBehaviour
{

    GameManager gameMgr;

    public RectTransform tr_scrollView;
    public RectTransform tr_content;
    public RectTransform tr_disable;

    [Header("Buttons")]
    public Button btn_toggleWindow;

    public GameObject btn_origin;

    public List<GameObject> list_btnPool = new List<GameObject>();
    public List<GameObject> list_btnActive = new List<GameObject>();

    Dictionary<string, float> dic_clipLength = new Dictionary<string, float>();

    [Header("Status")]
    public string currentAnimation = null;
    public TextMeshProUGUI txt_currentAnim;

    [SerializeField]
    bool isScrollActive = true;

    Coroutine currentCoroutine = null;

    bool isFirst = true;
    private void Awake()
    {
        gameMgr = GameManager.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        btn_toggleWindow.onClick.AddListener(ToggleAnimationUI);
    }

    public void ToggleAnimationUI()
    {
        if (isScrollActive == true)
        {
            isScrollActive = false;
            SetAnimationUI(isScrollActive);
        }
        else
        {
            isScrollActive = true;
            SetAnimationUI(isScrollActive);
        }
    }

    public void SetAnimationUI(bool isActive)
    {
        if (isActive)
        {
            tr_scrollView.anchoredPosition = new Vector2(tr_scrollView.anchoredPosition.x, 0);
            txt_currentAnim.gameObject.SetActive(true);
            isScrollActive = true;
        }
        else
        {
            tr_scrollView.anchoredPosition = new Vector2(tr_scrollView.anchoredPosition.x, -Screen.height * 0.5f);
            txt_currentAnim.gameObject.SetActive(false);
            isScrollActive = false;
        }
    }

    /// <summary>
    /// 12/19/2023-LYI
    /// 현재 선택된 캐릭터 기준 애니메이션 긁어오기
    /// </summary>
    public void AnimationUIInit()
    {
        currentAnimation = null;
        txt_currentAnim.text = "Null";

        if (gameMgr.spawnARCharacter == null)
        {
            Debug.Log("Current character is null!!!");
            return;
        }

        for (int i = 0; i < gameMgr.spawnARCharacter.m_animator.runtimeAnimatorController.animationClips.Length; i++)
        {
            string s = null;
            s = gameMgr.spawnARCharacter.m_animator.runtimeAnimatorController.animationClips[i].name;

            GameObject btn = gameMgr.objPoolingMgr.CreateObject(list_btnPool, btn_origin, Vector3.zero, tr_content);
            btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = s;

            btn.GetComponent<Button>().onClick.AddListener(() => ButtonSetAnimation(s));

            if (!dic_clipLength.ContainsKey(s))
            {
                dic_clipLength.Add(s, gameMgr.spawnARCharacter.m_animator.runtimeAnimatorController.animationClips[i].length);

            }
            list_btnActive.Add(btn);
        }

        list_btnActive.OrderByDescending(btn => btn.name);

        int height = 50 + 175;

        if (gameMgr.spawnARCharacter.m_animator.runtimeAnimatorController.animationClips.Length % 3 == 0)
        {
            height = 50 + 175 * gameMgr.spawnARCharacter.m_animator.runtimeAnimatorController.animationClips.Length / 3 + 1;
        }
        else
        {
            height = 50 + 175 * (gameMgr.spawnARCharacter.m_animator.runtimeAnimatorController.animationClips.Length / 3 + 2);
        }

        tr_scrollView.sizeDelta = new Vector3(tr_scrollView.sizeDelta.x, Screen.height * 0.5f);
        tr_content.sizeDelta = new Vector2(tr_content.sizeDelta.x, height);


        if (isFirst)
        {
            SetAnimationUI(true);
            isFirst = false;
        }
        else
        {
            SetAnimationUI(!isScrollActive);
        }

    }


    public void ButtonReset()
    {
        if (list_btnActive.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < list_btnActive.Count; i++)
        {
            gameMgr.objPoolingMgr.ObjectInit(list_btnPool, list_btnActive[i], tr_disable);
        }
        list_btnActive.Clear();
    }

    public void ButtonSetAnimation(string name)
    {
        currentAnimation = name;
        txt_currentAnim.text = currentAnimation.ToString();

        if (gameMgr.spawnARCharacter == null)
        {
            Debug.Log("Current character is null!!!");
            return;
        }

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        currentCoroutine = StartCoroutine(WaitForAnimationThenPlayDefault());

        gameMgr.spawnARCharacter.m_animator.Play(currentAnimation, -1, 0);
    }

    public void PlayCurrentAnimation()
    {
        if (gameMgr.spawnARCharacter == null)
        {
            Debug.Log("Current character is null!!!");
            return;
        }

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        currentCoroutine = StartCoroutine(WaitForAnimationThenPlayDefault());

        gameMgr.spawnARCharacter.m_animator.Play(currentAnimation, -1, 0);
    }

    IEnumerator WaitForAnimationThenPlayDefault()
    {
        if (currentAnimation != null)
        {
            // 현재 재생 중인 애니메이션의 길이를 기다림
            yield return new WaitForSeconds(dic_clipLength[currentAnimation]);

            // Default state 재생
            gameMgr.spawnARCharacter.m_animator.Play("DefaultState");
        }
    }
}