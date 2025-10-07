using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 레이와 상호작용하는 큐브 버튼
/// </summary>
public class UICube : MonoBehaviour
{
    public Text toggleText;
    public Character header;
    void Awake()
    {
       toggleText =  transform.parent.GetChild(1).GetChild(0).GetComponent<Text>();
    }

    private void Start()
    {
        if (header.gameObject.activeSelf)
        {
            toggleText.text = "On";
        }
        else
        {
            toggleText.text = "Off";
        }
    }
    public void ToggleCharacter()
    {
        if (header.gameObject.activeSelf)
        {
            header.gameObject.SetActive(false);
            int random = Random.Range(0, header.movePoints.Length);
            header.gameObject.transform.position = header.movePoints[random].position;
            toggleText.text = "Off";
        }
        else
        {
            header.gameObject.SetActive(true);
            toggleText.text = "On";
        }
    }
}
