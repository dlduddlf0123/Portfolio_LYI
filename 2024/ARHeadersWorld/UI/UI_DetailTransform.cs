using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;


public class UI_DetailTransform : MonoBehaviour
{
    GameManager gameMgr;

    [Header("Transform")]
    public Slider slider_x;
    public Slider slider_z;

    public Button btn_confirm;

    [Header("Light")]
    public Button btn_lightAuto;
    public Button btn_lightFixed;
    public Button btn_toggleShadow;

    bool isShadowActive;
    bool isFirst = true;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
    }


    private void OnEnable()
    {
        DetailTransformUIInit();
    }

    public void DetailTransformUIInit()
    {
        gameMgr = GameManager.Instance;
        if (gameMgr.spawnARCharacter == null)
        {
            Debug.Log("Character is Null!!");
            return;
        }
      
        SetSliderValuePositionX(gameMgr.spawnARCharacter.transform.position.x);
        SetSliderValuePositionZ(gameMgr.spawnARCharacter.transform.position.z);

        if (gameMgr.isLightAuto)
        {
            ButtonLightAuto();
        }
        else
        {
            ButtonLightFixed();
        }

        if (isFirst)
        {
            slider_x.onValueChanged.AddListener(ChangeSliderPositionX);
            slider_z.onValueChanged.AddListener(ChangeSliderPositionZ);

            btn_confirm.onClick.AddListener(ConfirmButton);

            btn_lightAuto.onClick.AddListener(ButtonLightAuto);
            btn_lightFixed.onClick.AddListener(ButtonLightFixed);
            btn_toggleShadow.onClick.AddListener(ButtonToggleShadow);

            isFirst = false;
        }
    }

    public void SetSliderValuePositionX(float value)
    {
        slider_x.minValue = value - 0.5f;
        slider_x.maxValue = value + 0.5f;
       // slider_x.value = gameMgr.spawnARCharacter.transform.position.x;
        slider_x.SetValueWithoutNotify(gameMgr.spawnARCharacter.transform.position.x);
    }
    public void SetSliderValuePositionZ(float value)
    {
        slider_z.minValue = value - 0.5f;
        slider_z.maxValue = value + 0.5f;
       // slider_z.value = gameMgr.spawnARCharacter.transform.position.z;

        slider_z.SetValueWithoutNotify(gameMgr.spawnARCharacter.transform.position.z);
    }


    void ChangeSliderPositionX(float value)
    {
        if (gameMgr.spawnARCharacter == null)
        {
            return;
        }

        gameMgr.spawnARCharacter.transform.position = new Vector3(
            value,
            gameMgr.spawnARCharacter.transform.position.y,
             gameMgr.spawnARCharacter.transform.position.z);
    }
    void ChangeSliderPositionZ(float value)
    {
        if (gameMgr.spawnARCharacter == null)
        {
            return;
        }

        gameMgr.spawnARCharacter.transform.position = new Vector3(
            gameMgr.spawnARCharacter.transform.position.x,
             gameMgr.spawnARCharacter.transform.position.y,
             value);
    }

    void ConfirmButton()
    {
        gameMgr.ChangeGameMode(GameMode.ANIMATION);
    }



    void  ButtonLightAuto()
    {
        gameMgr.ChangeLightAuto(true);
        btn_lightAuto.image.sprite = gameMgr.uiMgr.sprite_yellow;
        btn_lightFixed.image.sprite = gameMgr.uiMgr.sprite_blue;
    }

    void ButtonLightFixed()
    {
        gameMgr.ChangeLightAuto(false);
        btn_lightAuto.image.sprite = gameMgr.uiMgr.sprite_blue;
        btn_lightFixed.image.sprite = gameMgr.uiMgr.sprite_yellow;
    }


    void ButtonToggleShadow()
    {
        SetShadowActive(!isShadowActive);
    }

    void SetShadowActive(bool isActive)
    {
        if (isActive)
        {
            gameMgr.light_ar.shadows = LightShadows.Soft;
            gameMgr.light_normal.shadows = LightShadows.Soft;
            btn_toggleShadow.image.sprite = gameMgr.uiMgr.sprite_blue;
            isShadowActive = true;
        }
        else
        {
            gameMgr.light_ar.shadows = LightShadows.None;
            gameMgr.light_normal.shadows = LightShadows.None;
            btn_toggleShadow.image.sprite = gameMgr.uiMgr.sprite_red;
            isShadowActive = false;
        }
    }




}
