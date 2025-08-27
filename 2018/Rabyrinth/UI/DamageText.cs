using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rabyrinth.ReadOnlys;
public class DamageText : MonoBehaviour {
    private GameManager GameMgr = null;
    Text UI_Text;

    public void Awake()
    {
        GameMgr = MonoSingleton<GameManager>.Inst;
        UI_Text = GetComponent<Text>();
    }

    public void SetRewordText(Queue<DamageText> _qPool, Vector3 _pos, string _str)
    {
        gameObject.SetActive(true);

        UI_Text.rectTransform.sizeDelta = new Vector2(90 + 17.5f * _str.Length, UI_Text.rectTransform.rect.height);
        UI_Text.text = "+ " + _str;
        StartCoroutine(SetAction(_qPool));
    }

    public void Init(Queue<DamageText> _qDamageText,DamageText_Type _DType , Vector3 pos, string strDam, bool isCritical = false)
    {
        gameObject.SetActive(true);
        transform.localPosition = pos;

        if (_DType == DamageText_Type.NPC)
            UI_Text.color = isCritical ? Color.yellow : Color.white;
        else
            UI_Text.color = isCritical ? Color.magenta : Color.red;

        UI_Text.text = strDam;

        StartCoroutine(setInActive(_qDamageText));
    }

    IEnumerator setInActive(Queue<DamageText> _qDamageText)
    {
        float time = 0.0f;

        while (time <= 0.2f)
        {
            //time += Time.deltaTime;

            //if (time >= 0.4f)
            //{
            //    alpha = 1.0f - time / 1.0f;
            //    SetAlpha(UI_Text, alpha);
            //}
            //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 4, 0);
            time += Time.deltaTime;
            UI_Text.fontSize = 200 - (int)(time*10*70);
            
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        UI_Text.fontSize = 150;
        _qDamageText.Enqueue(this);

        gameObject.SetActive(false);
        SetAlpha(UI_Text);
    }

    IEnumerator SetAction(Queue<DamageText> _qPool)
    {
        float time = 0.0f;
        float alpha = 0.0f;
        while (time <= 0.2f)
        {
            time += Time.deltaTime;

            if (time >= 0.4f)
            {
                alpha = 1.0f - time / 1.0f;
                SetAlpha(UI_Text, alpha);
            }
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 4, 0);

            yield return null;
        }
        _qPool.Enqueue(this);

        gameObject.SetActive(false);
        SetAlpha(UI_Text);
    }

    private void SetAlpha(Text _text, float _alpha = 1.0f)
    {
        _text.color = new Color(
            _text.color.r,
            _text.color.g,
            _text.color.b,
            _alpha);
    }
}
