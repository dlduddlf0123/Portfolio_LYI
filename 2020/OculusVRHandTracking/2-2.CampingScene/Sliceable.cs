using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliceable : MonoBehaviour
{
    public Material sliceMat;

    public bool isSlice = true;
    float coolTime = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        isSlice = true;
        StartCoolDown();
    }

    public void StartCoolDown()
    {
        if (!isSlice)
        {
            return;
        }
        StopAllCoroutines();
        StartCoroutine(CoolTime());
    }

    public IEnumerator CoolTime()
    {
        yield return new WaitForSeconds(coolTime);
        isSlice = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Slicer"))
        {
            if (!isSlice)
            {
                isSlice = true;
                StartCoolDown();
                MeshCut.Cut(gameObject, collision.gameObject.transform.position, collision.gameObject.transform.forward, sliceMat);

            }
        }
    }

}
