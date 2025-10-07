using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SizeChanger1 : MonoBehaviour
{
    Scrollbar scrollBar;
    public Cinemachine.CinemachineVirtualCamera changeTr;

    // Start is called before the first frame update
    void Start()
    {
        scrollBar = GetComponent<Scrollbar>();

        scrollBar.onValueChanged.AddListener(
            (value) =>
            changeTr.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>().m_CameraDistance = value*10 + 5);
    }

}
