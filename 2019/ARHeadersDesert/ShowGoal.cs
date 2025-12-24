using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGoal : MonoBehaviour
{
    public GameManager gameMgr { get; set; }
    
    // Start is called before the first frame update
    void Awake()
    {
        gameMgr = GameManager.Instance;
    }

    private void OnEnable()
    {
        gameMgr.cutSceneMgr.EndCutScene();
    }
}
