using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PinchPowerUI : MonoBehaviour
{
    public HandInteract leftHand;
    public HandInteract rightHand;

    public Text[] leftPinches;
    public Text[] rightPinches;

    public Text leftHandGesture;
    public Text rightHandGesture;

    public Text[] leftHandState;
    public Text[] rightHandState;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 5; i++)
        {
            leftPinches[i].text = leftHand.arr_fingerStrength[i].ToString();
            rightPinches[i].text = rightHand.arr_fingerStrength[i].ToString();
        }
        leftHandGesture.text = "Gesture: " + leftHand.handGesture.ToString();
        rightHandGesture.text = "Gesture: " + rightHand.handGesture.ToString();

        for (int i = 0; i < leftHandState.Length; i++)
        {
            leftHandState[i].text = leftHandState[i].name + ": " + leftHand.arr_state[i].ToString();
            rightHandState[i].text = rightHandState[i].name + ": " + rightHand.arr_state[i].ToString();
        }
    }
}
