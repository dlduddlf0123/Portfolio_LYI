using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandFollower : MonoBehaviour
{
    public float moveSpeed = 8f;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = GameManager.Instance.handCtrl.handColl.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.handCtrl.handColl.activeSelf)
        {
            transform.position = Vector3.Lerp(transform.position, GameManager.Instance.handCtrl.handColl.transform.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
