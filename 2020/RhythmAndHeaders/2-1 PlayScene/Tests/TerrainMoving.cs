using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMoving : MonoBehaviour
{
    public Terrain terrain1;
    public Terrain terrain2;
    public GameObject terrainChecker;
    // Start is called before the first frame update
    private void Awake()
    {
        terrainChecker.transform.position = terrain1.transform.position - new Vector3(terrain1.terrainData.size.x * 1.3f, 0, 0);
    }
    private void Update()
    {
        terrain1.gameObject.transform.Translate(-20*Time.deltaTime, 0, 0);
        terrain2.gameObject.transform.Translate(-20 * Time.deltaTime, 0, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Terrain"))
        other.gameObject.transform.Translate(terrain1.terrainData.size.x * 2, 0, 0);
    }
}
