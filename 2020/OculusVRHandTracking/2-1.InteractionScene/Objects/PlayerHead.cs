using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHead : MonoBehaviour
{
    GameManager gameMgr;

    Vector3 headPosition;
    RaycastHit hit;

    private float timer;
    private bool isStare;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        headPosition = gameMgr.mainCam.transform.position;
        timer = 3f;
        isStare = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position, fwd*100, Color.red, 0.1f);
        if (Physics.Raycast(transform.position, fwd, out hit, 100))
        {
            if (hit.collider.CompareTag("Bed"))
            {
                isStare = true;
                if (isStare == true)
                {
                    timer -= Time.deltaTime;
                }
                if (timer <= 0)
                {
                    gameMgr.StartCoroutine(gameMgr.NextDay());
                    timer = 3f;
                }
            }

        }
        else if (hit.collider == null)
        {
            isStare = false;
            timer = 3f;
        }
    }
}