using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Transform NPC_Pool;

    private GameManager GameMgr;

	private void Awake ()
    {
        GameMgr = MonoSingleton<GameManager>.Inst;

        GameMgr.spawnManager.setNPC_Pool(NPC_Pool);

        GameMgr.spawnManager.FieldList.Clear();
        for (int index = 0; index < 6; index++)
            GameMgr.spawnManager.FieldList.Add(transform.GetChild(0).GetChild(index));

        int rand = Random.Range(0, 2);

        if (GameMgr.isEvent)
        {
            GameMgr.spawnManager.isReverse = false;
            GameMgr.spawnManager.StartPos = transform.GetChild(3);
        }
        else
        {
            GameMgr.spawnManager.isReverse = rand != 0 ? true : false;
            GameMgr.spawnManager.StartPos = transform.GetChild(3 + rand);
        }

        if (GameMgr.spawnManager.isReverse)
            GameMgr.spawnManager.FieldList.Reverse();

        switch(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex)
        {
            case 2:
                GameMgr.Main_Cam.backgroundColor = new Color(0.09f, 0.105f, 0.07f, 1.0f);
                break;
            case 3:
                GameMgr.Main_Cam.backgroundColor = new Color(0.09f, 0.105f, 0.07f, 1.0f);
                break;
            case 4:
                GameMgr.Main_Cam.backgroundColor = new Color(0.055f, 0.066f, 0.22f, 1.0f);
                break;
            case 5:
                GameMgr.Main_Cam.backgroundColor = new Color(0.07f, 0.08f, 0.07f, 0.09f);
                break;
            case 6:
                GameMgr.Main_Cam.backgroundColor = new Color(0.07f, 0.08f, 0.07f, 0.09f);
                break;
        }

        StartCoroutine(StartScene());
	}

    private IEnumerator StartScene()
    {
        yield return new WaitForSeconds(1.5f);

        GameMgr.Main_UI.SetLoading(false);
        GameMgr.spawnManager.SpawnStart();
    }
}
