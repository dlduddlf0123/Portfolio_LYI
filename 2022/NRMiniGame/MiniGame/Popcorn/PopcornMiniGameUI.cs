using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
public class PopcornMiniGameUI : MiniGameUI
{
    public TextMeshProUGUI game_text_popcorn { get; set; }
    // Start is called before the first frame update
    void Awake()
    {
        game_text_time = stage_gameUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        game_text_popcorn = stage_gameUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        UIInit();
    }
    public void GameUIChangePopcornText(int popcorn,int maxPopcorn)
    {
        game_text_popcorn.text = popcorn.ToString() +"/" + maxPopcorn.ToString();
    }

}
