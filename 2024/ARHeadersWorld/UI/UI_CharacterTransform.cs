using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;



/// <summary>
/// 12/18/2023-LYI
/// 캐릭터 선택 후 지면에 생성한 경우 활성화
/// 캐릭터 위치와 로테이션, 스케일 값 조정
/// </summary>
public class UI_CharacterTransform : MonoBehaviour
{
    GameManager gameMgr;
    public CharacterTransformRay transformRay;

    public Slider slider_position;
    public Slider slider_rotation;
    public Slider slider_scale;

    public Button btn_confirm;

    bool isFirst = true;


    public void CharacterTransformUIInit()
    {
        gameMgr = GameManager.Instance;

        if (gameMgr.spawnARCharacter == null)
        {
            Debug.Log("Character is Null!!");
            return;
        }

        SetSliderValuePosition(gameMgr.spawnARCharacter.transform.position.y);
        SetSliderValueRotation(slider_rotation, 0, 360, gameMgr.spawnARCharacter.transform.rotation.y);
        SetSliderValueScale(slider_scale, gameMgr.spawnARCharacter.transform.localScale.x);

        if (isFirst)
        {
            slider_position.onValueChanged.AddListener(ChangeSliderPosition);
            slider_rotation.onValueChanged.AddListener(ChangeSliderRotation);
            slider_scale.onValueChanged.AddListener(ChangeSliderScale);

            btn_confirm.onClick.AddListener(ConfirmButton);
            isFirst = false;
        }

    }

    public void SetSliderValuePosition(float value)
    {
        slider_position.minValue = value - 0.5f;
        slider_position.maxValue = value + 0.5f;
       // slider_position.value = gameMgr.spawnARCharacter.transform.position.y;
        slider_position.SetValueWithoutNotify(gameMgr.spawnARCharacter.transform.position.y);
    }
    void SetSliderValueRotation(Slider slider, float minValue, float maxValue, float defaultValue)
    {
        slider.minValue = minValue;
        slider.maxValue = maxValue;
       //slider.value = ScaleFloatToRotation(defaultValue);
        slider.SetValueWithoutNotify(ScaleFloatToRotation(defaultValue));
    }
    float ScaleFloatToRotation(float value)
    {
        Quaternion quaternion = new Quaternion(0, value, 0, 1);
        Vector3 eulerAngles = quaternion.eulerAngles;
        float f = eulerAngles.y;
        return f; 
    }

    void SetSliderValueScale(Slider slider, float value)
    {
        slider.minValue = 0;
        slider.maxValue = value + value * 5;
        //slider.value = value;
        slider.SetValueWithoutNotify(value);
    }

    void ChangeSliderPosition(float value)
    {
        if (gameMgr.spawnARCharacter == null)
        {
            return;
        }

        gameMgr.spawnARCharacter.transform.position = new Vector3(
            gameMgr.spawnARCharacter.transform.position.x,
            value,
             gameMgr.spawnARCharacter.transform.position.z);
    }
    void ChangeSliderRotation(float value)
    {
        if (gameMgr.spawnARCharacter == null)
        {
            return;
        }
        gameMgr.spawnARCharacter.transform.rotation = Quaternion.Euler(
            gameMgr.spawnARCharacter.transform.rotation.x,
            value, 
            gameMgr.spawnARCharacter.transform.rotation.z);
    }
    void ChangeSliderScale(float value)
    {
        if (gameMgr.spawnARCharacter == null)
        {
            return;
        }
        gameMgr.spawnARCharacter.transform.localScale = Vector3.one * value;
    }

    void ConfirmButton()
    {
        gameMgr.ChangeGameMode(GameMode.ANIMATION);
    }


}
