using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunMapScroller : MonoBehaviour
{
    public float moveSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveGround());
    }

    
    IEnumerator MoveGround()
    {
        while (true)
        {
            transform.Translate(new Vector3(0, 0, -0.01f * moveSpeed));
            yield return new WaitForSeconds(0.01f);
        }
    }
}
