using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Logo : MonoBehaviour
{
    private Image Logo_Image;
    private GameManager GameMgr;

    private void Awake()
    {
        GameMgr = MonoSingleton<GameManager>.Inst;

        Logo_Image = transform.GetChild(0).GetComponent<Image>();

        StartCoroutine(SceneLoad());
    }

    // 로고 보여지기 시작후 2초 대기
    private IEnumerator SceneLoad()
    {
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(FadeAction());
    }

    // 로고 페이드 액션 수행후 게임 매니저 데이터 로딩 및 초기화가 완료 되었으면 씬 로드
    private IEnumerator FadeAction()
    {
        float time = 0.0f;
        float alpha = 0.0f;

        while (time < 1.0f)
        {
            alpha = 1.0f - time / 1.0f;
            time += Time.deltaTime;

            SetAlpha(Logo_Image, alpha);

            yield return null;
        }
        GameMgr.LogoEnd();
        Destroy(this.gameObject);
    }

    private void SetAlpha(Image _sprite, float _alpha = 1.0f)
    {
        _sprite.color = new Color(
            _sprite.color.r,
            _sprite.color.g,
            _sprite.color.b,
            _alpha);
    }
}
