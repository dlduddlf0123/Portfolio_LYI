using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingIcon : MonoBehaviour
{
    Image img_loading;
    float rot = 0f;
    public float speed = 1f;
    public float rotspeed = 1f;
    bool isFlip = false;

    void Awake()
    {
        img_loading = GetComponent<Image>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (img_loading.fillAmount >= 1 || img_loading.fillAmount <= 0)
        {
            if(isFlip)
            { img_loading.fillAmount = 0; }
            else
            { img_loading.fillAmount = 1;
                rot = 0;
            }

            isFlip = !isFlip;
            img_loading.fillClockwise = !isFlip;
        }
        if (isFlip)
        {
            img_loading.fillAmount -= speed * Time.deltaTime;
        }
        else
        {
            img_loading.fillAmount += speed * Time.deltaTime;
        }
        rot -= speed * Time.deltaTime * rotspeed;
        transform.rotation = new Quaternion(0, 0, rot, 1);
    }
}
