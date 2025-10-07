using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    public MotionCommand[] arr_commands;

    // Start is called before the first frame update
    void Awake()
    {
        arr_commands = GetComponentsInChildren<MotionCommand>();
    }

    /// <summary>
    /// 트리거가 작동될 때 / 조건이 만족되었을 때
    /// 손 근처에 이 오브젝트(충돌체)를 위치시킨다
    /// HandInteract 에서 호출
    /// </summary>
    public void StartMotionCommand(HandInteract _hand)
    {
        transform.position = _hand.skeleton.Bones[10].Transform.position;
        transform.LookAt(GameManager.Instance.mainCam.transform);

        for (int _cmdIndex = 0; _cmdIndex < arr_commands.Length; _cmdIndex++)
        {
            switch (arr_commands[_cmdIndex].typeMotion)
            {
                case MotionType.CALL:
                    break;
                case MotionType.ROLL:
                    for (int _partIndex = 0; _partIndex < arr_commands[_cmdIndex].commandParts.Length; _partIndex++)
                    {
                        arr_commands[_cmdIndex].commandParts[_partIndex].gameObject.SetActive(true);
                    }
                    break;
                default:
                    break;
            }
            arr_commands[_cmdIndex].gameObject.SetActive(true);
        }
        gameObject.SetActive(true);
    }

    public void StopMotionCommand()
    {
        for (int i = 0; i < arr_commands.Length; i++)
        {
            arr_commands[i].motionStep = 0;
        }
        gameObject.SetActive(false);
    }
}
