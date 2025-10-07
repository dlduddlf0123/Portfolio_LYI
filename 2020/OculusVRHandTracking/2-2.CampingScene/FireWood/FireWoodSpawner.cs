using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWoodSpawner : MonoBehaviour
{
    public MiniGameFireWood fireWoodMgr;
    public Transform spawnPoint;
    public GameObject spawnObject;

    public List<GameObject> list_fireWood = new List<GameObject>();


    public void Spawn()
    {
        if (fireWoodMgr.statMiniGame != MiniGameState.PLAYING)
        {
            return;
        }
        StopAllCoroutines();
        StartCoroutine(lateSpawn());
    }

    public void GetScore()
    {
        fireWoodMgr.GetScore(100);
    }

    IEnumerator lateSpawn()
    {
        yield return new WaitForSeconds(1f);
        if (list_fireWood.Count <= 2)
        {
            GameObject go = Instantiate(spawnObject);
            go.transform.position = spawnPoint.position + new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f));
            go.transform.GetComponent<FireWoodColl>().spawner = this;
            go.transform.GetChild(0).GetComponent<FireWood>().spawner = this;
            list_fireWood.Add(go);
        }
        else
        {
            GameObject go = list_fireWood[0];
            go.transform.GetChild(0).GetComponent<FireWood>().Init();
            go.transform.position = spawnPoint.position + new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f));
            go.transform.rotation = spawnPoint.rotation;
            go.SetActive(true);
            list_fireWood.RemoveAt(0);
        }
    }
}
