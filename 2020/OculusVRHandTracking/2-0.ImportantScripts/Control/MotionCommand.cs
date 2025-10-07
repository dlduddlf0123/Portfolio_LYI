using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MotionCommand : MonoBehaviour
{
    GameManager gameMgr;
    StageManager stageMgr;
    CommandManager cmdMgr;

    public MotionType typeMotion;

    public CommandPart[] commandParts;

    public int motionStep = 0;
    public int finalStep = 0;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        stageMgr = gameMgr.currentPlay.GetComponent<StageManager>();
        cmdMgr = transform.parent.GetComponent<CommandManager>();

        commandParts = GetComponentsInChildren<CommandPart>();
    }

   

    /// <summary>
    /// 충돌 시 해당 커맨드에 맞는 동작 호출
    /// 현재 선택된 캐릭터에만 동작 적용
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (typeMotion)
            {
                case MotionType.CALL:
                    //부르기
                    if (stageMgr.interactHeader != null)
                    {
                        motionStep++;
                        if (motionStep >= finalStep)
                        {
                            stageMgr.interactHeader.OrderAction(1);
                            gameMgr.PlayEffect(transform.position, gameMgr.particles[0]);
                            cmdMgr.StopMotionCommand();
                        }
                    }
                    break;
                case MotionType.ROLL:
                    break;
                default:
                    break;
            }

        }
    }

    public void CheckMotionRoll()
    {
        Debug.Log("motionStep: " + motionStep);
        if (motionStep >= finalStep)
        {
            stageMgr.interactHeader.OrderAction(3);
            gameMgr.PlayEffect(transform.position, gameMgr.particles[0]);
            cmdMgr.StopMotionCommand();
        }
    }
}
