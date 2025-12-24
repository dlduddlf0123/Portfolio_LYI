using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformUI : MonoBehaviour
{
    GameManager gameMgr;

    Text txt_center;


    void Awake()
    {
        gameMgr = GameManager.Instance;
        txt_center = transform.GetChild(1).GetComponent<Text>();
    }
    private void Start()
    {
        GetComponent<Canvas>().worldCamera = gameMgr.mainCam;
    }

    public void OnStartUI()
    {
        txt_center.text = "Start";
        StartCoroutine(UIShortActive(txt_center.gameObject, 2));
    }

    public void OnClearUI()
    {
        txt_center.text = "Clear";
        StartCoroutine(UIFleek(txt_center.gameObject, false));
    }

    public IEnumerator UIShortActive(GameObject _go, float _time)
    {
        _go.SetActive(true);
        yield return new WaitForSeconds(_time);
        _go.SetActive(false);
    }

    public IEnumerator UIFleek(GameObject _go, bool _active)
    {
        for (int i = 0; i < 5; i++)
        {
            _go.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            _go.SetActive(true);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.5f);
        _go.gameObject.SetActive(_active);
    }
    
}
