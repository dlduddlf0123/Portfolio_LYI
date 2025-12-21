using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBoat : InteractionManager
{
    Character header;
    public GameObject boat;
    Transform firstParent;

    public Transform endPos;
    public float speed = 1f;
    float wind = 0.0f;
    protected override void DoAwake()
    {
        header = gameMgr.currentEpisode.currentStage.header;
    }

    private void Update()
    {
        if (wind > 0)
        {
            wind -= Time.deltaTime *speed;
            boat.transform.position = Vector3.Lerp(boat.transform.position, endPos.position, Time.deltaTime * speed * wind);

            if (Vector3.Distance(boat.transform.position,endPos.position) < 0.1f &&
                gameMgr.statGame == GameStatus.GAME)
            {
                EndInteraction();
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") &&
            gameMgr.statGame == GameStatus.GAME &&
            gameMgr.currentEpisode.currentStage.currentInteraction == 5)
        {
            wind++;
        }
    }


    public override void StartInteraction()
    {
        base.StartInteraction();

        firstParent = header.transform.parent;
        header.transform.parent = boat.transform;
        header.transform.localPosition = Vector3.up * 0.1f;

        list_guidePosition.Add(transform.position);
        PlayGuideParticle();
    }

    public override void EndInteraction()
    {
        StopGuideParticle();

        base.EndInteraction();
        boat.transform.position = endPos.transform.position;
        header.transform.localPosition = Vector3.up * 0.1f;
        header.transform.parent = firstParent;
        header.transform.localRotation = Quaternion.Euler(Vector3.up * 90f);
        gameObject.SetActive(false);
    }

    public void TurnSignal(Transform _target)
    {
        gameMgr.currentEpisode.currentStage.header.TurnLook(_target);
    }
}
