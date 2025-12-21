using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PauseCountdown : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public int timeCount;
    public bool isCountOver;
    public GameObject pauseButton;
    // Start is called before the first frame update
    void Awake()
    {
        isCountOver = false;
    }
    private void Update()
    {
        timeText.text = string.Format("{0:f0}", timeCount);
        
        if(timeCount ==0)
        {
            pauseButton.GetComponent<Button>().interactable = true;
            Time.timeScale = 1;
            AudioListener.pause = false;
            gameObject.SetActive(false);
        }
    }
}
