using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPlaneChecker : MonoBehaviour
{
    public ARDummyColl[] arr_coll;

    public SpriteRenderer m_sprite;
    public Sprite sprite_default;
    public Sprite sprite_ok;

    public Material transparentMat;
    public Material defaultMat;

    public void CheckAllIn()
    {
        int count = 0;
        for (int i = 0; i < arr_coll.Length; i++)
        {
            if (arr_coll[i].isColled )
            {
                count++;
            }
        }

        if (count == arr_coll.Length)
        {
            GetComponent<Renderer>().material = defaultMat;
            m_sprite.sprite = sprite_ok;

            GameManager.Instance.uiMgr.ui_ready.ready_btn_start.gameObject.SetActive(true);
        }
        else
        {
            m_sprite.sprite = sprite_default;

            GetComponent<Renderer>().material = transparentMat;
            GameManager.Instance.uiMgr.ui_ready.ready_btn_start.gameObject.SetActive(false);
        }
    }
}
